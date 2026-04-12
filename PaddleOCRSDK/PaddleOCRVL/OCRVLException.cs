using System;

namespace PaddleOCRSDK
{
    public class OCRVLException : Exception
    {
        public OCRVLException(string message) : base(message)
        {
        }
    }
}