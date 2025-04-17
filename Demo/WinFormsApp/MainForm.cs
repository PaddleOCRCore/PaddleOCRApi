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
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;
using WinFormsApp.Services;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        StringBuilder message = new StringBuilder();
        private readonly IOCRService ocrService;
        public static bool use_gpu = false;//是否使用GPU
        public static int gpu_id = 0;//GPUId
        public static int cpu_threads = 30; //CPU预测时的线程数
        public static int cpu_mem = 4000;//CPU内存占用上限，单位MB。-1表示不限制，达到上限将自动回收
        public static string RecFilepath = "";
        public static bool outPutJson = false;//是否输出JSON
        public static int recCount = 1; //OCR识别时同一张图片模拟调用接口次数
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
                comboBoxJson.SelectedIndex = 0;
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
            //LogMessage($"{DateTime.Now:HH:mm:ss.fff}:正在初始化,请稍后...");
            try
            {
                OCREngine.use_gpu = use_gpu;
                OCREngine.gpu_id = gpu_id;
                OCREngine.cpu_threads = cpu_threads;
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
                }
                else
                {
                    this.buttonRecTable.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now:HH:mm:ss.fff}:OCR初始化失败:{ex.Message}");
            }
        }
        private string RecOCR(string filePath)
        {
            string result = "";
            var stopwatch = new Stopwatch();
            var startTime = DateTime.Now;
            LogMessage($"Image: {filePath}");
            LogMessage($"开始时间: {startTime:HH:mm:ss.fff}");
            stopwatch.Start();
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
            var endTime = DateTime.Now;
            LogMessage($"结束时间: {endTime:HH:mm:ss.fff}");
            LogMessage($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
            LogMessage(result);
            if (outPutJson)
                LogMessage($"输出json: {ocrResult.JsonText}");
            return result;
        }

        private void buttonRec_Click(object sender, EventArgs e)
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
                            string filePath = Path.GetFullPath(regfile);
                            recFileName = Path.Combine(RecFilepath, Path.GetFileName(regfile));
                            result = RecOCR(filePath);
                        }
                    }
                    pictureBoxImg.Image = ImageTools.LoadImage(recFileName);
                }
                OpenFileDialog1.Dispose();

            }
            catch (Exception ex)
            {
                LogMessage(ex.Message);
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
            // 定义输出文件夹和文件名
            string outputFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");
            string htmlfile = Path.Combine(outputFolder, $"{Path.GetFileNameWithoutExtension(filePath)}.html");

            // 确保输出文件夹存在
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            using (StreamWriter sw = new StreamWriter(htmlfile, false, System.Text.Encoding.GetEncoding("utf-8")))
            {
                sw.Write(ocrResult);
            }
            try
            {
                // 使用默认的浏览器打开HTML文件
                Process.Start(new ProcessStartInfo
                {
                    FileName = htmlfile,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"无法打开HTML文件：{ex.Message}");
                LogMessage($"无法打开HTML文件：{ex.Message}");
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
                        pictureBoxImg.Image = ImageTools.LoadImage(filePath);
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
            string url = "https://paddlepaddle.github.io/PaddleOCR/latest/ppocr/model_list.html";
            try
            {
                Process.Start(new ProcessStartInfo(url)
                {
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"无法打开网页：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void buttonGetBase64_Click(object sender, EventArgs e)
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
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"{DateTime.Now:HH:mm:ss.fff}:使用GPU时请下载对应的paddle_inference解压" + Environment.NewLine); 
                    sb.Append($"解压后将以下dll文件复制到程序运行文件夹中：" + Environment.NewLine);
                    sb.Append($"paddle\\lib目录下的common.dll和paddle_inference.dll" + Environment.NewLine);
                    sb.Append($"third_party\\install\\mkldnn\\lib目录下的mkldnn.dll" + Environment.NewLine);
                    sb.Append($"third_party\\install\\mklml\\lib目录下的libiomp5md.dll和mklml.dll" + Environment.NewLine);
                    sb.Append($"安装指定版本的CUDA以及CUDNN" + Environment.NewLine);
                    sb.Append($"复制对应的CUDNN中的cudnn64_8.dll(CUDNN8的文件名)或cudnn64_9.dll(CUDNN9的文件名)到程序运行文件夹中" + Environment.NewLine);
                    sb.Append($"C:\\Program Files\\NVIDIA GPU Computing Toolkit\\CUDA\\v12.4\\bin\\cudnn64_8.dll" + Environment.NewLine);
                    LogMessage(sb.ToString());
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

        private void comboBoxJson_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.comboBoxJson.SelectedIndex)
            {
                case 0:
                    outPutJson = false;
                    break;
                case 1:
                    outPutJson = true;
                    break;
                default:
                    outPutJson = false;
                    break;
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
