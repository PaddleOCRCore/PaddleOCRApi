using PaddleOCRSDK;
using SkiaSharp;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace WinFormsApp
{
    /// <summary>
    /// OCR-VL识别
    /// </summary>
    public partial class MainForm
    {
        private CancellationTokenSource? ocrVlOperationCts;
        private bool isOCRVLBusy;

        private void comboBoxPrompt_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPrompt.SelectedItem != null)
            {
                textBoxOCRVLPrompt.Text = comboBoxPrompt.SelectedItem.ToString();
            }
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
            bool enabled = !isOCRVLBusy && !checkBoxOCRVLDocAnalysis.Checked;
            textBoxOCRVLPrompt.Enabled = enabled;
            comboBoxPrompt.Enabled = enabled;
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
            UpdateOCRVLPromptState();
        }

        private void SetOCRVLBusy(bool busy)
        {
            isOCRVLBusy = busy;
            buttonOCRVLCancel.Enabled = busy;
            buttonOCRVLInit.Enabled = !busy && !isOCRVLInitSuccess;
            buttonOCRVLFreeEngine.Enabled = !busy && isOCRVLInitSuccess;
            buttonOCRVLRec.Enabled = !busy && isOCRVLInitSuccess;
            buttonOCRVLRecPDF.Enabled = !busy && isOCRVLInitSuccess;
            SetOCRVLParameterControlsEnabled(!busy && !isOCRVLInitSuccess);
        }

        private CancellationToken BeginOCRVLOperation()
        {
            ocrVlOperationCts?.Dispose();
            ocrVlOperationCts = new CancellationTokenSource();
            SetOCRVLBusy(true);
            return ocrVlOperationCts.Token;
        }

        private void EndOCRVLOperation()
        {
            SetOCRVLBusy(false);
            ocrVlOperationCts?.Dispose();
            ocrVlOperationCts = null;
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

        private async Task<string> RecOCRVLAsync(string filePath, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            string prompt = GetOCRVLPrompt();
            var startTime = DateTime.Now;
            LogOCRVLMessage($"Image: {Path.GetFileName(filePath)}");
            LogOCRVLMessage("正在识别，请稍后...");
            LogOCRVLMessage($"开始时间: {startTime:HH:mm:ss.fff}");

            if (isOCRVLLayoutAnalysis)
            {
                string layoutJson = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ocrvlService.DetectLayout(filePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                LayoutDetectResult layoutResult = ocrvlService.ParseLayoutResult(layoutJson);
                LogOCRVLMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
                LogOCRVLMessage($"用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                LogOCRVLMessage("识别结果：");

                StringBuilder builder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(layoutResult.Markdown))
                {
                    builder.AppendLine("Markdown:");
                    builder.AppendLine(NormalizeOCRVLDisplayText(layoutResult.Markdown).Trim());
                }

                if (!string.IsNullOrWhiteSpace(layoutJson))
                {
                    if (builder.Length > 0)
                    {
                        builder.AppendLine();
                    }

                    builder.AppendLine("JSON:");
                    builder.AppendLine(FormatJsonSafe(layoutJson));
                }

                string result = builder.ToString().Trim();
                LogOCRVLMessage(result);
                LogOCRVLMessage("===============================================");
                ShowOCRVLPreview(filePath);
                return result;
            }

            VLChatResult chatResult = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ocrvlService.Chat(prompt, filePath);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            LogOCRVLMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
            LogOCRVLMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogOCRVLMessage("识别结果：");

            string chatText = chatResult.Code == 1
                ? NormalizeOCRVLDisplayText(chatResult.Content)
                : NormalizeOCRVLDisplayText(chatResult.ErrorMsg);
            LogOCRVLMessage(chatText);
            LogOCRVLMessage("===============================================");
            ShowOCRVLPreview(filePath);
            return chatText;
        }

        private string ConvertPdfFirstPageToImage(string pdfFilePath, string suffix, CancellationToken cancellationToken)
        {
            SKBitmap? bitmap = null;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                using (MemoryStream memoryStream = new MemoryStream(pdfBytes))
                {
                    bitmap = PDFtoImage.Conversion.ToImage(memoryStream);
                }

                cancellationToken.ThrowIfCancellationRequested();
                byte[] imageBytes;
                using (SKImage image = SKImage.FromBitmap(bitmap))
                using (SKData data = image.Encode(SKEncodedImageFormat.Png, 100))
                {
                    imageBytes = data.ToArray();
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
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "配置文件(*.yaml;*.yml;*.json)|*.yaml;*.yml;*.json|所有文件|*.*";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBoxOCRVLConfigPath.Text = openFileDialog.FileName;
            }
        }

        private void checkBoxOCRVLDocAnalysis_CheckedChanged(object? sender, EventArgs e)
        {
            UpdateOCRVLPromptState();
            UpdateOCRVLStatus(checkBoxOCRVLDocAnalysis.Checked
                ? "已启用版面分析，识别后优先预览 output/layout_overlay_page_0.png"
                : "已切换为普通 OCR-VL 模式");
        }

        private async void buttonOCRVLInit_Click(object? sender, EventArgs e)
        {
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

            CancellationToken cancellationToken = BeginOCRVLOperation();
            try
            {
                textBoxOCRVLResult.Clear();
                isOCRVLLayoutAnalysis = checkBoxOCRVLDocAnalysis.Checked;
                UpdateOCRVLStatus("正在初始化 OCR-VL 引擎...");

                await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (isOCRVLLayoutAnalysis)
                    {
                        ocrvlService.InitDoc(configPath);
                    }
                    else
                    {
                        ocrvlService.Init(configPath);
                    }
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                isOCRVLInitSuccess = true;
                string mode = isOCRVLLayoutAnalysis ? "版面分析模式(InitDoc)" : "普通模式(Init)";
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化成功，当前为{mode}");
                UpdateOCRVLStatus($"初始化成功，当前为{mode}");
            }
            catch (OperationCanceledException)
            {
                try
                {
                    ocrvlService.FreeDocAnalyser();
                    ocrvlService.FreeEngine();
                }
                catch
                {
                }

                isOCRVLInitSuccess = false;
                UpdateOCRVLStatus("初始化已取消");
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化已取消");
            }
            catch (Exception ex)
            {
                isOCRVLInitSuccess = false;
                UpdateOCRVLStatus($"初始化失败: {ex.Message}");
                LogOCRVLMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR-VL 初始化失败: {ex.Message}");
            }
            finally
            {
                EndOCRVLOperation();
            }
        }

        private async void buttonOCRVLRec_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.tif;*.tiff|所有文件|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCRVLOperation();
            try
            {
                textBoxOCRVLResult.Clear();
                foreach (var filePath in openFileDialog.FileNames)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await RecOCRVLAsync(Path.GetFullPath(filePath), cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                LogOCRVLMessage("OCR-VL 任务已取消");
                UpdateOCRVLStatus("任务已取消");
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"图片识别异常: {ex.Message}");
                UpdateOCRVLStatus($"图片识别异常: {ex.Message}");
            }
            finally
            {
                EndOCRVLOperation();
            }
        }

        private async void buttonOCRVLRecPDF_Click(object? sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCRVLOperation();
            try
            {
                textBoxOCRVLResult.Clear();
                string pdfPath = Path.GetFullPath(openFileDialog.FileName);
                LogOCRVLMessage($"正在处理PDF文件: {pdfPath}");
                string imagePath = await Task.Run(() => ConvertPdfFirstPageToImage(pdfPath, "_ocrvl", cancellationToken), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                LogOCRVLMessage($"已保存转换后的图片到uploads: {imagePath}");
                await RecOCRVLAsync(imagePath, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogOCRVLMessage("PDF识别已取消");
                UpdateOCRVLStatus("PDF识别已取消");
            }
            catch (Exception ex)
            {
                LogOCRVLMessage($"PDF识别异常: {ex.Message}");
                UpdateOCRVLStatus($"PDF识别异常: {ex.Message}");
            }
            finally
            {
                EndOCRVLOperation();
            }
        }

        private void buttonOCRVLCancel_Click(object? sender, EventArgs e)
        {
            ocrVlOperationCts?.Cancel();
            UpdateOCRVLStatus("已请求取消，当前 DLL 调用返回后会停止后续任务");
        }

        private void buttonOCRVLFreeEngine_Click(object? sender, EventArgs e)
        {
            try
            {
                if (isOCRVLBusy)
                {
                    buttonOCRVLCancel_Click(sender, e);
                    return;
                }

                ocrvlService.FreeDocAnalyser();
                ocrvlService.FreeEngine();
                isOCRVLInitSuccess = false;
                isOCRVLLayoutAnalysis = false;
                SetOCRVLBusy(false);

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
    }
}
