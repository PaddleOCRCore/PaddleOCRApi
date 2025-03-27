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
    /// 是否使用单字节编码
    /// </summary>
    /// <param name="useANSI"></param>
    __declspec(dllimport) void __stdcall EnableANSIResult(bool useANSI);

    /// <summary>
    /// 是否使用json格式返回结果，默认true
    /// </summary>
    /// <param name="useANSI"></param>
    __declspec(dllimport) void __stdcall EnableJsonResult(bool enable);
    __declspec(dllimport) bool __stdcall Init(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer, const char* keys,
        const OCRParameter parameter);
    __declspec(dllimport) bool __stdcall Initjson(const char* det_infer,
        const char* cls_infer,
        const char* rec_infer,
        const char* keys,
        const char* parameterjson);
    __declspec(dllimport) bool __stdcall DynamicInit(SyncParameter parameter);
    __declspec(dllimport) const wchar_t* __stdcall Detect(const char* imageFile);
    __declspec(dllimport) const wchar_t* __stdcall DetectByte(const unsigned char* imagebytedata, size_t size);
    __declspec(dllimport) const wchar_t* __stdcall DetectBase64(const char* imagebase64);
    __declspec(dllimport) int __stdcall FreeEngine();
    __declspec(dllimport) wchar_t* __stdcall GetError();
    __declspec(dllimport) bool __stdcall InitTable(const char* det_infer,
        const char* rec_infer,
        const char* keys,
        const char* table_model_dir,
        const char* table_char_dict_path, TableParameter parameter);
    __declspec(dllimport) bool __stdcall InitTablejson(
        const char* det_infer,
        const char* rec_infer,
        const char* keys,
        const char* table_model_dir,
        const char* table_char_dict_path, const char* parameterjson);
    __declspec(dllimport) const wchar_t* __stdcall DetectTable(const char* imageFile);
    __declspec(dllimport) const wchar_t* __stdcall DetectTableByte(const unsigned char* imagebytedata, size_t size);
    __declspec(dllimport) const wchar_t* __stdcall DetectTableBase64(const char* imagebase64);
    __declspec(dllimport) int __stdcall FreeTableEngine();
}