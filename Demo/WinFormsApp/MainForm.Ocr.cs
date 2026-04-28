using PaddleOCRSDK;
using SkiaSharp;
using System.Diagnostics;
using System.Text;
using WinFormsApp.Services;
using WinFormsApp.Utils;

namespace WinFormsApp
{
    /// <summary>
    /// PaddleOCR识别
    /// </summary>
    public partial class MainForm
    {
        private CancellationTokenSource? ocrOperationCts;
        private bool isOCRBusy;

        private CancellationToken BeginOCROperation()
        {
            ocrOperationCts?.Dispose();
            ocrOperationCts = new CancellationTokenSource();
            SetOCRBusy(true);
            return ocrOperationCts.Token;
        }

        private void EndOCROperation()
        {
            SetOCRBusy(false);
            ocrOperationCts?.Dispose();
            ocrOperationCts = null;
        }

        private void SetOCRBusy(bool busy)
        {
            isOCRBusy = busy;
            buttonCancelOCR.Enabled = busy;
            buttonInit.Enabled = !busy && !isInitSuccess;
            buttonFreeEngine.Enabled = !busy && isInitSuccess;
            buttonRec.Enabled = !busy && isOCRTextReady;
            buttonRecClipboard.Enabled = !busy && isOCRTextReady;
            buttonRecPDF.Enabled = !busy && isOCRTextReady;
            buttonRecStructure.Enabled = !busy && isOCRStructureReady;
            buttonPostFile.Enabled = !busy;
            buttonGetBase64.Enabled = !busy;
            buttonDownModels.Enabled = !busy;
            comboBoxModel.Enabled = !busy && !isInitSuccess;
            chkUseGpu.Enabled = !busy && !isInitSuccess;
            chkUseTensorRT.Enabled = !busy && !isInitSuccess && chkUseGpu.Checked;
            chkJson.Enabled = !busy;
            chkReturnWordBox.Enabled = !busy && !isInitSuccess;
            numericUpDownThread.Enabled = !busy;
            numDowncpu_threads.Enabled = !busy && !isInitSuccess;
            numDowngpu_id.Enabled = !busy && !isInitSuccess;
            numericUpDowncpu_mem.Enabled = !busy && !isInitSuccess;
        }

        private async void buttonInit_Click(object sender, EventArgs e)
        {
            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                isInitSuccess = false;
                OCREngine.use_gpu = use_gpu;
                OCREngine.gpu_id = gpu_id;
                OCREngine.cpu_threads = cpu_threads;
                OCREngine.cpu_mem = cpu_mem;
                OCREngine.use_tensorrt = use_tensorrt;
                OCREngine.return_word_box = chkReturnWordBox.Checked;

                if (model_type == 0)
                {
                    OCREngine.det_infer = "PP-OCRv5_mobile_det_infer";
                    OCREngine.rec_infer = "PP-OCRv5_mobile_rec_infer";
                    OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                }
                else if (model_type == 1)
                {
                    OCREngine.det_infer = "PP-OCRv5_server_det_infer";
                    OCREngine.rec_infer = "PP-OCRv5_server_rec_infer";
                    OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                }
                else
                {
                    OCREngine.det_infer = "PP-OCRv4_mobile_det_infer";
                    OCREngine.rec_infer = "PP-OCRv4_mobile_rec_infer";
                    OCREngine.cls_infer = "PP-LCNet_x1_0_textline_ori";
                }

                string initmsg = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return OCREngine.GetOCREngine();
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                LogMessage(string.IsNullOrEmpty(initmsg)
                    ? $"{DateTime.Now:HH:mm:ss.fff}:文本识别初始化成功！"
                    : $"{DateTime.Now:HH:mm:ss.fff}:{initmsg}");

                bool textReady = initmsg.IndexOf("初始化成功", StringComparison.Ordinal) >= 0;

                initmsg = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return OCREngine.GetOCRStructureEngine();
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                LogMessage(string.IsNullOrEmpty(initmsg)
                    ? $"{DateTime.Now:HH:mm:ss.fff}:版面识别初始化成功！"
                    : $"{DateTime.Now:HH:mm:ss.fff}:{initmsg}");

                bool structureReady = initmsg.IndexOf("初始化成功", StringComparison.Ordinal) >= 0;
                isOCRTextReady = textReady;
                isOCRStructureReady = structureReady;
                isInitSuccess = textReady || structureReady;
            }
            catch (OperationCanceledException)
            {
                OCREngine.FreeEngine();
                isOCRTextReady = false;
                isOCRStructureReady = false;
                isInitSuccess = false;
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR初始化已取消");
            }
            catch (Exception ex)
            {
                isOCRTextReady = false;
                isOCRStructureReady = false;
                isInitSuccess = false;
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR初始化失败:{ex.Message}");
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async Task<string> RecOCRAsync(string filePath, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            LogMessage($"开始时间: {DateTime.Now:HH:mm:ss.fff}");

            OCRResult ocrResult = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ocrService.Detect(filePath);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            string result = BuildOCRText(ocrResult);
            LogMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogOCRResult(result, ocrResult);
            LogMessage("===============================================");
            return result;
        }

        private static string BuildOCRText(OCRResult ocrResult)
        {
            if (ocrResult.Code != 1)
            {
                return ocrResult.ErrorMsg;
            }

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in ocrResult.WordsResult)
            {
                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
                stringBuilder.Append(item.Words);
            }
            return stringBuilder.ToString();
        }

        private void LogOCRResult(string result, OCRResult ocrResult)
        {
            if (!string.IsNullOrEmpty(result))
            {
                LogMessage(result);
            }
            else
            {
                LogMessage("识别失败:" + ocrService.GetError());
            }

            if (outPutJson && !string.IsNullOrEmpty(ocrResult.JsonText))
            {
                LogMessage($"输出json: {FormatJsonSafe(ocrResult.JsonText)}");
            }
        }

        private async void buttonRec_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();

                for (int i = 0; i < recCount; i++)
                {
                    foreach (var regfile in openFileDialog.FileNames)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        LogMessage($"Image: {Path.GetFileName(regfile)}");
                        string filePath = Path.GetFullPath(regfile);
                        string recFileName = Path.Combine(RecFilepath, Path.GetFileName(regfile));
                        await RecOCRAsync(filePath, cancellationToken);
                        if (File.Exists(recFileName))
                        {
                            pictureBoxImg.ImgPath = recFileName;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("OCR文本识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async void buttonRecClipboard_Click(object sender, EventArgs e)
        {
            if (!Clipboard.ContainsImage())
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:剪贴板中没有图片！");
                return;
            }

            Image? clipboardImage = Clipboard.GetImage();
            if (clipboardImage == null)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:无法从剪贴板获取图片！");
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();

                string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                Directory.CreateDirectory(uploadsPath);
                string clipboardImagePath = Path.Combine(uploadsPath, $"clipboard_{DateTime.Now:yyyyMMddHHmmss}.png");
                using (clipboardImage)
                {
                    await Task.Run(() =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        clipboardImage.Save(clipboardImagePath, System.Drawing.Imaging.ImageFormat.Png);
                    }, cancellationToken);
                }

                LogMessage($"剪贴板图片已保存到: {clipboardImagePath}");
                LogMessage("Image: 剪贴板图片");
                OCRResult ocrResult = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ocrService.Detect(clipboardImagePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                string result = BuildOCRText(ocrResult);
                LogOCRResult(result, ocrResult);

                string resultImagePath = Path.Combine(RecFilepath, Path.GetFileName(clipboardImagePath));
                if (File.Exists(resultImagePath))
                {
                    pictureBoxImg.ImgPath = resultImagePath;
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("剪贴板OCR识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:剪贴板OCR识别异常:{ex.Message}");
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async Task<string> RecOCRFromPDFAsync(string pdfFilePath, CancellationToken cancellationToken)
        {
            SKBitmap? bitmap = null;
            try
            {
                var stopwatch = Stopwatch.StartNew();
                LogMessage($"PDF文件: {pdfFilePath}");
                LogMessage($"开始时间: {DateTime.Now:HH:mm:ss.fff}");

                string uploadImagePath = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    byte[] pdfBytes = File.ReadAllBytes(pdfFilePath);
                    using MemoryStream memoryStream = new MemoryStream(pdfBytes);
                    bitmap = PDFtoImage.Conversion.ToImage(memoryStream);

                    using SKImage image = SKImage.FromBitmap(bitmap);
                    using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
                    byte[] imageBytes = data.ToArray();

                    string uploadsPath = Path.Combine(Application.StartupPath, "uploads");
                    Directory.CreateDirectory(uploadsPath);
                    string path = Path.Combine(uploadsPath, Path.GetFileNameWithoutExtension(pdfFilePath) + "_page1.png");
                    File.WriteAllBytes(path, imageBytes);
                    return path;
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                LogMessage($"已保存转换后的图片到uploads: {uploadImagePath}");
                OCRResult ocrResult = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ocrService.Detect(uploadImagePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();

                string result = BuildOCRText(ocrResult);
                LogMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
                LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                LogOCRResult(result, ocrResult);
                return result;
            }
            finally
            {
                bitmap?.Dispose();
            }
        }

        private async void buttonRecPDF_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "PDF文件(*.pdf)|*.pdf";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                string filePath = Path.GetFullPath(openFileDialog.FileName);
                LogMessage($"正在处理PDF文件: {filePath}");
                string result = await RecOCRFromPDFAsync(filePath, cancellationToken);
                if (!string.IsNullOrEmpty(result))
                {
                    string uploadFileName = Path.GetFileNameWithoutExtension(filePath) + "_page1.png";
                    string recFileName = Path.Combine(RecFilepath, uploadFileName);
                    if (File.Exists(recFileName))
                    {
                        pictureBoxImg.ImgPath = recFileName;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("PDF识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage($"PDF识别异常: {ex.Message}");
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async Task<string> RecOCRStructureAsync(string filePath, CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();
            LogMessage($"Image: {filePath}");
            LogMessage($"开始时间: {DateTime.Now:HH:mm:ss.fff}");

            string layoutJson = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return ocrService.DetectLayout(filePath);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            LogMessage($"结束时间: {DateTime.Now:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogMessage("版面识别结果:");
            LogMessage(FormatJsonSafe(layoutJson));
            return layoutJson;
        }

        private async void buttonRecStructure_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                textBoxResult.Text = "";
                message = new StringBuilder();
                foreach (var regfile in openFileDialog.FileNames)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string filePath = Path.GetFullPath(regfile);
                    string result = await RecOCRStructureAsync(filePath, cancellationToken);
                    try
                    {
                        LayoutDetectResult layoutResult = ocrService.ParseLayoutResult(result);
                        if (!string.IsNullOrEmpty(layoutResult.VisPath) && File.Exists(layoutResult.VisPath))
                        {
                            pictureBoxImg.ImgPath = layoutResult.VisPath;
                        }
                        else if (File.Exists(filePath))
                        {
                            pictureBoxImg.ImgPath = filePath;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogMessage($"版面识别结果解析失败:{ex.Message}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                LogMessage("OCR版面识别已取消");
            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private void buttonDownModels_Click(object sender, EventArgs e)
        {
            string urlV5 = "https://www.paddleocr.ai/latest/version3.x/pipeline_usage/OCR.html";
            try
            {
                LogMessage($"PP-OCRv5/PP-OCRv4模型下载地址：{urlV5}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开网页：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void buttonGetBase64_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                string filePath = openFileDialog.FileName;
                string base64 = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return ImageTools.GetBase64FromImage(filePath);
                }, cancellationToken);
                textBoxResult.Text = base64;
            }
            catch (OperationCanceledException)
            {
                LogMessage("获取图片Base64已取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"图片格式异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private async void buttonPostFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBoxApiAddress.Text.Trim()))
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:WebApi地址不能为空！");
                return;
            }

            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
            openFileDialog.Multiselect = false;
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            CancellationToken cancellationToken = BeginOCROperation();
            try
            {
                string filePath = openFileDialog.FileName;
                string apiAddress = textBoxApiAddress.Text.Trim();
                textBoxResult.Text = await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    return HttpHelper.PostFile(apiAddress, filePath);
                }, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                LogMessage("API接口测试已取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"调用接口异常：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndOCROperation();
            }
        }

        private void buttonCancelOCR_Click(object sender, EventArgs e)
        {
            ocrOperationCts?.Cancel();
            LogMessage("已请求取消，当前 DLL/网络调用返回后会停止后续任务");
        }

        private void chkUseGpu_CheckedChanged(object sender, EventArgs e)
        {
            use_gpu = chkUseGpu.Checked;
            chkUseTensorRT.Enabled = chkUseGpu.Checked && chkUseGpu.Enabled;
            if (use_gpu)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append($"{DateTime.Now:HH:mm:ss.fff}:使用GPU时请下载对应的paddle_inference解压" + Environment.NewLine);
                sb.Append("解压后将以下dll文件复制到程序运行文件夹中：" + Environment.NewLine);
                sb.Append("paddle\\lib目录下的common.dll和paddle_inference.dll" + Environment.NewLine);
                sb.Append("third_party\\install\\mkldnn\\lib目录下的mkldnn.dll" + Environment.NewLine);
                sb.Append("third_party\\install\\mklml\\lib目录下的libiomp5md.dll和mklml.dll" + Environment.NewLine);
                sb.Append("安装指定版本的CUDA以及CUDNN" + Environment.NewLine);
                sb.Append("复制对应的CUDA/CUDNN DLL到程序运行文件夹中" + Environment.NewLine);
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
            if (numDowngpu_id.Value >= 0)
            {
                gpu_id = Convert.ToInt32(numDowngpu_id.Value);
            }
        }

        private void numDowncpu_threads_ValueChanged(object sender, EventArgs e)
        {
            if (numDowncpu_threads.Value > 0)
            {
                cpu_threads = Convert.ToInt32(numDowncpu_threads.Value);
            }
        }

        private void numericUpDowncpu_mem_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDowncpu_mem.Value > 0)
            {
                cpu_mem = Convert.ToInt32(numericUpDowncpu_mem.Value);
            }
        }

        private void numericUpDownThread_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownThread.Value > 0)
            {
                recCount = Convert.ToInt32(numericUpDownThread.Value);
            }
        }

        private void comboBoxModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            model_type = comboBoxModel.SelectedIndex;
        }

        private void buttonFreeEngine_Click(object sender, EventArgs e)
        {
            if (isOCRBusy)
            {
                buttonCancelOCR_Click(sender, e);
                return;
            }

            OCREngine.FreeEngine();
            isOCRTextReady = false;
            isOCRStructureReady = false;
            isInitSuccess = false;
            SetOCRBusy(false);
            LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR引擎已释放！");
        }
    }
}
