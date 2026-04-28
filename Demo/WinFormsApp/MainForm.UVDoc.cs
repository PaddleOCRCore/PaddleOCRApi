using PaddleOCRSDK;
using System.Diagnostics;

namespace WinFormsApp
{
    /// <summary>
    /// 文档图像矫正
    /// </summary>
    public partial class MainForm
    {
        private IUVDocService? uvdocService;
        private string? currentImagePath;
        private string? outputImagePath;
        private CancellationTokenSource? uvDocOperationCts;
        private bool isUVDocBusy;

        private void InitializeUVDocParameters()
        {
            chkUVDocUseGpu.Checked = false;
            numUVDocCpuThreads.Value = Environment.ProcessorCount;
            numUVDocGpuId.Value = 0;
            numUVDocGpuMem.Value = 2000;
            chkUVDocUseTensorRT.Checked = false;
        }

        private CancellationToken BeginUVDocOperation()
        {
            uvDocOperationCts?.Dispose();
            uvDocOperationCts = new CancellationTokenSource();
            SetUVDocBusy(true);
            return uvDocOperationCts.Token;
        }

        private void EndUVDocOperation()
        {
            SetUVDocBusy(false);
            uvDocOperationCts?.Dispose();
            uvDocOperationCts = null;
        }

        private void SetUVDocBusy(bool busy)
        {
            isUVDocBusy = busy;
            bool initialized = uvdocService?.IsInitialized == true;

            btnUVDocCancel.Enabled = busy;
            btnUVDocInitialize.Enabled = !busy && !initialized;
            btnUVDocUpload.Enabled = !busy && initialized;
            btnUVDocProcess.Enabled = !busy && initialized && !string.IsNullOrEmpty(currentImagePath);
            btnUVDocSave.Enabled = !busy && !string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath);
            btnUVDocFreeEngine.Enabled = !busy && initialized;
            SetUVDocParameterControlsEnabled(!busy && !initialized);
        }

        private async void btnUVDocInitialize_Click(object sender, EventArgs e)
        {
            CancellationToken cancellationToken = BeginUVDocOperation();
            try
            {
                await InitializeUVDocEngineAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                uvdocService?.FreeUVDocEngine();
                uvdocService = null;
                UpdateUVDocStatus("初始化已取消");
            }
            catch (Exception ex)
            {
                UpdateUVDocStatus($"初始化异常: {ex.Message}");
                MessageBox.Show($"初始化异常: {ex.Message}\n\n请确保 PaddleOCR.dll 及其依赖库在程序目录中。",
                    "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                EndUVDocOperation();
            }
        }

        private async Task InitializeUVDocEngineAsync(CancellationToken cancellationToken)
        {
            uvdocService?.FreeUVDocEngine();
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

            string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "UVDoc_infer");
            if (!Directory.Exists(modelPath))
            {
                modelPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "models", "UVDoc_infer"));
            }

            UpdateUVDocStatus($"正在初始化模型: {modelPath}");
            bool initialized = await Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                return uvdocService.Initialize(modelPath, parameter);
            }, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (initialized)
            {
                UpdateUVDocStatus($"模型初始化成功！[GPU: {(parameter.use_gpu ? "是" : "否")}, CPU线程: {parameter.cpu_threads}]");
                return;
            }

            string error = uvdocService.GetLastError();
            uvdocService.FreeUVDocEngine();
            uvdocService = null;
            UpdateUVDocStatus($"模型初始化失败: {error}");
            MessageBox.Show($"模型初始化失败！\n错误信息: {error}\n\n请确保 models/UVDoc_infer 目录存在且包含正确的模型文件。",
                "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            numUVDocGpuId.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
            numUVDocGpuMem.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
            chkUVDocUseTensorRT.Enabled = chkUVDocUseGpu.Checked && chkUVDocUseGpu.Enabled;
        }

        private void btnUVDocUpload_Click(object sender, EventArgs e)
        {
            using OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp;*.tiff|所有文件|*.*";
            openFileDialog.Title = "选择要矫正的图像";

            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                currentImagePath = openFileDialog.FileName;
                SetPictureBoxImage(pictureBoxOriginal, currentImagePath);
                SetPictureBoxImage(pictureBoxOutput, null);

                outputImagePath = null;
                SetUVDocBusy(false);
                UpdateUVDocStatus($"已加载图像: {Path.GetFileName(currentImagePath)}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"加载图像失败: {ex.Message}");
            }
        }

        private async void btnUVDocProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath) || uvdocService == null)
            {
                MessageBox.Show("请先上传图像并确保引擎已初始化", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            CancellationToken cancellationToken = BeginUVDocOperation();
            try
            {
                UpdateUVDocStatus("正在处理图像，请稍候...");

                string tempDir = Path.Combine(Path.GetTempPath(), "PaddleDocVision");
                Directory.CreateDirectory(tempDir);
                outputImagePath = Path.Combine(tempDir, $"output_{DateTime.Now:yyyyMMddHHmmss}.jpg");

                Stopwatch stopwatch = Stopwatch.StartNew();
                await Task.Run(() =>
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    uvdocService.UVDocImageFile(currentImagePath, outputImagePath);
                }, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                stopwatch.Stop();

                if (File.Exists(outputImagePath))
                {
                    SetPictureBoxImage(pictureBoxOutput, outputImagePath);
                    UpdateUVDocStatus($"图像处理完成！耗时: {stopwatch.Elapsed.TotalMilliseconds:F0} ms");
                }
                else
                {
                    UpdateUVDocStatus($"处理失败：输出文件未生成（耗时: {stopwatch.Elapsed.TotalMilliseconds:F0} ms）");
                    MessageBox.Show("处理失败：输出文件未生成", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (OperationCanceledException)
            {
                UpdateUVDocStatus("图像处理已取消");
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
                EndUVDocOperation();
            }
        }

        private void btnUVDocCancel_Click(object sender, EventArgs e)
        {
            uvDocOperationCts?.Cancel();
            UpdateUVDocStatus("已请求取消，当前 DLL 调用返回后会停止后续任务");
        }

        private void btnUVDocSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputImagePath) || !File.Exists(outputImagePath))
            {
                MessageBox.Show("没有可保存的图像", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JPEG图像|*.jpg|PNG图像|*.png|所有文件|*.*";
            saveFileDialog.Title = "保存矫正后的图像";
            saveFileDialog.FileName = $"corrected_{DateTime.Now:yyyyMMddHHmmss}.jpg";

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

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

        private void UpdateUVDocStatus(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(UpdateUVDocStatus), message);
                return;
            }

            lblUVDocStatus.Text = $"状态: {message}";
        }

        private void btnUVDocFreeEngine_Click(object sender, EventArgs e)
        {
            try
            {
                if (isUVDocBusy)
                {
                    btnUVDocCancel_Click(sender, e);
                    return;
                }

                uvdocService?.FreeUVDocEngine();
                uvdocService = null;

                SetPictureBoxImage(pictureBoxOriginal, null);
                SetPictureBoxImage(pictureBoxOutput, null);

                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }

                outputImagePath = null;
                currentImagePath = null;
                SetUVDocBusy(false);
                UpdateUVDocStatus("引擎已释放！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"释放引擎失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateUVDocStatus($"释放失败: {ex.Message}");
            }
        }

        private static void SetPictureBoxImage(PictureBox pictureBox, string? path)
        {
            Image? oldImage = pictureBox.Image;
            pictureBox.Image = string.IsNullOrEmpty(path) ? null : CloneImageFromFile(path);
            oldImage?.Dispose();
        }
    }
}
