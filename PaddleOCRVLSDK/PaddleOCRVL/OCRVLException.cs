using System;

namespace PaddleOCRVLSDK
{
    public class OCRVLException : Exception
    {
        public OCRVLException(string message) : base(message)
        {
        }
    }
}