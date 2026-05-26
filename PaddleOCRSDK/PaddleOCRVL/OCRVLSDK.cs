using System;
using System.Runtime.InteropServices;

namespace PaddleOCRSDK
{
    /// <summary>
    /// PaddleOCR-VL 原生 DLL 接口声明。
    /// </summary>
    internal static class OCRVLSDK
    {
        /// <summary>
        /// PaddleOCR-VL 原生 DLL 名称。
        /// </summary>
        internal const string DllFileName = "llamaocr-vl";

        /// <summary>
        /// 初始化 VL OCR 引擎。
        /// </summary>
        /// <param name="configPath">UTF-8 编码的配置文件路径指针。</param>
        /// <returns>初始化结果，1 表示成功。</returns>
        [DllImport(DllFileName, EntryPoint = "Init", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int Init(IntPtr configPath);

        /// <summary>
        /// 使用图片路径执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">UTF-8 编码的提示词指针。</param>
        /// <param name="imagePath">UTF-8 编码的图片路径指针。</param>
        /// <returns>识别结果字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "Chat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr Chat(IntPtr prompt, IntPtr imagePath);

        /// <summary>
        /// 使用图片字节数据执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">UTF-8 编码的提示词指针。</param>
        /// <param name="imageData">图片字节数据。</param>
        /// <param name="imageSize">图片字节数据长度。</param>
        /// <returns>识别结果字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "ChatData", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr ChatData(IntPtr prompt, byte[] imageData, UIntPtr imageSize);

        /// <summary>
        /// 使用 Base64 图片数据执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">UTF-8 编码的提示词指针。</param>
        /// <param name="base64Image">UTF-8 编码的 Base64 图片数据指针。</param>
        /// <returns>识别结果字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "ChatBase64", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr ChatBase64(IntPtr prompt, IntPtr base64Image);

        /// <summary>
        /// 使用 OpenCV Mat 执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">UTF-8 编码的提示词指针。</param>
        /// <param name="cvMat">OpenCV Mat 指针。</param>
        /// <returns>识别结果字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "ChatMat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr ChatMat(IntPtr prompt, IntPtr cvMat);

        /// <summary>
        /// 释放原生接口返回的结果字符串缓冲区。
        /// </summary>
        /// <param name="ptr">需要释放的结果指针。</param>
        [DllImport(DllFileName, EntryPoint = "FreeResultBuffer", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void FreeResultBuffer(IntPtr ptr);

        /// <summary>
        /// 释放 VL OCR 引擎。
        /// </summary>
        [DllImport(DllFileName, EntryPoint = "FreeEngine", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void FreeEngine();

        /// <summary>
        /// 获取原生接口最后一次错误信息。
        /// </summary>
        /// <returns>错误信息字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "GetError", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr GetError();

        /// <summary>
        /// 获取 PaddleOCR-VL 授权请求码。
        /// </summary>
        /// <returns>授权请求码字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "GetLicenseRequestCode", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr GetLicenseRequestCode();

        /// <summary>
        /// 激活 PaddleOCR-VL 授权。
        /// </summary>
        /// <param name="licenseFile">UTF-8 编码的授权文件路径指针。</param>
        /// <returns>激活是否成功。</returns>
        [DllImport(DllFileName, EntryPoint = "ActivateLicense", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern bool ActivateLicense(IntPtr licenseFile);

        /// <summary>
        /// 获取 PaddleOCR-VL 授权状态。
        /// </summary>
        /// <returns>授权状态 JSON 字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "GetLicenseStatus", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr GetLicenseStatus();

        /// <summary>
        /// 初始化文档结构化分析引擎。
        /// </summary>
        /// <param name="configPath">UTF-8 编码的配置文件路径指针。</param>
        /// <returns>初始化结果，1 表示成功。</returns>
        [DllImport(DllFileName, EntryPoint = "InitDoc", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern int InitDoc(IntPtr configPath);

        /// <summary>
        /// 使用图片路径执行文档结构化分析。
        /// </summary>
        /// <param name="imagePath">UTF-8 编码的图片路径指针。</param>
        /// <returns>版面分析 JSON 字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "DocChat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr DocChat(IntPtr imagePath);

        /// <summary>
        /// 使用图片字节数据执行文档结构化分析。
        /// </summary>
        /// <param name="imageData">图片字节数据。</param>
        /// <param name="imageSize">图片字节数据长度。</param>
        /// <returns>版面分析 JSON 字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "DocChatData", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr DocChatData(byte[] imageData, UIntPtr imageSize);

        /// <summary>
        /// 使用 Base64 图片数据执行文档结构化分析。
        /// </summary>
        /// <param name="base64Image">UTF-8 编码的 Base64 图片数据指针。</param>
        /// <returns>版面分析 JSON 字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "DocChatBase64", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr DocChatBase64(IntPtr base64Image);

        /// <summary>
        /// 使用 OpenCV Mat 执行文档结构化分析。
        /// </summary>
        /// <param name="cvMat">OpenCV Mat 指针。</param>
        /// <returns>版面分析 JSON 字符串指针，使用后需要调用 <see cref="FreeResultBuffer"/> 释放。</returns>
        [DllImport(DllFileName, EntryPoint = "DocChatMat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr DocChatMat(IntPtr cvMat);

        /// <summary>
        /// 释放文档结构化分析引擎。
        /// </summary>
        [DllImport(DllFileName, EntryPoint = "FreeDocAnalyser", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void FreeDocAnalyser();
    }
}
