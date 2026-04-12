using System;
using PaddleOCRVLSDK.Models;

namespace PaddleOCRVLSDK
{
    [Flags]
    public enum PocrOutputFormat
    {
        Markdown = 1,
        Json = 2,
        Both = Markdown | Json
    }

    public interface IOCRVLService
    {
        bool Init(string configPath);

        bool InitDoc(string configPath);

        VLChatResult Chat(string prompt, string imagePath);

        VLChatResult ChatData(byte[] imageData, string prompt);

        VLChatResult ChatBase64(string prompt, string base64Image);

        VLChatResult ChatMat(string prompt, IntPtr cvMat);

        VLDocumentResult DocChat(string imagePath, PocrOutputFormat outputFormat = PocrOutputFormat.Both);

        VLDocumentResult DocChatData(byte[] imageData, PocrOutputFormat outputFormat = PocrOutputFormat.Both);

        VLDocumentResult DocChatBase64(string base64Image, PocrOutputFormat outputFormat = PocrOutputFormat.Both);

        VLDocumentResult DocChatMat(IntPtr cvMat, PocrOutputFormat outputFormat = PocrOutputFormat.Both);

        string GetLastError();

        void FreeEngine();

        void FreeDocAnalyser();
    }
}