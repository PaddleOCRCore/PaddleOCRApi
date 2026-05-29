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
using System.Threading.Tasks;

namespace OCRCoreService.Controllers
{
    /// <summary>
    /// PaddleOCR、PP-Structure通用接口
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]
    public class OCRServiceController : ActionBase
    {
        private readonly ILogger<OCRServiceController> logger;
        private readonly OCREngine ocrEngine;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_logger"></param>
        /// <param name="_ocrEngine"></param>
        public OCRServiceController(ILogger<OCRServiceController> _logger, OCREngine _ocrEngine)
        {
            logger = _logger;
            ocrEngine = _ocrEngine;
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
        public ActionResult GetIdCard([FromServices] IWebHostEnvironment env, [FromBody] RequestIdOcr request)
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
            OCRResult ocrResult = ocrEngine.OcrService.DetectBase64(request.Base64String);            
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in ocrResult.WordsResult)
            {
                if (!string.IsNullOrEmpty(item.Words))
                {
                    //if (stringBuilder.Length > 0)
                    //{
                    //    stringBuilder.Append(Environment.NewLine);
                    //}
                    if (item.Words.Contains("性别"))
                    {
                        stringBuilder.Append(",");
                    }
                    else if (item.Words.Contains("民族"))
                    {
                        stringBuilder.Append(",");
                    }
                    else if (item.Words.Contains("出生"))
                    {
                        stringBuilder.Append(",");
                    }
                    else if (item.Words.Contains("住址"))
                    {
                        stringBuilder.Append(",");
                    }
                    else if (item.Words.Contains("号码"))
                    {
                        stringBuilder.Append(",");
                    }
                    stringBuilder.Append(item.Words);
            }
            }
            result=stringBuilder.ToString();
            logger.LogTrace(result);
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
                Regex regex = new Regex(@"姓名(?<name>[^\s]+),性别(?<gender>[男女]),民族(?<nation>.+?),出生(?<birth>.+?),住址(?<address>.+?),公民身份号码(?<idNumber>\d{17}[\dXx])");

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
                    string idNumber = match.Groups["idNumber"].Value;
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
                    logger.LogTrace($"身份证头像面匹配成功:{jsonResult.ToString()}");
                }
                else
                {
                    jsonResult = new
                    {
                        姓名 = "",
                        性别 = "",
                        民族 = "",
                        出生 = "",
                        住址 = "",
                        公民身份号码 = "",
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
                    text= result
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
                    logger.LogTrace($"身份证国徽面匹配成功:{jsonResult.ToString()}");
                }
                return OKResult(jsonResult);
            }
        }
        #endregion

        #region 通用文字识别
        /// <summary>
        /// 通用文字识别，上传图片Base64编码
        /// </summary>
        /// <param name="request">RequestOcrByte.Base64String：Base64编码</param>
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
            OCRResult ocrResult = ocrEngine.OcrService.DetectBase64(request.Base64String);
            logger.LogTrace($"OCR识别成功:{ocrResult.JsonText}");
            if (string.Equals(request.ResultType, "text", StringComparison.OrdinalIgnoreCase))
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

        /// <summary>
        /// 通用文字识别，上传图片字节码
        /// </summary>
        /// <param name="request">RequestOcrByte.ImageByte：byte[]字节码</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOCRByte([FromBody] RequestOcrByte request)
        {
            string result = "";
            if (request == null || request.ImageByte == null || request.ImageByte.Length == 0)
            {
                return (BadResult("识别失败:图片不存在！"));
            }

            OCRResult ocrResult = ocrEngine.OcrService.Detect(request.ImageByte);
            logger.LogTrace($"OCR识别成功:{ocrResult.JsonText}");
            if (string.Equals(request.ResultType, "text", StringComparison.OrdinalIgnoreCase))
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

        #region 通用文字识别
        /// <summary>
        /// 通用文字识别，直接上传图片即可，无需保存图片
        /// </summary>
        /// <param name="request">上传文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetOCRFile(IFormFile request)
        {
            string result = "";
            if (request.Length==0)
            {
                return (BadResult("识别失败:图片不存在！"));
            }
            using (MemoryStream ms = new MemoryStream())
            {
                await request.CopyToAsync(ms);
                var imageByte = ms.ToArray();
                logger.LogTrace($"获取到图片:{imageByte.ToString()}");
                OCRResult ocrResult = ocrEngine.OcrService.Detect(imageByte);
                logger.LogTrace($"OCR识别成功:{ocrResult.JsonText}");
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
            return OKResult(result);
        }
        /// <summary>
        /// 通用文字识别，直接上传图片即可，无需保存图片，返回json
        /// </summary>
        /// <param name="request">上传文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetOCRJsonFile(IFormFile request)
        {
            string result = "";
            if (request.Length == 0)
            {
                return (BadResult("识别失败:图片不存在！"));
            }
            using (MemoryStream ms = new MemoryStream())
            {
                await request.CopyToAsync(ms);
                var imageByte = ms.ToArray();
                logger.LogTrace($"获取到图片:{imageByte.ToString()}");
                OCRResult ocrResult = ocrEngine.OcrService.Detect(imageByte);
                logger.LogTrace($"OCR识别成功:{ocrResult.JsonText}");
                result = ocrResult.JsonText;
            }
            return OKResult(result);
        }
        #endregion

        #region 版面识别
        /// <summary>
        /// 文档版面识别（Base64）
        /// </summary>
        /// <param name="request">版面识别请求参数</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetOCRLayout([FromBody] RequestLayoutOcr request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Base64String))
            {
                return BadResult("识别失败:图片不存在！");
            }

            try
            {
                string initMessage = ocrEngine.EnsureStructureEngine();
                if (!string.IsNullOrWhiteSpace(initMessage))
                {
                    return BadResult("版面识别引擎初始化失败:" + initMessage);
                }

                string layoutJson = ocrEngine.OcrService.DetectLayoutBase64(request.Base64String);
                logger.LogTrace($"版面识别成功:{layoutJson}");
                return OKResult(layoutJson);
            }
            catch (Exception ex)
            {
                return BadResult("版面识别失败:" + ex.Message);
            }
        }

        /// <summary>
        /// 文档版面识别（文件上传）
        /// </summary>
        /// <param name="request">上传文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetOCRLayoutFile(IFormFile request)
        {
            if (request == null || request.Length == 0)
            {
                return BadResult("识别失败:图片不存在！");
            }

            try
            {
                string initMessage = ocrEngine.EnsureStructureEngine();
                if (!string.IsNullOrWhiteSpace(initMessage))
                {
                    return BadResult("版面识别引擎初始化失败:" + initMessage);
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    await request.CopyToAsync(ms);
                    byte[] imageByte = ms.ToArray();
                    string layoutJson = ocrEngine.OcrService.DetectLayoutByte(imageByte);
                    logger.LogTrace($"版面识别成功:{layoutJson}");
                    return OKResult(layoutJson);
                }
            }
            catch (Exception ex)
            {
                return BadResult("版面识别失败:" + ex.Message);
            }
        }
        #endregion
    }
}
