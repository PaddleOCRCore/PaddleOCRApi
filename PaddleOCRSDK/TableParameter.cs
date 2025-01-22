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

using System.Runtime.InteropServices;
namespace PaddleOCRSDK
{
    /// <summary>
    /// OCR表格识别模型参数
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class TableParameter:OCRParameter
    {
        /// <summary>
        /// 表格识别模型输入图像长边大小，最终图像大小为（table_max_len，table_max_len）,默认488
        /// </summary>
        public int table_max_len { get; set; } = 488;
        /// <summary>
        /// 是否合并空单元格
        /// </summary>
        [field: MarshalAs(UnmanagedType.I1)]
        public bool merge_empty_cell { get; set; } = true;
        /// <summary>
        /// 批量识别数量
        /// </summary>
        public int table_batch_num { get; set; } = 1;        

    }
}


