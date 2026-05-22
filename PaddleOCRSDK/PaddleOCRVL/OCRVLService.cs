using System;
using Newtonsoft.Json;
using PaddleOCRSDK.Models;

namespace PaddleOCRSDK
{
    /// <summary>
    /// llamaocr-vl.dll 的 .NET 服务封装。
    /// </summary>
    public class OCRVLService : IOCRVLService, IDisposable
    {
        private static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Include
        };

        private bool _ocrInitialized;
        private bool _documentInitialized;
        private readonly object _syncRoot = new object();
        private bool _disposed;

        public bool Init(string configPath)
        {
            EnsureNotDisposed();
            ValidateRequiredString(configPath, nameof(configPath));

            lock (_syncRoot)
            {
                int result = OCRVLSDK.Init(configPath);
                if (result != 1)
                {
                    throw new OCRVLException($"初始化 VL OCR 引擎失败: {GetLastError()}");
                }

                _ocrInitialized = true;
                return true;
            }
        }

        public bool InitDoc(string configPath)
        {
            EnsureNotDisposed();
            ValidateRequiredString(configPath, nameof(configPath));

            lock (_syncRoot)
            {
                int result = OCRVLSDK.InitDoc(configPath);
                if (result != 1)
                {
                    throw new OCRVLException($"初始化文档分析引擎失败: {GetLastError()}");
                }

                _documentInitialized = true;
                return true;
            }
        }

        /// <summary>
        /// 获取 PaddleOCR-VL 当前机器的加密授权申请码。
        /// </summary>
        /// <returns>授权申请码</returns>
        public string GetLicenseRequestCode()
        {
            EnsureNotDisposed();

            IntPtr ptrResult = IntPtr.Zero;
            try
            {
                ptrResult = OCRVLSDK.GetLicenseRequestCode();
                if (ptrResult == IntPtr.Zero)
                {
                    return string.Empty;
                }

                return MarshalUtf8.PtrToStringUTF8(ptrResult);
            }
            finally
            {
                if (ptrResult != IntPtr.Zero)
                {
                    OCRVLSDK.FreeResultBuffer(ptrResult);
                }
            }
        }

        /// <summary>
        /// 激活 PaddleOCR-VL 授权文件。
        /// </summary>
        /// <param name="licenseFile">授权文件路径</param>
        /// <returns>激活成功返回 true，失败返回 false</returns>
        public bool ActivateLicense(string licenseFile)
        {
            EnsureNotDisposed();
            if (string.IsNullOrWhiteSpace(licenseFile))
            {
                return false;
            }

            return OCRVLSDK.ActivateLicense(licenseFile);
        }

        /// <summary>
        /// 获取 PaddleOCR-VL 当前授权状态 JSON。
        /// </summary>
        /// <returns>授权状态 JSON 字符串</returns>
        public string GetLicenseStatus()
        {
            EnsureNotDisposed();

            IntPtr ptrResult = IntPtr.Zero;
            try
            {
                ptrResult = OCRVLSDK.GetLicenseStatus();
                if (ptrResult == IntPtr.Zero)
                {
                    return string.Empty;
                }

                return MarshalUtf8.PtrToStringUTF8(ptrResult);
            }
            finally
            {
                if (ptrResult != IntPtr.Zero)
                {
                    OCRVLSDK.FreeResultBuffer(ptrResult);
                }
            }
        }

        /// <summary>
        /// 获取 PaddleOCR-VL 当前授权状态对象。
        /// </summary>
        /// <returns>授权状态对象</returns>
        public LicenseStatus GetLicenseStatusInfo()
        {
            string json = GetLicenseStatus();
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<LicenseStatus>(json, JsonSettings);
        }

        public VLChatResult Chat(string prompt, string imagePath)
        {
            EnsureOcrInitialized();
            ValidateRequiredString(prompt, nameof(prompt));
            ValidateRequiredString(imagePath, nameof(imagePath));

            return ExecuteChat(() => OCRVLSDK.Chat(prompt, imagePath), "VL OCR 返回空结果");
        }

        public VLChatResult ChatData(byte[] imageData, string prompt)
        {
            EnsureOcrInitialized();
            ValidateImageData(imageData, nameof(imageData));
            ValidateRequiredString(prompt, nameof(prompt));

            return ExecuteChat(() => OCRVLSDK.ChatData(prompt, imageData, new UIntPtr((uint)imageData.LongLength)), "VL OCR 返回空结果");
        }

        public VLChatResult ChatBase64(string prompt, string base64Image)
        {
            EnsureOcrInitialized();
            ValidateRequiredString(prompt, nameof(prompt));
            ValidateRequiredString(base64Image, nameof(base64Image));

            return ExecuteChat(() => OCRVLSDK.ChatBase64(prompt, base64Image), "VL OCR 返回空结果");
        }

        public VLChatResult ChatMat(string prompt, IntPtr cvMat)
        {
            EnsureOcrInitialized();
            ValidateRequiredString(prompt, nameof(prompt));
            if (cvMat == IntPtr.Zero)
            {
                throw new ArgumentException("cvMat 不能为空", nameof(cvMat));
            }

            return ExecuteChat(() => OCRVLSDK.ChatMat(prompt, cvMat), "VL OCR 返回空结果");
        }

        /// <summary>
        /// 执行文档版面分析（包含版面检测、表格识别、公式识别等）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayout(string imagefile)
        {
            EnsureDocumentInitialized();
            ValidateRequiredString(imagefile, nameof(imagefile));

            return ExecuteStructure(() => OCRVLSDK.DocChat(imagefile));
        }

        /// <summary>
        /// 执行文档版面分析（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayoutByte(byte[] imagebyte)
        {
            EnsureDocumentInitialized();
            ValidateImageData(imagebyte, nameof(imagebyte));

            return ExecuteStructure(() => OCRVLSDK.DocChatData(imagebyte, new UIntPtr((ulong)imagebyte.LongLength)));
        }

        /// <summary>
        /// 执行文档版面分析（OpenCV Mat 输入）
        /// </summary>
        /// <param name="ptr_cvmat">OpenCV Mat 指针</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayoutMat(IntPtr ptr_cvmat)
        {
            EnsureDocumentInitialized();
            if (ptr_cvmat == IntPtr.Zero)
            {
                throw new ArgumentException("ptr_cvmat 不能为空", nameof(ptr_cvmat));
            }

            return ExecuteStructure(() => OCRVLSDK.DocChatMat(ptr_cvmat));
        }

        /// <summary>
        /// 执行文档版面分析（Base64 编码输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        public string DetectLayoutBase64(string base64)
        {
            EnsureDocumentInitialized();
            ValidateRequiredString(base64, nameof(base64));

            return ExecuteStructure(() => OCRVLSDK.DocChatBase64(base64));
        }

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（文件路径输入）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult DetectLayoutParsed(string imagefile)
        {
            var json = DetectLayout(imagefile);
            return ParseLayoutResult(json);
        }

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult DetectLayoutByteParsed(byte[] imagebyte)
        {
            var json = DetectLayoutByte(imagebyte);
            return ParseLayoutResult(json);
        }

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（Base64 输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult DetectLayoutBase64Parsed(string base64)
        {
            var json = DetectLayoutBase64(base64);
            return ParseLayoutResult(json);
        }

        /// <summary>
        /// 解析文档版面识别 JSON 结果为结构化对象
        /// 仅支持新版 DLL 输出结构
        /// </summary>
        /// <param name="json">DetectLayout 返回的 JSON 字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        public LayoutDetectResult ParseLayoutResult(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new OCRVLException("OCR版面识别结果为空");
            }

            try
            {
                var result = JsonConvert.DeserializeObject<LayoutDetectResult>(json, JsonSettings);
                if (result == null)
                {
                    throw new OCRVLException("OCR版面识别结果解析失败: JSON对象为空");
                }
                return result;
            }
            catch (JsonException ex)
            {
                throw new OCRVLException("OCR版面识别结果解析失败:" + ex.Message);
            }
        }

        public string GetLastError()
        {
            EnsureNotDisposed();

            try
            {
                IntPtr errorPtr = OCRVLSDK.GetError();
                try
                {
                    return MarshalUtf8.PtrToStringUTF8(errorPtr) ?? string.Empty;
                }
                finally
                {
                    if (errorPtr != IntPtr.Zero)
                    {
                        OCRVLSDK.FreeResultBuffer(errorPtr);
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public void FreeEngine()
        {
            if (_disposed)
            {
                return;
            }

            lock (_syncRoot)
            {
                OCRVLSDK.FreeEngine();
                _ocrInitialized = false;
            }
        }

        public void FreeDocAnalyser()
        {
            if (_disposed)
            {
                return;
            }

            lock (_syncRoot)
            {
                OCRVLSDK.FreeDocAnalyser();
                _documentInitialized = false;
            }
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            FreeDocAnalyser();
            FreeEngine();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private VLChatResult ExecuteChat(Func<IntPtr> invoker, string emptyResultMessage)
        {
            lock (_syncRoot)
            {
                IntPtr resultPtr = IntPtr.Zero;
                try
                {
                    resultPtr = invoker();
                    if (resultPtr == IntPtr.Zero)
                    {
                        return CreateChatErrorResult(GetLastError(), "VL OCR 调用失败");
                    }

                    string content = MarshalUtf8.PtrToStringUTF8(resultPtr);
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return CreateChatErrorResult(GetLastError(), emptyResultMessage);
                    }

                    return new VLChatResult
                    {
                        Content = content
                    };
                }
                catch (Exception ex)
                {
                    return CreateChatErrorResult(ex.Message, "VL OCR 调用异常");
                }
                finally
                {
                    if (resultPtr != IntPtr.Zero)
                    {
                        OCRVLSDK.FreeResultBuffer(resultPtr);
                    }
                }
            }
        }

        private string ExecuteStructure(Func<IntPtr> invoker)
        {
            lock (_syncRoot)
            {
                IntPtr resultPtr = IntPtr.Zero;
                try
                {
                    resultPtr = invoker();
                    return GetStructureResult(resultPtr);
                }
                catch (OCRVLException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new OCRVLException("OCR版面识别失败:" + ex.Message);
                }
            }
        }

        private string GetStructureResult(IntPtr ptrResult)
        {
            string result = string.Empty;
            if (ptrResult == IntPtr.Zero)
            {
                var lastErr = GetLastError();
                if (!string.IsNullOrEmpty(lastErr))
                {
                    throw new OCRVLException("OCR内部错误：" + lastErr);
                }
                return result;
            }
            try
            {
                result = MarshalUtf8.PtrToStringUTF8(ptrResult);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    // 与 OCRService 保持一致：版面结果至少应是可解析的JSON对象。
                    ParseLayoutResult(result);
                }
            }
            catch (Exception ex)
            {
                throw new OCRVLException("OCR版面识别失败:" + ex.Message);
            }
            finally
            {
                if (ptrResult != IntPtr.Zero)
                {
                    OCRVLSDK.FreeResultBuffer(ptrResult);
                }
            }
            return result;
        }

        private static VLChatResult CreateChatErrorResult(string details, string message)
        {
            return new VLChatResult
            {
                Code = 0,
                ErrorMsg = CombineMessage(message, details)
            };
        }

        private static string CombineMessage(string message, string details)
        {
            return string.IsNullOrWhiteSpace(details) ? message : message + ": " + details;
        }

        private static void ValidateRequiredString(string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("参数不能为空", paramName);
            }
        }

        private static void ValidateImageData(byte[] imageData, string paramName)
        {
            if (imageData == null || imageData.Length == 0)
            {
                throw new ArgumentException("图像数据不能为空", paramName);
            }
        }

        private void EnsureOcrInitialized()
        {
            EnsureNotDisposed();
            if (!_ocrInitialized)
            {
                throw new InvalidOperationException("VL OCR 引擎未初始化");
            }
        }

        private void EnsureDocumentInitialized()
        {
            EnsureNotDisposed();
            if (!_documentInitialized)
            {
                throw new InvalidOperationException("文档分析引擎未初始化");
            }
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(OCRVLService));
            }
        }
    }
}
