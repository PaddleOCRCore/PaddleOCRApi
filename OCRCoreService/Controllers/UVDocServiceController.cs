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
using PaddleOCRSDK;

namespace OCRCoreService.Controllers
{
    /// <summary>
    /// UVDoc文本图像矫正服务控制器
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]
    public class UVDocServiceController : ActionBase
    {
        private readonly ILogger<UVDocServiceController> logger;
        private readonly IUVDocService uvdocService;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="_logger">日志</param>
        /// <param name="_uvdocService">UVDoc服务</param>
        public UVDocServiceController(ILogger<UVDocServiceController> _logger, IUVDocService _uvdocService)
        {
            logger = _logger;
            uvdocService = _uvdocService;
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
                message = "UVDoc文档矫正服务已启动",
                initialized = uvdocService.IsInitialized,
                timestamp = DateTime.Now
            });
        }

        #region 文本图像矫正 - Base64
        /// <summary>
        /// 文本图像矫正，上传图片Base64编码
        /// </summary>
        /// <param name="request">请求参数</param>
        /// <returns>返回矫正后的图像Base64编码</returns>
        [HttpPost]
        public async Task<ActionResult> UVDocBase64([FromBody] RequestUVDocBase64 request)
        {
            if (string.IsNullOrEmpty(request.Base64String))
            {
                return BadResult("矫正失败:图片不存在！");
            }

            try
            {
                logger.LogInformation("开始处理Base64文档矫正请求");

                // 创建临时目录
                string tempDir = Path.Combine(Path.GetTempPath(), "UVDoc");
                Directory.CreateDirectory(tempDir);

                // 生成输出文件路径
                string outputPath = Path.Combine(tempDir, $"uvdoc_output_{Guid.NewGuid()}.jpg");

                // 处理Base64图像
                await uvdocService.UVDocBase64ImageAsync(request.Base64String, outputPath);

                // 读取结果并转换为Base64
                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
                string base64Output = Convert.ToBase64String(imageBytes);

                // 清理临时文件
                System.IO.File.Delete(outputPath);

                logger.LogInformation("Base64文档矫正成功");

                return OKResult(new
                {
                    base64Image = base64Output,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理Base64文档矫正时出错");
                string error = uvdocService.GetLastError();
                return BadResult($"矫正失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }
        #endregion

        #region 文本图像矫正 - 文件上传
        /// <summary>
        /// 文本图像矫正，直接上传图片文件
        /// </summary>
        /// <param name="file">上传的图片文件</param>
        /// <returns>返回矫正后的图像文件</returns>
        [HttpPost]
        public async Task<ActionResult> UVDocFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadResult("矫正失败:图片文件不存在！");
            }

            try
            {
                logger.LogInformation($"开始处理文件上传文档矫正请求，文件名: {file.FileName}");

                // 创建临时目录
                string tempDir = Path.Combine(Path.GetTempPath(), "UVDoc");
                Directory.CreateDirectory(tempDir);

                // 保存上传的文件
                string inputPath = Path.Combine(tempDir, $"uvdoc_input_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
                string outputPath = Path.Combine(tempDir, $"uvdoc_output_{Guid.NewGuid()}.jpg");

                using (var stream = new FileStream(inputPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 处理图像
                await uvdocService.UVDocImageFileAsync(inputPath, outputPath);

                // 读取结果
                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(outputPath);

                // 清理临时文件
                System.IO.File.Delete(inputPath);
                System.IO.File.Delete(outputPath);

                logger.LogInformation("文件上传文档矫正成功");

                return File(imageBytes, "image/jpeg", "corrected.jpg");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理文件上传文档矫正时出错");
                string error = uvdocService.GetLastError();
                return BadResult($"矫正失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }

        /// <summary>
        /// 文本图像矫正，直接上传图片文件，返回Base64
        /// </summary>
        /// <param name="file">上传的图片文件</param>
        /// <returns>返回矫正后的图像Base64编码</returns>
        [HttpPost]
        public async Task<ActionResult> UVDocFileToBase64(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadResult("矫正失败:图片文件不存在！");
            }

            try
            {
                logger.LogInformation($"开始处理文件上传文档矫正请求(返回Base64)，文件名: {file.FileName}");

                // 创建临时目录
                string tempDir = Path.Combine(Path.GetTempPath(), "UVDoc");
                Directory.CreateDirectory(tempDir);

                // 保存上传的文件
                string inputPath = Path.Combine(tempDir, $"uvdoc_input_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");
                string outputPath = Path.Combine(tempDir, $"uvdoc_output_{Guid.NewGuid()}.jpg");

                using (var stream = new FileStream(inputPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // 处理图像
                await uvdocService.UVDocImageFileAsync(inputPath, outputPath);

                // 读取结果并转换为Base64
                byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(outputPath);
                string base64Output = Convert.ToBase64String(imageBytes);

                // 清理临时文件
                System.IO.File.Delete(inputPath);
                System.IO.File.Delete(outputPath);

                logger.LogInformation("文件上传文档矫正成功(返回Base64)");

                return OKResult(new
                {
                    base64Image = base64Output,
                    originalFileName = file.FileName,
                    timestamp = DateTime.Now
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理文件上传文档矫正时出错(返回Base64)");
                string error = uvdocService.GetLastError();
                return BadResult($"矫正失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }

        /// <summary>
        /// 文本图像矫正，上传字节数组
        /// </summary>
        /// <param name="file">上传的图片文件</param>
        /// <returns>返回矫正后的图像字节数组</returns>
        [HttpPost]
        public async Task<ActionResult> UVDocBytes(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadResult("矫正失败:图片文件不存在！");
            }

            try
            {
                logger.LogInformation($"开始处理字节数组文档矫正请求");

                // 创建临时目录
                string tempDir = Path.Combine(Path.GetTempPath(), "UVDoc");
                Directory.CreateDirectory(tempDir);

                // 读取上传的文件为字节数组
                byte[] imageBytes;
                using (var ms = new MemoryStream())
                {
                    await file.CopyToAsync(ms);
                    imageBytes = ms.ToArray();
                }

                // 生成输出文件路径
                string outputPath = Path.Combine(tempDir, $"uvdoc_output_{Guid.NewGuid()}.jpg");

                // 处理字节数组
                await uvdocService.UVDocImageBytesAsync(imageBytes, outputPath);

                // 读取结果
                byte[] resultBytes = await System.IO.File.ReadAllBytesAsync(outputPath);

                // 清理临时文件
                System.IO.File.Delete(outputPath);

                logger.LogInformation("字节数组文档矫正成功");

                return File(resultBytes, "image/jpeg", "corrected.jpg");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "处理字节数组文档矫正时出错");
                string error = uvdocService.GetLastError();
                return BadResult($"矫正失败: {ex.Message}{(string.IsNullOrEmpty(error) ? "" : $" | DLL错误: {error}")}");
            }
        }
        #endregion
    }

    /// <summary>
    /// UVDoc Base64请求参数
    /// </summary>
    public class RequestUVDocBase64
    {
        /// <summary>
        /// 图片Base64编码
        /// </summary>
        public string Base64String { get; set; } = string.Empty;
    }
}
