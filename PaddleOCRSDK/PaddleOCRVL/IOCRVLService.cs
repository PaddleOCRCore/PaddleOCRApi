using System;
using PaddleOCRSDK.Models;

namespace PaddleOCRSDK
{
    public interface IOCRVLService
    {
        bool Init(string configPath);

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

        VLChatResult Chat(string prompt, string imagePath);

        VLChatResult ChatData(byte[] imageData, string prompt);

        VLChatResult ChatBase64(string prompt, string base64Image);

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

        string GetLastError();

        void FreeEngine();

        void FreeDocAnalyser();
    }
}
