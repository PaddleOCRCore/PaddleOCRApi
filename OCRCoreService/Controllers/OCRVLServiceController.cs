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

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using OCRCoreService.Services;
using PaddleOCRSDK;

namespace OCRCoreService.Controllers
{
    /// <summary>
    /// PaddleOCR-VL1.5接口，含版面分析
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]
    public class OCRVLServiceController : ActionBase
    {
        private readonly ILogger<OCRVLServiceController> logger;
        private readonly IOCRVLService ocrvlService;
        private readonly OCRVLConfig ocrvlConfig;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_logger">日志</param>
        /// <param name="_ocrvlEngine">OCR-VL引擎</param>
        /// <param name="_ocrvlConfig">OCR-VL配置</param>
        public OCRVLServiceController(ILogger<OCRVLServiceController> _logger, OCRVLEngine _ocrvlEngine, OCRVLConfig _ocrvlConfig)
        {
            logger = _logger;
            ocrvlService = _ocrvlEngine.OcrVlService;
            ocrvlConfig = _ocrvlConfig;
        }

        /// <summary>
        /// 检查服务状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Get()
        {
            return OKResult(new
            {
                message = "OCR-VL视觉语言识别服务已启动",
                timestamp = DateTime.Now
            });
        }

        #region OCR-VL 识别 - Base64（未启用版面分析）
        /// <summary>
        /// OCR-VL识别，上传图片Base64编码和提示词（未启用版面分析时使用）
        /// </summary>
        /// <param name="request">请求参数，包含提示词和图片Base64</param>
        /// <returns>返回识别结果文本</returns>
        [HttpPost]
        public ActionResult GetOCRVL([FromBody] RequestOCRVL request)
        {
            if (string.IsNullOrEmpty(request.Base64String))
            {
                return BadResult("识别失败:图片不存在！");
            }
            if (string.IsNullOrEmpty(request.Prompt))
            {
                return BadResult("识别失败:提示词不能为空！");
            }

            try
            {
                logger.LogInformation("开始处理OCR-VL Base64识别请求");
                VLChatResult result = ocrvlService.ChatBase64(request.Prompt, request.Base64String);
                if (result.Code != 1)
                {
                    logger.LogWarning($"OCR-VL识别失败: {result.ErrorMsg}");
                    return BadResult($"识别失败: {result.ErrorMsg}");
                }
                logger.LogInformation("OCR-VL Base64识别成功");
                if (result.Code == 1)
                {
                    return OKResult(result.Content);
                }
                else
                {
                    return BadResult(result.ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理OCR-VL Base64识别时出错");
                string error = ocrvlService.GetLastError();
                return BadResult($"识别失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }
        #endregion

        #region OCR-VL 识别 - 文件上传（未启用版面分析）
        /// <summary>
        /// OCR-VL识别，上传图片文件和提示词（未启用版面分析时使用）
        /// </summary>
        /// <param name="file">上传的图片文件</param>
        /// <param name="prompt">提示词</param>
        /// <returns>返回识别结果文本</returns>
        [HttpPost]
        public async Task<ActionResult> GetOCRVLFile(IFormFile file, [FromForm] string prompt)
        {
            if (file == null || file.Length == 0)
            {
                return BadResult("识别失败:图片文件不存在！");
            }
            if (string.IsNullOrEmpty(prompt))
            {
                return BadResult("识别失败:提示词不能为空！");
            }

            try
            {
                logger.LogInformation($"开始处理OCR-VL文件识别请求，文件名: {file.FileName}");

                // 读取文件字节并调用
                using var memStream = new MemoryStream();
                await file.CopyToAsync(memStream);
                byte[] imageData = memStream.ToArray();

                VLChatResult result = ocrvlService.ChatData(imageData, prompt);
                if (result.Code != 1)
                {
                    logger.LogWarning($"OCR-VL文件识别失败: {result.ErrorMsg}");
                    return BadResult($"识别失败: {result.ErrorMsg}");
                }
                logger.LogInformation("OCR-VL文件识别成功");
                if (result.Code == 1)
                {
                    return OKResult(result.Content);
                }
                else
                {
                    return BadResult(result.ErrorMsg);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理OCR-VL文件识别时出错");
                string error = ocrvlService.GetLastError();
                return BadResult($"识别失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }
        #endregion

        #region DOC-VL 版面分析 - Base64（启用版面分析时）
        /// <summary>
        /// DOC-VL版面分析，上传图片Base64编码（启用版面分析时使用）
        /// </summary>
        /// <param name="request">请求参数，包含图片Base64</param>
        /// <returns>返回版面分析结果（Markdown和JSON）</returns>
        [HttpPost]
        public ActionResult GetDOCVL([FromBody] RequestDocVL request)
        {
            if (string.IsNullOrEmpty(request.Base64String))
            {
                return BadResult("识别失败:图片不存在！");
            }

            try
            {
                logger.LogInformation("开始处理DOC-VL Base64版面分析请求");
                string layoutJson = ocrvlService.DetectLayoutBase64(request.Base64String);
                LayoutDetectResult layoutResult = ocrvlService.ParseLayoutResult(layoutJson);
                logger.LogInformation("DOC-VL Base64版面分析成功");
                return OKResult(layoutResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理DOC-VL Base64版面分析时出错");
                string error = ocrvlService.GetLastError();
                return BadResult($"识别失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }
        #endregion

        #region DOC-VL 版面分析 - 文件上传（启用版面分析时）
        /// <summary>
        /// DOC-VL版面分析，上传图片文件（启用版面分析时使用）
        /// </summary>
        /// <param name="file">上传的图片文件</param>
        /// <returns>返回版面分析结果（Markdown和JSON）</returns>
        [HttpPost]
        public async Task<ActionResult> GetDOCVLFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadResult("识别失败:图片文件不存在！");
            }

            try
            {
                logger.LogInformation($"开始处理DOC-VL文件版面分析请求，文件名: {file.FileName}");

                // 读取文件字节并调用
                using var memStream = new MemoryStream();
                await file.CopyToAsync(memStream);
                byte[] imageData = memStream.ToArray();

                string layoutJson = ocrvlService.DetectLayoutByte(imageData);
                LayoutDetectResult layoutResult = ocrvlService.ParseLayoutResult(layoutJson);
                logger.LogInformation("DOC-VL文件版面分析成功");
                return OKResult(layoutResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理DOC-VL文件版面分析时出错");
                string error = ocrvlService.GetLastError();
                return BadResult($"识别失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }
        #endregion
    }
}
