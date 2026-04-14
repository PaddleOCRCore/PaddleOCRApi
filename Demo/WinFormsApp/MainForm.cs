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

using PaddleOCRSDK;
using SkiaSharp;
using Microsoft.Win32.SafeHandles;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using WinFormsApp.Services;
using WinFormsApp.Utils;


namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        StringBuilder message = new StringBuilder();
        private readonly IOCRService ocrService;
        private readonly IOCRVLService ocrvlService;
        public static bool use_gpu = false;//是否使用GPU
        public static int gpu_id = 0;//GPUId
        public static int cpu_threads = Environment.ProcessorCount; //CPU预测时的线程数
        public static int cpu_mem = 4000;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        public static string RecFilepath = "";
        public static bool outPutJson = false;//是否输出JSON
        public static bool use_tensorrt = false;//使用GPU预测时，是否启动tensorrt
        public static int recCount = 1; //OCR识别时同一张图片模拟调用接口次数
        public static int model_type = 0;//模型类型：0是V5 Mobile，1是V5 Server， 2是V4 Mobile
        private bool isInitSuccess = false; // OCR是否初始化成功

        // 图像矫正相关字段
        private IUVDocService? uvdocService;
        private string? currentImagePath;
        private string? outputImagePath;
        private System.Diagnostics.Stopwatch? processStopwatch;

        // OCR-VL相关字段
        private bool isOCRVLInitSuccess = false;
        private bool isOCRVLLayoutAnalysis = false;

        public MainForm()
        {
            InitializeComponent();
            ocrService = OCREngine.ocrService;
            ocrvlService = new OCRVLService();
            this.ActiveControl = pictureBoxImg;
            this.FormClosing += MainForm_FormClosing;
        }
        // comboBoxPrompt选中项变化时，将textBoxOCRVLPrompt的值设置为对应项
        private void comboBoxPrompt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPrompt.SelectedItem != null)
            {
                textBoxOCRVLPrompt.Text = comboBoxPrompt.SelectedItem.ToString();
            }
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string github = "--QQ群：475159576 https://github.com/PaddleOCRCore/PaddleOCRApi";
                this.Text = AssemblyHepler.AssemblyTitle + " V"+AssemblyHepler.AssemblyVersion + github;
                comboBoxPrompt.Items.AddRange(new object[] {
                "OCR:",
                "Table Recognition:",
                "Formula Recognition:",
                "Chart Recognition:",
                "Spotting:",
                "Seal Recognition:"
                });
                // 设置comboBoxPrompt默认选中第一个
                comboBoxPrompt.SelectedIndex = 0;

                comboBoxModel.SelectedIndex = 0;
                RecFilepath = Path.Combine(Application.StartupPath, "output");
                if (!Directory.Exists(RecFilepath))
                {
                    Directory.CreateDirectory(RecFilepath);
                }
                // 创建uploads目录用于存储上传的原始图片
                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }
                buttonFreeEngine.Enabled = false;
                InitializeOCRVLDefaults();

                // 初始化图像矫正功能参数
                InitializeUVDocParameters();
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
                textBoxResult.Text = message.ToString();
            }
        }

        private void InitializeUVDocParameters()
        {
            chkUVDocUseGpu.Checked = false;
            numUVDocCpuThreads.Value = Environment.ProcessorCount;
            numUVDocGpuId.Value = 0;
            numUVDocGpuMem.Value = 2000;
            chkUVDocUseTensorRT.Checked = false;
        }

        private void InitializeOCRVLDefaults()
        {
            string defaultConfigPath = Path.Combine(Application.StartupPath, "configs", "PaddleOCR-VL-1.5.yaml");
            textBoxOCRVLConfigPath.Text = defaultConfigPath;
            textBoxOCRVLPrompt.Text = "OCR:";
            checkBoxOCRVLDocAnalysis.Checked = false;
            UpdateOCRVLPromptState();
            UpdateOCRVLStatus("就绪");
        }

        private void UpdateOCRVLPromptState()
        {
            if (checkBoxOCRVLDocAnalysis.Checked)
            {
                textBoxOCRVLPrompt.Enabled = false;
                comboBoxPrompt.Enabled = false;
            }
            else
            {
                textBoxOCRVLPrompt.Enabled = true;
                comboBoxPrompt.Enabled=true;
            }
        }

        private void UpdateOCRVLStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateOCRVLStatus), message);
                return;
            }

            labelOCRVLStatus.Text = $"状态: {message}";
        }

        private void LogOCRVLMessage(string infoValue)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(LogOCRVLMessage), infoValue);
                return;
            }

            textBoxOCRVLResult.AppendText(infoValue);
            textBoxOCRVLResult.AppendText(Environment.NewLine);
            textBoxOCRVLResult.SelectionStart = textBoxOCRVLResult.Text.Length;
            textBoxOCRVLResult.ScrollToCaret();
        }

        private void SetOCRVLParameterControlsEnabled(bool enabled)
        {
            textBoxOCRVLConfigPath.Enabled = enabled;
            buttonOCRVLBrowseConfig.Enabled = enabled;
            checkBoxOCRVLDocAnalysis.Enabled = enabled;
        }

        private string GetOCRVLPrompt()
        {
            string prompt = textBoxOCRVLPrompt.Text.Trim();
            return string.IsNullOrWhiteSpace(prompt) ? "OCR:" : prompt;
        }

        private string GetOCRVLPreviewPath(string inputImagePath)
        {
            if (isOCRVLLayoutAnalysis)
            {
                string overlayPath = Path.Combine(RecFilepath, "layout_overlay_page_0.png");
                if (File.Exists(overlayPath))
                {
                    return overlayPath;
                }
            }

            return inputImagePath;
        }

        private static string NormalizeOCRVLDisplayText(string? text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            string normalized = text;

            for (int i = 0; i < 3; i++)
            {
                string replaced = normalized
                    .Replace("\\u000D\\u000A", "\n")
                    .Replace("\\u000d\\u000a", "\n")
                    .Replace("\\u000A", "\n")
                    .Replace("\\u000a", "\n")
                    .Replace("\\u000D", "\n")
                    .Replace("\\u000d", "\n")
                    .Replace("\\r\\n", "\n")
                    .Replace("\\n", "\n")
                    .Replace("\\r", "\n")
                    .Replace("\\t", "\t");

                if (replaced == normalized)
                {
                    break;
                }

                normalized = replaced;
            }

            normalized = Regex.Replace(
                normalized,
                @"\\u([0-9a-fA-F]{4})",
                match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString());

            return normalized
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("\n", Environment.NewLine);
        }

        private void ShowOCRVLPreview(string inputImagePath)
        {
            string previewPath = GetOCRVLPreviewPath(inputImagePath);
            if (File.Exists(previewPath))
            {
                pictureBoxOCRVL.ImgPath = previewPath;
            }
        }

        private async Task<string> RecOCRVLAsync(string filePath)
        {
            var stopwatch = new Stopwatch();
            string prompt = GetOCRVLPrompt();
            var startTime = DateTime.Now;
            LogOCRVLMessage($"Image: {Path.GetFileName(filePath)}");
            LogOCRVLMessage("正在识别，请稍后...");
            LogOCRVLMessage($"开始时间: {startTime:HH:mm:ss.fff}");
            stopwatch.Start();

            if (isOCRVLLayoutAnalysis)
            {
                VLDocumentResult docResult = await Task.Run(() => ocrvlService.DocChat(filePath, PocrOutputFormat.Both));
                var endTime = DateTime.Now;
                LogOCRVLMessage($"结束时间: {endTime:HH:mm:ss.fff}");
                LogOCRVLMessage($"用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                LogOCRVLMessage($"识别结果：");
                if (docResult.Code != 1)
                {
                    LogOCRVLMessage(docResult.ErrorMsg);
                    LogOCRVLMessage("===============================================");
                    return docResult.ErrorMsg;
                }

                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(docResult.Markdown))
                {
                    builder.AppendLine("Markdown:");
                    builder.AppendLine(NormalizeOCRVLDisplayText(docResult.Markdown).Trim());
                }
                if (!string.IsNullOrWhiteSpace(docResult.JsonText))
                {
                    if (builder.Length > 0)
                    {
                        builder.AppendLine();
                    }
                    builder.AppendLine("JSON:");
                    builder.AppendLine(FormatJsonSafe(docResult.JsonText));
                }

                string result = builder.ToString().Trim();
                LogOCRVLMessage(result);
                LogOCRVLMessage("===============================================");
                ShowOCRVLPreview(filePath);
                return result;
            }

            VLChatResult chatResult = await Task.Run(() => ocrvlService.Chat(prompt, filePath));
            var finishTime = DateTime.Now;
            LogOCRVLMessage($"结束时间: {finishTime:HH:mm:ss.fff}");
            LogOCRVLMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogOCRVLMessage($"识别结果：");

            string chatText = chatResult.Code == 1
                ? NormalizeOCRVLDisplayText(chatResult.Content)
                : NormalizeOCRVLDisplayText(chatResult.ErrorMsg);
            LogOCRVLMessage(chatText);
            LogOCRVLMessage("===============================================");
            ShowOCRVLPreview(filePath);
            return chatText;
        }

        private string ConvertPdfFirstPageToImage(string pdfFilePath, string suffix)
        {
            SKBitmap? bitmap = null;
            try
            {
                byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                using (MemoryStream memoryStream = new MemoryStream(pdfBytes))
                {
                    bitmap = PDFtoImage.Conversion.ToImage(memoryStream);
                }

                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (SKImage image = SKImage.FromBitmap(bitmap))
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        imageBytes = data.ToArray();
                    }
                }

                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                Directory.CreateDirectory(uploadsPath);

                string imagePath = Path.Combine(
                    uploadsPath,
                    Path.GetFileNameWithoutExtension(pdfFilePath) + suffix + "_page1.png");

                File.WriteAllBytes(imagePath, imageBytes);
                return imagePath;
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        private void buttonOCRVLBrowseConfig_Click(object? sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "配置文件(*.yaml;*.yml;*.json)|*.yaml;*.yml;*.json|所有文件|*.*";
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxOCRVLConfigPath.Text = openFileDialog.FileName;
                }
            }
        }

        private void checkBoxOCRVLDocAnalysis_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateOCRVLPromptState();
            UpdateOCRVLStatus(checkBoxOCRVLDocAnalysis.Checked
                ? "已启用版面分析，识别后优先预览 output/layout_overlay_page_0.png"
                : "已切换为普通 OCR-VL 模式");
        }

        private void buttonOCRVLInit_Click(object? sender, EventArgs e)
        {
            try
            {
                textBoxOCRVLResult.Clear();
                string configPath = textBoxOCRVLConfigPath.Text.Trim();
                if (string.IsNullOrWhiteSpace(configPath))
                {
                    UpdateOCRVLStatus("配置文件不能为空");
                    MessageBox.Show("请先选择 YAML 配置文件。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!File.Exists(configPath))
                {
                    UpdateOCRVLStatus("配置文件不存在");
                    MessageBox.Show($"配置文件不存在：{configPath}", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                isOCRVLLayoutAnalysis = checkBoxOCRVLDocAnalysis.Checked;
                UpdateOCRVLStatus("正在初始化 OCR-VL 引擎...");

                if (isOCRVLLayoutAnalysis)
                {
                    ocrvlService.InitDoc(configPath);
                }
                else
                {
                    ocrvlService.Init(configPath);
                }

                isOCRVLInitSuccess = true;
                buttonOCRVLInit.Enabled = false;
                buttonOCRVLFreeEngine.Enabled = true;
                buttonOCRVLRec.Enabled = true;
                buttonOCRVLRecPDF.Enabled = true;
                SetOCRVLParameterControlsEnabled(false);

                string mode = isOCRVLLayoutAnalysis ? "版面分析模式(InitDoc)" : "普通模式(Init)";
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化成功，当前为{mode}");
                UpdateOCRVLStatus($"初始化成功，当前为{mode}");
            }
            catch (Exception ex)
            {
                isOCRVLInitSuccess = false;
                UpdateOCRVLStatus($"初始化失败: {ex.Message}");
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化失败: {ex.Message}");
            }
        }

        // 支持多选图片识别，结果全部显示
        private async void buttonOCRVLRec_Click(object? sender, EventArgs e)
        {
            try
            {
                textBoxOCRVLResult.Clear();
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|所有文件|*.*";
                    openFileDialog.Multiselect = true;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        foreach (var filePath in openFileDialog.FileNames)
                        {
                            string absPath = Path.GetFullPath(filePath);
                            string result = await RecOCRVLAsync(absPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"图片识别异常: {ex.Message}");
                UpdateOCRVLStatus($"图片识别异常: {ex.Message}");
            }
        }

        private async void buttonOCRVLRecPDF_Click(object? sender, EventArgs e)
        {
            try
            {
                textBoxOCRVLResult.Clear();
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                    openFileDialog.Multiselect = false;
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string pdfPath = Path.GetFullPath(openFileDialog.FileName);
                        LogOCRVLMessage($"正在处理PDF文件: {pdfPath}");
                        string imagePath = ConvertPdfFirstPageToImage(pdfPath, "_ocrvl");
                        LogOCRVLMessage($"已保存转换后的图片到uploads: {imagePath}");
                        await RecOCRVLAsync(imagePath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"PDF识别异常: {ex.Message}");
                UpdateOCRVLStatus($"PDF识别异常: {ex.Message}");
            }
        }

        private void buttonOCRVLFreeEngine_Click(object? sender, EventArgs e)
        {
            try
            {
                ocrvlService.FreeDocAnalyser();
                ocrvlService.FreeEngine();
                isOCRVLInitSuccess = false;
                isOCRVLLayoutAnalysis = false;

                buttonOCRVLInit.Enabled = true;
                buttonOCRVLFreeEngine.Enabled = false;
                buttonOCRVLRec.Enabled = false;
                buttonOCRVLRecPDF.Enabled = false;
                SetOCRVLParameterControlsEnabled(true);

                textBoxOCRVLResult.Clear();
                pictureBoxOCRVL.Image?.Dispose();
                pictureBoxOCRVL.Image = null;

                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 引擎已释放！");
                UpdateOCRVLStatus("引擎已释放");
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"释放 OCR-VL 引擎失败: {ex.Message}");
                UpdateOCRVLStatus($"释放失败: {ex.Message}");
            }
        }

        private void buttonInit_Click(object sender, EventArgs e)
        {
            //LogMessage($"{DateTime.Now:HH:mm:ss.fff}:正在初始化,请稍后...");
            try
            {
                this.isInitSuccess = false;
                OCREngine.use_gpu = use_gpu;
                OCREngine.gpu_id = gpu_id;
                OCREngine.cpu_threads = cpu_threads;
                OCREngine.use_tensorrt = use_tensorrt;
                OCREngine.return_word_box = chkReturnWordBox.Checked;
                if (model_type == 0)
                {
                    OCREngine.det_infer = "PP-OCRv5_mobile_det_infer";//OCR V5检测模型
                    OCREngine.rec_infer = "PP-OCRv5_mobile_rec_infer";//OCR V5识别模型
                    OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                }
                else if (model_type == 1)
                {
                    OCREngine.det_infer = "PP-OCRv5_server_det_infer";//OCR V5检测模型
                    OCREngine.rec_infer = "PP-OCRv5_server_rec_infer";//OCR V5识别模型
                    OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                }
                else
                {
                    OCREngine.det_infer = "PP-OCRv4_mobile_det_infer";//OCR V4检测模型
                    OCREngine.rec_infer = "PP-OCRv4_mobile_rec_infer";//OCR V4识别模型
                    OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";//PaddleOCR3.2不再支持ch_ppocr_mobile_v2.0_cls_infer
                }
                string initmsg = OCREngine.GetOCREngine();
                if (string.IsNullOrEmpty(initmsg))
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:文本识别初始化成功！");
                }
                else
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:{initmsg}");
                }
                if (initmsg.IndexOf("初始化成功") >= 0)
                {
                    this.buttonRec.Enabled = true;
                    this.buttonRecClipboard.Enabled = true;
                    this.buttonRecPDF.Enabled = true;
                    this.isInitSuccess = true;
                }
                else
                {
                    this.buttonRec.Enabled = false;
                }
                initmsg = OCREngine.GetOCRTableEngine();
                if (string.IsNullOrEmpty(initmsg))
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:表格识别初始化成功！");
                }
                else
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:{initmsg}");
                }
                if (initmsg.IndexOf("初始化成功") >= 0)
                {
                    this.buttonRecTable.Enabled = true;
                    this.isInitSuccess = true;
                }
                else
                {
                    this.buttonRecTable.Enabled = false;
                }

                if (this.isInitSuccess)
                {
                    this.buttonInit.Enabled = false;
                    this.buttonFreeEngine.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR初始化失败:{ex.Message}");
            }
        }
        public static string FormatJsonSafe(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                return JsonSerializer.Serialize(doc, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }
            catch (System.Exception)
            {
                return Regex.Replace(
                    json,
                    @"\\u([0-9a-fA-F]{4})",
                    match => ((char)Convert.ToInt32(match.Groups[1].Value, 16)).ToString())
                    .Replace("\\r\\n", Environment.NewLine)
                    .Replace("\\n", Environment.NewLine)
                    .Replace("\\r", Environment.NewLine);
            }
        }
        private async Task<string> RecOCRAsync(string filePath)
        {
            string result = "";
            var stopwatch = new Stopwatch();
            var startTime = DateTime.Now;
            LogMessage($"开始时间: {startTime:HH:mm:ss.fff}");
            stopwatch.Start();
            //Mat image = Cv2.ImRead(filePath, ImreadModes.Color);
            OCRResult ocrResult = await Task.Run(() => ocrService.Detect(filePath));
            //OCRResult ocrResult = ocrService.DetectMat(image.CvPtr);使用OpenCvSharp4时,可传入DetectMat(image.CvPtr)
            StringBuilder stringBuilder = new StringBuilder();
            if (ocrResult.Code == 1)
            {
                foreach (var item in ocrResult.WordsResult)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(Environment.NewLine);
                    }
                    stringBuilder.Append(item.Words);
                }
                result = stringBuilder.ToString();
            }
            else
            {
                result = ocrResult.ErrorMsg;
            }
            var endTime = DateTime.Now;
            LogMessage($"结束时间: {endTime:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            if (!string.IsNullOrEmpty(result))
            {
                LogMessage(result);
                if (outPutJson)
                {
                    if (!string.IsNullOrEmpty(ocrResult.JsonText))
                    {
                        string formattedJson = FormatJsonSafe(ocrResult.JsonText);
                        LogMessage($"输出json: {formattedJson}");
                    }
                }
            }
            else
            {
                LogMessage("识别失败:" + ocrService.GetError());
                if (!string.IsNullOrEmpty(ocrResult.JsonText))
                {
                    string formattedJson = FormatJsonSafe(ocrResult.JsonText);
                    LogMessage($"输出json: {formattedJson}");
                }
            }
            LogMessage("===============================================");
            return result;
        }

        private async void buttonRec_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                string result = "";
                string recFileName = "";
                OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
                OpenFileDialog1.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
                OpenFileDialog1.Multiselect = true;
                if (DialogResult.OK == OpenFileDialog1.ShowDialog())
                {
                    for (int i = 0; i < recCount; i++)//模拟循环OCR识别
                    {
                        foreach (var regfile in OpenFileDialog1.FileNames)
                        {
                            LogMessage($"Image: {Path.GetFileName(regfile)}");
                            string filePath = Path.GetFullPath(regfile);
                            recFileName = Path.Combine(RecFilepath, Path.GetFileName(regfile));
                            result = await RecOCRAsync(filePath);
                            if (File.Exists(recFileName))
                            {
                                pictureBoxImg.BeginInvoke(() =>
                                {
                                    pictureBoxImg.ImgPath = recFileName;
                                });
                            }
                        }
                    }
                }
                OpenFileDialog1.Dispose();

            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
        }

        private void buttonRecClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();

                // 检查剪贴板是否包含图片
                if (!Clipboard.ContainsImage())
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:剪贴板中没有图片！");
                    return;
                }

                // 从剪贴板获取图片
                Image clipboardImage = Clipboard.GetImage();
                if (clipboardImage == null)
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:无法从剪贴板获取图片！");
                    return;
                }

                // 保存剪贴板图片到uploads目录
                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                string clipboardImagePath = Path.Combine(uploadsPath, $"clipboard_{DateTime.Now:yyyyMMddHHmmss}.png");
                clipboardImage.Save(clipboardImagePath, System.Drawing.Imaging.ImageFormat.Png);
                LogMessage($"剪贴板图片已保存到: {clipboardImagePath}");

                // 进行OCR识别
                var stopwatch = new Stopwatch();
                var startTime = DateTime.Now;
                LogMessage($"Image: 剪贴板图片");
                LogMessage($"开始时间: {startTime:HH:mm:ss.fff}");
                stopwatch.Start();

                OCRResult ocrResult = ocrService.Detect(clipboardImagePath);

                StringBuilder stringBuilder = new StringBuilder();
                string result = "";
                if (ocrResult.Code == 1)
                {
                    foreach (var item in ocrResult.WordsResult)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                        stringBuilder.Append(item.Words);
                    }
                    result = stringBuilder.ToString();
                }
                else
                {
                    result = ocrResult.ErrorMsg;
                }

                var endTime = DateTime.Now;
                LogMessage($"结束时间: {endTime:HH:mm:ss.fff}");
                LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");

                if (!string.IsNullOrEmpty(result))
                {
                    LogMessage(result);
                    if (outPutJson)
                    {
                        if (!string.IsNullOrEmpty(ocrResult.JsonText))
                        {
                            string formattedJson = FormatJsonSafe(ocrResult.JsonText);
                            LogMessage($"输出json: {formattedJson}");
                        }
                    }
                    // 显示识别结果图片（OCR引擎自动生成到output目录）
                    string resultImageFileName = Path.GetFileName(clipboardImagePath);
                    string resultImagePath = Path.Combine(RecFilepath, resultImageFileName);
                    if (File.Exists(resultImagePath))
                    {
                        pictureBoxImg.ImgPath = resultImagePath;
                    }
                }
                else
                {
                    LogMessage("识别失败:" + ocrService.GetError());
                    if (!string.IsNullOrEmpty(ocrResult.JsonText))
                    {
                        string formattedJson = FormatJsonSafe(ocrResult.JsonText);
                        LogMessage($"输出json: {formattedJson}");
                    }
                }

                clipboardImage.Dispose();
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:剪贴板OCR识别异常:{ex.Message}");
            }
        }

        private string RecOCRFromPDF(string pdfFilePath)
        {
            string result = "";
            SKBitmap? bitmap = null;
            string uploadImagePath = "";

            try
            {
                var stopwatch = new Stopwatch();
                var startTime = DateTime.Now;
                LogMessage($"PDF文件: {pdfFilePath}");
                LogMessage($"开始时间: {startTime:HH:mm:ss.fff}");
                stopwatch.Start();

                // 读取PDF文件到内存流
                byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                using (MemoryStream memoryStream = new MemoryStream(pdfBytes))
                {
                    try
                    {
                        // 将PDF第一页转换为SKBitmap
                        bitmap = PDFtoImage.Conversion.ToImage(memoryStream);
                        LogMessage("PDF转图片成功");
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"PDF转图片失败: {ex.Message}");
                        return "";
                    }
                }

                // 将SKBitmap转换为byte数组
                byte[] imageBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (SKImage image = SKImage.FromBitmap(bitmap))
                    using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        imageBytes = data.ToArray();
                    }
                }

                // 保存转换后的图片到uploads目录
                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                uploadImagePath = Path.Combine(uploadsPath, Path.GetFileNameWithoutExtension(pdfFilePath) + "_page1.png");
                File.WriteAllBytes(uploadImagePath, imageBytes);
                LogMessage($"已保存转换后的图片到uploads: {uploadImagePath}");

                // 进行OCR识别，传入文件路径
                OCRResult ocrResult = ocrService.Detect(uploadImagePath);
                StringBuilder stringBuilder = new StringBuilder();

                if (ocrResult.Code == 1)
                {
                    foreach (var item in ocrResult.WordsResult)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                        stringBuilder.Append(item.Words);
                    }
                    result = stringBuilder.ToString();
                }
                else
                {
                    result = ocrResult.ErrorMsg;
                }

                var endTime = DateTime.Now;
                LogMessage($"结束时间: {endTime:HH:mm:ss.fff}");
                LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");

                if (!string.IsNullOrEmpty(result))
                {
                    LogMessage(result);
                    if (outPutJson)
                    {
                        if (!string.IsNullOrEmpty(ocrResult.JsonText))
                        {
                            string formattedJson = FormatJsonSafe(ocrResult.JsonText);
                            LogMessage($"输出json: {formattedJson}");
                        }
                    }
                }
                else
                {
                    LogMessage("识别失败:" + ocrService.GetError());
                    if (!string.IsNullOrEmpty(ocrResult.JsonText))
                    {
                        string formattedJson = FormatJsonSafe(ocrResult.JsonText);
                        LogMessage($"输出json: {formattedJson}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"PDF识别异常: {ex.Message}");
            }
            finally
            {
                // 释放bitmap资源
                bitmap?.Dispose();
            }

            return result;
        }
        private void buttonRecPDF_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                string result = "";
                string recFileName = "";
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
                openFileDialog.Multiselect = false;

                if (DialogResult.OK == openFileDialog.ShowDialog())
                {
                    string filePath = Path.GetFullPath(openFileDialog.FileName);
                    LogMessage($"正在处理PDF文件: {filePath}");

                    result = RecOCRFromPDF(filePath);

                    // 显示识别结果图片（OCR引擎自动生成到output目录，文件名基于uploads中的图片）
                    if (!string.IsNullOrEmpty(result))
                    {
                        string uploadFileName = Path.GetFileNameWithoutExtension(filePath) + "_page1.png";
                        recFileName = Path.Combine(RecFilepath, uploadFileName);
                        if (File.Exists(recFileName))
                        {
                            pictureBoxImg.ImgPath = recFileName;
                        }
                    }
                }
                openFileDialog.Dispose();
            }
            catch (Exception ex)
            {
                LogMessage($"PDF识别异常: {ex.Message}");
            }
        }

        private string RecOCRTable(string filePath)
        {
            var stopwatch = new Stopwatch();
            var startTime = DateTime.Now;
            LogMessage($"Image: {filePath}");
            LogMessage($"开始时间: {startTime:HH:mm:ss.fff}");
            stopwatch.Start();

            string ocrResult = ocrService.DetectTable(filePath);
            string css = "<style>table{ border-spacing: 0;} td { border: 1px solid black;}</style>";
            ocrResult = ocrResult.Replace("<html>", "<html>" + css);

            // 定义输出文件夹和文件名（使用RecFilepath变量，即output目录）
            string htmlfile = Path.Combine(RecFilepath, $"{Path.GetFileNameWithoutExtension(filePath)}.html");

            // 确保输出文件夹存在
            if (!Directory.Exists(RecFilepath))
            {
                Directory.CreateDirectory(RecFilepath);
            }

            // 写入HTML文件
            try
            {
                using (StreamWriter sw = new StreamWriter(htmlfile, false, System.Text.Encoding.GetEncoding("utf-8")))
                {
                    sw.Write(ocrResult);
                }
                LogMessage($"表格识别结果已保存到: {htmlfile}");

                // 使用默认的浏览器打开HTML文件
                Process.Start(new ProcessStartInfo
                {
                    FileName = htmlfile,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                LogMessage($"保存或打开HTML文件失败：{ex.Message}");
            }

            var endTime = DateTime.Now;
            LogMessage($"结束时间: {endTime:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogMessage(ocrResult);

            return ocrResult;
        }
        private void buttonRecTable_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                string result = "";
                OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
                OpenFileDialog1.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
                OpenFileDialog1.Multiselect = false;
                if (DialogResult.OK == OpenFileDialog1.ShowDialog())
                {
                    foreach (var regfile in OpenFileDialog1.FileNames)
                    {
                        string filePath = Path.GetFullPath(regfile);
                        result = RecOCRTable(filePath);
                        if (File.Exists(filePath))
                        {
                            pictureBoxImg.ImgPath = filePath;
                        }
                    }
                }
                OpenFileDialog1.Dispose();

            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
        }

        private void buttonDownModels_Click(object sender, EventArgs e)
        {
            // 定义要打开的 URL
            string urlV5 = "https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html";
            try
            {
                //Process.Start(new ProcessStartInfo(urlV4)
                //{
                //    UseShellExecute = true
                //});
                LogMessage($"PP-OCRv5/PP-OCRv4模型下载地址：{urlV5}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开网页：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonGetBase64_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
                OpenFileDialog1.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
                OpenFileDialog1.Multiselect = false;
                if (DialogResult.OK == OpenFileDialog1.ShowDialog())
                {
                    string filePath = OpenFileDialog1.FileName;
                    string base64 = ImageTools.GetBase64FromImage(filePath);
                    textBoxResult.Text = base64;
                }
                OpenFileDialog1.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"图片格式异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonPostFile_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.textBoxApiAddress.Text.Trim()))
                {
                    LogMessage($"{DateTime.Now:HH:mm:ss.fff}:WebApi地址不能为空！");
                }
                OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
                OpenFileDialog1.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
                OpenFileDialog1.Multiselect = false;
                if (DialogResult.OK == OpenFileDialog1.ShowDialog())
                {
                    string filePath = OpenFileDialog1.FileName;
                    textBoxResult.Text = HttpHelper.PostFile(this.textBoxApiAddress.Text.Trim(), filePath);
                }
                OpenFileDialog1.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"调用接口异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void chkUseGpu_CheckedChanged(object sender, EventArgs e)
        {
            use_gpu = chkUseGpu.Checked;
            chkUseTensorRT.Enabled = chkUseGpu.Checked && chkUseGpu.Enabled;
            if (use_gpu)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{DateTime.Now:HH:mm:ss.fff}:使用GPU时请下载对应的paddle_inference解压" + Environment.NewLine);
                sb.Append($"解压后将以下dll文件复制到程序运行文件夹中：" + Environment.NewLine);
                sb.Append($"paddle\\lib目录下的common.dll和paddle_inference.dll" + Environment.NewLine);
                sb.Append($"third_party\\install\\mkldnn\\lib目录下的mkldnn.dll" + Environment.NewLine);
                sb.Append($"third_party\\install\\mklml\\lib目录下的libiomp5md.dll和mklml.dll" + Environment.NewLine);
                sb.Append($"安装指定版本的CUDA以及CUDNN" + Environment.NewLine);
                sb.Append($"复制对应的cublas64_12.dll、cublasLt64_12.dll、cudnn_cnn64_9.dll、cudnn_engines_precompiled64_9.dll、cudnn_engines_runtime_compiled64_9.dll、cudnn_graph64_9.dll、cudnn_heuristic64_9.dll、cudnn_ops64_9.dll、cudnn64_9.dll到程序运行文件夹中" + Environment.NewLine);
                sb.Append($"位于C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v12.9\\bin" + Environment.NewLine);
                LogMessage(sb.ToString());
            }
        }

        private void chkJson_CheckedChanged(object sender, EventArgs e)
        {
            outPutJson = chkJson.Checked;
        }

        private void chkUseTensorRT_CheckedChanged(object sender, EventArgs e)
        {
            use_tensorrt = chkUseTensorRT.Checked;
        }

        private void numDowngpu_id_ValueChanged(object sender, EventArgs e)
        {
            if (this.numDowngpu_id.Value > 0)
            {
                gpu_id = Convert.ToInt32(this.numDowngpu_id.Value);
            }
        }
        private void numDowncpu_threads_ValueChanged(object sender, EventArgs e)
        {
            if (this.numDowncpu_threads.Value > 0)
            {
                cpu_threads = Convert.ToInt32(this.numDowncpu_threads.Value);
            }
        }
        private void numericUpDowncpu_mem_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDowncpu_mem.Value > 0)
                cpu_mem = Convert.ToInt32(numericUpDowncpu_mem.Value);
        }

        private void numericUpDownThread_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownThread.Value > 0)
                recCount = Convert.ToInt32(numericUpDownThread.Value);
        }

        #region LogMessage
        public void LogMessage(string infoValue)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() =>
                {
                    textBoxResult.AppendText(infoValue);
                    textBoxResult.AppendText(Environment.NewLine);
                    textBoxResult.SelectionStart = textBoxResult.Text.Length;
                    textBoxResult.ScrollToCaret();
                }));
            }
            else
            {
                textBoxResult.AppendText(infoValue);
                textBoxResult.AppendText(Environment.NewLine);
                textBoxResult.SelectionStart = textBoxResult.Text.Length;
                textBoxResult.ScrollToCaret();
            }
        }

        #endregion

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            model_type = comboBoxModel.SelectedIndex;
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (isInitSuccess)
            {
                OCREngine.FreeEngine();
            }
            if (isOCRVLInitSuccess)
            {
                try
                {
                    ocrvlService.FreeDocAnalyser();
                    ocrvlService.FreeEngine();
                }
                catch
                {
                }
            }
            // 释放图像矫正引擎
            try
            {
                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }
            }
            catch { }
            uvdocService?.FreeUVDocEngine();
        }

        #region 图像矫正功能事件处理

        private void btnUVDocInitialize_Click(object sender, EventArgs e)
        {
            InitializeUVDocEngine();
        }

        private void InitializeUVDocEngine()
        {
            try
            {
                // 创建新的服务实例
                uvdocService = new UVDocService();
                var parameter = new UVDocParameter
                {
                    enable_mkldnn = true,
                    cpu_threads = (int)numUVDocCpuThreads.Value,
                    use_gpu = chkUVDocUseGpu.Checked,
                    gpu_id = (int)numUVDocGpuId.Value,
                    gpu_mem = (int)numUVDocGpuMem.Value,
                    use_tensorrt = chkUVDocUseTensorRT.Checked
                };
                // 模型路径（相对于程序目录）
                string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "UVDoc_infer");

                // 如果模型不存在，尝试使用上级目录的模型
                if (!Directory.Exists(modelPath))
                {
                    modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "models", "UVDoc_infer");
                    modelPath = Path.GetFullPath(modelPath);
                }

                UpdateUVDocStatus($"正在初始化模型: {modelPath}");

                // 禁用参数控件
                SetUVDocParameterControlsEnabled(false);
                btnUVDocInitialize.Enabled = false;

                if (uvdocService.Initialize(modelPath, parameter))
                {
                    UpdateUVDocStatus($"模型初始化成功！[GPU: {(parameter.use_gpu ? "是" : "否")}, CPU线程: {parameter.cpu_threads}]");
                    btnUVDocUpload.Enabled = true; // 允许上传图片
                    btnUVDocProcess.Enabled = false; // 等待上传图片
                    btnUVDocFreeEngine.Enabled = true; // 允许释放引擎
                }
                else
                {
                    string error = uvdocService.GetLastError();
                    UpdateUVDocStatus($"模型初始化失败: {error}");
                    MessageBox.Show($"模型初始化失败！\n错误信息: {error}\n\n请确保 models/UVDoc_infer 目录存在且包含正确的模型文件。",
                        "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetUVDocParameterControlsEnabled(true);
                    btnUVDocInitialize.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                UpdateUVDocStatus($"初始化异常: {ex.Message}");
                MessageBox.Show($"初始化异常: {ex.Message}\n\n请确保 PaddleOCR.dll 及其依赖库在程序目录中。",
                    "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetUVDocParameterControlsEnabled(true);
                btnUVDocInitialize.Enabled = true;
            }
        }

        private void SetUVDocParameterControlsEnabled(bool enabled)
        {
            chkUVDocUseGpu.Enabled = enabled;
            numUVDocCpuThreads.Enabled = enabled;
            numUVDocGpuId.Enabled = enabled && chkUVDocUseGpu.Checked;
            numUVDocGpuMem.Enabled = enabled && chkUVDocUseGpu.Checked;
            chkUVDocUseTensorRT.Enabled = enabled && chkUVDocUseGpu.Checked;
        }

        private void chkUVDocUseGpu_CheckedChanged(object sender, EventArgs e)
        {
            // GPU相关选项只在启用GPU时可用
            numUVDocGpuId.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
            numUVDocGpuMem.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
            chkUVDocUseTensorRT.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
        }

        private void btnUVDocUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|所有文件|*.*";
                openFileDialog.Title = "选择要矫正的图像";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        currentImagePath = openFileDialog.FileName;

                        // 显示原始图像
                        using (var fs = new FileStream(currentImagePath, FileMode.Open, FileAccess.Read))
                        {
                            pictureBoxOriginal.Image?.Dispose();
                            pictureBoxOriginal.Image = Image.FromStream(fs);
                        }

                        // 清空输出图像
                        pictureBoxOutput.Image?.Dispose();
                        pictureBoxOutput.Image = null;

                        btnUVDocProcess.Enabled = true;
                        btnUVDocSave.Enabled = false;
                        UpdateUVDocStatus($"已加载图像: {Path.GetFileName(currentImagePath)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"加载图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateUVDocStatus($"加载图像失败: {ex.Message}");
                    }
                }
            }
        }

        private async void btnUVDocProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath) || uvdocService == null)
            {
                MessageBox.Show("请先上传图像并确保引擎已初始化", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnUVDocProcess.Enabled = false;
                btnUVDocUpload.Enabled = false;
                btnUVDocSave.Enabled = false;
                UpdateUVDocStatus("正在处理图像，请稍候...");

                // 生成输出文件路径
                string tempDir = Path.Combine(Path.GetTempPath(), "PaddleDocVision");
                Directory.CreateDirectory(tempDir);
                outputImagePath = Path.Combine(tempDir, $"output_{DateTime.Now:yyyyMMddHHmmss}.jpg");

                // 开始计时
                processStopwatch = System.Diagnostics.Stopwatch.StartNew();

                // 使用异步方法处理图像
                await uvdocService.UVDocImageFileAsync(currentImagePath, outputImagePath);

                // 停止计时
                processStopwatch.Stop();
                double elapsedMilliseconds = processStopwatch.Elapsed.TotalMilliseconds;

                // 显示处理后的图像
                if (File.Exists(outputImagePath))
                {
                    using (var fs = new FileStream(outputImagePath, FileMode.Open, FileAccess.Read))
                    {
                        pictureBoxOutput.Image?.Dispose();
                        pictureBoxOutput.Image = Image.FromStream(fs);
                    }

                    UpdateUVDocStatus($"图像处理完成！耗时: {elapsedMilliseconds:F0} ms");
                    btnUVDocSave.Enabled = true;
                }
                else
                {
                    UpdateUVDocStatus($"处理失败：输出文件未生成（耗时: {elapsedMilliseconds:F0} ms）");
                    MessageBox.Show("处理失败：输出文件未生成", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string error = uvdocService?.GetLastError() ?? "";
                string message = $"处理图像时出错: {ex.Message}";
                if (!string.IsNullOrEmpty(error))
                {
                    message += $"\nDLL错误信息: {error}";
                }

                MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"处理失败: {ex.Message}");
            }
            finally
            {
                btnUVDocProcess.Enabled = true;
                btnUVDocUpload.Enabled = true;
            }
        }

        private void btnUVDocSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputImagePath) || !File.Exists(outputImagePath))
            {
                MessageBox.Show("没有可保存的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JPEG图像|*.jpg|PNG图像|*.png|所有文件|*.*";
                saveFileDialog.Title = "保存矫正后的图像";
                saveFileDialog.FileName = $"corrected_{DateTime.Now:yyyyMMddHHmmss}.jpg";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        File.Copy(outputImagePath, saveFileDialog.FileName, true);
                        UpdateUVDocStatus($"图像已保存: {saveFileDialog.FileName}");
                        MessageBox.Show("图像保存成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateUVDocStatus($"保存失败: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateUVDocStatus(string message)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<string>(UpdateUVDocStatus), message);
                return;
            }

            lblUVDocStatus.Text = $"状态: {message}";
        }

        private void btnUVDocFreeEngine_Click(object sender, EventArgs e)
        {
            try
            {
                // 释放引擎
                uvdocService?.FreeUVDocEngine();
                uvdocService = null;

                // 清空图像
                pictureBoxOriginal.Image?.Dispose();
                pictureBoxOriginal.Image = null;
                pictureBoxOutput.Image?.Dispose();
                pictureBoxOutput.Image = null;

                // 清理临时文件
                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }
                outputImagePath = null;
                currentImagePath = null;

                // 恢复按钮状态
                btnUVDocInitialize.Enabled = true;
                btnUVDocFreeEngine.Enabled = false;
                btnUVDocUpload.Enabled = false;
                btnUVDocProcess.Enabled = false;
                btnUVDocSave.Enabled = false;

                // 恢复参数控件
                SetUVDocParameterControlsEnabled(true);

                UpdateUVDocStatus("引擎已释放！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"释放引擎失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"释放失败: {ex.Message}");
            }
        }

        #endregion

        private void buttonFreeEngine_Click(object sender, EventArgs e)
        {
            OCREngine.FreeEngine();
            this.isInitSuccess = false;
            this.buttonInit.Enabled = true;
            this.buttonFreeEngine.Enabled = false;
            this.buttonRec.Enabled = false;
            this.buttonRecClipboard.Enabled = false;
            this.buttonRecPDF.Enabled = false;
            this.buttonRecTable.Enabled = false;
            LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR引擎已释放！");
        }


    }
}
