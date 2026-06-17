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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OCRCoreService.Services;
using PaddleOCRSDK;
using PDFtoImage;
using SkiaSharp;
using System.Text;

namespace OCRCoreService.Controllers
{
    /// <summary>
    /// PaddleOCR 在线 demo支持PaddleOCRv5、PP-Structure、PaddleOCR-VL1.5
    /// </summary>
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]/[action]")]
    public class OCRDemoController : ActionBase
    {
        private const long MaxImageSize = 10L * 1024 * 1024;
        private const long MaxPdfSize = 200L * 1024 * 1024;
        private const int MaxPdfPages = 1000;
        private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".bmp",
            ".tif",
            ".tiff"
        };
        private static readonly HashSet<string> SupportedUploadExtensions = new(SupportedImageExtensions, StringComparer.OrdinalIgnoreCase)
        {
            ".pdf"
        };

        private readonly ILogger<OCRDemoController> logger;
        private readonly OCREngine ocrEngine;
        private readonly IServiceProvider serviceProvider;

        public OCRDemoController(
            ILogger<OCRDemoController> logger,
            OCREngine ocrEngine,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.ocrEngine = ocrEngine;
            this.serviceProvider = serviceProvider;
        }

        /// <summary>
        /// WebApi首页Demo调用专用接口
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Analyze(IFormFile file, [FromForm] string model, [FromForm] int pageIndex = 1)
        {
            string modelKey = NormalizeModel(model);
            if (string.IsNullOrWhiteSpace(modelKey))
            {
                return BadResult("请选择解析模型。");
            }

            if (file == null || file.Length == 0)
            {
                return BadResult("解析失败：文件不存在。");
            }

            string extension = Path.GetExtension(file.FileName);
            if (!SupportedUploadExtensions.Contains(extension))
            {
                return BadResult("解析失败：当前演示支持 PDF、PNG、JPG、BMP、TIF 文件。");
            }

            if (IsPdf(extension))
            {
                if (file.Length > MaxPdfSize)
                {
                    return BadResult("解析失败：PDF 文件不能超过 200MB。");
                }
            }
            else if (file.Length > MaxImageSize)
            {
                return BadResult("解析失败：单张图片不能超过 10MB。");
            }

            try
            {
                using var memStream = new MemoryStream();
                await file.CopyToAsync(memStream);
                byte[] uploadData = memStream.ToArray();
                bool isPdf = IsPdf(extension);
                int pageCount = 1;
                int currentPageIndex = 1;
                byte[] imageData;
                if (isPdf)
                {
                    pageCount = GetPdfPageCount(uploadData);
                    if (pageCount <= 0)
                    {
                        throw new InvalidOperationException("PDF has no pages.");
                    }

                    if (pageCount > MaxPdfPages)
                    {
                        throw new InvalidOperationException($"PDF page count cannot exceed {MaxPdfPages}.");
                    }

                    currentPageIndex = Math.Clamp(pageIndex, 1, pageCount);
                    imageData = ConvertPdfPageToPng(uploadData, currentPageIndex);
                }
                else
                {
                    imageData = uploadData;
                }

                logger.LogInformation("开始处理 Demo 解析请求，模型: {Model}, 文件名: {FileName}", modelKey, file.FileName);

                OCRDemoAnalyzeResult result = modelKey switch
                {
                    "paddleocr-vl-1.5" => AnalyzeWithPaddleOCRVL(imageData, file.FileName),
                    "paddleocr-vl-1.6" => AnalyzeWithPaddleOCRVL(imageData, file.FileName),
                    "pp-ocrv5" => AnalyzeWithPPOCRv5(imageData, file.FileName),
                    "pp-ocrv6" => AnalyzeWithPPOCRv6(imageData, file.FileName),
                    "pp-structure" => AnalyzeWithPPStructure(imageData, file.FileName),
                    _ => throw new InvalidOperationException("不支持的解析模型。")
                };

                if (isPdf)
                {
                    result.PageIndex = currentPageIndex;
                    result.PageCount = pageCount;
                    result.PreviewImage = "data:image/png;base64," + Convert.ToBase64String(imageData);
                }

                return OKResult(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Demo 解析失败，模型: {Model}, 文件名: {FileName}", modelKey, file.FileName);
                return BadResult("解析失败：" + ex.Message);
            }
        }

        private OCRDemoAnalyzeResult AnalyzeWithPaddleOCRVL(byte[] imageData, string fileName)
        {
            OCRVLEngine? ocrvlEngine = serviceProvider.GetService<OCRVLEngine>();
            if (ocrvlEngine == null)
            {
                throw new InvalidOperationException("OCR-VL 服务未启用，请检查 OCRVLConfig.enabled 配置。");
            }

            IOCRVLService ocrvlService = ocrvlEngine.OcrVlService;
            string layoutJson = ocrvlService.DetectLayoutByte(imageData);
            LayoutDetectResult layoutResult = ocrvlService.ParseLayoutResult(layoutJson);
            return CreateLayoutResponse("paddleocr-vl-1.6", "PaddleOCR-VL-1.6", fileName, layoutJson, layoutResult);
        }

        private OCRDemoAnalyzeResult AnalyzeWithPPOCRv5(byte[] imageData, string fileName)
        {
            OCRResult ocrResult = ocrEngine.OcrService.Detect(imageData);
            if (ocrResult.Code != 1)
            {
                throw new InvalidOperationException(ocrResult.ErrorMsg);
            }

            string text = BuildOcrText(ocrResult);
            return new OCRDemoAnalyzeResult
            {
                Model = "pp-ocrv5",
                ModelName = "PP-OCRv5",
                FileName = fileName,
                Content = text,
                Markdown = text,
                JsonText = ocrResult.JsonText ?? string.Empty,
                Raw = ocrResult,
                Boxes = BuildOcrBoxes(ocrResult)
            };
        }

        private OCRDemoAnalyzeResult AnalyzeWithPPOCRv6(byte[] imageData, string fileName)
        {
            OCRResult ocrResult = ocrEngine.OcrService.Detect(imageData);
            if (ocrResult.Code != 1)
            {
                throw new InvalidOperationException(ocrResult.ErrorMsg);
            }

            string text = BuildOcrText(ocrResult);
            return new OCRDemoAnalyzeResult
            {
                Model = "pp-ocrv6",
                ModelName = "PP-OCRv6",
                FileName = fileName,
                Content = text,
                Markdown = text,
                JsonText = ocrResult.JsonText ?? string.Empty,
                Raw = ocrResult,
                Boxes = BuildOcrBoxes(ocrResult)
            };
        }

        private OCRDemoAnalyzeResult AnalyzeWithPPStructure(byte[] imageData, string fileName)
        {
            string initMessage = ocrEngine.EnsureStructureEngine();
            if (!string.IsNullOrWhiteSpace(initMessage))
            {
                throw new InvalidOperationException("版面分析引擎初始化失败：" + initMessage);
            }

            string layoutJson = ocrEngine.OcrService.DetectLayoutByte(imageData);
            LayoutDetectResult layoutResult = ocrEngine.OcrService.ParseLayoutResult(layoutJson);
            return CreateLayoutResponse("pp-structure", "PP-Structure", fileName, layoutJson, layoutResult);
        }

        private static bool IsPdf(string extension)
        {
            return string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase);
        }

        private static int GetPdfPageCount(byte[] pdfData)
        {
            using MemoryStream pdfStream = new(pdfData);
            return Conversion.GetPageCount(pdfStream, leaveOpen: false);
        }

        private static byte[] ConvertPdfPageToPng(byte[] pdfData, int pageIndex)
        {
            int pageCount = GetPdfPageCount(pdfData);
            if (pageCount <= 0)
            {
                throw new InvalidOperationException("PDF 没有可解析页面。");
            }

            if (pageCount > MaxPdfPages)
            {
                throw new InvalidOperationException("PDF 页数不能超过 1000 页。");
            }

            using MemoryStream pdfStream = new(pdfData);
            using SKBitmap bitmap = Conversion.ToImage(
                pdfStream,
                page: pageIndex - 1,
                leaveOpen: false,
                password: null,
                options: new RenderOptions(Dpi: 180));
            using SKImage image = SKImage.FromBitmap(bitmap);
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        private static OCRDemoAnalyzeResult CreateLayoutResponse(
            string model,
            string modelName,
            string fileName,
            string layoutJson,
            LayoutDetectResult layoutResult)
        {
            string markdown = ExtractLayoutText(layoutResult);
            return new OCRDemoAnalyzeResult
            {
                Model = model,
                ModelName = modelName,
                FileName = fileName,
                Content = markdown,
                Markdown = markdown,
                JsonText = layoutJson,
                Raw = layoutResult,
                Boxes = BuildLayoutBoxes(layoutResult)
            };
        }

        private static string NormalizeModel(string model)
        {
            return (model ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "paddleocr-vl-1.5" or "paddleocr-vl" or "vl" => "paddleocr-vl-1.5",
                "paddleocr-vl-1.6" or "paddleocr-vl" or "vl" => "paddleocr-vl-1.6",
                "pp-ocrv5" or "ppocrv5" or "ocr" => "pp-ocrv5",
                "pp-ocrv6" or "ppocrv6" => "pp-ocrv6",
                "pp-structure" or "ppstructure" or "structure" => "pp-structure",
                _ => string.Empty
            };
        }

        private static string BuildOcrText(OCRResult ocrResult)
        {
            StringBuilder builder = new();
            foreach (PaddleOCRSDK.JsonResult item in ocrResult.WordsResult)
            {
                if (string.IsNullOrWhiteSpace(item.Words))
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder.Append(item.Words.Trim());
            }

            return builder.ToString();
        }

        private static List<OCRDemoBox> BuildOcrBoxes(OCRResult ocrResult)
        {
            List<OCRDemoBox> boxes = new();
            foreach (PaddleOCRSDK.JsonResult item in ocrResult.WordsResult)
            {
                var points = item.Location
                    .Where(point => point.x.HasValue && point.y.HasValue)
                    .ToList();
                if (points.Count == 0)
                {
                    continue;
                }

                int left = points.Min(point => point.x!.Value);
                int top = points.Min(point => point.y!.Value);
                int right = points.Max(point => point.x!.Value);
                int bottom = points.Max(point => point.y!.Value);
                boxes.Add(new OCRDemoBox
                {
                    Label = "text",
                    Text = item.Words ?? string.Empty,
                    X = left,
                    Y = top,
                    Width = Math.Max(0, right - left),
                    Height = Math.Max(0, bottom - top),
                    Score = item.Score
                });
            }

            return boxes;
        }

        private static List<OCRDemoBox> BuildLayoutBoxes(LayoutDetectResult layoutResult)
        {
            List<OCRDemoBox> boxes = new();
            if (layoutResult.ParsingResList != null)
            {
                foreach (LayoutBlockResult block in layoutResult.ParsingResList)
                {
                    OCRDemoBox? box = CreateBoxFromBbox(block.BlockBbox);
                    if (box == null)
                    {
                        continue;
                    }

                    box.BlockId = block.BlockId;
                    box.BlockOrder = block.BlockOrder;
                    box.Label = block.BlockLabel ?? "block";
                    box.Text = block.BlockContent ?? string.Empty;
                    box.Score = block.Score;
                    boxes.Add(box);
                }
            }

            if (boxes.Count == 0 && layoutResult.LayoutDetRes?.Boxes != null)
            {
                foreach (LayoutDetectionBox block in layoutResult.LayoutDetRes.Boxes)
                {
                    OCRDemoBox? box = CreateBoxFromBbox(block.Coordinate);
                    if (box == null)
                    {
                        continue;
                    }

                    box.Label = block.Label ?? "block";
                    box.Score = block.Score;
                    boxes.Add(box);
                }
            }

            return boxes;
        }

        private static OCRDemoBox? CreateBoxFromBbox(IReadOnlyList<double>? bbox)
        {
            if (bbox == null || bbox.Count < 4)
            {
                return null;
            }

            double left = Math.Min(bbox[0], bbox[2]);
            double top = Math.Min(bbox[1], bbox[3]);
            double right = Math.Max(bbox[0], bbox[2]);
            double bottom = Math.Max(bbox[1], bbox[3]);
            return new OCRDemoBox
            {
                X = left,
                Y = top,
                Width = Math.Max(0, right - left),
                Height = Math.Max(0, bottom - top)
            };
        }

        private static string ExtractLayoutText(LayoutDetectResult layoutResult)
        {
            if (!string.IsNullOrWhiteSpace(layoutResult.Markdown))
            {
                return layoutResult.Markdown.Trim();
            }

            StringBuilder builder = new();
            if (layoutResult.ParsingResList != null)
            {
                foreach (LayoutBlockResult block in layoutResult.ParsingResList
                    .OrderBy(block => block.BlockOrder ?? block.BlockId ?? int.MaxValue))
                {
                    if (string.IsNullOrWhiteSpace(block.BlockContent))
                    {
                        continue;
                    }

                    if (builder.Length > 0)
                    {
                        builder.AppendLine().AppendLine();
                    }

                    builder.Append(block.BlockContent.Trim());
                }
            }

            if (builder.Length == 0 && layoutResult.OverallOcrRes?.RecTexts != null)
            {
                foreach (string text in layoutResult.OverallOcrRes.RecTexts)
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        continue;
                    }

                    if (builder.Length > 0)
                    {
                        builder.AppendLine();
                    }

                    builder.Append(text.Trim());
                }
            }

            return builder.ToString();
        }
    }

    public class OCRDemoAnalyzeResult
    {
        public string Model { get; set; } = string.Empty;

        public string ModelName { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public string Markdown { get; set; } = string.Empty;

        public string JsonText { get; set; } = string.Empty;

        public string PreviewImage { get; set; } = string.Empty;

        public int? PageIndex { get; set; }

        public int? PageCount { get; set; }

        public object? Raw { get; set; }

        public List<OCRDemoBox> Boxes { get; set; } = new();
    }

    public class OCRDemoBox
    {
        public string Label { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public int? BlockId { get; set; }

        public int? BlockOrder { get; set; }

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public double? Score { get; set; }
    }
}
