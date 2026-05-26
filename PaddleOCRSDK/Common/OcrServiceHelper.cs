using System;
using Newtonsoft.Json;
using PaddleOCRSDK.Models;

namespace PaddleOCRSDK
{
    internal static class OcrServiceHelper
    {
        internal static readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.None,
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            NullValueHandling = NullValueHandling.Include
        };

        internal static string ReadNativeString(Func<IntPtr> getResult, Action<IntPtr> freeResult)
        {
            IntPtr ptrResult = IntPtr.Zero;
            try
            {
                ptrResult = getResult();
                return PtrToString(ptrResult);
            }
            finally
            {
                FreeNativeBuffer(ptrResult, freeResult);
            }
        }

        internal static string PtrToString(IntPtr ptr)
        {
            return ptr == IntPtr.Zero ? string.Empty : MarshalUtf8.PtrToStringUTF8(ptr);
        }

        internal static string GetLastError(Func<IntPtr> getError, Action<IntPtr> freeResult)
        {
            try
            {
                return ReadNativeString(getError, freeResult);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        internal static LicenseStatus DeserializeLicenseStatus(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<LicenseStatus>(json, JsonSettings);
        }

        internal static LayoutDetectResult ParseLayoutResult(string json, Func<string, Exception> createException)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw createException("OCR识别结果为空");
            }

            try
            {
                LayoutDetectResult result = LayoutJsonHelper.DeserializeLayoutResult(json, JsonSettings);
                if (result == null)
                {
                    throw createException("OCR识别结果解析失败: JSON结果为空");
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw createException("OCR识别结果解析失败:" + ex.Message);
            }
        }

        internal static string GetStructureResult(
            IntPtr ptrResult,
            Func<string> getLastError,
            Action<IntPtr> freeResult,
            Func<string, LayoutDetectResult> parseLayoutResult,
            Func<string, Exception> createException)
        {
            if (ptrResult == IntPtr.Zero)
            {
                string lastErr = getLastError();
                if (!string.IsNullOrEmpty(lastErr))
                {
                    throw createException("OCR识别失败" + lastErr);
                }

                return string.Empty;
            }

            try
            {
                string result = PtrToString(ptrResult);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    parseLayoutResult(result);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw createException("OCR识别异常:" + ex.Message);
            }
            finally
            {
                FreeNativeBuffer(ptrResult, freeResult);
            }
        }

        internal static T DeserializeObject<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default(T);
            }

            return (T)JsonConvert.DeserializeObject(json, typeof(T), JsonSettings);
        }

        private static void FreeNativeBuffer(IntPtr ptr, Action<IntPtr> freeResult)
        {
            if (ptr != IntPtr.Zero)
            {
                freeResult(ptr);
            }
        }
    }
}
