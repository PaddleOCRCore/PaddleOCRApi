namespace PaddleOCRVLSDK.Models
{
    /// <summary>
    /// VL OCR 调用结果。
    /// </summary>
    public class VLChatResult
    {
        /// <summary>
        /// 调用状态，1 表示成功，0 表示失败。
        /// </summary>
        public int Code { get; set; } = 1;

        /// <summary>
        /// 错误信息。
        /// </summary>
        public string ErrorMsg { get; set; } = string.Empty;

        /// <summary>
        /// 返回的原始文本内容。
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}