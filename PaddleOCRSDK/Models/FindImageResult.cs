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

namespace PaddleOCRSDK.Models
{
    /// <summary>
    /// 以图找图结果
    /// </summary>
    public class FindImageResult
    {
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 结果数据
        /// </summary>
        public FindImageData Data { get; set; }
    }

    /// <summary>
    /// 以图找图数据
    /// </summary>
    public class FindImageData
    {
        /// <summary>
        /// 是否找到
        /// </summary>
        public bool Found { get; set; }

        /// <summary>
        /// 匹配置信度 [0, 1]
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// 位置坐标（四个角点）
        /// </summary>
        public PointLocation[] Location { get; set; }

        /// <summary>
        /// 图像尺寸
        /// </summary>
        public ImageSize ImageSize { get; set; }
    }

    /// <summary>
    /// 点坐标
    /// </summary>
    public class PointLocation
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public int Y { get; set; }
    }

    /// <summary>
    /// 图像尺寸
    /// </summary>
    public class ImageSize
    {
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
    }
}
