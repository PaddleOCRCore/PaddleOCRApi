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
    public class RequestOcr
    {
        /// <summary>
        /// 图片Base64字符串
        /// </summary>
        public string Base64String { get; set; }
        /// <summary>
        /// front：身份证含照片的一面；back：身份证带国徽的一面。
        /// </summary>
        public string id_card_side { get; set; } = "front";
        /// <summary>
        /// 返回类型
        /// </summary>
        public string ResultType { get; set; } = "text";
    }
}
