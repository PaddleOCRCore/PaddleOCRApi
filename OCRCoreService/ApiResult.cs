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

using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.InteropServices;

namespace OCRCoreService
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionBase : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OResult"></param>
        /// <returns></returns>
        protected ObjectResult OKResult(object OResult)
        {
            ApiResult result = new ApiResult();
            result.Status = HttpStatusCode.OK;
            result.Data = OResult;
            result.ErrorMessage = "";
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="OResult"></param>
        /// <returns></returns>
        protected ObjectResult BadResult(object OResult)
        {
            ApiResult result = new ApiResult();
            result.Status = HttpStatusCode.BadRequest;
            result.Data = "";
            result.ErrorMessage = OResult.ToString();
            return Ok(result);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode Status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ErrorMessage { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestIdOcr
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// front：身份证含照片的一面；back：身份证带国徽的一面。
        /// </summary>
        public string id_card_side { get; set; } = "front";
    }

    /// <summary>
    /// 
    /// </summary>
    public class RequestOcr
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ResultType { get; set; } = "text";
    }

    /// <summary>
    /// 
    /// </summary>
    public class OCRConfig
    {
        /// <summary>
        /// det_infer模型路径
        /// </summary>
        public string det_infer { get; set; }
        /// <summary>
        /// cls_infer模型路径
        /// </summary>
        public string cls_infer { get; set; }
        /// <summary>
        /// rec_infer模型路径
        /// </summary>
        public string rec_infer { get; set; }
        /// <summary>
        /// ppocr_keys.txt文件名全路径
        /// </summary>
        public string keyFile { get; set; }
        /// <summary>
        /// 表格识别模型inference model地址
        /// </summary>
        public string table_model_dir { get; set; }
        /// <summary>
        /// 表格识别字典文件
        /// </summary>
        public string table_dict_path { get; set; }
        /// <summary>
        /// 是否使用GPU
        /// </summary>
        public bool use_gpu { get; set; } = false;
        /// <summary>
        /// 是否使用mkldnn库
        /// </summary>
        public bool enable_mkldnn { get; set; } = true;
        /// <summary>
        /// GPU id，使用GPU时有效
        /// </summary>
        public int gpu_id { get; set; } = 0;
        /// <summary>
        /// 使用GPU时内存
        /// </summary>
        public int gpu_mem { get; set; } = 4000;
        /// <summary>
        /// CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        /// </summary>
        public int cpu_mem { get; set; } = 0;
        /// <summary>
        /// CPU预测时的线程数，在机器核数充足的情况下，该值越大，预测速度越快
        /// </summary>
        public int cpu_threads { get; set; } = 30;
    }
}
