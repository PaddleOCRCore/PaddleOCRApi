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
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace PaddleOCRSDK
{
    public class OCRResult
    {
        /// <summary>
        /// 文本块列表
        /// </summary>
        public List<JsonResult> WordsResult { get; set; } = new List<JsonResult>();
        /// <summary>
        /// 识别结果文本
        /// </summary>
        public string Text => this.ToString();

        /// <summary>
        /// 识别结果文本Json格式
        /// </summary>
        public string JsonText { get; set; }
    }

    /// <summary>
    /// 识别结果
    /// </summary>
    public class JsonResult
    {
        /// <summary>
        /// 文本块文本
        /// </summary>
        public string Words { get; set; }
        /// <summary>
                                         /// 文本块四周顶点坐标列表
                                         /// </summary>
        public List<OCRLocation> Location { get; set; } = new List<OCRLocation>();
        /// <summary>
        ///文本识别置信度
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        ///角度分类置信度
        /// </summary>
        public float Cls_score { get; set; }
        /// <summary>
        ///角度分类标签
        /// </summary>
        public int CLs_label { get; set; }
    }

    /// <summary>
    /// OCR坐标对象
    /// </summary>
    public class OCRLocation
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public int x;
        /// <summary>
        /// Y坐标
        /// </summary>
        public int y;
    }
}
