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
    /// 是否生成日志
    /// </summary>
    /// <param name="useLog"></param>
    /// <returns></returns>
    __declspec(dllimport) void __stdcall EnableLog(bool useLog);
    /// <summary>
    /// JSON输出是否使用ASCII编码，为true是返回Ascii编码
    /// </summary>
    /// <param name="useANSI"></param>
    __declspec(dllimport) void __stdcall EnableASCIIResult(bool useASCII);

    /// <summary>
    /// 是否使用json格式返回结果，默认true
    /// </summary>
    /// <param name="useANSI"></param>
    __declspec(dllimport) void __stdcall EnableJsonResult(bool enable);
    __declspec(dllimport) bool __stdcall Init(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const OCRParameter parameter);
    __declspec(dllimport) bool __stdcall Initjson(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* parameterjson);
    __declspec(dllimport) bool __stdcall DynamicInit(SyncParameter parameter);
    __declspec(dllimport) const char* __stdcall Detect(const char* imageFile);
    __declspec(dllimport) const char* __stdcall DetectMat(const cv::Mat& cvmat);
    __declspec(dllimport) const char* __stdcall DetectByte(const unsigned char* imagebytedata, size_t size);
    __declspec(dllimport) const char* __stdcall DetectBase64(const char* imagebase64);
    __declspec(dllimport) int __stdcall FreeEngine();
    __declspec(dllimport) char* __stdcall GetError();
    __declspec(dllimport) bool __stdcall InitTable(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* table_model_dir,
        const char* table_char_dict_path, TableParameter parameter);
    __declspec(dllimport) bool __stdcall InitTablejson(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* table_model_dir,
        const char* table_char_dict_path, const char* parameterjson);
    __declspec(dllimport) const char* __stdcall DetectTable(const char* imageFile);
    __declspec(dllimport) const char* __stdcall DetectTableByte(const unsigned char* imagebytedata, size_t size);
    __declspec(dllimport) const char* __stdcall DetectTableBase64(const char* imagebase64);
    __declspec(dllimport) int __stdcall FreeTableEngine();
    __declspec(dllimport) void __stdcall FreeResultBuffer(void* buffer);
}