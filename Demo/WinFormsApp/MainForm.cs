// Copyright (c) 2025 PaddleOCRCore All Rights Reserved.
// https://github.com/PaddleOCRCore/PaddleOCRApi.git
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using PaddleOCRSDK;
using PaddleOCRSDK.Models;
using SkiaSharp;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using WinFormsApp.Services;
using WinFormsApp.Utils;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        #region Public
        private StringBuilder message = new StringBuilder();
        private readonly IOCRService ocrService;
        private readonly IOCRVLService ocrvlService;

        public static bool use_gpu = false;
        public static int gpu_id = 0;
        public static int cpu_threads = Environment.ProcessorCount;
        public static int cpu_mem = 4000;
        public static string RecFilepath = "";
        public static bool outPutJson = false;
        public static bool use_tensorrt = false;
        public static int recCount = 1;
        public static int model_type = 0;
        public static int layout_type = 0;

        private bool isInitSuccess;
        private bool isOCRTextReady;
        private bool isOCRStructureReady;
        private bool isOCRVLInitSuccess;
        private bool isOCRVLLayoutAnalysis;
        #endregion

        #region PaddleOCR
        private CancellationTokenSource? ocrOperationCts;
        private bool isOCRBusy;
        #endregion

        #region PaddleOCR-VL
        private CancellationTokenSource? ocrVlOperationCts;
        private bool isOCRVLBusy;
        #endregion

        #region Paddle-UVDoc
        private IUVDocService? uvdocService;
        private string? currentImagePath;
        private string? outputImagePath;
        private CancellationTokenSource? uvDocOperationCts;
        private bool isUVDocBusy;
        #endregion

        #region Public
        public MainForm()
        {
            InitializeComponent();
            ocrService = OCREngine.ocrService;
            ocrvlService = new OCRVLService();
            ActiveControl = pictureBoxImg;
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string github = "--QQ群：475159576 https://github.com/PaddleOCRCore/PaddleOCRApi";
                Text = AssemblyHepler.AssemblyTitle + " V" + AssemblyHepler.AssemblyVersion + github;
                comboBoxPrompt.Items.AddRange(new object[]
                {
                    "OCR:",
                    "Table Recognition:",
                    "Formula Recognition:",
                    "Chart Recognition:",
                    "Spotting:",
                    "Seal Recognition:"
                });
                comboBoxPrompt.SelectedIndex = 0;
                comboBoxModel.SelectedIndex = 0;
                comboBoxLayoutModel.SelectedIndex = 0;
                RecFilepath = Path.Combine(Application.StartupPath, "output");
                Directory.CreateDirectory(RecFilepath);
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "uploads"));

                buttonFreeEngine.Enabled = false;
                toolStripMenuItemCheckLicense.Enabled = true;
                InitializeOCRVLDefaults();
                InitializeUVDocParameters();
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:使用前请先点击【下载OCR模型】下载版面识别及AI大模型");
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
                textBoxResult.Text = message.ToString();
            }
        }

        public static string FormatJsonSafe(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch
            {
                return Regex.Replace(
                    json,
                    @"\\u([0-9a-fA-F]{4})",
                    match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString())
                    .Replace("\\r\\n", Environment.NewLine)
                    .Replace("\\n", Environment.NewLine)
                    .Replace("\\r", Environment.NewLine);
            }
        }

        private static string NormalizeEscapedDisplayText(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string normalized = text;

            for (int i = 0; i < 3; i++)
            {
                string replaced = normalized
                    .Replace("\\u000D\\u000A", "\n")
                    .Replace("\\u000d\\u000a", "\n")
                    .Replace("\\u000A", "\n")
                    .Replace("\\u000a", "\n")
                    .Replace("\\u000D", "\n")
                    .Replace("\\u000d", "\n")
                    .Replace("\\r\\n", "\n")
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\n")
                    .Replace("\\t", "\t");

                if (replaced == normalized)
                {
                    break;
                }

                normalized = replaced;
            }

            normalized = Regex.Replace(
                normalized,
                @"\\u([0-9a-fA-F]{4})",
                match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());

            return normalized
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }

        private static string ExtractLayoutMarkdown(string layoutJson)
        {
            try
            {
                using JsonDocument doc = JsonDocument.Parse(layoutJson);
                if (doc.RootElement.ValueKind == JsonValueKind.Object
                    && doc.RootElement.TryGetProperty("markdown", out JsonElement markdownElement)
                    && markdownElement.ValueKind == JsonValueKind.String)
                {
                    return NormalizeEscapedDisplayText(markdownElement.GetString()).Trim();
                }
            }
            catch
            {
            }

            return string.Empty;
        }

        private void LogLayoutResult(string layoutJson)
        {
            if (!outPutJson)
            {
                string markdown = ExtractLayoutMarkdown(layoutJson);
                LogMessage(string.IsNullOrWhiteSpace(markdown) ? "Markdown为空或不存在。" : markdown);
                return;
            }
            LogMessage(FormatJsonSafe(layoutJson));
        }

        private static string? ResolveExistingImagePath(string? imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                return null;
            }

            string fullPath = Path.IsPathRooted(imagePath)
                ? imagePath
                : Path.Combine(Application.StartupPath, imagePath);

            fullPath = Path.GetFullPath(fullPath);
            return File.Exists(fullPath) ? fullPath : null;
        }

        private static string BuildLicenseStatusText(LicenseStatus status, bool licenseFileActivated, string title = "PaddleOCR")
        {
            var sb = new StringBuilder();
            string state = status.Activated ? "已授权" : "未授权";
            string gpuState = status.AllowGpu ? "允许" : "不允许";
            string machineState = status.MachineBound
                ? (status.MachineMatch ? "已绑定并匹配当前设备" : "已绑定但不匹配当前设备")
                : "未绑定设备";

            sb.AppendLine($"{DateTime.Now:HH:mm:ss.fff}: {title} GPU授权状态检查");
            sb.AppendLine("===============================================");
            sb.AppendLine($"授权模块: {title}");
            sb.AppendLine($"授权文件自动激活: {(licenseFileActivated ? "成功" : "未激活或未找到默认授权文件")}");
            sb.AppendLine($"授权状态: {state}");
            sb.AppendLine($"产品名称: {DisplayValue(status.ProductName)}");
            if (licenseFileActivated)
            {
                sb.AppendLine($"客户信息: {DisplayValue(status.Customer)}");
                sb.AppendLine($"授权编号: {DisplayValue(status.LicenseId)}");
                sb.AppendLine($"授权版本: {DisplayValue(status.ProductVersion)}");
                sb.AppendLine($"授权平台: {DisplayValue(status.Platforms == null || status.Platforms.Count == 0 ? "" : string.Join(", ", status.Platforms))}");
                sb.AppendLine($"GPU权限: {gpuState}");
                sb.AppendLine($"设备绑定: {machineState}");
                sb.AppendLine($"绑定模式: {DisplayValue(status.BindMode)}");
                sb.AppendLine($"授权匹配: {(status.MachineMatch ? "匹配" : "不匹配")}");
                sb.AppendLine($"开始时间: {FormatLicenseTime(status.StartTime)}");
                sb.AppendLine($"到期时间: {FormatLicenseTime(status.ExpireTime)}");

                sb.AppendLine($"授权产品: {DisplayValue(status.Products == null || status.Products.Count == 0 ? "" : string.Join(", ", status.Products))}");

                if (!string.IsNullOrWhiteSpace(status.CurrentMachineCode))
                {
                    sb.AppendLine($"当前机器码: {status.CurrentMachineCode}");
                }

                if (!string.IsNullOrWhiteSpace(status.MachineCode))
                {
                    sb.AppendLine($"授权机器码: {status.MachineCode}");
                }
            }

            sb.AppendLine(status.Activated && status.AllowGpu
                ? "当前授权可用于GPU初始化。"
                : "当前授权不可用于GPU初始化，请确认授权文件、设备绑定、有效期和GPU权限。");
            sb.AppendLine("===============================================");
            return sb.ToString();
        }

        private static string DisplayValue(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        }

        private static string FormatLicenseTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "-";
            }

            if (DateTimeOffset.TryParse(
                value,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                out var utcTime))
            {
                return $"{utcTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}";
            }

            return value;
        }

        public void LogMessage(string infoValue)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(LogMessage), infoValue);
                return;
            }

            textBoxResult.AppendText(infoValue);
            textBoxResult.AppendText(Environment.NewLine);
            textBoxResult.SelectionStart = textBoxResult.Text.Length;
            textBoxResult.ScrollToCaret();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            ocrOperationCts?.Cancel();
            ocrVlOperationCts?.Cancel();
            uvDocOperationCts?.Cancel();

            if (isInitSuccess)
            {
                OCREngine.FreeEngine();
            }

            if (isOCRVLInitSuccess)
            {
                try
                {
                    ocrvlService.FreeDocAnalyser();
                    ocrvlService.FreeEngine();
                }
                catch
                {
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }
            }
            catch
            {
            }

            uvdocService?.FreeUVDocEngine();
            ocrOperationCts?.Dispose();
            ocrVlOperationCts?.Dispose();
            uvDocOperationCts?.Dispose();
        }

        private static Image CloneImageFromFile(string path)
        {
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using Image image = Image.FromStream(stream);
            return new Bitmap(image);
        }
        #endregion

        #region PaddleOCR
        private CancellationToken BeginOCROperation()
        {
            ocrOperationCts?.Dispose();
            ocrOperationCts = new CancellationTokenSource();
            SetOCRBusy(true);
            return ocrOperationCts.Token;
        }

        private void EndOCROperation()
        {
            SetOCRBusy(false);
            ocrOperationCts?.Dispose();
            ocrOperationCts = null;
        }

        private void SetOCRBusy(bool busy)
        {
            isOCRBusy = busy;
            buttonInit.Enabled = !busy && !isInitSuccess;
            buttonFreeEngine.Enabled = !busy && isInitSuccess;
            buttonRec.Enabled = !busy && isOCRTextReady;
            buttonRecClipboard.Enabled = !busy && isOCRTextReady;
            buttonRecPDF.Enabled = !busy && isOCRStructureReady;
            buttonRecStructure.Enabled = !busy && isOCRStructureReady;
            comboBoxLayoutModel.Enabled = !busy && !isOCRStructureReady;
            buttonPostFile.Enabled = !busy;
            buttonGetBase64.Enabled = !busy;
            toolStripMenuItemDownloadOcrModels.Enabled = !busy;
            toolStripMenuItemCheckLicense.Enabled = !busy;
            comboBoxModel.Enabled = !busy && !isInitSuccess;
            chkUseGpu.Enabled = !busy && !isInitSuccess;
            chkUseTensorRT.Enabled = !busy && !isInitSuccess && chkUseGpu.Checked;
            chkJson.Enabled = !busy;
            chkReturnWordBox.Enabled = !busy && !isInitSuccess;
            numericUpDownThread.Enabled = !busy;
            numDowncpu_threads.Enabled = !busy && !isInitSuccess;
            numDowngpu_id.Enabled = !busy && !isInitSuccess;
            numericUpDowncpu_mem.Enabled = !busy && !isInitSuccess;
        }

        private void buttonCheckLicense_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxResult.Clear();
                message = new StringBuilder();

                bool licenseFileActivated = OCREngine.ActivateLicenseIfExists();
                LicenseStatus? status = ocrService.GetLicenseStatusInfo();

                if (status == null)
                {
                    string error = ocrService.GetError();
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}: 未获取到授权状态。{error}");
                    return;
                }

                LogMessage(BuildLicenseStatusText(status, licenseFileActivated));
                LogOCRVLLicenseStatus();
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}: 获取授权状态失败: {ex.Message}");
                string error = ocrService.GetError();
                if (!string.IsNullOrWhiteSpace(error))
                {
                    LogMessage($"DLL错误信息: {error}");
                }
            }
        }

        private void LogOCRVLLicenseStatus()
        {
            string licensePath = OCREngine.ResolveLicensePath();
            bool vlLicenseActivated = !string.IsNullOrWhiteSpace(licensePath)
                && File.Exists(licensePath)
                && ocrvlService.ActivateLicense(licensePath);
            LicenseStatus? vlStatus = ocrvlService.GetLicenseStatusInfo();
            if (vlStatus == null)
            {
                return;
            }

            LogMessage(BuildLicenseStatusText(vlStatus, vlLicenseActivated, "PaddleOCR-VL"));
        }

        private async void buttonInit_Click(object sender, EventArgs e)
        {
            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                isInitSuccess = false;
                OCREngine.use_gpu = use_gpu;
                OCREngine.gpu_id = gpu_id;
                OCREngine.cpu_threads = cpu_threads;
                OCREngine.cpu_mem = cpu_mem;
                OCREngine.use_tensorrt = use_tensorrt;
                OCREngine.return_word_box = chkReturnWordBox.Checked;

                switch (model_type)
                {
                    case 0:
                        OCREngine.det_infer = "PP-OCRv6_tiny_det_infer";
                        OCREngine.rec_infer = "PP-OCRv6_tiny_rec_infer";
                        OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                        break;
                    case 1:
                        OCREngine.det_infer = "PP-OCRv6_small_det_infer";
                        OCREngine.rec_infer = "PP-OCRv6_small_rec_infer";
                        OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                        break;
                    case 2:
                        OCREngine.det_infer = "PP-OCRv5_mobile_det_infer";
                        OCREngine.rec_infer = "PP-OCRv5_mobile_rec_infer";
                        OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                        break;
                    case 3:
                        OCREngine.det_infer = "PP-OCRv5_server_det_infer";
                        OCREngine.rec_infer = "PP-OCRv5_server_rec_infer";
                        OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                        break;
                    case 4:
                        OCREngine.det_infer = "PP-OCRv4_mobile_det_infer";
                        OCREngine.rec_infer = "PP-OCRv4_mobile_rec_infer";
                        OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                        break;
                }
                switch (layout_type)
                {
                    case 0:
                        OCREngine.layout_model_dir = "PP-DocLayoutV3_infer";
                        break;
                    case 1:
                        OCREngine.layout_model_dir = "PP-DocLayoutV2_infer";
                        break;
                    case 2:
                        OCREngine.layout_model_dir = "PP-DocLayout_plus-L_infer";
                        break;
                    case 3:
                        OCREngine.layout_model_dir = "PP-DocBlockLayout_infer";
                        break;
                    default:
                        OCREngine.layout_model_dir = "PP-DocLayoutV3_infer";
                        break;

                }

                string initmsg = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return OCREngine.GetOCREngine();
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                LogMessage(string.IsNullOrEmpty(initmsg)
                    ? $"{DateTime.Now:HH:mm:ss.fff}:文本识别初始化成功！"
                    : $"{DateTime.Now:HH:mm:ss.fff}:{initmsg}");

                bool textReady = initmsg.IndexOf("初始化成功", StringComparison.Ordinal) >= 0;

                initmsg = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return OCREngine.GetOCRStructureEngine();
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                LogMessage(string.IsNullOrEmpty(initmsg)
                    ? $"{DateTime.Now:HH:mm:ss.fff}:版面识别初始化成功！"
                    : $"{DateTime.Now:HH:mm:ss.fff}:{initmsg}");

                bool structureReady = initmsg.IndexOf("初始化成功", StringComparison.Ordinal) >= 0;
                isOCRTextReady = textReady;
                isOCRStructureReady = structureReady;
                isInitSuccess = textReady || structureReady;
            }
            catch (OperationCanceledException)
            {
                OCREngine.FreeEngine();
                isOCRTextReady = false;
                isOCRStructureReady = false;
                isInitSuccess = false;
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR初始化已取消");
            }
            catch (Exception ex)
            {
                isOCRTextReady = false;
                isOCRStructureReady = false;
                isInitSuccess = false;
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR初始化失败:{ex.Message}");
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async Task<string> RecOCRAsync(string filePath, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            LogMessage($"开始时间: {DateTime.Now:HH:mm:ss.fff}");

            OCRResult ocrResult = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ocrService.Detect(filePath);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            string result = BuildOCRText(ocrResult);
            LogMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogOCRResult(result, ocrResult);
            LogMessage("===============================================");
            return result;
        }

        private static string BuildOCRText(OCRResult ocrResult)
        {
            if (ocrResult.Code != 1)
            {
                return ocrResult.ErrorMsg;
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in ocrResult.WordsResult)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
                stringBuilder.Append(item.Words);
            }
            return stringBuilder.ToString();
        }

        private void LogOCRResult(string result, OCRResult ocrResult)
        {
            if (!string.IsNullOrEmpty(result))
            {
                LogMessage(result);
            }
            else
            {
                LogMessage("识别失败:" + ocrService.GetError());
            }

            if (outPutJson && !string.IsNullOrEmpty(ocrResult.JsonText))
            {
                LogMessage($"输出json: {FormatJsonSafe(ocrResult.JsonText)}");
            }
        }

        private async void buttonRec_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();

                for (int i = 0; i < recCount; i++)
                {
                    foreach (var regfile in openFileDialog.FileNames)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        LogMessage($"Image: {Path.GetFileName(regfile)}");
                        string filePath = Path.GetFullPath(regfile);
                        string recFileName = Path.Combine(RecFilepath, Path.GetFileName(regfile));
                        await RecOCRAsync(filePath, cancellationToken);
                        if (File.Exists(recFileName))
                        {
                            pictureBoxImg.ImgPath = recFileName;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("OCR文本识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async void buttonRecClipboard_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage())
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:剪贴板中没有图片！");
                return;
            }

            Image? clipboardImage = Clipboard.GetImage();
            if (clipboardImage == null)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:无法从剪贴板获取图片！");
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();

                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                Directory.CreateDirectory(uploadsPath);
                string clipboardImagePath = Path.Combine(uploadsPath, $"clipboard_{DateTime.Now:yyyyMMddHHmmss}.png");
                using (clipboardImage)
                {
                    await Task.Run(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        clipboardImage.Save(clipboardImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }, cancellationToken);
                }

                LogMessage($"剪贴板图片已保存到: {clipboardImagePath}");
                LogMessage("Image: 剪贴板图片");
                OCRResult ocrResult = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ocrService.Detect(clipboardImagePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                string result = BuildOCRText(ocrResult);
                LogOCRResult(result, ocrResult);

                string resultImagePath = Path.Combine(RecFilepath, Path.GetFileName(clipboardImagePath));
                if (File.Exists(resultImagePath))
                {
                    pictureBoxImg.ImgPath = resultImagePath;
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("剪贴板OCR识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:剪贴板OCR识别异常:{ex.Message}");
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async Task<string> RecOCRFromPDFAsync(string pdfFilePath, CancellationToken cancellationToken)
        {
            SKBitmap? bitmap = null;
            try
            {
                var stopwatch = Stopwatch.StartNew();
                LogMessage($"PDF文件: {pdfFilePath}");
                LogMessage($"开始时间: {DateTime.Now:HH:mm:ss.fff}");

                string uploadImagePath = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                    using MemoryStream memoryStream = new MemoryStream(pdfBytes);
                    bitmap = PDFtoImage.Conversion.ToImage(memoryStream);

                    using SKImage image = SKImage.FromBitmap(bitmap);
                    using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
                    byte[] imageBytes = data.ToArray();

                    string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                    Directory.CreateDirectory(uploadsPath);
                    string path = Path.Combine(uploadsPath, Path.GetFileNameWithoutExtension(pdfFilePath) + "_page1.png");
                    File.WriteAllBytes(path, imageBytes);
                    return path;
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                LogMessage($"已保存转换后的图片到uploads: {uploadImagePath}");
                string layoutJson = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ocrService.DetectLayout(uploadImagePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                LogMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
                LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                LogMessage("版面识别结果:");
                LogLayoutResult(layoutJson);
                return layoutJson;
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        private async void buttonRecPDF_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                string filePath = Path.GetFullPath(openFileDialog.FileName);
                LogMessage($"正在处理PDF文件: {filePath}");
                string result = await RecOCRFromPDFAsync(filePath, cancellationToken);
                if (!string.IsNullOrEmpty(result))
                {

                    LayoutDetectResult layoutResult = ocrService.ParseLayoutResult(result);
                    string? resultPath = ResolveExistingImagePath(layoutResult.VisPath);
                    if (!string.IsNullOrEmpty(resultPath))
                    {
                        pictureBoxImg.ImgPath = resultPath;
                    }
                    else
                    {
                        string uploadFileName = Path.GetFileNameWithoutExtension(filePath) + "_page1.png";
                        string recFileName = Path.Combine(RecFilepath, uploadFileName);
                        if (File.Exists(recFileName))
                        {
                            pictureBoxImg.ImgPath = recFileName;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("PDF识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage($"PDF识别异常: {ex.Message}");
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async Task<string> RecOCRStructureAsync(string filePath, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            LogMessage($"Image: {filePath}");
            LogMessage($"开始时间: {DateTime.Now:HH:mm:ss.fff}");

            string layoutJson = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ocrService.DetectLayout(filePath);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            LogMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogMessage("版面识别结果:");
            LogLayoutResult(layoutJson);
            return layoutJson;
        }

        private async void buttonRecStructure_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                foreach (var regfile in openFileDialog.FileNames)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string filePath = Path.GetFullPath(regfile);
                    string result = await RecOCRStructureAsync(filePath, cancellationToken);
                    try
                    {
                        LayoutDetectResult layoutResult = ocrService.ParseLayoutResult(result);
                        string? resultPath = ResolveExistingImagePath(layoutResult.VisPath);
                        if (!string.IsNullOrEmpty(resultPath))
                        {
                            pictureBoxImg.ImgPath = resultPath;
                        }
                        else if (File.Exists(filePath))
                        {
                            pictureBoxImg.ImgPath = filePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"版面识别结果解析失败:{ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("OCR版面识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private void buttonGetLicenseRequestCode_Click(object sender, EventArgs e)
        {
            try
            {
                string requestCode = ocrService.GetLicenseRequestCode();
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}: GPU授权申请码");
                LogMessage("===============================================");
                if (string.IsNullOrWhiteSpace(requestCode))
                {
                    LogMessage("未获取到GPU授权申请码。");
                    return;
                }

                LogMessage(requestCode);
                LogMessage("===============================================");
                Clipboard.SetText(requestCode);
                LogMessage("GPU授权申请码已复制到剪贴板");
                LogMessage("点击菜单【GPU授权】-【免费试用GPU】申请授权文件");
                LogMessage("授权文件paddleocr.lic请放到models目录，然后再初始化OCR");
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}: 获取GPU授权申请码失败:{ex.Message}");
            }
        }

        private void ToolStripMenuItemApplyGPUTrial_Click(object sender, EventArgs e)
        {
            OpenUrl("http://ocr.axinw.com");
        }
        private void buttonDownModels_Click(object sender, EventArgs e)
        {
            string urlV5 = "https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html";
            string downloaderFileName = "PaddleOCRModelsDownloader.exe";
            string downloaderPath = Path.Combine(AppContext.BaseDirectory, downloaderFileName);
            try
            {
                if (!File.Exists(downloaderPath))
                {
                    MessageBox.Show($"未找到模型下载程序：{downloaderPath}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ProcessStartInfo startInfo = new()
                {
                    FileName = downloaderPath,
                    WorkingDirectory = Path.GetDirectoryName(downloaderPath) ?? AppContext.BaseDirectory,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:已启动模型下载程序：{downloaderFileName}");
                LogMessage($"PP-OCRv5/PP-OCRv4模型手动下载地址：{urlV5}");

            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法启动模型下载程序：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolStripMenuItemVisitGitHub_Click(object sender, EventArgs e)
        {
            OpenUrl("https://github.com/PaddleOCRCore/PaddleOCRApi");
        }

        private void toolStripMenuItemVisitGitee_Click(object sender, EventArgs e)
        {
            OpenUrl("https://gitee.com/corallite/PaddleOCRApi");
        }

        private void toolStripMenuItemChangelog_Click(object sender, EventArgs e)
        {
            OpenUrl("https://gitee.com/corallite/PaddleOCRApi/blob/main/docs/CHANGELOG.md");
        }

        private void toolStripMenuItemCppInterface_Click(object sender, EventArgs e)
        {
            OpenUrl("https://gitee.com/corallite/PaddleOCRApi/blob/main/docs/PaddleOCR-Interface.md");
        }

        private void toolStripMenuItemFaq_Click(object sender, EventArgs e)
        {
            OpenUrl("https://gitee.com/corallite/PaddleOCRApi/blob/main/docs/FAQ.md");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开链接：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonGetBase64_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                string filePath = openFileDialog.FileName;
                string base64 = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ImageTools.GetBase64FromImage(filePath);
                }, cancellationToken);
                textBoxResult.Text = base64;
            }
            catch (OperationCanceledException)
            {
                LogMessage("获取图片Base64已取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"图片格式异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async void buttonPostFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxApiAddress.Text.Trim()))
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:WebApi地址不能为空！");
                return;
            }

            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                string filePath = openFileDialog.FileName;
                string apiAddress = textBoxApiAddress.Text.Trim();
                textBoxResult.Text = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return HttpHelper.PostFile(apiAddress, filePath);
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogMessage("API接口测试已取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"调用接口异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private void buttonCancelOCR_Click(object sender, EventArgs e)
        {
            ocrOperationCts?.Cancel();
            LogMessage("已请求取消，当前 DLL/网络调用返回后会停止后续任务");
        }

        private void chkUseGpu_CheckedChanged(object sender, EventArgs e)
        {
            use_gpu = chkUseGpu.Checked;
            chkUseTensorRT.Enabled = chkUseGpu.Checked && chkUseGpu.Enabled;
            if (use_gpu)
            {
                LogMessage("使用GPU需要有GPU版本的Paddle推理库，请确认您已配置环境并已取得License授权");
                LogMessage("申请GPU授权文件paddleocr.lic请放到models目录。");
                //StringBuilder sb = new StringBuilder();
                //sb.Append($"{DateTime.Now:HH:mm:ss.fff}:使用GPU时请下载对应的paddle_inference解压" + Environment.NewLine);
                //sb.Append("解压后将以下dll文件复制到程序运行文件夹中：" + Environment.NewLine);
                //sb.Append("paddle\\lib目录下的common.dll和paddle_inference.dll" + Environment.NewLine);
                //sb.Append("third_party\\install\\mkldnn\\lib目录下的mkldnn.dll" + Environment.NewLine);
                //sb.Append("third_party\\install\\mklml\\lib目录下的libiomp5md.dll和mklml.dll" + Environment.NewLine);
                //sb.Append("安装指定版本的CUDA以及CUDNN" + Environment.NewLine);
                //sb.Append("复制对应的CUDA/CUDNN DLL到程序运行文件夹中" + Environment.NewLine);
                //LogMessage(sb.ToString());
            }
        }

        private void chkJson_CheckedChanged(object sender, EventArgs e)
        {
            outPutJson = chkJson.Checked;
        }

        private void chkUseTensorRT_CheckedChanged(object sender, EventArgs e)
        {
            use_tensorrt = chkUseTensorRT.Checked;
        }

        private void numDowngpu_id_ValueChanged(object sender, EventArgs e)
        {
            if (numDowngpu_id.Value >= 0)
            {
                gpu_id = Convert.ToInt32(numDowngpu_id.Value);
            }
        }

        private void numDowncpu_threads_ValueChanged(object sender, EventArgs e)
        {
            if (numDowncpu_threads.Value > 0)
            {
                cpu_threads = Convert.ToInt32(numDowncpu_threads.Value);
            }
        }

        private void numericUpDowncpu_mem_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDowncpu_mem.Value > 0)
            {
                cpu_mem = Convert.ToInt32(numericUpDowncpu_mem.Value);
            }
        }

        private void numericUpDownThread_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownThread.Value > 0)
            {
                recCount = Convert.ToInt32(numericUpDownThread.Value);
            }
        }

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            model_type = comboBoxModel.SelectedIndex;
        }

        private void comboBoxLayoutModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            layout_type = comboBoxLayoutModel.SelectedIndex;
        }

        private void buttonFreeEngine_Click(object sender, EventArgs e)
        {
            if (isOCRBusy)
            {
                buttonCancelOCR_Click(sender, e);
                return;
            }

            OCREngine.FreeEngine();
            isOCRTextReady = false;
            isOCRStructureReady = false;
            isInitSuccess = false;
            SetOCRBusy(false);
            LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR引擎已释放！");
        }
        #endregion

        #region PaddleOCR-VL
        private void comboBoxPrompt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPrompt.SelectedItem != null)
            {
                textBoxOCRVLPrompt.Text = comboBoxPrompt.SelectedItem.ToString();
            }
        }
        private void InitializeOCRVLDefaults()
        {
            comboBoxVLConfig.SelectedIndex = 0;
            textBoxOCRVLPrompt.Text = "OCR:";
            checkBoxOCRVLDocAnalysis.Checked = false;
            UpdateOCRVLPromptState();
            UpdateOCRVLStatus("就绪");
        }

        private void UpdateOCRVLPromptState()
        {
            bool enabled = !isOCRVLBusy && !checkBoxOCRVLDocAnalysis.Checked;
            textBoxOCRVLPrompt.Enabled = enabled;
            comboBoxPrompt.Enabled = enabled;
        }

        private void UpdateOCRVLStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateOCRVLStatus), message);
                return;
            }

            labelOCRVLStatus.Text = $"状态: {message}";
        }

        private void LogOCRVLMessage(string infoValue)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(LogOCRVLMessage), infoValue);
                return;
            }

            textBoxOCRVLResult.AppendText(infoValue);
            textBoxOCRVLResult.AppendText(Environment.NewLine);
            textBoxOCRVLResult.SelectionStart = textBoxOCRVLResult.Text.Length;
            textBoxOCRVLResult.ScrollToCaret();
        }

        private void SetOCRVLParameterControlsEnabled(bool enabled)
        {
            textBoxOCRVLConfigPath.Enabled = enabled;
            buttonOCRVLBrowseConfig.Enabled = enabled;
            checkBoxOCRVLDocAnalysis.Enabled = enabled;
            UpdateOCRVLPromptState();
        }

        private void SetOCRVLBusy(bool busy)
        {
            isOCRVLBusy = busy;
            buttonOCRVLCancel.Enabled = busy;
            buttonOCRVLInit.Enabled = !busy && !isOCRVLInitSuccess;
            buttonOCRVLFreeEngine.Enabled = !busy && isOCRVLInitSuccess;
            buttonOCRVLRec.Enabled = !busy && isOCRVLInitSuccess;
            buttonOCRVLRecPDF.Enabled = !busy && isOCRVLInitSuccess;
            SetOCRVLParameterControlsEnabled(!busy && !isOCRVLInitSuccess);
        }

        private CancellationToken BeginOCRVLOperation()
        {
            ocrVlOperationCts?.Dispose();
            ocrVlOperationCts = new CancellationTokenSource();
            SetOCRVLBusy(true);
            return ocrVlOperationCts.Token;
        }

        private void EndOCRVLOperation()
        {
            SetOCRVLBusy(false);
            ocrVlOperationCts?.Dispose();
            ocrVlOperationCts = null;
        }

        private string GetOCRVLPrompt()
        {
            string prompt = textBoxOCRVLPrompt.Text.Trim();
            return string.IsNullOrWhiteSpace(prompt) ? "OCR:" : prompt;
        }

        private static string NormalizeOCRVLDisplayText(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string normalized = text;

            for (int i = 0; i < 3; i++)
            {
                string replaced = normalized
                    .Replace("\\u000D\\u000A", "\n")
                    .Replace("\\u000d\\u000a", "\n")
                    .Replace("\\u000A", "\n")
                    .Replace("\\u000a", "\n")
                    .Replace("\\u000D", "\n")
                    .Replace("\\u000d", "\n")
                    .Replace("\\r\\n", "\n")
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\n")
                    .Replace("\\t", "\t");

                if (replaced == normalized)
                {
                    break;
                }

                normalized = replaced;
            }

            normalized = Regex.Replace(
                normalized,
                @"\\u([0-9a-fA-F]{4})",
                match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());

            return normalized
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }

        private void ShowOCRVLPreview(string inputImagePath)
        {
            if (File.Exists(inputImagePath))
            {
                pictureBoxOCRVL.ImgPath = inputImagePath;
            }
        }

        private async Task<string> RecOCRVLAsync(string filePath, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            string prompt = GetOCRVLPrompt();
            var startTime = DateTime.Now;
            LogOCRVLMessage($"Image: {Path.GetFileName(filePath)}");
            LogOCRVLMessage("正在识别，请稍后...");
            LogOCRVLMessage($"开始时间: {startTime:HH:mm:ss.fff}");

            if (isOCRVLLayoutAnalysis)
            {
                string layoutJson = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ocrvlService.DetectLayout(filePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                LayoutDetectResult layoutResult = ocrvlService.ParseLayoutResult(layoutJson);
                LogOCRVLMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
                LogOCRVLMessage($"用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                LogOCRVLMessage("识别结果：");

                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(layoutResult.Markdown))
                {
                    builder.AppendLine("Markdown:");
                    builder.AppendLine(NormalizeOCRVLDisplayText(layoutResult.Markdown).Trim());
                }

                if (!string.IsNullOrWhiteSpace(layoutJson))
                {
                    if (builder.Length > 0)
                    {
                        builder.AppendLine();
                    }

                    builder.AppendLine("JSON:");
                    builder.AppendLine(FormatJsonSafe(layoutJson));
                }

                string result = builder.ToString().Trim();
                LogOCRVLMessage(result);
                LogOCRVLMessage("===============================================");
                string? resultPath = ResolveExistingImagePath(layoutResult.VisPath);
                if (!string.IsNullOrEmpty(resultPath))
                {
                    pictureBoxOCRVL.ImgPath = resultPath;
                }
                else if (File.Exists(filePath))
                {
                    pictureBoxOCRVL.ImgPath = filePath;
                }
                return result;
            }

            VLChatResult chatResult = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ocrvlService.Chat(prompt, filePath);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            LogOCRVLMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
            LogOCRVLMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogOCRVLMessage("识别结果：");

            string chatText = chatResult.Code == 1
                ? NormalizeOCRVLDisplayText(chatResult.Content)
                : NormalizeOCRVLDisplayText(chatResult.ErrorMsg);
            LogOCRVLMessage(chatText);
            LogOCRVLMessage("===============================================");
            ShowOCRVLPreview(filePath);
            return chatText;
        }

        private string ConvertPdfFirstPageToImage(string pdfFilePath, string suffix, CancellationToken cancellationToken)
        {
            SKBitmap? bitmap = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                using (MemoryStream memoryStream = new MemoryStream(pdfBytes))
                {
                    bitmap = PDFtoImage.Conversion.ToImage(memoryStream);
                }

                cancellationToken.ThrowIfCancellationRequested();
                byte[] imageBytes;
                using (SKImage image = SKImage.FromBitmap(bitmap))
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    imageBytes = data.ToArray();
                }

                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                Directory.CreateDirectory(uploadsPath);

                string imagePath = Path.Combine(
                    uploadsPath,
                    Path.GetFileNameWithoutExtension(pdfFilePath) + suffix + "_page1.png");

                File.WriteAllBytes(imagePath, imageBytes);
                return imagePath;
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        private void buttonOCRVLBrowseConfig_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "配置文件(*.yaml;*.yml;*.json)|*.yaml;*.yml;*.json|所有文件|*.*";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxOCRVLConfigPath.Text = openFileDialog.FileName;
            }
        }

        private void checkBoxOCRVLDocAnalysis_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateOCRVLPromptState();
            if (checkBoxOCRVLDocAnalysis.Checked)
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:CPU模式不建议使用版面分析，可使用小图体验");
            UpdateOCRVLStatus(checkBoxOCRVLDocAnalysis.Checked
                ? "已启用版面分析，识别后优先预览 output/xxx.png"
                : "已切换为普通 OCR-VL 模式");
        }

        private void comboBoxVLConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ConfigPath = string.Empty;
            switch (comboBoxVLConfig.SelectedIndex)
            {
                case 0:
                    ConfigPath = Path.Combine(Application.StartupPath, "configs", "PaddleOCR-VL-1.5.yaml");
                    break;
                case 1:
                    ConfigPath = Path.Combine(Application.StartupPath, "configs", "PaddleOCR-VL-1.6.yaml");
                    break;
                case 2:
                    ConfigPath = Path.Combine(Application.StartupPath, "configs", "Qwen3VL-4B.yaml");
                    break;
                case 3:
                    ConfigPath = Path.Combine(Application.StartupPath, "configs", "FireRed-OCR.yaml");
                    break;
                default:
                    ConfigPath = Path.Combine(Application.StartupPath, "configs", "PaddleOCR-VL-1.5.yaml");
                    break;
            }
            if (!File.Exists(ConfigPath))
            {
                LogOCRVLMessage($"配置文件不存在,请先下载：{ConfigPath}");
                return;
            }
            textBoxOCRVLConfigPath.Text = ConfigPath;
        }

        private async void buttonOCRVLInit_Click(object? sender, EventArgs e)
        {
            string configPath = textBoxOCRVLConfigPath.Text.Trim();
            if (string.IsNullOrWhiteSpace(configPath))
            {
                UpdateOCRVLStatus("配置文件不能为空");
                MessageBox.Show("请先选择 YAML 配置文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(configPath))
            {
                UpdateOCRVLStatus("配置文件不存在");
                MessageBox.Show($"配置文件不存在：{configPath}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CancellationToken cancellationToken = BeginOCRVLOperation();
            try
            {
                textBoxOCRVLResult.Clear();
                isOCRVLLayoutAnalysis = checkBoxOCRVLDocAnalysis.Checked;
                UpdateOCRVLStatus("正在初始化 OCR-VL 引擎...");

                await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string licensePath = OCREngine.ResolveLicensePath();
                    if (!string.IsNullOrWhiteSpace(licensePath) && File.Exists(licensePath))
                    {
                        ocrvlService.ActivateLicense(licensePath);
                    }

                    if (isOCRVLLayoutAnalysis)
                    {
                        ocrvlService.InitDoc(configPath);
                    }
                    else
                    {
                        ocrvlService.Init(configPath);
                    }
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                isOCRVLInitSuccess = true;
                string mode = isOCRVLLayoutAnalysis ? "版面分析模式(InitDoc)" : "普通模式(Init)";
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化成功，当前为{mode}");
                UpdateOCRVLStatus($"初始化成功，当前为{mode}");
            }
            catch (OperationCanceledException)
            {
                try
                {
                    ocrvlService.FreeDocAnalyser();
                    ocrvlService.FreeEngine();
                }
                catch
                {
                }

                isOCRVLInitSuccess = false;
                UpdateOCRVLStatus("初始化已取消");
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化已取消");
            }
            catch (Exception ex)
            {
                isOCRVLInitSuccess = false;
                UpdateOCRVLStatus($"初始化失败: {ex.Message}");
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化失败: {ex.Message}");
            }
            finally
            {
                EndOCRVLOperation();
            }
        }

        private async void buttonOCRVLRec_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|所有文件|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCRVLOperation();
            try
            {
                textBoxOCRVLResult.Clear();
                foreach (var filePath in openFileDialog.FileNames)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await RecOCRVLAsync(Path.GetFullPath(filePath), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                LogOCRVLMessage("OCR-VL 任务已取消");
                UpdateOCRVLStatus("任务已取消");
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"图片识别异常: {ex.Message}");
                UpdateOCRVLStatus($"图片识别异常: {ex.Message}");
            }
            finally
            {
                EndOCRVLOperation();
            }
        }

        private async void buttonOCRVLRecPDF_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCRVLOperation();
            try
            {
                textBoxOCRVLResult.Clear();
                string pdfPath = Path.GetFullPath(openFileDialog.FileName);
                LogOCRVLMessage($"正在处理PDF文件: {pdfPath}");
                string imagePath = await Task.Run(() => ConvertPdfFirstPageToImage(pdfPath, "_ocrvl", cancellationToken), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                LogOCRVLMessage($"已保存转换后的图片到uploads: {imagePath}");
                await RecOCRVLAsync(imagePath, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogOCRVLMessage("PDF识别已取消");
                UpdateOCRVLStatus("PDF识别已取消");
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"PDF识别异常: {ex.Message}");
                UpdateOCRVLStatus($"PDF识别异常: {ex.Message}");
            }
            finally
            {
                EndOCRVLOperation();
            }
        }

        private void buttonOCRVLCancel_Click(object? sender, EventArgs e)
        {
            ocrVlOperationCts?.Cancel();
            UpdateOCRVLStatus("已请求取消，当前 DLL 调用返回后会停止后续任务");
        }

        private void buttonOCRVLFreeEngine_Click(object? sender, EventArgs e)
        {
            try
            {
                if (isOCRVLBusy)
                {
                    buttonOCRVLCancel_Click(sender, e);
                    return;
                }

                ocrvlService.FreeDocAnalyser();
                ocrvlService.FreeEngine();
                isOCRVLInitSuccess = false;
                isOCRVLLayoutAnalysis = false;
                SetOCRVLBusy(false);

                textBoxOCRVLResult.Clear();
                pictureBoxOCRVL.Image?.Dispose();
                pictureBoxOCRVL.Image = null;

                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 引擎已释放！");
                UpdateOCRVLStatus("引擎已释放");
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"释放 OCR-VL 引擎失败: {ex.Message}");
                UpdateOCRVLStatus($"释放失败: {ex.Message}");
            }
        }
        #endregion

        #region Paddle-UVDoc
        private void InitializeUVDocParameters()
        {
            chkUVDocUseGpu.Checked = false;
            numUVDocCpuThreads.Value = Environment.ProcessorCount;
            numUVDocGpuId.Value = 0;
            numUVDocGpuMem.Value = 2000;
            chkUVDocUseTensorRT.Checked = false;
        }

        private CancellationToken BeginUVDocOperation()
        {
            uvDocOperationCts?.Dispose();
            uvDocOperationCts = new CancellationTokenSource();
            SetUVDocBusy(true);
            return uvDocOperationCts.Token;
        }

        private void EndUVDocOperation()
        {
            SetUVDocBusy(false);
            uvDocOperationCts?.Dispose();
            uvDocOperationCts = null;
        }

        private void SetUVDocBusy(bool busy)
        {
            isUVDocBusy = busy;
            bool initialized = uvdocService?.IsInitialized == true;

            btnUVDocCancel.Enabled = busy;
            btnUVDocInitialize.Enabled = !busy && !initialized;
            btnUVDocUpload.Enabled = !busy && initialized;
            btnUVDocProcess.Enabled = !busy && initialized && !string.IsNullOrEmpty(currentImagePath);
            btnUVDocSave.Enabled = !busy && !string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath);
            btnUVDocFreeEngine.Enabled = !busy && initialized;
            SetUVDocParameterControlsEnabled(!busy && !initialized);
        }

        private async void btnUVDocInitialize_Click(object sender, EventArgs e)
        {
            CancellationToken cancellationToken = BeginUVDocOperation();
            try
            {
                await InitializeUVDocEngineAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                uvdocService?.FreeUVDocEngine();
                uvdocService = null;
                UpdateUVDocStatus("初始化已取消");
            }
            catch (Exception ex)
            {
                UpdateUVDocStatus($"初始化异常: {ex.Message}");
                MessageBox.Show($"初始化异常: {ex.Message}\n\n请确保 PaddleOCR.dll 及其依赖库在程序目录中。",
                    "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndUVDocOperation();
            }
        }

        private async Task InitializeUVDocEngineAsync(CancellationToken cancellationToken)
        {
            uvdocService?.FreeUVDocEngine();
            uvdocService = new UVDocService();
            var parameter = new UVDocParameter
            {
                enable_mkldnn = true,
                cpu_threads = (int)numUVDocCpuThreads.Value,
                use_gpu = chkUVDocUseGpu.Checked,
                gpu_id = (int)numUVDocGpuId.Value,
                gpu_mem = (int)numUVDocGpuMem.Value,
                use_tensorrt = chkUVDocUseTensorRT.Checked
            };

            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "UVDoc_infer");
            if (!Directory.Exists(modelPath))
            {
                modelPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "models", "UVDoc_infer"));
            }

            UpdateUVDocStatus($"正在初始化模型: {modelPath}");
            bool initialized = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return uvdocService.Initialize(modelPath, parameter);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (initialized)
            {
                UpdateUVDocStatus($"模型初始化成功！[GPU: {(parameter.use_gpu ? "是" : "否")}, CPU线程: {parameter.cpu_threads}]");
                return;
            }

            string error = uvdocService.GetLastError();
            uvdocService.FreeUVDocEngine();
            uvdocService = null;
            UpdateUVDocStatus($"模型初始化失败: {error}");
            MessageBox.Show($"模型初始化失败！\n错误信息: {error}\n\n请确保 models/UVDoc_infer 目录存在且包含正确的模型文件。",
                "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SetUVDocParameterControlsEnabled(bool enabled)
        {
            chkUVDocUseGpu.Enabled = enabled;
            numUVDocCpuThreads.Enabled = enabled;
            numUVDocGpuId.Enabled = enabled && chkUVDocUseGpu.Checked;
            numUVDocGpuMem.Enabled = enabled && chkUVDocUseGpu.Checked;
            chkUVDocUseTensorRT.Enabled = enabled && chkUVDocUseGpu.Checked;
        }

        private void chkUVDocUseGpu_CheckedChanged(object sender, EventArgs e)
        {
            numUVDocGpuId.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
            numUVDocGpuMem.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
            chkUVDocUseTensorRT.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
        }

        private void btnUVDocUpload_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|所有文件|*.*";
            openFileDialog.Title = "选择要矫正的图像";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                currentImagePath = openFileDialog.FileName;
                SetPictureBoxImage(pictureBoxOriginal, currentImagePath);
                SetPictureBoxImage(pictureBoxOutput, null);

                outputImagePath = null;
                SetUVDocBusy(false);
                UpdateUVDocStatus($"已加载图像: {Path.GetFileName(currentImagePath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"加载图像失败: {ex.Message}");
            }
        }

        private async void btnUVDocProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath) || uvdocService == null)
            {
                MessageBox.Show("请先上传图像并确保引擎已初始化", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CancellationToken cancellationToken = BeginUVDocOperation();
            try
            {
                UpdateUVDocStatus("正在处理图像，请稍候...");

                string tempDir = Path.Combine(Path.GetTempPath(), "PaddleDocVision");
                Directory.CreateDirectory(tempDir);
                outputImagePath = Path.Combine(tempDir, $"output_{DateTime.Now:yyyyMMddHHmmss}.jpg");

                Stopwatch stopwatch = Stopwatch.StartNew();
                await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    uvdocService.UVDocImageFile(currentImagePath, outputImagePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                stopwatch.Stop();

                if (File.Exists(outputImagePath))
                {
                    SetPictureBoxImage(pictureBoxOutput, outputImagePath);
                    UpdateUVDocStatus($"图像处理完成！耗时: {stopwatch.Elapsed.TotalMilliseconds:F0} ms");
                }
                else
                {
                    UpdateUVDocStatus($"处理失败：输出文件未生成（耗时: {stopwatch.Elapsed.TotalMilliseconds:F0} ms）");
                    MessageBox.Show("处理失败：输出文件未生成", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (OperationCanceledException)
            {
                UpdateUVDocStatus("图像处理已取消");
            }
            catch (Exception ex)
            {
                string error = uvdocService?.GetLastError() ?? "";
                string message = $"处理图像时出错: {ex.Message}";
                if (!string.IsNullOrEmpty(error))
                {
                    message += $"\nDLL错误信息: {error}";
                }

                MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"处理失败: {ex.Message}");
            }
            finally
            {
                EndUVDocOperation();
            }
        }

        private void btnUVDocCancel_Click(object sender, EventArgs e)
        {
            uvDocOperationCts?.Cancel();
            UpdateUVDocStatus("已请求取消，当前 DLL 调用返回后会停止后续任务");
        }

        private void btnUVDocSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputImagePath) || !File.Exists(outputImagePath))
            {
                MessageBox.Show("没有可保存的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG图像|*.jpg|PNG图像|*.png|所有文件|*.*";
            saveFileDialog.Title = "保存矫正后的图像";
            saveFileDialog.FileName = $"corrected_{DateTime.Now:yyyyMMddHHmmss}.jpg";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                File.Copy(outputImagePath, saveFileDialog.FileName, true);
                UpdateUVDocStatus($"图像已保存: {saveFileDialog.FileName}");
                MessageBox.Show("图像保存成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"保存失败: {ex.Message}");
            }
        }

        private void UpdateUVDocStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateUVDocStatus), message);
                return;
            }

            lblUVDocStatus.Text = $"状态: {message}";
        }

        private void btnUVDocFreeEngine_Click(object sender, EventArgs e)
        {
            try
            {
                if (isUVDocBusy)
                {
                    btnUVDocCancel_Click(sender, e);
                    return;
                }

                uvdocService?.FreeUVDocEngine();
                uvdocService = null;

                SetPictureBoxImage(pictureBoxOriginal, null);
                SetPictureBoxImage(pictureBoxOutput, null);

                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }

                outputImagePath = null;
                currentImagePath = null;
                SetUVDocBusy(false);
                UpdateUVDocStatus("引擎已释放！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"释放引擎失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"释放失败: {ex.Message}");
            }
        }

        private static void SetPictureBoxImage(PictureBox pictureBox, string? path)
        {
            Image? oldImage = pictureBox.Image;
            pictureBox.Image = string.IsNullOrEmpty(path) ? null : CloneImageFromFile(path);
            oldImage?.Dispose();
        }
        #endregion
    }
}
