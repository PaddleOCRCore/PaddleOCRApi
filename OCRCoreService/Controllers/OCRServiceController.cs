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
using OCRCoreService.Services;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using PaddleOCRSDK;
using System.Text.RegularExpressions;

namespace OCRCoreService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]
    public class OCRServiceController : ActionBase
    {
        private readonly ILogger<OCRServiceController> _logger;
        private readonly OCREngine _ocrEngine;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public OCRServiceController(ILogger<OCRServiceController> logger, OCREngine ocrEngine)
        {
            _logger = logger;
            _ocrEngine = ocrEngine;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get()
        {
            return OKResult("接口已正式启用，仅支持64位模式");
        }
        #region 身份证识别
        /// <summary>
        /// 身份证识别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[TypeFilter(typeof(WebApiActionAttribute))]
        public ActionResult GetIdCard([FromServices] IWebHostEnvironment env, [FromBody] RequestOcr request)
        {
            string result = "";
            if (string.IsNullOrEmpty(request.Base64String))
            {
                return (BadResult("识别失败:图片不存在！"));
            }
            string beginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var webSiteUrl = $"UploadFile{Path.DirectorySeparatorChar}OCRService{Path.DirectorySeparatorChar}";
            string fileNameSeg = Guid.NewGuid().ToString() + ".jpg";
            string fileDir = Path.Combine(env.WebRootPath, webSiteUrl);
            string filePath = Path.Combine(fileDir, fileNameSeg);
            if (!System.IO.Directory.Exists(fileDir))
            {
                System.IO.Directory.CreateDirectory(fileDir);
            }
            //OCRResult ocrResult = engine.ocrEngine.DetectText(ImageBeauty.Base64StringToImage(request.Base64String));
            OCRResult ocrResult = _ocrEngine.OcrService.DetectBase64(request.Base64String);            
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in ocrResult.WordsResult)
            {
                if (!string.IsNullOrEmpty(item.Words))
                {
                    //if (stringBuilder.Length > 0)
                    //{
                    //    stringBuilder.Append(Environment.NewLine);
                    //}
                    stringBuilder.Append(item.Words);
            }
            }
            result=stringBuilder.ToString();
            if (request.id_card_side.Equals("front"))
            {
                var jsonResult = new
                {
                    姓名 = "",
                    性别 = "",
                    民族 = "",
                    出生 = "",
                    住址 = "",
                    公民身份号码 = "",
                    text = ""
                };
                // 定义正则表达式
                Regex regex = new Regex(@"姓名(?<name>[^\s]+)性别(?<gender>[男女])民族(?<nation>.+?)出生(?<birth>.+?)住址(?<address>.+?)公民身份号码(?<id>\d{18})");

                // 执行匹配
                Match match = regex.Match(result);
                if (match.Success)
                {
                    // 提取信息
                    string name = match.Groups["name"].Value;
                    string gender = match.Groups["gender"].Value;
                    string nation = match.Groups["nation"].Value;
                    string birth = match.Groups["birth"].Value;
                    string address = match.Groups["address"].Value;
                    string idNumber = match.Groups["id"].Value;
                    // 构建JSON对象
                    jsonResult = new
                    {
                        姓名 = name,
                        性别 = gender,
                        民族 = nation,
                        出生 = birth,
                        住址 = address,
                        公民身份号码 = idNumber,
                        text = result
                    };
                }
                return OKResult(jsonResult);
            }
            else
            {
                var jsonResult = new
                {
                    签发机关 = "",
                    有效期限 = "",
                    text= ""
                };
                // 定义正则表达式
                Regex regex = new Regex(@"签发机关(?<issuingAuthority>.+?)有效期限(?<validityPeriod>.+)$");

                // 执行匹配
                Match match = regex.Match(result);
                if (match.Success)
                {
                    // 提取信息
                    string issuingAuthority = match.Groups["issuingAuthority"].Value;
                    string validityPeriod = match.Groups["validityPeriod"].Value;
                    // 构建JSON对象
                    jsonResult = new
                    {
                        签发机关 = issuingAuthority,
                        有效期限 = validityPeriod,
                        text = result
                    };
                }
                return OKResult(jsonResult);
            }
        }
        #endregion

        #region 通用文字识别
        /// <summary>
        /// 通用文字识别
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[TypeFilter(typeof(WebApiActionAttribute))]
        public ActionResult GetOCRText([FromServices] IWebHostEnvironment env, [FromBody] RequestOcr request)
        {
            string result = "";
            if (string.IsNullOrEmpty(request.Base64String))
            {
                return (BadResult("识别失败:图片不存在！"));
            }
            OCRResult ocrResult = _ocrEngine.OcrService.DetectBase64(request.Base64String);
            if (request.ResultType.Equals("text", StringComparison.OrdinalIgnoreCase))
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var item in ocrResult.WordsResult)
                {
                    if (!string.IsNullOrEmpty(item.Words))
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                        stringBuilder.Append(item.Words);
                    }
                }
                result = stringBuilder.ToString();
            }
            else
            {
                result = ocrResult.JsonText;
            }
            return OKResult(result);
        }
        #endregion
    }
}
