using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRVLSDK
{
    internal static class MarshalUtf8
    {
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                return null;
            }

#if NET5_0_OR_GREATER
            return Marshal.PtrToStringUTF8(ptr);
#else
            int length = 0;
            while (Marshal.ReadByte(ptr, length) != 0)
            {
                length++;
            }

            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            return Encoding.UTF8.GetString(bytes);
#endif
        }
    }
}