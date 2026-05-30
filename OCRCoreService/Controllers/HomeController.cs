using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OCRCoreService.Services;
using PaddleOCRSDK;
using PaddleOCRSDK.Models;
using System.Globalization;
using System.Net;

namespace OCRCoreService.Controllers
{
    /// <summary>
    /// 默认页面
    /// </summary>
    public class HomeController : Controller
    {
        private const long MaxLicenseSize = 1024 * 1024;
        private readonly ILogger<HomeController> logger;
        private readonly OCREngine ocrEngine;
        private readonly IOCRService ocrService;
        private readonly IServiceProvider serviceProvider;
        private readonly OCRVLConfig? ocrvlConfig;

        public HomeController(
            ILogger<HomeController> logger,
            OCREngine ocrEngine,
            IOCRService ocrService,
            IServiceProvider serviceProvider,
            OCRVLConfig? ocrvlConfig = null)
        {
            this.logger = logger;
            this.ocrEngine = ocrEngine;
            this.ocrService = ocrService;
            this.serviceProvider = serviceProvider;
            this.ocrvlConfig = ocrvlConfig;
        }

        /// <summary>
        /// 默认视图
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取GPU授权申请码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetLicenseRequestCode()
        {
            try
            {
                string requestCode = ocrService.GetLicenseRequestCode();
                if (string.IsNullOrWhiteSpace(requestCode))
                {
                    return Json(BadApiResult("未获取到GPU授权申请码。"));
                }

                return Json(OkApiResult(new
                {
                    requestCode,
                    message = "GPU授权申请码已生成。"
                }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取GPU授权申请码失败");
                return Json(BadApiResult("获取GPU授权申请码失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 查看当前GPU授权状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetLicenseStatus()
        {
            try
            {
                ocrEngine.ActivateLicenseIfExists();
                LicenseStatus? status = ocrService.GetLicenseStatusInfo();
                if (status == null)
                {
                    string error = ocrService.GetError();
                    return Json(BadApiResult("未获取到授权状态。" + error));
                }

                LicenseValidationResult result = CreateValidationResult("PaddleOCR", status.Activated, status, ocrService.GetError());
                List<LicenseValidationResult> results = new() { result };
                if (IsOcrVlEnabled)
                {
                    OCRVLEngine? ocrvlEngine = serviceProvider.GetService<OCRVLEngine>();
                    if (ocrvlEngine != null)
                    {
                        ocrvlEngine.ActivateLicenseIfExists();
                        LicenseStatus? vlStatus = ocrvlEngine.OcrVlService.GetLicenseStatusInfo();
                        if (vlStatus != null)
                        {
                            results.Add(CreateValidationResult("PaddleOCR-VL", vlStatus.Activated, vlStatus, ocrvlEngine.OcrVlService.GetLastError()));
                        }
                    }
                }

                return Json(OkApiResult(new
                {
                    modules = results,
                    statusText = BuildLicenseSummary(results, GetModelsLicensePath(), false)
                }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "获取GPU授权状态失败");
                return Json(BadApiResult("获取GPU授权状态失败：" + ex.Message));
            }
        }

        /// <summary>
        /// 上传并校验授权文件
        /// </summary>
        /// <param name="file">授权文件</param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(MaxLicenseSize)]
        public async Task<IActionResult> UploadLicense(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(BadApiResult("授权文件不能为空。"));
            }

            if (file.Length > MaxLicenseSize)
            {
                return Json(BadApiResult("授权文件不能超过1MB。"));
            }

            string extension = Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".lic", StringComparison.OrdinalIgnoreCase))
            {
                return Json(BadApiResult("请上传.lic授权文件。"));
            }

            string tempDirectory = Path.Combine(Path.GetTempPath(), "PaddleOCRApi");
            Directory.CreateDirectory(tempDirectory);
            string tempPath = Path.Combine(tempDirectory, $"{Guid.NewGuid():N}.lic");

            try
            {
                await using (FileStream stream = System.IO.File.Create(tempPath))
                {
                    await file.CopyToAsync(stream);
                }

                LicenseValidationResult ocrResult = ValidateOcrLicense(tempPath);
                List<LicenseValidationResult> results = new() { ocrResult };
                if (IsOcrVlEnabled)
                {
                    results.Add(ValidateOcrVlLicense(tempPath));
                }

                bool canSave = results.Any(result => result.Activated);
                if (!canSave)
                {
                    return Json(BadApiResult("授权文件无效，未保存到Models目录。", new
                    {
                        licenseSaved = false,
                        modules = results
                    }));
                }

                string licensePath = GetModelsLicensePath();
                Directory.CreateDirectory(Path.GetDirectoryName(licensePath)!);
                System.IO.File.Copy(tempPath, licensePath, true);

                return Json(OkApiResult(new
                {
                    licenseSaved = true,
                    licensePath,
                    modules = results,
                    statusText = BuildLicenseSummary(results, licensePath, true)
                }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "上传授权文件失败");
                return Json(BadApiResult("上传授权文件失败：" + ex.Message));
            }
            finally
            {
                TryDelete(tempPath);
            }
        }

        /// <summary>
        /// 全局错误视图
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            return View();
        }

        private bool IsOcrVlEnabled => ocrvlConfig?.enabled == true;

        private LicenseValidationResult ValidateOcrLicense(string licensePath)
        {
            try
            {
                bool activated = ocrService.ActivateLicense(licensePath);
                LicenseStatus? status = ocrService.GetLicenseStatusInfo();
                return CreateValidationResult("PaddleOCR", activated, status, ocrService.GetError());
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "PaddleOCR授权校验失败");
                return new LicenseValidationResult
                {
                    Module = "PaddleOCR",
                    Activated = false,
                    Message = ex.Message
                };
            }
        }

        private LicenseValidationResult ValidateOcrVlLicense(string licensePath)
        {
            OCRVLEngine? ocrvlEngine = serviceProvider.GetService<OCRVLEngine>();
            IOCRVLService ocrVlService = ocrvlEngine?.OcrVlService ?? new OCRVLService();

            try
            {
                bool activated = ocrVlService.ActivateLicense(licensePath);
                LicenseStatus? status = ocrVlService.GetLicenseStatusInfo();
                return CreateValidationResult("PaddleOCR-VL", activated, status, ocrVlService.GetLastError());
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "PaddleOCR-VL授权校验失败");
                return new LicenseValidationResult
                {
                    Module = "PaddleOCR-VL",
                    Activated = false,
                    Message = ex.Message
                };
            }
            finally
            {
                if (ocrvlEngine == null && ocrVlService is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private static LicenseValidationResult CreateValidationResult(
            string module,
            bool activateResult,
            LicenseStatus? status,
            string? error)
        {
            bool activated = activateResult;
            return new LicenseValidationResult
            {
                Module = module,
                Available = true,
                Activated = activated,
                AllowGpu = status?.AllowGpu ?? false,
                MachineMatch = status?.MachineMatch ?? false,
                ProductName = status?.ProductName,
                Customer = status?.Customer,
                LicenseId = status?.LicenseId,
                ProductVersion = status?.ProductVersion,
                Platforms = status?.Platforms == null || status.Platforms.Count == 0 ? "" : string.Join(", ", status.Platforms),
                Products = status?.Products == null || status.Products.Count == 0 ? "" : string.Join(", ", status.Products),
                StartTime = FormatLicenseTime(status?.StartTime),
                ExpireTime = FormatLicenseTime(status?.ExpireTime),
                CurrentMachineCode = status?.CurrentMachineCode,
                MachineCode = status?.MachineCode,
                Message = activated ? "授权文件有效。" : (string.IsNullOrWhiteSpace(error) ? "授权文件对该模块无效。" : error)
            };
        }

        private static string GetModelsLicensePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "paddleocr.lic");
        }

        private static string BuildLicenseSummary(
            IEnumerable<LicenseValidationResult> results,
            string licensePath,
            bool licenseSaved)
        {
            List<string> lines = new()
            {
                $"{DateTime.Now:HH:mm:ss.fff}: GPU授权状态检查",
                "===============================================",
                licenseSaved ? $"授权文件已保存: {licensePath}" : $"授权文件路径: {licensePath}"
            };

            foreach (LicenseValidationResult result in results)
            {
                lines.Add("");
                lines.Add($"授权模块: {result.Module}");
                lines.Add($"授权状态: {(result.Activated ? "已授权" : "未授权")}");
                if (result.Activated)
                {
                    lines.Add($"产品名称: {DisplayValue(result.ProductName)}");
                    lines.Add($"客户信息: {DisplayValue(result.Customer)}");
                    lines.Add($"授权编号: {DisplayValue(result.LicenseId)}");
                    lines.Add($"授权版本: {DisplayValue(result.ProductVersion)}");
                    lines.Add($"授权平台: {DisplayValue(result.Platforms)}");
                    lines.Add($"授权产品: {DisplayValue(result.Products)}");
                    lines.Add($"GPU权限: {(result.AllowGpu ? "允许" : "不允许")}");
                    lines.Add($"授权匹配: {(result.MachineMatch ? "匹配" : "不匹配")}");
                    lines.Add($"开始时间: {DisplayValue(result.StartTime)}");
                    lines.Add($"到期时间: {DisplayValue(result.ExpireTime)}");
                    if (!string.IsNullOrWhiteSpace(result.CurrentMachineCode))
                    {
                        lines.Add($"当前机器码: {result.CurrentMachineCode}");
                    }

                    if (!string.IsNullOrWhiteSpace(result.MachineCode))
                    {
                        lines.Add($"授权机器码: {result.MachineCode}");
                    }
                }
                lines.Add($"说明: {DisplayValue(result.Message)}");
            }

            lines.Add("===============================================");
            return string.Join(Environment.NewLine, lines);
        }

        private static string DisplayValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string FormatLicenseTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out DateTimeOffset utcTime))
            {
                return $"{utcTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}";
            }

            return value;
        }

        private static ApiResult OkApiResult(object data)
        {
            return new ApiResult
            {
                Status = HttpStatusCode.OK,
                Data = data,
                ErrorMessage = ""
            };
        }

        private static ApiResult BadApiResult(string message, object? data = null)
        {
            return new ApiResult
            {
                Status = HttpStatusCode.BadRequest,
                Data = data ?? "",
                ErrorMessage = message
            };
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch
            {
            }
        }

        private sealed class LicenseValidationResult
        {
            public string Module { get; set; } = "";
            public bool Available { get; set; } = true;
            public bool Activated { get; set; }
            public bool AllowGpu { get; set; }
            public bool MachineMatch { get; set; }
            public string? ProductName { get; set; }
            public string? Customer { get; set; }
            public string? LicenseId { get; set; }
            public string? ProductVersion { get; set; }
            public string? Platforms { get; set; }
            public string? Products { get; set; }
            public string? StartTime { get; set; }
            public string? ExpireTime { get; set; }
            public string? CurrentMachineCode { get; set; }
            public string? MachineCode { get; set; }
            public string? Message { get; set; }
        }
    }
}
