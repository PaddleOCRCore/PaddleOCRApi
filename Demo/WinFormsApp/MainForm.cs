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

using Newtonsoft.Json;
using PaddleOCRSDK;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WinFormsApp.Services;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        StringBuilder message = new StringBuilder();
        private readonly IOCRService ocrService;
        public MainForm()
        {
            InitializeComponent();
            ocrService = OCREngine.ocrService;
        }

        private void buttonInit_Click(object sender, EventArgs e)
        {
            try
            {
                string initmsg= OCREngine.GetOCREngine();
                if (string.IsNullOrEmpty(initmsg))
                {
                    message.Append("初始化成功！\r\n");
                }
                else
                {
                    message.Append($"{initmsg}\r\n");
                }
                textBoxResult.Text = message.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                message = new StringBuilder();
                string result = "";
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
                    string filePath = OpenFileDialog1.FileName;
                    //string filePath = Path.Combine(AppContext.BaseDirectory, "inference", "1231.jpeg");
                    //message.Append(filePath);
                    textBoxResult.Text = message.ToString();
                    OCRResult ocrResult = ocrService.Detect(filePath);
                    StringBuilder stringBuilder = new StringBuilder();
                    foreach (var item in ocrResult.WordsResult)
                    {
                        //if (stringBuilder.Length > 0)
                        //{
                        //    stringBuilder.Append(Environment.NewLine);
                        //}
                        stringBuilder.Append(item.Words);
                    }
                    result = stringBuilder.ToString();
                    string id_card_side = "front";
                    if (id_card_side.Equals("front"))
                    {
                        var jsonResult = new
                        {
                            姓名 = "",
                            性别 = "",
                            民族 = "",
                            出生 = "",
                            住址 = "",
                            公民身份号码 = "",
                            text = ""
                        };
                        // 定义正则表达式
                        Regex regex = new Regex(@"姓名(?<name>[^\s]+)性别(?<gender>[男女])民族(?<nation>.+?)出生(?<birth>.+?)住址(?<address>.+?)公民身份(?<zheng>.+?)号码(?<id>\d{18})");
                        // 执行匹配
                        Match match = regex.Match(result);
                        if (match.Success)
                        {
                            // 提取信息
                            string name = match.Groups["name"].Value;
                            string gender = match.Groups["gender"].Value;
                            string nation = match.Groups["nation"].Value;
                            string birth = match.Groups["birth"].Value;
                            string address = match.Groups["address"].Value;
                            string idNumber = match.Groups["id"].Value;
                            // 构建JSON对象
                            jsonResult = new
                            {
                                姓名 = name,
                                性别 = gender,
                                民族 = nation,
                                出生 = birth,
                                住址 = address,
                                公民身份号码 = idNumber,
                                text = result
                            };
                            result = JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
                        }
                    }
                    else if (id_card_side.Equals("back"))
                    {
                        var jsonResult = new
                        {
                            签发机关 = "",
                            有效期限 = "",
                            text = ""
                        };
                        // 定义正则表达式
                        Regex regex = new Regex(@"签发机关(?<issuingAuthority>.+?)有效期限(?<validityPeriod>.+)$");

                        // 执行匹配
                        Match match = regex.Match(result);
                        if (match.Success)
                        {
                            // 提取信息
                            string issuingAuthority = match.Groups["issuingAuthority"].Value;
                            string validityPeriod = match.Groups["validityPeriod"].Value;
                            // 构建JSON对象
                            jsonResult = new
                            {
                                签发机关 = issuingAuthority,
                                有效期限 = validityPeriod,
                                text = result
                            };
                            result = JsonConvert.SerializeObject(jsonResult, Formatting.Indented);
                        }
                    }
                    message.Append(result);
                    message.Append(Environment.NewLine);
                    stopwatch.Stop();
                    var endTime = DateTime.Now;
                    message.Append($"结束时间: {endTime:yyyy-MM-dd HH:mm:ss.fff}");
                    message.Append(Environment.NewLine);
                    message.Append($"总用时: {stopwatch.ElapsedMilliseconds} 毫秒");
                    message.Append(Environment.NewLine);
                    message.Append($"输出json: {ocrResult.JsonText}");
                }
                OpenFileDialog1.Dispose();
                textBoxResult.Text = message.ToString();
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
                textBoxResult.Text = message.ToString();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
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
                string base64 = GetBase64FromImage(filePath);
                textBoxResult.Text = base64;
            }
            OpenFileDialog1.Dispose();
        }
        #region 图片路径转Base64
        public static string GetBase64FromImage(string strPath)
        {
            string strbaser64 = "";
            try
            {
                using (BinaryReader reader = new BinaryReader(File.Open(strPath, FileMode.Open)))
                {
                    FileInfo fi = new FileInfo(strPath);
                    byte[] bytes = reader.ReadBytes((int)fi.Length);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        strbaser64 = Convert.ToBase64String(bytes);
                        return strbaser64;
                    }
                }
            }
            catch (Exception)
            {
                return strbaser64;
            }
        }
        #endregion
    }
}
