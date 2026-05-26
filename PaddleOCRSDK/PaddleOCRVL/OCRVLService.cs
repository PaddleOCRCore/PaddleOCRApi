using System;
using PaddleOCRSDK.Models;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{
    /// <summary>
    /// llamaocr-vl.dll 的 .NET 服务封装。
    /// </summary>
    public class OCRVLService : IOCRVLService, IDisposable
    {
        private bool _ocrInitialized;
        private bool _documentInitialized;
        private readonly object _syncRoot = new object();
        private bool _disposed;

        /// <summary>
        /// 初始化 VL OCR 引擎。
        /// </summary>
        /// <param name="configPath">VL OCR 配置文件路径。</param>
        /// <returns>初始化成功返回 true。</returns>
        public bool Init(string configPath)
        {
            EnsureNotDisposed();
            ValidateRequiredString(configPath, nameof(configPath));

            lock (_syncRoot)
            {
                int result = InvokeWithUtf8(configPath, OCRVLSDK.Init);
                if (result != 1)
                {
                    throw new OCRVLException($"初始化 VL OCR 引擎失败: {GetLastError()}");
                }

                _ocrInitialized = true;
                return true;
            }
        }

        /// <summary>
        /// 初始化文档结构化分析引擎。
        /// </summary>
        /// <param name="configPath">文档结构化分析配置文件路径。</param>
        /// <returns>初始化成功返回 true。</returns>
        public bool InitDoc(string configPath)
        {
            EnsureNotDisposed();
            ValidateRequiredString(configPath, nameof(configPath));

            lock (_syncRoot)
            {
                int result = InvokeWithUtf8(configPath, OCRVLSDK.InitDoc);
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
            return OcrServiceHelper.ReadNativeString(OCRVLSDK.GetLicenseRequestCode, OCRVLSDK.FreeResultBuffer);
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

            return InvokeWithUtf8(licenseFile, OCRVLSDK.ActivateLicense);
        }

        /// <summary>
        /// 获取 PaddleOCR-VL 当前授权状态 JSON。
        /// </summary>
        /// <returns>授权状态 JSON 字符串</returns>
        public string GetLicenseStatus()
        {
            EnsureNotDisposed();
            return OcrServiceHelper.ReadNativeString(OCRVLSDK.GetLicenseStatus, OCRVLSDK.FreeResultBuffer);
        }

        /// <summary>
        /// 获取 PaddleOCR-VL 当前授权状态对象。
        /// </summary>
        /// <returns>授权状态对象</returns>
        public LicenseStatus GetLicenseStatusInfo()
        {
            return OcrServiceHelper.DeserializeLicenseStatus(GetLicenseStatus());
        }

        /// <summary>
        /// 根据提示词和图片路径执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">提示词。</param>
        /// <param name="imagePath">图片文件路径。</param>
        /// <returns>VL 对话识别结果。</returns>
        public VLChatResult Chat(string prompt, string imagePath)
        {
            EnsureOcrInitialized();
            ValidateRequiredString(prompt, nameof(prompt));
            ValidateRequiredString(imagePath, nameof(imagePath));

            return ExecuteChat(() => ChatNative(prompt, imagePath), "VL OCR 返回空结果");
        }

        /// <summary>
        /// 根据提示词和图片字节数据执行 VL 对话识别。
        /// </summary>
        /// <param name="imageData">图片字节数据。</param>
        /// <param name="prompt">提示词。</param>
        /// <returns>VL 对话识别结果。</returns>
        public VLChatResult ChatData(byte[] imageData, string prompt)
        {
            EnsureOcrInitialized();
            ValidateImageData(imageData, nameof(imageData));
            ValidateRequiredString(prompt, nameof(prompt));

            return ExecuteChat(() => ChatDataNative(prompt, imageData, new UIntPtr((ulong)imageData.LongLength)), "VL OCR 返回空结果");
        }

        /// <summary>
        /// 根据提示词和 Base64 图片数据执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">提示词。</param>
        /// <param name="base64Image">Base64 图片数据。</param>
        /// <returns>VL 对话识别结果。</returns>
        public VLChatResult ChatBase64(string prompt, string base64Image)
        {
            EnsureOcrInitialized();
            ValidateRequiredString(prompt, nameof(prompt));
            ValidateRequiredString(base64Image, nameof(base64Image));

            return ExecuteChat(() => ChatBase64Native(prompt, base64Image), "VL OCR 返回空结果");
        }

        /// <summary>
        /// 根据提示词和 OpenCV Mat 执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">提示词。</param>
        /// <param name="cvMat">OpenCV Mat 指针。</param>
        /// <returns>VL 对话识别结果。</returns>
        public VLChatResult ChatMat(string prompt, IntPtr cvMat)
        {
            EnsureOcrInitialized();
            ValidateRequiredString(prompt, nameof(prompt));
            if (cvMat == IntPtr.Zero)
            {
                throw new ArgumentException("cvMat 不能为空", nameof(cvMat));
            }

            return ExecuteChat(() => ChatMatNative(prompt, cvMat), "VL OCR 返回空结果");
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

            return ExecuteStructure(() => DocChatNative(imagefile));
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

            return ExecuteStructure(() => DocChatBase64Native(base64));
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
            return OcrServiceHelper.ParseLayoutResult(json, message => new OCRVLException(message));
        }

        /// <summary>
        /// 获取 PaddleOCR-VL 原生接口最后一次错误信息。
        /// </summary>
        /// <returns>错误信息。</returns>
        public string GetLastError()
        {
            EnsureNotDisposed();

            return OcrServiceHelper.GetLastError(OCRVLSDK.GetError, OCRVLSDK.FreeResultBuffer);
        }

        /// <summary>
        /// 释放 VL OCR 引擎。
        /// </summary>
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

        /// <summary>
        /// 释放文档结构化分析引擎。
        /// </summary>
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

        /// <summary>
        /// 释放 VL OCR 和文档结构化分析引擎资源。
        /// </summary>
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

                    string content = OcrServiceHelper.PtrToString(resultPtr);
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
                    return OcrServiceHelper.GetStructureResult(
                        resultPtr,
                        GetLastError,
                        OCRVLSDK.FreeResultBuffer,
                        ParseLayoutResult,
                        message => new OCRVLException(message));
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

        private static IntPtr ChatNative(string prompt, string imagePath)
        {
            return InvokeWithUtf8(prompt, imagePath, OCRVLSDK.Chat);
        }

        private static IntPtr ChatDataNative(string prompt, byte[] imageData, UIntPtr imageSize)
        {
            return InvokeWithUtf8(prompt, promptPtr => OCRVLSDK.ChatData(promptPtr, imageData, imageSize));
        }

        private static IntPtr ChatBase64Native(string prompt, string base64Image)
        {
            return InvokeWithUtf8(prompt, base64Image, OCRVLSDK.ChatBase64);
        }

        private static IntPtr ChatMatNative(string prompt, IntPtr cvMat)
        {
            return InvokeWithUtf8(prompt, promptPtr => OCRVLSDK.ChatMat(promptPtr, cvMat));
        }

        private static IntPtr DocChatNative(string imagePath)
        {
            return InvokeWithUtf8(imagePath, OCRVLSDK.DocChat);
        }

        private static IntPtr DocChatBase64Native(string base64Image)
        {
            return InvokeWithUtf8(base64Image, OCRVLSDK.DocChatBase64);
        }

        private static T InvokeWithUtf8<T>(string value, Func<IntPtr, T> invoker)
        {
            IntPtr valuePtr = AllocUtf8(value);
            try
            {
                return invoker(valuePtr);
            }
            finally
            {
                FreeUtf8(valuePtr);
            }
        }

        private static T InvokeWithUtf8<T>(string first, string second, Func<IntPtr, IntPtr, T> invoker)
        {
            IntPtr firstPtr = AllocUtf8(first);
            IntPtr secondPtr = AllocUtf8(second);
            try
            {
                return invoker(firstPtr, secondPtr);
            }
            finally
            {
                FreeUtf8(firstPtr);
                FreeUtf8(secondPtr);
            }
        }

        private static IntPtr AllocUtf8(string value)
        {
            if (value == null)
            {
                return IntPtr.Zero;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(value + "\0");
            IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
            Marshal.Copy(bytes, 0, ptr, bytes.Length);
            return ptr;
        }

        private static void FreeUtf8(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(ptr);
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
