// Copyright (c) 2025 PaddleOCRCore All Rights Reserved.
// https://github.com/PaddleOCRCore/PaddleOCRApi.git
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Drawing;
using System.Runtime.InteropServices;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace PaddleOCRSDK
{
    /// <summary>
    /// PaddleOCR识别引擎对象
    /// </summary>
    public abstract class CoreOCRBase 
    {
        /// <summary>
        /// PaddleOCR.dll自定义加载路径，默认为空，如果指定则需在引擎实例化前赋值。
        /// </summary>
        public static string PaddleOCRdllPath { get; set; }

        internal const string dllName = "PaddleOCR.dll";
        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadDll(String path);

        [DllImport(dllName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr GetError();

        /// <summary>
        /// 初始化
        /// </summary>
        public CoreOCRBase()
        {
            var temp = JsonHelper.DeObject<TextArea>("{}");
            try
            {
                if (string.IsNullOrEmpty(PaddleOCRdllPath))
                {
                    PaddleOCRdllPath = GetDllDirectory();
                }
                if (!string.IsNullOrEmpty(PaddleOCRdllPath))
                {
                    string Envpath = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Process);
                    if (!string.IsNullOrEmpty(Envpath))
                    {
                        Environment.SetEnvironmentVariable("path", Envpath + ";" + PaddleOCRdllPath, EnvironmentVariableTarget.Process);
                        LoadDll(System.IO.Path.Combine(PaddleOCRdllPath, dllName));
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("设置自定义加载路径失败。" + e.Message);
            }
        }
        #region private
        /// <summary>
        /// 获取程序的当前路径;
        /// </summary>
        /// <returns></returns>
        private static string GetDllDirectory()
        {
            string root = GetModelPath();
            var fileinfos = new DirectoryInfo(root).GetFiles(dllName, SearchOption.AllDirectories);
            if (fileinfos != null && fileinfos.Length > 0)
            {
                root = fileinfos.First().DirectoryName;
            }
            return root;
        }
        /// <summary>
        /// 获取程序的当前路径;
        /// </summary>
        /// <returns></returns>
        public static string GetModelPath()
        {
            string root = AppDomain.CurrentDomain.BaseDirectory;
#if NET46_OR_GREATER || NETCOREAPP
            root = AppContext.BaseDirectory;
#endif
            return root;
        }

        /// <summary>
        /// Convert Image to Byte[]
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        internal protected byte[] ImageToBytes(Image image)
        {
            ImageFormat format = image.RawFormat;
            using (MemoryStream ms = new MemoryStream())
            {
                if (format.Guid == ImageFormat.Jpeg.Guid)
                {
                    image.Save(ms, ImageFormat.Jpeg);
                }
                else if (format.Guid == ImageFormat.Png.Guid)
                {
                    image.Save(ms, ImageFormat.Png);
                }
                else if (format.Guid == ImageFormat.Bmp.Guid)
                {
                    image.Save(ms, ImageFormat.Bmp);
                }
                else if (format.Guid == ImageFormat.Gif.Guid)
                {
                    image.Save(ms, ImageFormat.Gif);
                }
                else if (format.Guid == ImageFormat.Icon.Guid)
                {
                    image.Save(ms, ImageFormat.Icon);
                }
                else
                {
                    image.Save(ms, ImageFormat.Png);
                }
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Read(buffer, 0, buffer.Length);
                return buffer;
            }
        }

        #endregion
        /// <summary>
        /// 释放内存
        /// </summary>
        public virtual void Dispose()
        {
        }
        /// <summary>
        /// 获取底层错误信息
        /// </summary>
        /// <returns></returns>
        public virtual string GetLastError()
        {
            string err = "";
            try
            {
                var errptr = GetError();
                if (errptr != IntPtr.Zero)
                {
                    err = Marshal.PtrToStringAnsi(errptr);
                    Marshal.FreeHGlobal(errptr);
                }
            }
            catch (Exception e)
            {
                err = e.Message;
            }
            return err;
        }

    }
}