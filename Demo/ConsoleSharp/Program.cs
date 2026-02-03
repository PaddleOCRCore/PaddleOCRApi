using PaddleOCRSDK;
using System;
using System.Diagnostics;
using System.Text;

namespace ConsoleSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("=== PaddleOCR 批量识别程序 ===\n");

            // 初始化OCR引擎
            InitializeOCREngine();

            // 获取图片目录
            Console.WriteLine("请输入图片所在目录路径 (按Enter使用当前images目录):");
            string? imagePath = Console.ReadLine();            
            if (string.IsNullOrWhiteSpace(imagePath))
            {
                imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"images");
            }
            // 验证目录是否存在
            if (!Directory.Exists(imagePath))
            {
                Console.WriteLine($"错误：目录 '{imagePath}' 不存在！");
                return;
            }

            // 获取目录下所有图片
            string[] imageExtensions = { "*.jpg", "*.jpeg", "*.png", "*.bmp", "*.gif" };
            List<string> imageFiles = new List<string>();

            foreach (string extension in imageExtensions)
            {
                imageFiles.AddRange(Directory.GetFiles(imagePath, extension, SearchOption.TopDirectoryOnly));
            }

            if (imageFiles.Count == 0)
            {
                Console.WriteLine($"目录 '{imagePath}' 中没有找到任何图片文件");
                return;
            }

            Console.WriteLine($"\n找到 {imageFiles.Count} 张图片，开始识别...\n");

            // 处理每张图片
            int successCount = 0;
            int failureCount = 0;

            foreach (string imageFile in imageFiles)
            {
                string fileName = Path.GetFileName(imageFile);
                Console.WriteLine($"正在处理: {fileName}");

                string result = RecognizeImage(imageFile);

                if (!string.IsNullOrEmpty(result))
                {
                    successCount++;
                    Console.WriteLine($"识别结果:\n{result}\n");
                    Console.WriteLine(new string('-', 50) + "\n");
                }
                else
                {
                    failureCount++;
                    Console.WriteLine("识别失败\n");
                }
            }

            // 输出统计结果
            Console.WriteLine("\n=== 识别完成 ===");
            Console.WriteLine($"成功: {successCount}");
            Console.WriteLine($"失败: {failureCount}");
            Console.WriteLine("\n按任意键退出...");
            Console.ReadKey();
        }

        /// <summary>
        /// 初始化OCR引擎
        /// </summary>
        static void InitializeOCREngine()
        {
            Console.WriteLine("正在初始化OCR引擎...");
            try
            {
                // 初始化
                IOCRService ocrService = new OCRService();
                ocrService.EnableLog(false);
                ocrService.InitDefaultOCREngine("models",true);

                Console.WriteLine("OCR引擎初始化成功！\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化失败: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 识别单张图片
        /// </summary>
        static string RecognizeImage(string filePath)
        {
            try
            {
                IOCRService ocrService = new OCRService();
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // 调用OCR识别
                OCRResult ocrResult = ocrService.Detect(filePath);

                stopwatch.Stop();

                if (ocrResult.Code == 1 && ocrResult.WordsResult.Count > 0)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in ocrResult.WordsResult)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                        stringBuilder.Append(item.Words);
                    }

                    Console.WriteLine($"识别耗时: {stopwatch.ElapsedMilliseconds}ms");
                    return stringBuilder.ToString();
                }
                else
                {
                    Console.WriteLine($"错误: {ocrResult.ErrorMsg}");
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"异常: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
