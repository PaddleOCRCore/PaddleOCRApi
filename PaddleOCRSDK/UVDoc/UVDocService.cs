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
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace PaddleOCRSDK
{
    /// <summary>
    /// UVDoc文本图像矫正服务实现
    /// </summary>
    public class UVDocService : IUVDocService
    {
        private bool _initialized = false;
        private readonly object _lock = new object();

        /// <summary>
        /// 是否已初始化
        /// </summary>
        public bool IsInitialized => _initialized;

        /// <summary>
        /// 初始化UVDoc引擎（使用参数结构体）
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <param name="parameter">参数</param>
        /// <returns></returns>
        public bool Initialize(string modelPath, UVDocParameter parameter)
        {
            lock (_lock)
            {
                _initialized = UVDocSDK.InitUVDoc(modelPath, parameter);
                if (!_initialized)
                {
                    string error = GetLastError();
                    Console.WriteLine($"初始化失败: {error}");
                }
                return _initialized;
            }
        }

        /// <summary>
        /// 初始化UVDoc引擎（使用JSON参数）
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <param name="jsonParams">JSON参数</param>
        /// <returns></returns>
        public bool InitializeWithJson(string modelPath, string jsonParams)
        {
            lock (_lock)
            {
                _initialized = UVDocSDK.InitUVDocjson(modelPath, jsonParams);
                if (!_initialized)
                {
                    string error = GetLastError();
                    Console.WriteLine($"初始化失败: {error}");
                }
                return _initialized;
            }
        }

        /// <summary>
        /// 处理图像文件
        /// </summary>
        /// <param name="inputPath">输入图像路径</param>
        /// <param name="outputPath">输出图像路径</param>
        public void UVDocImageFile(string inputPath, string outputPath)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("引擎未初始化");
            }

            lock (_lock)
            {
                UVDocSDK.UVDocImageFile(inputPath, outputPath);
            }
        }

        /// <summary>
        /// 异步处理图像文件
        /// </summary>
        /// <param name="inputPath">输入图像路径</param>
        /// <param name="outputPath">输出图像路径</param>
        /// <returns></returns>
        public Task UVDocImageFileAsync(string inputPath, string outputPath)
        {
            return Task.Run(() => UVDocImageFile(inputPath, outputPath));
        }

        /// <summary>
        /// 处理图像字节数组
        /// </summary>
        /// <param name="imageBytes">图像字节数组</param>
        /// <param name="outputPath">输出路径</param>
        public void UVDocImageBytes(byte[] imageBytes, string outputPath)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("引擎未初始化");
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentException("输出路径不能为空", nameof(outputPath));
            }

            lock (_lock)
            {
                UVDocSDK.UVDocByte(imageBytes, imageBytes.Length, outputPath);
            }
        }

        /// <summary>
        /// 异步处理图像字节数组
        /// </summary>
        /// <param name="imageBytes">图像字节数组</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns></returns>
        public Task UVDocImageBytesAsync(byte[] imageBytes, string outputPath)
        {
            return Task.Run(() => UVDocImageBytes(imageBytes, outputPath));
        }

        /// <summary>
        /// 处理Base64编码的图像
        /// </summary>
        /// <param name="base64String">Base64字符串</param>
        /// <param name="outputPath">输出路径</param>
        public void UVDocBase64Image(string base64String, string outputPath)
        {
            if (!_initialized)
            {
                throw new InvalidOperationException("引擎未初始化");
            }

            if (string.IsNullOrEmpty(outputPath))
            {
                throw new ArgumentException("输出路径不能为空", nameof(outputPath));
            }

            lock (_lock)
            {
                UVDocSDK.UVDocBase64(base64String, outputPath);
            }
        }

        /// <summary>
        /// 异步处理Base64编码的图像
        /// </summary>
        /// <param name="base64String">Base64字符串</param>
        /// <param name="outputPath">输出路径</param>
        /// <returns></returns>
        public Task UVDocBase64ImageAsync(string base64String, string outputPath)
        {
            return Task.Run(() => UVDocBase64Image(base64String, outputPath));
        }

        /// <summary>
        /// 获取最后一次错误信息
        /// </summary>
        /// <returns></returns>
        public string GetLastError()
        {
            string lastErr = "";
            try
            {
                var ret = UVDocSDK.GetError();
                if (ret != IntPtr.Zero)
                {
                    lastErr = MarshalUtf8.PtrToStringUTF8(ret);
                    //Marshal.FreeCoTaskMem(ret); 改为调用SDK的释放接口
                    OCRSDK.FreeResultBuffer(ret);
                }
            }
            catch (Exception e)
            {
                lastErr = e.Message;
            }
            return lastErr;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                if (_initialized)
                {
                    UVDocSDK.FreeUVDocEngine();
                    _initialized = false;
                }
            }
            GC.SuppressFinalize(this);
        }
    }
}
