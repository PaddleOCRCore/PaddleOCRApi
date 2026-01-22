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
using System;
using System.Runtime.InteropServices;

namespace PaddleOCRSDK
{
    /// <summary>
    /// 调用PaddlerOCR.dll动态链接库
    /// </summary>
    internal class OCRSDK
    {
        internal const string dllFileName = "PaddleOCR";

        /// <summary>
        /// 是否生成日志
        /// </summary>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void EnableLog(bool useLog);
        /// <summary>
        /// JSON输出是否使用ASCII编码，为true是返回Ascii编码
        /// </summary>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void EnableASCIIResult(bool useASCII);
        /// <summary>
        /// 是否使用json格式返回结果，默认true
        /// </summary>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void EnableJsonResult(bool enable);
        /// <summary>
        /// 获取错误提示
        /// </summary>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr GetError();
        /// <summary>
        /// 初始化OCR文字识别
        /// </summary>
        /// <param name="det_infer"></param>
        /// <param name="cls_infer"></param>
        /// <param name="rec_infer"></param>
        /// <param name="keyfile"></param>
        /// <param name="ocrpara"></param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool Init(string det_infer, string cls_infer, string rec_infer, OCRParameter ocrpara);
        /// <summary>
        /// 初始化OCR文字识别
        /// </summary>
        /// <param name="det_infer"></param>
        /// <param name="cls_infer"></param>
        /// <param name="rec_infer"></param>
        /// <param name="keyfile"></param>
        /// <param name="parjson">json参数</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool Initjson(string det_infer, string cls_infer, string rec_infer, string parjson);
        /// <summary>
        /// OCR识别
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr Detect(string filename);
        /// <summary>
        /// OCR识别Mat
        /// </summary>
        /// <param name="cvmat">Mat</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectMat(IntPtr cvmat);
        /// <summary>
        /// OCR文字识别
        /// </summary>
        /// <param name="imagebyte">图片字节码</param>
        /// <param name="size">大小</param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectByte(byte[] imagebyte, long size);
        /// <summary>
        /// OCR文字识别
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectBase64(string base64);
        /// <summary>
        /// 释放OCR实例
        /// </summary>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int FreeEngine();
        /// <summary>
        /// 初始化OCR表格识别
        /// </summary>
        /// <param name="det_infer"></param>
        /// <param name="cls_infer"></param>
        /// <param name="rec_infer"></param>
        /// <param name="table_model_dir"></param>
        /// <param name="tablepara"></param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitTable(string det_infer, string cls_infer, string rec_infer, string table_model_dir,TableParameter tablepara);
        /// <summary>
        /// 初始化OCR表格识别
        /// </summary>
        /// <param name="det_infer"></param>
        /// <param name="cls_infer"></param>
        /// <param name="rec_infer"></param>
        /// <param name="table_model_dir"></param>
        /// <param name="parjson">json参数</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitTablejson(string det_infer, string cls_infer, string rec_infer, string table_model_dir,  string parjson);
        /// <summary>
        /// OCR表格识别
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectTable(string filename);
        /// <summary>
        /// OCR表格识别
        /// </summary>
        /// <param name="imagebyte"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectTableByte(byte[] imagebyte, long size);
        /// <summary>
        /// OCR表格识别
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectTableBase64(string base64);
        /// <summary>
        /// 释放OCR表格识别实例
        /// </summary>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void FreeTableEngine();
        /// <summary>
        /// OCR识别动态修改参数
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool DynamicInit(SyncParameter parameter);
        /// <summary>
        /// 调用OCR结果释放函数
        /// </summary>
        /// <param name="buffer"></param>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern void FreeResultBuffer(IntPtr buffer);

        // ==================== 以图找图API ====================
        /// <summary>
        /// 以图找图：在大图中查找小图，返回JSON字符串
        /// </summary>
        /// <param name="bigImagePath">大图路径</param>
        /// <param name="smallImagePath">小图路径</param>
        /// <param name="threshold">匹配阈值 [0, 1]，默认0.8</param>
        /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
        /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认false，滑块找图请将threshold改为0.2左右</param>
        /// <returns>返回JSON格式的匹配结果</returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr FindImage(string bigImagePath, string smallImagePath, double threshold, bool toGray, bool useSlideMatch);
    }
}
