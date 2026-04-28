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
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using WinFormsApp.Services;

namespace WinFormsApp
{
    public partial class MainForm : Form
    {
        private StringBuilder message = new StringBuilder();
        private readonly IOCRService ocrService;
        private readonly IOCRVLService ocrvlService;

        public static bool use_gpu = false;
        public static int gpu_id = 0;
        public static int cpu_threads = Environment.ProcessorCount;
        public static int cpu_mem = 4000;
        public static string RecFilepath = "";
        public static bool outPutJson = false;
        public static bool use_tensorrt = false;
        public static int recCount = 1;
        public static int model_type = 0;

        private bool isInitSuccess;
        private bool isOCRTextReady;
        private bool isOCRStructureReady;
        private bool isOCRVLInitSuccess;
        private bool isOCRVLLayoutAnalysis;

        public MainForm()
        {
            InitializeComponent();
            ocrService = OCREngine.ocrService;
            ocrvlService = new OCRVLService();
            ActiveControl = pictureBoxImg;
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string github = "--QQ群：475159576 https://github.com/PaddleOCRCore/PaddleOCRApi";
                Text = AssemblyHepler.AssemblyTitle + " V" + AssemblyHepler.AssemblyVersion + github;
                comboBoxPrompt.Items.AddRange(new object[]
                {
                    "OCR:",
                    "Table Recognition:",
                    "Formula Recognition:",
                    "Chart Recognition:",
                    "Spotting:",
                    "Seal Recognition:"
                });
                comboBoxPrompt.SelectedIndex = 0;

                comboBoxModel.SelectedIndex = 0;
                RecFilepath = Path.Combine(Application.StartupPath, "output");
                Directory.CreateDirectory(RecFilepath);
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "uploads"));

                buttonFreeEngine.Enabled = false;
                InitializeOCRVLDefaults();
                InitializeUVDocParameters();
            }
            catch (Exception ex)
            {
                message.Append(ex.Message);
                textBoxResult.Text = message.ToString();
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
            catch
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

        public void LogMessage(string infoValue)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(LogMessage), infoValue);
                return;
            }

            textBoxResult.AppendText(infoValue);
            textBoxResult.AppendText(Environment.NewLine);
            textBoxResult.SelectionStart = textBoxResult.Text.Length;
            textBoxResult.ScrollToCaret();
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            ocrOperationCts?.Cancel();
            ocrVlOperationCts?.Cancel();
            uvDocOperationCts?.Cancel();

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

            try
            {
                if (!string.IsNullOrEmpty(outputImagePath) && File.Exists(outputImagePath))
                {
                    File.Delete(outputImagePath);
                }
            }
            catch
            {
            }

            uvdocService?.FreeUVDocEngine();
            ocrOperationCts?.Dispose();
            ocrVlOperationCts?.Dispose();
            uvDocOperationCts?.Dispose();
        }

        private static Image CloneImageFromFile(string path)
        {
            using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using Image image = Image.FromStream(stream);
            return new Bitmap(image);
        }
    }
}
