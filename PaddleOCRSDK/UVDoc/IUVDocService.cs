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
using System.Threading.Tasks;

namespace PaddleOCRSDK
{
    /// <summary>
    /// UVDoc文本图像矫正服务接口
    /// </summary>
    public interface IUVDocService
    {
        /// <summary>
        /// 是否已初始化
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// 初始化UVDoc引擎（使用参数结构体）
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        bool Initialize(string modelPath, UVDocParameter parameter);

        /// <summary>
        /// 初始化UVDoc引擎（使用JSON参数）
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <param name="jsonParams">JSON参数</param>
        /// <returns></returns>
        bool InitializeWithJson(string modelPath, string jsonParams);

        /// <summary>
        /// 处理图像文件
        /// </summary>
        /// <param name="inputPath">输入图像路径</param>
        /// <param name="outputPath">输出图像路径</param>
        void UVDocImageFile(string inputPath, string outputPath);

        /// <summary>
        /// 异步处理图像文件
        /// </summary>
        /// <param name="inputPath">输入图像路径</param>
        /// <param name="outputPath">输出图像路径</param>
        /// <returns></returns>
        Task UVDocImageFileAsync(string inputPath, string outputPath);

        /// <summary>
        /// 处理图像字节数组
        /// </summary>
        /// <param name="imageBytes">图像字节数组</param>
        /// <param name="outputPath">输出路径</param>
        void UVDocImageBytes(byte[] imageBytes, string outputPath);

        /// <summary>
        /// 异步处理图像字节数组
        /// </summary>
        /// <param name="imageBytes">图像字节数组</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns></returns>
        Task UVDocImageBytesAsync(byte[] imageBytes, string outputPath);

        /// <summary>
        /// 处理Base64编码的图像
        /// </summary>
        /// <param name="base64String">Base64字符串</param>
        /// <param name="outputPath">输出路径</param>
        void UVDocBase64Image(string base64String, string outputPath);

        /// <summary>
        /// 异步处理Base64编码的图像
        /// </summary>
        /// <param name="base64String">Base64字符串</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns></returns>
        Task UVDocBase64ImageAsync(string base64String, string outputPath);

        /// <summary>
        /// 获取最后一次错误信息
        /// </summary>
        /// <returns></returns>
        string GetLastError();
        /// <summary>
        /// 释放实例
        /// </summary>
        void FreeUVDocEngine();
    }
}
