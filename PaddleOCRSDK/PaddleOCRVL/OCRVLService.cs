using System;
using Newtonsoft.Json.Linq;

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

        public VLDocumentResult DocChat(string imagePath, PocrOutputFormat outputFormat = PocrOutputFormat.Both)
        {
            EnsureDocumentInitialized();
            ValidateRequiredString(imagePath, nameof(imagePath));

            return ExecuteDocument(() => OCRVLSDK.DocChat(imagePath), outputFormat);
        }

        public VLDocumentResult DocChatData(byte[] imageData, PocrOutputFormat outputFormat = PocrOutputFormat.Both)
        {
            EnsureDocumentInitialized();
            ValidateImageData(imageData, nameof(imageData));

            return ExecuteDocument(() => OCRVLSDK.DocChatData(imageData, new UIntPtr((uint)imageData.LongLength)), outputFormat);
        }

        public VLDocumentResult DocChatBase64(string base64Image, PocrOutputFormat outputFormat = PocrOutputFormat.Both)
        {
            EnsureDocumentInitialized();
            ValidateRequiredString(base64Image, nameof(base64Image));

            return ExecuteDocument(() => OCRVLSDK.DocChatBase64(base64Image), outputFormat);
        }

        public VLDocumentResult DocChatMat(IntPtr cvMat, PocrOutputFormat outputFormat = PocrOutputFormat.Both)
        {
            EnsureDocumentInitialized();
            if (cvMat == IntPtr.Zero)
            {
                throw new ArgumentException("cvMat 不能为空", nameof(cvMat));
            }

            return ExecuteDocument(() => OCRVLSDK.DocChatMat(cvMat), outputFormat);
        }

        public string GetLastError()
        {
            EnsureNotDisposed();

            try
            {
                IntPtr errorPtr = OCRVLSDK.GetError();
                return MarshalUtf8.PtrToStringUTF8(errorPtr) ?? string.Empty;
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
                        OCRVLSDK.FreeMemory(resultPtr);
                    }
                }
            }
        }

        private VLDocumentResult ExecuteDocument(Func<IntPtr> invoker, PocrOutputFormat outputFormat)
        {
            lock (_syncRoot)
            {
                IntPtr resultPtr = IntPtr.Zero;
                try
                {
                    resultPtr = invoker();
                    if (resultPtr == IntPtr.Zero)
                    {
                        return CreateDocumentErrorResult(GetLastError(), "文档分析调用失败");
                    }

                    string content = MarshalUtf8.PtrToStringUTF8(resultPtr);
                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return CreateDocumentErrorResult(GetLastError(), "文档分析返回空结果");
                    }

                    return BuildDocumentResult(content, outputFormat);
                }
                catch (Exception ex)
                {
                    return CreateDocumentErrorResult(ex.Message, "文档分析调用异常");
                }
                finally
                {
                    if (resultPtr != IntPtr.Zero)
                    {
                        OCRVLSDK.FreeMemory(resultPtr);
                    }
                }
            }
        }

        private static VLDocumentResult BuildDocumentResult(string content, PocrOutputFormat outputFormat)
        {
            var result = new VLDocumentResult
            {
                Content = content
            };

            // 新版接口返回JSON文本，markdown位于JSON字段中。
            string markdownFromJson = TryExtractMarkdown(content);

            if (outputFormat == PocrOutputFormat.Markdown)
            {
                result.Markdown = markdownFromJson;
                return result;
            }

            if (outputFormat == PocrOutputFormat.Json)
            {
                result.JsonText = content;
                return result;
            }

            string[] parts = content.Split(new[] { OCRVLSDK.OutputDelimiter }, 2, StringSplitOptions.None);
            if (parts.Length == 2)
            {
                result.Markdown = parts[0];
                result.JsonText = parts[1];
            }
            else
            {
                result.Markdown = markdownFromJson;
                result.JsonText = content;
            }

            return result;
        }

        private static string TryExtractMarkdown(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return string.Empty;
            }

            try
            {
                JToken root = JToken.Parse(content);
                string markdown = root.SelectToken("markdown")?.ToString();
                if (!string.IsNullOrWhiteSpace(markdown))
                {
                    return markdown;
                }

                markdown = root.SelectToken("data.markdown")?.ToString();
                if (!string.IsNullOrWhiteSpace(markdown))
                {
                    return markdown;
                }
            }
            catch
            {
                // 非JSON内容时，保持兼容，直接返回原文。
            }

            return content;
        }

        private static VLChatResult CreateChatErrorResult(string details, string message)
        {
            return new VLChatResult
            {
                Code = 0,
                ErrorMsg = CombineMessage(message, details)
            };
        }

        private static VLDocumentResult CreateDocumentErrorResult(string details, string message)
        {
            return new VLDocumentResult
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