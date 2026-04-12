using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{

    /// <summary>
    /// 跨版本 Marshal.UTF8 工具类
    /// .NET Framework 没有 PtrToStringUTF8，这里统一提供。
    /// </summary>
    public static class MarshalUtf8
    {
        /// <summary>
        /// 将 UTF-8 零结尾字节序列转换为托管字符串。
        /// 支持 .NET Framework 2.0+ / .NET Core 1.x+ / .NET 5+
        /// </summary>
        public static string PtrToStringUTF8(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero) return null;

            // .NET 5+ 原生支持 PtrToStringUTF8
#if NET5_0_OR_GREATER
        return Marshal.PtrToStringUTF8(ptr);
#else
            return PtrToStringUTF8_Manual(ptr);
#endif
        }

#if !NET5_0_OR_GREATER
        /// <summary>
        /// .NET Framework / Core 的手动实现
        /// </summary>
        private static string PtrToStringUTF8_Manual(IntPtr ptr)
        {
            // 1. 计算长度（到零字节为止）
            int len = 0;
            while (Marshal.ReadByte(ptr, len) != 0) len++;

            // 2. 复制到托管数组
            byte[] bytes = new byte[len];
            Marshal.Copy(ptr, bytes, 0, len);

            // 3. UTF-8 解码
            return Encoding.UTF8.GetString(bytes);
        }
#endif
    }
}