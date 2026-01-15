// Copyright (c) 2026 PaddleOCRCore All Rights Reserved.
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
using System;
using System.Runtime.InteropServices;

namespace PaddleOCRSDK
{
    /// <summary>
    /// UVDoc文本图像矫正服务，调用PaddleOCR动态链接库（跨平台：Windows .dll / Linux .so）
    /// </summary>
    internal class UVDocSDK
    {
        internal const string dllFileName = "PaddleOCR";
        /// <summary>
        /// 获取错误提示
        /// </summary>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr GetError();
        /// <summary>
        /// 初始化文本图像矫正模块-传入参数结构体
        /// </summary>
        /// <param name="uvdoc_infer">UVDoc模型路径</param>
        /// <param name="uvdocpara">参数结构体</param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitUVDoc(string uvdoc_infer, UVDocParameter uvdocpara);
        /// <summary>
        /// 初始化文本图像矫正模块2
        /// </summary>
        /// <param name="uvdoc_infer">UVDoc模型路径</param>
        /// <param name="parjson">json参数</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitUVDocjson(string uvdoc_infer, string parjson);
        /// <summary>
        /// 文本图像矫正-传入Image路径
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <param name="filename">输出文件路径</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void UVDocImageFile(string filename, string outputfilepath);
        /// <summary>
        /// 文本图像矫正-传入Mat
        /// </summary>
        /// <param name="cvmat">Mat</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void UVDocMat(IntPtr cvmat);
        /// <summary>
        /// 文本图像矫正-传入Byte
        /// </summary>
        /// <param name="imagebyte">图片字节码</param>
        /// <param name="size">大小</param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void UVDocByte(byte[] imagebyte, long size, string outputfilepath);
        /// <summary>
        /// 文本图像矫正-传入图片Base64
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void UVDocBase64(string base64, string outputfilepath);
        /// <summary>
        /// 释放文本图像矫正实例
        /// </summary>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int FreeUVDocEngine();
    }
}
