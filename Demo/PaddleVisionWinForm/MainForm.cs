using PaddleOCRSDK;

namespace PaddleVisionWinForm
{
    public partial class MainForm : Form
    {
        private IUVDocService? uvdocService;
        private string? currentImagePath;
        private string? outputImagePath;
        private System.Diagnostics.Stopwatch? processStopwatch;

        public MainForm()
        {
            InitializeComponent();
            LoadDefaultParameters();
        }

        private void LoadDefaultParameters()
        {
            chkUseGpu.Checked = false;
            numCpuThreads.Value = Environment.ProcessorCount;
            numGpuId.Value = 0;
            numGpuMem.Value = 2000;
            chkUseTensorRT.Checked = false;
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            InitializeEngine();
        }

        private void InitializeEngine()
        {
            try
            {
                // 释放旧的引擎实例
                uvdocService?.Dispose();
                
                // 创建新的服务实例
                uvdocService = new UVDocService();
                var parameter = new UVDocParameter
                {
                    enable_mkldnn = true,
                    cpu_threads = (int)numCpuThreads.Value,
                    use_gpu = chkUseGpu.Checked,
                    gpu_id = (int)numGpuId.Value,
                    gpu_mem = (int)numGpuMem.Value,
                    use_tensorrt = chkUseTensorRT.Checked
                };
                // 模型路径（相对于程序目录）
                string modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models", "UVDoc_infer");

                // 如果模型不存在，尝试使用上级目录的模型
                if (!Directory.Exists(modelPath))
                {
                    modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "models", "UVDoc_infer");
                    modelPath = Path.GetFullPath(modelPath);
                }

                UpdateStatus($"正在初始化模型: {modelPath}");

                // 禁用参数控件
                SetParameterControlsEnabled(false);
                btnInitialize.Enabled = false;

                if (uvdocService.Initialize(modelPath, parameter))
                {
                    UpdateStatus($"模型初始化成功！[GPU: {(parameter.use_gpu ? "是" : "否")}, CPU线程: {parameter.cpu_threads}]");
                    btnUpload.Enabled = true; // 允许上传图片
                    btnProcess.Enabled = false; // 等待上传图片
                }
                else
                {
                    string error = uvdocService.GetLastError();
                    UpdateStatus($"模型初始化失败: {error}");
                    MessageBox.Show($"模型初始化失败！\n错误信息: {error}\n\n请确保 models/UVDoc_infer 目录存在且包含正确的模型文件。",
                        "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    SetParameterControlsEnabled(true);
                    btnInitialize.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"初始化异常: {ex.Message}");
                MessageBox.Show($"初始化异常: {ex.Message}\n\n请确保 PaddleDocVision.dll 及其依赖库在程序目录中。",
                    "初始化错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetParameterControlsEnabled(true);
                btnInitialize.Enabled = true;
            }
        }

        private void SetParameterControlsEnabled(bool enabled)
        {
            chkUseGpu.Enabled = enabled;
            numCpuThreads.Enabled = enabled;
            numGpuId.Enabled = enabled;
            numGpuMem.Enabled = enabled;
            chkUseTensorRT.Enabled = enabled;
        }

        private void chkUseGpu_CheckedChanged(object sender, EventArgs e)
        {
            // GPU相关选项只在启用GPU时可用
            numGpuId.Enabled = chkUseGpu.Checked && chkUseGpu.Enabled;
            numGpuMem.Enabled = chkUseGpu.Checked && chkUseGpu.Enabled;
            chkUseTensorRT.Enabled = chkUseGpu.Checked && chkUseGpu.Enabled;
        }

        private void btnUpload_Click(object sender, EventArgs e)
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

                        btnProcess.Enabled = true;
                        btnSave.Enabled = false;
                        UpdateStatus($"已加载图像: {Path.GetFileName(currentImagePath)}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"加载图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus($"加载图像失败: {ex.Message}");
                    }
                }
            }
        }

        private async void btnProcess_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentImagePath) || uvdocService == null)
            {
                MessageBox.Show("请先上传图像并确保引擎已初始化", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                btnProcess.Enabled = false;
                btnUpload.Enabled = false;
                btnSave.Enabled = false;
                UpdateStatus("正在处理图像，请稍候...");

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

                    UpdateStatus($"图像处理完成！耗时: {elapsedMilliseconds:F0} ms");
                    btnSave.Enabled = true;
                }
                else
                {
                    UpdateStatus($"处理失败：输出文件未生成（耗时: {elapsedMilliseconds:F0} ms）");
                    MessageBox.Show("处理失败：输出文件未生成", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                string error = uvdocService.GetLastError();
                string message = $"处理图像时出错: {ex.Message}";
                if (!string.IsNullOrEmpty(error))
                {
                    message += $"\nDLL错误信息: {error}";
                }
                
                MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                UpdateStatus($"处理失败: {ex.Message}");
            }
            finally
            {
                btnProcess.Enabled = true;
                btnUpload.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
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
                        UpdateStatus($"图像已保存: {saveFileDialog.FileName}");
                        MessageBox.Show("图像保存成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存图像失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus($"保存失败: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateStatus(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateStatus), message);
                return;
            }

            lblStatus.Text = $"状态: {message}";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // 清理资源
            pictureBoxOriginal.Image?.Dispose();
            pictureBoxOutput.Image?.Dispose();
            uvdocService?.Dispose();
            // 删除临时文件
            try
            {
                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }
            }
            catch { }
        }
    }
}
