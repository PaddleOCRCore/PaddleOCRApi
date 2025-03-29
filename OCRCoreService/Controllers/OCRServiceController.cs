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
using Microsoft.AspNetCore.Cors.Infrastructure;

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
        private readonly IOCRService _ocrEngine;

        private static string det_infer = "ch_PP-OCRv4_det_infer";//OCR检测模型
        private static string cls_infer = "ch_ppocr_mobile_v2.0_cls_infer";
        private static string rec_infer = "ch_PP-OCRv4_rec_infer";//OCR识别模型
        private static string keys = "ppocr_keys.txt";
        public static bool use_gpu = false;//是否使用GPU
        public static int cpu_mem = 0;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        public static int gpu_id = 0;//GPUId
        private static bool enable_mkldnn = true;
        public static int cpu_threads = 30; //CPU预测时的线程数
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public OCRServiceController(ILogger<OCRServiceController> logger, IOCRService ocrEngine)
        {
            _logger = logger;
            _ocrEngine = ocrEngine;
            InitOCREngine();
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
        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        /// <returns></returns>
        private string InitOCREngine()
        {
            InitParamater para = new InitParamater();
            string root = AppDomain.CurrentDomain.BaseDirectory;
            string modelPathroot = Path.Combine(root, "models");//存放模型的目录，不允许修改
            para.det_infer = Path.Combine(modelPathroot, det_infer);
            para.cls_infer = Path.Combine(modelPathroot, cls_infer);
            para.rec_infer = Path.Combine(modelPathroot, rec_infer);
            para.keyFile = Path.Combine(modelPathroot, keys);

            OCRParameter oCRParameter = new OCRParameter();
            oCRParameter.use_gpu = use_gpu;
            oCRParameter.use_tensorrt = false;
            oCRParameter.gpu_id = gpu_id;
            oCRParameter.gpu_mem = 4000;
            oCRParameter.cpu_mem = cpu_mem;
            oCRParameter.cpu_threads = cpu_threads;//提升CPU速度，优化此参数
            oCRParameter.enable_mkldnn = enable_mkldnn;
            oCRParameter.cls = false;
            oCRParameter.det = true;
            oCRParameter.use_angle_cls = false;
            oCRParameter.det_db_score_mode = true;
            oCRParameter.max_side_len = 960;
            oCRParameter.rec_img_h = 48;
            oCRParameter.rec_img_w = 320;
            oCRParameter.det_db_thresh = 0.3f;
            oCRParameter.det_db_box_thresh = 0.618f;
            oCRParameter.visualize = true;

            para.ocrpara = oCRParameter;
            para.paraType = EnumParaType.Class;
            //string ocrJson = "{\"use_gpu\": true,\"gpu_id\": 0,\"gpu_mem\": 4000,\"enable_mkldnn\": true,\"rec_img_h\": 48,\"rec_img_w\": 320,\"cls\":false,\"det\":true,\"use_angle_cls\":false}";
            //初始化通用文字引擎
            string msg = "初始化成功";
            try
            {
                _ocrEngine.Init(para);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
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
            OCRResult ocrResult = _ocrEngine.DetectBase64(request.Base64String);            
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

            OCRResult ocrResult = _ocrEngine.DetectBase64(request.Base64String);
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
