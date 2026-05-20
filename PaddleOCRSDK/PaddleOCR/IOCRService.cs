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
        /// 初始化版面识别引擎默认V5模型，使用CPU及mkldnn
        /// </summary>
        /// <param name="modelsPath">模型所在目录，如models</param>
        /// <param name="useV5">是否使用v5_mobile模型，为False使用v4_mobile</param>
        /// <returns></returns>
        string InitDefaultStructureEngine(string modelsPath, bool useV5);
        /// <summary>
        /// 初始化 OCR 引擎
        /// </summary>
        /// <param name="para">初始化参数（支持结构体或JSON格式）</param>
        /// <returns>初始化成功返回true，失败返回false</returns>
        bool Init(InitParamater para);

        /// <summary>
        /// 获取当前机器的加密授权申请码
        /// </summary>
        /// <returns>授权申请码</returns>
        string GetLicenseRequestCode();

        /// <summary>
        /// 激活授权文件
        /// </summary>
        /// <param name="licenseFile">授权文件路径</param>
        /// <returns>激活成功返回true，失败返回false</returns>
        bool ActivateLicense(string licenseFile);

        /// <summary>
        /// 获取当前授权状态JSON
        /// </summary>
        /// <returns>授权状态JSON字符串</returns>
        string GetLicenseStatus();

        /// <summary>
        /// 获取当前授权状态
        /// </summary>
        /// <returns>授权状态对象</returns>
        LicenseStatus GetLicenseStatusInfo();

        /// <summary>
        /// 对输入图片文件执行 OCR 检测并返回结果
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>OCR识别结果，包含文本框和置信度</returns>
        OCRResult Detect(string imagefile);

        /// <summary>
        /// 对输入图片字节数组执行 OCR 检测并返回结果
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>OCR识别结果，包含文本框和置信度</returns>
        OCRResult Detect(byte[] imagebyte);
        /// <summary>
        /// 对输入 OpenCV Mat 执行 OCR 检测并返回结果
        /// </summary>
        /// <param name="ptr_cvmat">OpenCV Mat 指针</param>
        /// <returns>OCR识别结果，包含文本框和置信度</returns>
        OCRResult DetectMat(IntPtr ptr_cvmat);
        /// <summary>
        /// 对 Base64 编码的图片执行 OCR 检测并返回结果
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>OCR识别结果，包含文本框和置信度</returns>
        OCRResult DetectBase64(string base64);
        /// <summary>
        /// 对内存截图数据执行 OCR 检测并返回结果
        /// </summary>
        /// <param name="screenshotData">截图字节数组</param>
        /// <returns>OCR识别结果，包含文本框和置信度</returns>
        OCRResult DetectScreenShot(byte[] screenshotData);
        /// <summary>
        /// 获取上一次操作的错误信息
        /// </summary>
        /// <returns>错误信息字符串，无错误时返回空字符串</returns>
        string GetError();
        /// <summary>
        /// 设置是否生成日志（默认为 true）
        /// </summary>
        /// <param name="useLog">true 启用日志，false 禁用日志</param>
        void EnableLog(bool useLog);
        /// <summary>
        /// 设置 JSON 输出是否使用 ASCII 编码（默认为 false）
        /// </summary>
        /// <param name="useANSI">true 使用 ASCII 编码，false 使用 UTF-8</param>
        void EnableASCIIResult(bool useANSI);
        /// <summary>
        /// 设置是否使用 JSON 格式返回结果（默认为 true）
        /// </summary>
        /// <param name="enableJson">true 使用 JSON 格式，false 使用其他格式</param>
        void EnableJsonResult(bool enableJson);
        /// <summary>
        /// 执行文档版面分析（包含版面检测、表格识别、公式识别等）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayout(string imagefile);
        /// <summary>
        /// 执行文档版面分析（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayoutByte(byte[] imagebyte);
        /// <summary>
        /// 执行文档版面分析（Base64 编码输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayoutBase64(string base64);
        /// <summary>
        /// 执行文档版面分析（OpenCV Mat 输入）
        /// </summary>
        /// <param name="ptr_cvmat">OpenCV Mat 指针</param>
        /// <returns>包含版面分析结果的 JSON 字符串</returns>
        string DetectLayoutMat(IntPtr ptr_cvmat);
        /// <summary>
        /// 释放并关闭 OCR 引擎，释放所有相关资源
        /// </summary>
        void FreeEngine();
        /// <summary>
        /// 释放并关闭版面识别引擎，释放所有相关资源
        /// </summary>
        void FreeStructureEngine();
        /// <summary>
        /// 在大图中查找小图（图像匹配）
        /// </summary>
        /// <param name="bigImagePath">大图路径</param>
        /// <param name="smallImagePath">小图路径</param>
        /// <param name="threshold">匹配阈值 [0, 1]，默认 0.8（滑块找图建议 0.2）</param>
        /// <param name="toGray">是否转换为灰度图进行匹配，默认 true</param>
        /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认 false</param>
        /// <returns>FindImageResult 对象，包含匹配结果和位置信息</returns>
        FindImageResult FindImage(string bigImagePath, string smallImagePath, double threshold = 0.8, bool toGray = true, bool useSlideMatch = false);

        /// <summary>
        /// 解析文档版面识别 JSON 结果为结构化对象（对应最新版 DLL 返回格式）
        /// </summary>
        /// <param name="json">DetectLayout 返回的 JSON 字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult ParseLayoutResult(string json);

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（文件路径输入）
        /// </summary>
        /// <param name="imagefile">输入图片文件路径</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult DetectLayoutParsed(string imagefile);

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（字节数组输入）
        /// </summary>
        /// <param name="imagebyte">图片字节数组</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult DetectLayoutByteParsed(byte[] imagebyte);

        /// <summary>
        /// 执行文档版面分析并返回结构化对象（Base64 输入）
        /// </summary>
        /// <param name="base64">Base64 编码的图片字符串</param>
        /// <returns>结构化的版面识别结果</returns>
        LayoutDetectResult DetectLayoutBase64Parsed(string base64);
    }
}
