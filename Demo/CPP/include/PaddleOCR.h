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

#pragma once
#include <string>
#include <opencv2/opencv.hpp>  // 使用OpenCV，若不使用DetectMat/DetectLayoutMat可不依赖OpenCV
#include <include/AI_Parameter.h>
#pragma comment(lib, "PaddleOCR.lib")

#ifdef _WIN32
#ifdef Paddle_Export
#define Paddle_API __declspec(dllexport)
#define CALL_CONV __stdcall
#else
#define Paddle_API __declspec(dllexport)
#define CALL_CONV __stdcall
#endif
#else
#define Paddle_API
#define CALL_CONV
#endif

extern "C" {
    /// <summary>
    /// 是否生成日志
    /// </summary>
    /// <param name="useLog"></param>
    /// <returns></returns>
    Paddle_API void CALL_CONV EnableLog(bool useLog);
    /// <summary>
    /// JSON输出是否使用ASCII编码，为true是返回Ascii编码
    /// </summary>
    /// <param name="useANSI"></param>
    Paddle_API void CALL_CONV EnableASCIIResult(bool useASCII);

    /// <summary>
    /// 是否使用json格式返回结果，默认false
    /// </summary>
    /// <param name="useANSI"></param>
    Paddle_API void CALL_CONV EnableJsonResult(bool enable);

    /// <summary>
    /// 获取授权注册申请码，注册机粘贴该字符串后解析机器信息并生成授权文件
    /// </summary>
    /// <returns>成功返回申请码字符串，失败返回错误信息字符串；调用方使用FreeResultBuffer释放</returns>
    Paddle_API const char* CALL_CONV GetLicenseRequestCode();

    /// <summary>
    /// 激活授权文件。每次使用前由调用方传入授权文件路径完成进程内授权。
    /// </summary>
    /// <param name="licensefile">加密授权文件路径</param>
    /// <returns>成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV ActivateLicense(const char* licensefile);

    /// <summary>
    /// 获取当前授权激活状态(JSON字符串)。返回内容包含程序名称、授权结果、客户信息、
    /// 授权编号、授权版本、授权平台、设备绑定状态、有效日期等。
    /// </summary>
    /// <returns>授权状态JSON字符串；调用方使用FreeResultBuffer释放</returns>
    Paddle_API const char* CALL_CONV GetLicenseStatus();
    /// <summary>
    /// 初始化OCR引擎并加载模型（传入结构体参数）
    /// </summary>
    /// <param name="det_infer">文本检测模型路径</param>
    /// <param name="cls_infer">方向分类模型路径（可选）</param>
    /// <param name="rec_infer">文本识别模型路径</param>
    /// <param name="parameter">OCR参数结构体</param>
    /// <returns>初始化成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV Init(const char* det_infer, const char* cls_infer,
        const char* rec_infer, const OCRParameter parameter);

    /// <summary>
    /// 初始化OCR引擎并加载模型（传入JSON参数字符串）
    /// </summary>
    /// <param name="det_infer">文本检测模型路径</param>
    /// <param name="cls_infer">方向分类模型路径（可选）</param>
    /// <param name="rec_infer">文本识别模型路径</param>
    /// <param name="parameterjson">JSON格式的参数字符串</param>
    /// <returns>初始化成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV Initjson(const char* det_infer, const char* cls_infer,
        const char* rec_infer, const char* parameterjson);

    /// <summary>
    /// 使用同步参数动态初始化引擎（适用于运行时配置）
    /// </summary>
    /// <param name="parameter">同步初始化参数</param>
    /// <returns>初始化成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV DynamicInit(SyncParameter parameter);

    /// <summary>
    /// 对输入图片文件执行OCR检测并返回结果字符串（JSON或自定义格式）
    /// </summary>
    /// <param name="imageFile">输入图片文件路径</param>
    /// <returns>返回指向结果字符串的指针（内部分配，使用FreeResultBuffer释放）</returns>
    Paddle_API const char* CALL_CONV Detect(const char* imageFile);

    /// <summary>
    /// 对输入OpenCV `cv::Mat` 执行OCR检测并返回结果字符串
    /// </summary>
    /// <param name="cvmat">输入的OpenCV Mat引用</param>
    /// <returns>返回指向结果字符串的指针（内部分配，使用FreeResultBuffer释放）</returns>
    Paddle_API const char* CALL_CONV DetectMat(const cv::Mat& cvmat);

    /// <summary>
    /// 对输入图片字节数组执行OCR检测并返回结果字符串
    /// </summary>
    /// <param name="imagebytedata">图片字节数组指针</param>
    /// <param name="size">字节数组长度（字节数）</param>
    /// <returns>返回指向结果字符串的指针（内部分配，使用FreeResultBuffer释放）</returns>
    Paddle_API const char* CALL_CONV DetectByte(const unsigned char* imagebytedata,
        size_t size);

    /// <summary>
    /// 对Base64编码的图片字符串执行OCR检测并返回结果字符串
    /// </summary>
    /// <param name="imagebase64">Base64编码的图片字符串</param>
    /// <returns>返回指向结果字符串的指针（内部分配，使用FreeResultBuffer释放）</returns>
    Paddle_API const char* CALL_CONV DetectBase64(const char* imagebase64);


    /// <summary>
    /// 对内存截图执行OCR检测并返回结果字符串
    /// </summary>
    /// <param name="data">内存截图数据指针</param>
    /// <param name="size">内存截图数据长度（字节数）</param>
    /// <returns>返回指向结果字符串的指针（内部分配，使用FreeResultBuffer释放）</returns>
    Paddle_API const char* CALL_CONV DetectScreenShot(unsigned char* data, int size);

    /// <summary>
    /// 释放并关闭OCR引擎，释放所有相关资源
    /// </summary>
    /// <returns>成功返回0，失败返回非0</returns>
    Paddle_API int CALL_CONV FreeEngine();

    /// <summary>
    /// 获取上一次操作的错误信息（调用者负责使用FreeResultBuffer释放返回的缓冲区）
    /// </summary>
    /// <returns>指向错误字符串的指针</returns>
    Paddle_API char* CALL_CONV GetError();

    /// <summary>
    /// 释放由 Detect* 函数返回的字符串缓冲区（跨平台安全）
    /// </summary>
    /// <param name="buffer">由 Detect 系列函数返回的指针</param>
    Paddle_API void CALL_CONV FreeResultBuffer(void* buffer);

    // ==================== 文档图像矫正API ====================
    /// <summary>
    /// 初始化文本图像矫正模块-传入参数结构体
    /// </summary>
    /// <param name="uvdoc_infer">UVDoc模型路径</param>
    /// <param name="uvdocpara">参数结构体</param>
    /// <returns>成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV InitUVDoc(const char* uvdoc_infer, UVDocParameter uvdocpara);

    /// <summary>
    /// 初始化文本图像矫正模块-传入JSON参数
    /// </summary>
    /// <param name="uvdoc_infer">UVDoc模型路径</param>
    /// <param name="parjson">json参数字符串</param>
    /// <returns>成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV InitUVDocjson(const char* uvdoc_infer, const char* parjson);

    /// <summary>
    /// 文本图像矫正-传入图像文件路径
    /// </summary>
    /// <param name="filename">输入文件路径</param>
    /// <param name="outputfilepath">输出文件路径</param>
    Paddle_API void CALL_CONV UVDocImageFile(const char* filename, const char* outputfilepath);

    /// <summary>
    /// 文本图像矫正-传入OpenCV Mat指针
    /// </summary>
    /// <param name="cvmat">OpenCV Mat指针</param>
    /// <param name="outputfilepath">输出文件路径(必须)</param>
    Paddle_API void CALL_CONV UVDocMat(void* cvmat, const char* outputfilepath);

    /// <summary>
    /// 文本图像矫正-传入图片字节数组
    /// </summary>
    /// <param name="imagebyte">图片字节数组</param>
    /// <param name="size">字节数组大小</param>
    /// <param name="outputfilepath">输出文件路径(必须)</param>
    Paddle_API void CALL_CONV UVDocByte(const unsigned char* imagebyte, long long size, const char* outputfilepath);

    /// <summary>
    /// 文本图像矫正-传入Base64编码的图片
    /// </summary>
    /// <param name="base64">Base64编码字符串</param>
    /// <param name="outputfilepath">输出文件路径(必须)</param>
    Paddle_API void CALL_CONV UVDocBase64(const char* base64, const char* outputfilepath);

    /// <summary>
    /// 释放文本图像矫正实例
    /// </summary>
    /// <returns>成功返回0，失败返回-1</returns>
    Paddle_API int CALL_CONV FreeUVDocEngine();

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
    /// <param name="table_cls_model_dir">表格有线/无线分类模型路径(可选)</param>
    /// <param name="wired_table_model_dir">有线表格结构识别模型路径(可选)</param>
    /// <param name="wireless_table_model_dir">无线表格结构识别模型路径(可选)</param>
    /// <param name="wired_table_cell_det_model_dir">有线表格单元格检测模型路径(可选)</param>
    /// <param name="wireless_table_cell_det_model_dir">无线表格单元格检测模型路径(可选)</param>
    /// <param name="formula_model_dir">公式识别模型路径(可选，NULL表示不使用)</param>
    /// <param name="seal_model_dir">印章识别模型路径(可选，NULL表示不使用)</param>
    /// <param name="doc_cls_infer">文档方向分类模型路径(可选，NULL表示不使用)</param>
    /// <param name="doc_unwarp_model">文档图像矫正模型路径(可选，NULL表示不使用)</param>
    /// <param name="region_model_dir">区域检测模型目录路径(可选，NULL表示不使用)</param>
    /// <param name="parameter">LayoutParameter参数结构体（值传递，参考Init中OCRParameter的模式）</param>
    /// <returns>初始化成功返回true，失败返回false</returns>
    Paddle_API bool CALL_CONV InitStructure(
        const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* layout_model_dir,
        const char* table_cls_model_dir,
        const char* wired_table_model_dir,
        const char* wireless_table_model_dir,
        const char* wired_table_cell_det_model_dir,
        const char* wireless_table_cell_det_model_dir,
        const char* formula_model_dir,
        const char* seal_model_dir,
        const char* doc_cls_infer,
        const char* doc_unwarp_model,
        const char* region_model_dir,
        const LayoutParameter parameter
    );

    /// <summary>
    /// 初始化结构化文档识别引擎（JSON参数版本）
    /// </summary>
    Paddle_API bool CALL_CONV InitStructurejson(
        const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* layout_model_dir,
        const char* table_cls_model_dir,
        const char* wired_table_model_dir,
        const char* wireless_table_model_dir,
        const char* wired_table_cell_det_model_dir,
        const char* wireless_table_cell_det_model_dir,
        const char* formula_model_dir,
        const char* seal_model_dir,
        const char* doc_cls_infer,
        const char* doc_unwarp_model,
        const char* region_model_dir,
        const char* parameterjson
    );

    /// <summary>
    /// 执行文档版面分析（扩展版本）
    /// 包含：文档预处理→版面检测→全局OCR→条件识别(表格/公式/印章/图表)→结果融合→版面排序
    /// 返回包含20类文档元素识别结果的JSON
    /// </summary>
    /// <param name="imageFile">输入图片文件路径</param>
    /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
    Paddle_API const char* CALL_CONV DetectLayout(const char* imageFile);

    /// <summary>
    /// 执行文档版面分析 - OpenCV Mat输入
    /// </summary>
    /// <param name="cvmat">OpenCV Mat引用</param>
    /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
    Paddle_API const char* CALL_CONV DetectLayoutMat(const cv::Mat& cvmat);

    /// <summary>
    /// 执行文档版面分析 - 字节数组输入
    /// </summary>
    /// <param name="imagebytedata">图片字节数组指针</param>
    /// <param name="size">字节数组长度(字节数)</param>
    /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
    Paddle_API const char* CALL_CONV DetectLayoutByte(
        const unsigned char* imagebytedata,
        size_t size
    );

    /// <summary>
    /// 执行文档版面分析 - Base64编码输入
    /// </summary>
    /// <param name="imagebase64">Base64编码的图片字符串</param>
    /// <returns>完整分析结果JSON字符串(需使用FreeResultBuffer释放)</returns>
    Paddle_API const char* CALL_CONV DetectLayoutBase64(const char* imagebase64);

    /// <summary>
    /// 释放文档版面分析引擎及所有相关资源
    /// </summary>
    /// <returns>成功返回0，失败返回非0</returns>
    Paddle_API int CALL_CONV FreeStructureEngine();

    // ==================== 以图找图API ====================
    /// <summary>
    /// 以图找图：传入图片路径，在大图中查找小图，返回JSON字符串。
    /// JSON格式: {"success":bool,"message":"...","matchCount":number,"data":[...]}
    /// 返回的字符串需要使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="bigImagePath">大图路径</param>
    /// <param name="smallImagePath">小图/模板图路径</param>
    /// <param name="threshold">匹配阈值 [0, 1]，默认0.8</param>
    /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
    /// <param name="useSlideMatch">是否使用滑块专用匹配，默认false</param>
    Paddle_API const char* FindImage(
        const char* bigImagePath,
        const char* smallImagePath,
        double threshold = 0.8,
        bool toGray = true,
        bool useSlideMatch = false
    );

    /// <summary>
    /// 以图找图：传入图片字节，在大图中查找小图，返回JSON字符串。
    /// 返回的字符串需要使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="bigImageBytes">大图压缩图片字节（如PNG/JPEG）</param>
    /// <param name="bigImageSize">大图字节长度</param>
    /// <param name="smallImageBytes">小图/模板压缩图片字节（如PNG/JPEG）</param>
    /// <param name="smallImageSize">小图字节长度</param>
    /// <param name="threshold">匹配阈值 [0, 1]，默认0.8</param>
    /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
    /// <param name="useSlideMatch">是否使用滑块专用匹配，默认false</param>
    Paddle_API const char* FindImageByte(
        const unsigned char* bigImageBytes,
        size_t bigImageSize,
        const unsigned char* smallImageBytes,
        size_t smallImageSize,
        double threshold = 0.8,
        bool toGray = true,
        bool useSlideMatch = false
    );

    /// <summary>
    /// 以图找图：传入OpenCV Mat指针，在大图中查找小图，返回JSON字符串。
    /// 返回的字符串需要使用 FreeResultBuffer 释放。
    /// </summary>
    /// <param name="bigImage">大图OpenCV Mat指针</param>
    /// <param name="smallImage">小图/模板OpenCV Mat指针</param>
    /// <param name="threshold">匹配阈值 [0, 1]，默认0.8</param>
    /// <param name="toGray">是否转换为灰度图进行匹配，默认true</param>
    /// <param name="useSlideMatch">是否使用滑块专用匹配，默认false</param>
    Paddle_API const char* FindImageMat(
        const cv::Mat* bigImage,
        const cv::Mat* smallImage,
        double threshold = 0.8,
        bool toGray = true,
        bool useSlideMatch = false
    );

}
