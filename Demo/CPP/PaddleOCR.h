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

#pragma once
#include <string>
#include <opencv2/opencv.hpp>//使用OpenCV4.10，若不使用DetectMat方法，可不依赖OpenCV
#include <include/AI_Parameter.h>
#pragma comment (lib,"PaddleOCR.lib")
#pragma once

extern "C" {
    /// <summary>
    /// 是否生成库内部日志，便于调试。
    /// </summary>
    /// <param name="useLog">为 true 则开启日志，false 则关闭。</param>
    __declspec(dllimport) void __stdcall EnableLog(bool useLog);

    /// <summary>
    /// 控制返回 JSON 中是否将非 ASCII 字符转为 ASCII 编码（如 \uXXXX）。
    /// </summary>
    /// <param name="useASCII">为 true 时返回 ASCII 编码，否则返回原始 UTF-8 字符串。</param>
    __declspec(dllimport) void __stdcall EnableASCIIResult(bool useASCII);

    /// <summary>
    /// 设置库返回结果的格式是否为 JSON。默认值为 true（返回 JSON）。
    /// </summary>
    /// <param name="enable">为 true 则以 JSON 格式返回结果，false 则使用自定义文本格式。</param>
    __declspec(dllimport) void __stdcall EnableJsonResult(bool enable);

    /// <summary>
    /// 获取上一次操作的错误信息。
    /// </summary>
    /// <returns>指向错误字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) char* __stdcall GetError();
    
    // ==================== 文字识别API ====================

    /// <summary>
    /// 初始化 OCR 引擎并加载模型（传入结构体参数）。
    /// </summary>
    /// <param name="det_infer">文本检测模型的推理引擎文件路径（det）</param>
    /// <param name="cls_infer">方向分类模型的推理文件路径（可选，cls）</param>
    /// <param name="rec_infer">文本识别模型的推理文件路径（rec）</param>
    /// <param name="parameter">OCR 参数结构体（见 AI_Parameter.h）</param>
    /// <returns>初始化成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool __stdcall Init(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const OCRParameter parameter);

    /// <summary>
    /// 初始化 OCR 引擎并加载模型（传入 JSON 参数字符串）。
    /// </summary>
    /// <param name="det_infer">文本检测模型路径</param>
    /// <param name="cls_infer">方向分类模型路径（可选）</param>
    /// <param name="rec_infer">文本识别模型路径</param>
    /// <param name="parameterjson">JSON 格式的参数字符串</param>
    /// <returns>初始化成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool __stdcall Initjson(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* parameterjson);

    /// <summary>
    /// 使用同步参数动态初始化引擎，适用于运行时根据配置调整参数并重新初始化。
    /// </summary>
    /// <param name="parameter">同步初始化参数结构体</param>
    /// <returns>成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool __stdcall DynamicInit(SyncParameter parameter);

    /// <summary>
    /// 对输入图片文件执行 OCR 检测并返回结果字符串（JSON 或自定义格式）。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="imageFile">输入图片文件路径</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall Detect(const char* imageFile);

    /// <summary>
    /// 对输入 OpenCV `cv::Mat` 执行 OCR 检测并返回结果字符串。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="cvmat">输入的 OpenCV Mat 引用</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall DetectMat(const cv::Mat& cvmat);

    /// <summary>
    /// 对输入图片字节数组执行 OCR 检测并返回结果字符串。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="imagebytedata">图片字节数组指针</param>
    /// <param name="size">字节数组长度（字节数）</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall DetectByte(const unsigned char* imagebytedata, size_t size);

    /// <summary>
    /// 对 Base64 编码的图片字符串执行 OCR 检测并返回结果字符串。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="imagebase64">Base64 编码的图片字符串</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall DetectBase64(const char* imagebase64);
    /// <summary>
    /// 对内存截图执行OCR检测并返回结果字符串
    /// </summary>
    /// <param name="data">内存截图数据指针</param>
    /// <param name="size">内存截图数据长度（字节数）</param>
    /// <returns>返回指向结果字符串的指针（内部分配，使用FreeResultBuffer释放）</returns>
    __declspec(dllimport) const char* __stdcall DetectScreenShot(unsigned char* data, int size);

    /// <summary>
    /// 释放并关闭 OCR 引擎，释放所有相关资源。
    /// </summary>
    /// <returns>成功返回 0，失败返回非 0。</returns>
    __declspec(dllimport) int __stdcall FreeEngine();

    // ==================== 表格识别API ====================

    /// <summary>
    /// 初始化表格识别引擎并加载模型（传入结构体参数）。
    /// </summary>
    /// <param name="det_infer">文本/表格检测模型路径</param>
    /// <param name="cls_infer">方向分类模型路径（可选）</param>
    /// <param name="rec_infer">文本识别模型路径</param>
    /// <param name="table_model_dir">表格识别模型目录路径</param>
    /// <param name="parameter">表格识别参数结构体</param>
    /// <returns>初始化成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool __stdcall InitTable(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* table_model_dir,TableParameter parameter);

    /// <summary>
    /// 初始化表格识别引擎并加载模型（传入 JSON 参数字符串）。
    /// </summary>
    /// <param name="det_infer">文本/表格检测模型路径</param>
    /// <param name="cls_infer">方向分类模型路径（可选）</param>
    /// <param name="rec_infer">文本识别模型路径</param>
    /// <param name="table_model_dir">表格识别模型目录路径</param>
    /// <param name="parameterjson">JSON 格式的参数字符串</param>
    /// <returns>初始化成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool __stdcall InitTablejson(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* table_model_dir, const char* parameterjson);

    /// <summary>
    /// 对输入图片文件执行表格识别并返回结果字符串（JSON 或自定义格式）。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="imageFile">输入图片文件路径</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall DetectTable(const char* imageFile);

    /// <summary>
    /// 对输入图片字节数组执行表格识别并返回结果字符串。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="imagebytedata">图片字节数组指针</param>
    /// <param name="size">字节数组长度（字节数）</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall DetectTableByte(const unsigned char* imagebytedata, size_t size);

    /// <summary>
    /// 对 Base64 编码的图片字符串执行表格识别并返回结果字符串。
    /// 返回值为库内部分配的缓冲区指针，调用者需使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="imagebase64">Base64 编码的图片字符串</param>
    /// <returns>指向结果字符串的指针（UTF-8），失败时可能返回 NULL。</returns>
    __declspec(dllimport) const char* __stdcall DetectTableBase64(const char* imagebase64);

    /// <summary>
    /// 释放并关闭表格识别引擎，释放所有相关资源。
    /// </summary>
    /// <returns>成功返回 0，失败返回非 0。</returns>
    __declspec(dllimport) int __stdcall FreeTableEngine();

    /// <summary>
    /// 释放由 Detect* / GetError 等函数返回的字符串缓冲区（跨平台安全）。
    /// </summary>
    /// <param name="buffer">由 Detect 系列函数返回的指针</param>
    __declspec(dllimport) void __stdcall FreeResultBuffer(void* buffer);

    // ==================== 文档图像矫正API ====================
    /// <summary>
    /// 初始化文本图像矫正模块，传入参数结构体。
    /// </summary>
    /// <param name="uvdoc_infer">UVDoc 模型路径</param>
    /// <param name="uvdocpara">UVDoc 参数结构体</param>
    /// <returns>成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool InitUVDoc(const char* uvdoc_infer, UVDocParameter uvdocpara);

    /// <summary>
    /// 初始化文本图像矫正模块，传入 JSON 参数字符串。
    /// </summary>
    /// <param name="uvdoc_infer">UVDoc 模型路径</param>
    /// <param name="parjson">JSON 参数字符串</param>
    /// <returns>成功返回 true，失败返回 false。</returns>
    __declspec(dllimport) bool InitUVDocjson(const char* uvdoc_infer, const char* parjson);

    /// <summary>
    /// 文本图像矫正：传入图像文件路径并输出到指定路径。
    /// </summary>
    /// <param name="filename">输入文件路径</param>
    /// <param name="outputfilepath">输出文件路径</param>
    __declspec(dllimport) void UVDocImageFile(const char* filename, const char* outputfilepath);

    /// <summary>
    /// 文本图像矫正：传入 OpenCV Mat 指针并输出到指定路径（输出路径必须指定）。
    /// </summary>
    /// <param name="cvmat">OpenCV Mat 指针</param>
    /// <param name="outputfilepath">输出文件路径（必须）</param>
    __declspec(dllimport) void UVDocMat(void* cvmat, const char* outputfilepath);

    /// <summary>
    /// 文本图像矫正：传入图片字节数组并输出到指定路径（输出路径必须指定）。
    /// </summary>
    /// <param name="imagebyte">图片字节数组</param>
    /// <param name="size">字节数组大小</param>
    /// <param name="outputfilepath">输出文件路径（必须）</param>
    __declspec(dllimport) void UVDocByte(const unsigned char* imagebyte, long long size, const char* outputfilepath);

    /// <summary>
    /// 文本图像矫正：传入 Base64 编码的图片并输出到指定路径（输出路径必须指定）。
    /// </summary>
    /// <param name="base64">Base64 编码字符串</param>
    /// <param name="outputfilepath">输出文件路径（必须）</param>
    __declspec(dllimport) void UVDocBase64(const char* base64, const char* outputfilepath);

    /// <summary>
    /// 释放文本图像矫正实例并回收资源。
    /// </summary>
    /// <returns>成功返回 0，失败返回 -1。</returns>
    __declspec(dllimport) int FreeUVDocEngine();


    // ==================== 以图找图API ====================
    /// <summary>
    /// 以图找图：在大图中查找小图，返回JSON字符串
    /// </summary>
    /// <param name="bigImagePath">大图路径</param>
    /// <param name="smallImagePath">小图路径</param>
    /// <param name="threshold">匹配阈值 [0, 1]，默认0.8</param>
    /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
    /// <param name="useSlideMatch">是否使用滑块验证匹配（边缘检测），默认false，滑块找图请将threshold改为0.2左右</param>
    /// <returns></returns>

    __declspec(dllimport) const char* __stdcall FindImage(
        const char* bigImagePath,
        const char* smallImagePath,
        double threshold = 0.8,
        bool toGray = true,
        bool useSlideMatch = false);
}