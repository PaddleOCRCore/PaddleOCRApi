namespace PaddleOCRSDK
{
    /// <summary>
    /// 文档分析结果。
    /// </summary>
    public class VLDocumentResult : VLChatResult
    {
        /// <summary>
        /// Markdown 内容。
        /// </summary>
        public string Markdown { get; set; } = string.Empty;

        /// <summary>
        /// JSON 内容。
        /// </summary>
        public string JsonText { get; set; } = string.Empty;
    }
}