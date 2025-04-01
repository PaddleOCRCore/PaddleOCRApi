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
using System.Collections.Generic;
using System.Text;

namespace PaddleOCRSDK
{
    public interface IOCRService
    {
        /// <summary>
        /// 初始化OCR引擎默认V4模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns>返回初始化结果</returns>
        string InitDefaultOCREngine(string modelsPath);
        /// <summary>
        /// 初始化表格识别引擎默认V4模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath"></param>
        /// <returns></returns>
        string InitDefaultTableEngine(string modelsPath);
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
        void EnableANSIResult(bool useANSI);
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
        /// <param name="imagebyte">图像文件</param>
        /// <returns>OCR识别结果</returns>
        string DetectTableBase64(string base64);
    }
}
