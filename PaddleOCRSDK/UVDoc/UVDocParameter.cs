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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{
    /// <summary>
    /// 文本图像矫正模块传入参数
    /// 模型下载：https://www.paddleocr.ai/latest/version3.x/module_usage/text_image_unwarping.html
    /// 依赖PaddleOCRUVDoc.dll动态链接库
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class UVDocParameter
    {
        /// <summary>
        /// CPU模式下，是否使用mkldnn库，默认true
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool enable_mkldnn { get; set; } = true;

        /// <summary>
        /// CPU模式下，预测时的线程数，默认10
        /// </summary>
        public int cpu_threads { get; set; } = 10;
        /// <summary>
        /// 是否使用GPU，默认false
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_gpu { get; set; } = false;
        /// <summary>
        /// GPU id，使用GPU时有效
        /// </summary>
        public int gpu_id { get; set; } = 0;
        /// <summary>
        /// 使用GPU时内存
        /// </summary>
        public int gpu_mem { get; set; } = 2000;

        /// <summary>
        /// 使用GPU预测时，是否启动tensorrt，默认false
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool use_tensorrt { get; set; } = false;
    }
}

