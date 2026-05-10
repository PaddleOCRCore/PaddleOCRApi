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

        #region OCR识别全局公共方法
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
        #endregion

        #region 通用OCR识别PP-OCRV4/PP-OCRV5
        /// <summary>
        /// 初始化OCR文字识别
        /// </summary>
        /// <param name="det_infer"></param>
        /// <param name="cls_infer"></param>
        /// <param name="rec_infer"></param>
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
        internal static extern IntPtr DetectByte(byte[] imagebyte, UIntPtr size);

        /// <summary>
        /// OCR文字识别
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectBase64(string base64);

        /// <summary>
        /// OCR文字识别（内存截图）
        /// </summary>
        /// <param name="data">截图字节数据</param>
        /// <param name="size">截图字节长度</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectScreenShot(byte[] data, int size);

        /// <summary>
        /// 释放OCR实例
        /// </summary>
        /// <returns></returns>

        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int FreeEngine();
        #endregion

        #region 版面结构识别PP-Structure
        // ==================== 扩展的版面结构识别API (支持20类文档元素) ====================
        // 包含：文档预处理、版面检测、全局OCR、条件识别(表格/公式/印章/图表)、结果融合、版面排序

        /// <summary>
        /// 初始化结构化文档识别引擎（扩展版本）
        /// 支持20类文档元素识别：版面检测、表格识别、公式识别、印章识别、图表转表等
        /// </summary>
        /// <param name="det_infer">文本检测模型路径</param>
        /// <param name="cls_infer">文本行方向分类模型路径(可选，NULL表示不使用)</param>
        /// <param name="rec_infer">文本识别模型路径</param>
        /// <param name="layout_model_dir">版面分析模型目录路径</param>
        /// <param name="table_model_dir">表格识别模型目录路径</param>
        /// <param name="formula_model_dir">公式识别模型路径(可选，NULL表示不使用)</param>
        /// <param name="seal_model_dir">印章识别模型路径(可选，NULL表示不使用)</param>
        /// <param name="doc_cls_infer">文档方向分类模型路径(可选，NULL表示不使用)</param>
        /// <param name="doc_unwarp_model">文档图像矫正模型路径(可选，NULL表示不使用)</param>
        /// <param name="region_model_dir">区域检测模型目录路径(可选，NULL表示不使用)</param>
        /// <param name="tablepara"></param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitStructure(
            string det_infer,
            string cls_infer,
            string rec_infer,
            string layout_model_dir,
            string table_model_dir,
            string formula_model_dir,
            string seal_model_dir,
            string doc_cls_infer,
            string doc_unwarp_model,
            string region_model_dir,
            LayoutParameter tablepara);
        // ==================== 扩展的版面结构识别API (支持20类文档元素) ====================
        // 包含：文档预处理、版面检测、全局OCR、条件识别(表格/公式/印章/图表)、结果融合、版面排序

        /// <summary>
        /// 初始化结构化文档识别引擎（扩展版本）
        /// 支持20类文档元素识别：版面检测、表格识别、公式识别、印章识别、图表转表等
        /// </summary>
        /// <param name="det_infer">文本检测模型路径</param>
        /// <param name="cls_infer">文本行方向分类模型路径(可选，NULL表示不使用)</param>
        /// <param name="rec_infer">文本识别模型路径</param>
        /// <param name="layout_model_dir">版面分析模型目录路径</param>
        /// <param name="table_model_dir">表格识别模型目录路径</param>
        /// <param name="formula_model_dir">公式识别模型路径(可选，NULL表示不使用)</param>
        /// <param name="seal_model_dir">印章识别模型路径(可选，NULL表示不使用)</param>
        /// <param name="doc_cls_infer">文档方向分类模型路径(可选，NULL表示不使用)</param>
        /// <param name="doc_unwarp_model">文档图像矫正模型路径(可选，NULL表示不使用)</param>
        /// <param name="region_model_dir">区域检测模型目录路径(可选，NULL表示不使用)</param>
        /// <param name="parjson">json参数</param>
        /// <returns></returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern bool InitStructurejson(
            string det_infer,
            string cls_infer,
            string rec_infer,
            string layout_model_dir,
            string table_model_dir,
            string formula_model_dir,
            string seal_model_dir,
            string doc_cls_infer,
            string doc_unwarp_model,
            string region_model_dir,
            string parjson);
        /// <summary>
        /// 执行文档版面分析（扩展版本）
        /// 包含：文档预处理→版面检测→全局OCR→条件识别(表格/公式/印章/图表)→结果融合→版面排序
        /// 返回包含20类文档元素识别结果的JSON
        /// </summary>
        /// <param name="imageFile">输入图片文件路径</param>
        /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectLayout(string filename);
        /// <summary>
        /// 执行文档版面分析 - OpenCV Mat输入
        /// </summary>
        /// <param name="cvmat">OpenCV Mat引用</param>
        /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectLayoutMat(IntPtr cvmat);

        /// <summary>
        /// 执行文档版面分析 - 字节数组输入
        /// </summary>
        /// <param name="imagebytedata">图片字节数组指针</param>
        /// <param name="size">字节数组长度(字节数)</param>
        /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectLayoutByte(byte[] imagebyte, UIntPtr size);

        /// <summary>
        /// 执行文档版面分析 - Base64编码输入
        /// </summary>
        /// <param name="imagebase64">Base64编码的图片字符串</param>
        /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern IntPtr DetectLayoutBase64(string base64);

        /// <summary>
        /// 释放文档版面分析引擎及所有相关资源
        /// </summary>
        /// <returns>成功返回0，失败返回非0</returns>
        [DllImport(dllFileName, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        internal static extern int FreeStructureEngine();
        #endregion

        #region 以图找图
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
        #endregion
    }
}
