using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{
    internal static class OCRVLSDK
    {
        internal const string DllFileName = "llamaocr-vl";
        internal const string OutputDelimiter = "\n---JSON---\n";

        internal static int Init(string configPath)
        {
            IntPtr configPathPtr = Utf8StringHandle.Alloc(configPath);
            try
            {
                return NativeInit(configPathPtr);
            }
            finally
            {
                Utf8StringHandle.Free(configPathPtr);
            }
        }

        internal static IntPtr Chat(string prompt, string imagePath)
        {
            IntPtr promptPtr = Utf8StringHandle.Alloc(prompt);
            IntPtr imagePathPtr = Utf8StringHandle.Alloc(imagePath);
            try
            {
                return NativeChat(promptPtr, imagePathPtr);
            }
            finally
            {
                Utf8StringHandle.Free(promptPtr);
                Utf8StringHandle.Free(imagePathPtr);
            }
        }

        internal static IntPtr ChatData(string prompt, byte[] imageData, UIntPtr imageSize)
        {
            IntPtr promptPtr = Utf8StringHandle.Alloc(prompt);
            try
            {
                return NativeChatData(promptPtr, imageData, imageSize);
            }
            finally
            {
                Utf8StringHandle.Free(promptPtr);
            }
        }

        internal static IntPtr ChatBase64(string prompt, string base64Image)
        {
            IntPtr promptPtr = Utf8StringHandle.Alloc(prompt);
            IntPtr base64Ptr = Utf8StringHandle.Alloc(base64Image);
            try
            {
                return NativeChatBase64(promptPtr, base64Ptr);
            }
            finally
            {
                Utf8StringHandle.Free(promptPtr);
                Utf8StringHandle.Free(base64Ptr);
            }
        }

        internal static IntPtr ChatMat(string prompt, IntPtr cvMat)
        {
            IntPtr promptPtr = Utf8StringHandle.Alloc(prompt);
            try
            {
                return NativeChatMat(promptPtr, cvMat);
            }
            finally
            {
                Utf8StringHandle.Free(promptPtr);
            }
        }

        [DllImport(DllFileName, EntryPoint = "FreeMemory", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void FreeMemory(IntPtr ptr);

        [DllImport(DllFileName, EntryPoint = "FreeEngine", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void FreeEngine();

        [DllImport(DllFileName, EntryPoint = "GetError", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr GetError();

        internal static int InitDoc(string configPath)
        {
            IntPtr configPathPtr = Utf8StringHandle.Alloc(configPath);
            try
            {
                return NativeInitDoc(configPathPtr);
            }
            finally
            {
                Utf8StringHandle.Free(configPathPtr);
            }
        }

        internal static IntPtr DocChat(string imagePath)
        {
            IntPtr imagePathPtr = Utf8StringHandle.Alloc(imagePath);
            try
            {
                return NativeDocChat(imagePathPtr);
            }
            finally
            {
                Utf8StringHandle.Free(imagePathPtr);
            }
        }

        [DllImport(DllFileName, EntryPoint = "DocChatData", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr DocChatData(
            byte[] imageData,
            UIntPtr imageSize);

        internal static IntPtr DocChatBase64(string base64Image)
        {
            IntPtr base64Ptr = Utf8StringHandle.Alloc(base64Image);
            try
            {
                return NativeDocChatBase64(base64Ptr);
            }
            finally
            {
                Utf8StringHandle.Free(base64Ptr);
            }
        }

        [DllImport(DllFileName, EntryPoint = "DocChatMat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern IntPtr DocChatMat(
            IntPtr cvMat);

        [DllImport(DllFileName, EntryPoint = "FreeDocAnalyser", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static extern void FreeDocAnalyser();

        [DllImport(DllFileName, EntryPoint = "Init", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int NativeInit(IntPtr configPath);

        [DllImport(DllFileName, EntryPoint = "Chat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NativeChat(IntPtr prompt, IntPtr imagePath);

        [DllImport(DllFileName, EntryPoint = "ChatData", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NativeChatData(IntPtr prompt, byte[] imageData, UIntPtr imageSize);

        [DllImport(DllFileName, EntryPoint = "ChatBase64", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NativeChatBase64(IntPtr prompt, IntPtr base64Image);

        [DllImport(DllFileName, EntryPoint = "ChatMat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NativeChatMat(IntPtr prompt, IntPtr cvMat);

        [DllImport(DllFileName, EntryPoint = "InitDoc", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern int NativeInitDoc(IntPtr configPath);

        [DllImport(DllFileName, EntryPoint = "DocChat", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NativeDocChat(IntPtr imagePath);

        [DllImport(DllFileName, EntryPoint = "DocChatBase64", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        private static extern IntPtr NativeDocChatBase64(IntPtr base64Image);

        private static class Utf8StringHandle
        {
            internal static IntPtr Alloc(string value)
            {
                if (value == null)
                {
                    return IntPtr.Zero;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(value + "\0");
                IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
                Marshal.Copy(bytes, 0, ptr, bytes.Length);
                return ptr;
            }

            internal static void Free(IntPtr ptr)
            {
                if (ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }
}