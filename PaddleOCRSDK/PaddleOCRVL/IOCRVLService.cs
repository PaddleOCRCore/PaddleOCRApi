using System;
using PaddleOCRSDK.Models;

namespace PaddleOCRSDK
{
    public interface IOCRVLService
    {
        /// <summary>
        /// 初始化 VL OCR 引擎。
        /// </summary>
        /// <param name="configPath">VL OCR 配置文件路径。</param>
        /// <returns>初始化成功返回 true。</returns>
        bool Init(string configPath);

        /// <summary>
        /// 初始化文档结构化分析引擎。
        /// </summary>
        /// <param name="configPath">文档结构化分析配置文件路径。</param>
        /// <returns>初始化成功返回 true。</returns>
        bool InitDoc(string configPath);

        /// <summary>
        /// 获取 PaddleOCR-VL 当前机器的加密授权申请码。
        /// </summary>
        /// <returns>授权申请码</returns>
        string GetLicenseRequestCode();

        /// <summary>
        /// 激活 PaddleOCR-VL 授权文件。
        /// </summary>
        /// <param name="licenseFile">授权文件路径</param>
        /// <returns>激活成功返回 true，失败返回 false</returns>
        bool ActivateLicense(string licenseFile);

        /// <summary>
        /// 获取 PaddleOCR-VL 当前授权状态 JSON。
        /// </summary>
        /// <returns>授权状态 JSON 字符串</returns>
        string GetLicenseStatus();

        /// <summary>
        /// 获取 PaddleOCR-VL 当前授权状态对象。
        /// </summary>
        /// <returns>授权状态对象</returns>
        LicenseStatus GetLicenseStatusInfo();

        /// <summary>
        /// 根据提示词和图片路径执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">提示词。</param>
        /// <param name="imagePath">图片文件路径。</param>
        /// <returns>VL 对话识别结果。</returns>
        VLChatResult Chat(string prompt, string imagePath);

        /// <summary>
        /// 根据提示词和图片字节数据执行 VL 对话识别。
        /// </summary>
        /// <param name="imageData">图片字节数据。</param>
        /// <param name="prompt">提示词。</param>
        /// <returns>VL 对话识别结果。</returns>
        VLChatResult ChatData(byte[] imageData, string prompt);

        /// <summary>
        /// 根据提示词和 Base64 图片数据执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">提示词。</param>
        /// <param name="base64Image">Base64 图片数据。</param>
        /// <returns>VL 对话识别结果。</returns>
        VLChatResult ChatBase64(string prompt, string base64Image);

        /// <summary>
        /// 根据提示词和 OpenCV Mat 执行 VL 对话识别。
        /// </summary>
        /// <param name="prompt">提示词。</param>
        /// <param name="cvMat">OpenCV Mat 指针。</param>
        /// <returns>VL 对话识别结果。</returns>
        VLChatResult ChatMat(string prompt, IntPtr cvMat);

        /// <summary>
        /// 执行文档版面分析（包含版面检测、表格识别、公式识别等）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayout(string imagefile);

        /// <summary>
        /// 执行文档版面分析（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayoutByte(byte[] imagebyte);

        /// <summary>
        /// 执行文档版面分析（Base64 编码输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayoutBase64(string base64);

        /// <summary>
        /// 执行文档版面分析（OpenCV Mat 输入）
        /// </summary>
        /// <param name="ptr_cvmat">OpenCV Mat 指针</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayoutMat(IntPtr ptr_cvmat);

        /// <summary>
        /// 解析文档版面识别 JSON 结果为结构化对象（对应最新版 DLL 返回格式）
        /// </summary>
        /// <param name="json">DetectLayout 返回的 JSON 字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult ParseLayoutResult(string json);

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（文件路径输入）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult DetectLayoutParsed(string imagefile);

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult DetectLayoutByteParsed(byte[] imagebyte);

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（Base64 输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult DetectLayoutBase64Parsed(string base64);

        /// <summary>
        /// 获取 PaddleOCR-VL 原生接口最后一次错误信息。
        /// </summary>
        /// <returns>错误信息。</returns>
        string GetLastError();

        /// <summary>
        /// 释放 VL OCR 引擎。
        /// </summary>
        void FreeEngine();

        /// <summary>
        /// 释放文档结构化分析引擎。
        /// </summary>
        void FreeDocAnalyser();
    }
}
