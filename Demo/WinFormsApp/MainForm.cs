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
using System.Diagnostics;
using System.Text;
using WinFormsApp.Services;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        StringBuilder message = new StringBuilder();
        private readonly IOCRService ocrService;
        public static bool use_gpu = true;//是否使用GPU
        public static int gpu_id = 0;//GPUId
        public static int cpu_threads = 30; //CPU预测时的线程数
        public static string RecFilepath="";
        public MainForm()
        {
            InitializeComponent();
            ocrService = OCREngine.ocrService;
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                comboBoxuse_gpu.SelectedIndex = 0;
                RecFilepath = Path.Combine(Application.StartupPath, "output");
                if (!Directory.Exists(RecFilepath))
                {
                    Directory.CreateDirectory(RecFilepath);
                }
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
                textBoxResult.Text = message.ToString();
            }
        }

        private void buttonInit_Click(object sender, EventArgs e)
        {
            try
            {
                LogMessage("正在初始化,请稍后...");
                //使用 Task 进行异步操作
                Task.Run(async () =>
                {
                    OCREngine.use_gpu = use_gpu;
                    OCREngine.gpu_id = gpu_id;
                    OCREngine.cpu_threads = cpu_threads;
                    string initmsg = OCREngine.GetOCREngine();
                    if (string.IsNullOrEmpty(initmsg))
                    {
                        LogMessage("初始化成功！");
                    }
                    else
                    {
                        LogMessage(initmsg);
                    }
                    await Task.CompletedTask;
                }).Wait();
                this.buttonInit.Enabled = false;
                this.comboBoxuse_gpu.Enabled = false;
                this.numDowngpu_id.Enabled = false;
                this.numDowncpu_threads.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonRec_Click(object sender, EventArgs e)
        {
            try
            {
                message = new StringBuilder();
                string result = "";
                string recFileName = "";
                OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
                OpenFileDialog1.Filter = "所有文件(*.jpg)|*.*|jpg(*.jpg)|*.png|png(*.png)|*.png|bmp(*.bmp)|*.bmp|jpeg(*.jpeg)|*.jpeg";
                OpenFileDialog1.Multiselect = false;
                if (DialogResult.OK == OpenFileDialog1.ShowDialog())
                {
                    var stopwatch = new Stopwatch();
                    var startTime = DateTime.Now;
                    message.Append($"开始时间: {startTime:yyyy-MM-dd HH:mm:ss.fff}");
                    message.Append(Environment.NewLine);
                    stopwatch.Start();
                    string filePath = Path.GetFullPath(OpenFileDialog1.FileName);
                    recFileName = Path.Combine(RecFilepath, Path.GetFileName(OpenFileDialog1.FileName));
                    textBoxResult.Text = message.ToString();
                    OCRResult ocrResult = ocrService.Detect(filePath);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in ocrResult.WordsResult)
                    {
                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Append(Environment.NewLine);
                        }
                        stringBuilder.Append(item.Words);
                    }
                    result = stringBuilder.ToString();
                    stopwatch.Stop();
                    var endTime = DateTime.Now;
                    message.Append($"结束时间: {endTime:yyyy-MM-dd HH:mm:ss.fff}");
                    message.Append(Environment.NewLine);
                    message.Append($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                    message.Append(Environment.NewLine);
                    message.Append(result);
                    message.Append(Environment.NewLine);
                    message.Append($"输出json: {ocrResult.JsonText}");

                }
                OpenFileDialog1.Dispose();
                textBoxResult.Text = message.ToString();
                pictureBoxImg.Image = ImageTools.LoadImage(recFileName);

            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
                textBoxResult.Text = message.ToString();
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            message = new StringBuilder();
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

        private void comboBoxuse_gpu_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBoxuse_gpu.SelectedIndex)
            {
                case 0:
                    use_gpu = false;
                    break;
                case 1:
                    use_gpu = true;
                    break;
                default:
                    use_gpu = false;
                    break;
            }
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
    }
}
