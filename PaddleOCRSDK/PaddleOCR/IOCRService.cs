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
using PaddleOCRSDK.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PaddleOCRSDK
{
    public interface IOCRService
    {
        /// <summary>
        /// 初始化OCR引擎默认V4模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath">模型所在目录，如models</param>
        /// <param name="useV5">是否使用v5_mobile模型，为False使用v4_mobile</param>
        /// <returns>返回初始化结果</returns>
        string InitDefaultOCREngine(string modelsPath, bool useV5);
        /// <summary>
        /// 初始化表格识别引擎默认V5模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath">模型所在目录，如models</param>
        /// <param name="useV5">是否使用v5_mobile模型，为False使用v4_mobile</param>
        /// <returns></returns>
        string InitDefaultTableEngine(string modelsPath, bool useV5);
        /// <summary>
        /// 初如化OCR
        /// </summary>
        /// <param name="para"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool Init(InitParamater para);

        /// <summary>
        /// 对图像文件进行文本识别
        /// </summary>
        /// <param name="imagefile">图像文件</param>
        /// <returns>OCR识别结果</returns>
        OCRResult Detect(string imagefile);

        /// <summary>
        /// 对图像文件进行文本识别
        /// </summary>
        /// <param name="imagebyte">图像文件</param>
        /// <returns>OCR识别结果</returns>
        OCRResult Detect(byte[] imagebyte);
        /// <summary>
        /// 对Mat进行文本识别
        /// </summary>
        /// <param name="ptr_cvmat">Mat</param>
        /// <returns></returns>
        OCRResult DetectMat(IntPtr ptr_cvmat);
        /// <summary>
        /// 对base64图像进行识别
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        OCRResult DetectBase64(string base64);
        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <returns></returns>
        string GetError();
        /// <summary>
        /// 是否生成日志，默认为true
        /// </summary>
        /// <param name="useLog"></param>
        void EnableLog(bool useLog);
        /// <summary>
        /// 是否使用单字节编码，默认为false
        /// </summary>
        /// <param name="useANSI"></param>
        void EnableASCIIResult(bool useANSI);
        /// <summary>
        /// 是否使用json格式返回结果，默认true
        /// </summary>
        /// <param name="enableJson"></param>
        void EnableJsonResult(bool enableJson);
        /// <summary>
        /// 对图像文件进行表格识别
        /// </summary>
        /// <param name="imagefile">图像文件</param>
        /// <returns>OCR识别结果</returns>
        string DetectTable(string imagefile);
        /// <summary>
        /// 对图像文件进行表格识别
        /// </summary>
        /// <param name="imagebyte">图像文件</param>
        /// <returns>OCR识别结果</returns>
        string DetectTableByte(byte[] imagebyte);
        /// <summary>
        /// 对图像文件进行表格识别
        /// </summary>
        /// <param name="base64">base64</param>
        /// <returns>OCR识别结果</returns>
        string DetectTableBase64(string base64);
        /// <summary>
        /// 释放OCR实例
        /// </summary>
        void FreeEngine();
        /// <summary>
        /// 释放OCR表格识别实例
        /// </summary>
        void FreeTableEngine();
        /// <summary>
        /// 以图找图：在大图中查找小图
        /// </summary>
        /// <param name="bigImagePath">大图路径</param>
        /// <param name="smallImagePath">小图路径</param>
        /// <param name="threshold">匹配阈值 [0, 1]，默认0.8。滑块找图建议0.2左右</param>
        /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
        /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认false</param>
        /// <returns>返回FindImageResult对象，包含匹配结果和位置信息</returns>
        FindImageResult FindImage(string bigImagePath, string smallImagePath, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false);
    }
}
