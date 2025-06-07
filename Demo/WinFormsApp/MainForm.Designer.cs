namespace WinFormsApp
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            buttonInit = new Button();
            buttonRec = new Button();
            textBoxResult = new TextBox();
            buttonGetBase64 = new Button();
            groupBox1 = new GroupBox();
            buttonPostFile = new Button();
            textBoxApiAddress = new TextBox();
            label8 = new Label();
            label7 = new Label();
            comboBoxModel = new ComboBox();
            buttonRecTable = new Button();
            buttonDownModels = new Button();
            numericUpDowncpu_mem = new NumericUpDown();
            label6 = new Label();
            numericUpDownThread = new NumericUpDown();
            label5 = new Label();
            label4 = new Label();
            comboBoxJson = new ComboBox();
            numDowncpu_threads = new NumericUpDown();
            label3 = new Label();
            numDowngpu_id = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            comboBoxuse_gpu = new ComboBox();
            pictureBoxImg = new PictureBox();
            groupBox2 = new GroupBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDowncpu_mem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownThread).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // buttonInit
            // 
            buttonInit.Location = new Point(606, 23);
            buttonInit.Name = "buttonInit";
            buttonInit.Size = new Size(87, 60);
            buttonInit.TabIndex = 0;
            buttonInit.Text = "初始化OCR";
            buttonInit.UseVisualStyleBackColor = true;
            buttonInit.Click += buttonInit_Click;
            // 
            // buttonRec
            // 
            buttonRec.Enabled = false;
            buttonRec.Location = new Point(748, 22);
            buttonRec.Name = "buttonRec";
            buttonRec.Size = new Size(120, 28);
            buttonRec.TabIndex = 1;
            buttonRec.Text = "OCR文本识别";
            buttonRec.UseVisualStyleBackColor = true;
            buttonRec.Click += buttonRec_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            textBoxResult.Location = new Point(534, 143);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ScrollBars = ScrollBars.Both;
            textBoxResult.Size = new Size(547, 537);
            textBoxResult.TabIndex = 2;
            // 
            // buttonGetBase64
            // 
            buttonGetBase64.Location = new Point(919, 22);
            buttonGetBase64.Name = "buttonGetBase64";
            buttonGetBase64.Size = new Size(120, 28);
            buttonGetBase64.TabIndex = 3;
            buttonGetBase64.Text = "获取图片Base64";
            buttonGetBase64.UseVisualStyleBackColor = true;
            buttonGetBase64.Click += buttonGetBase64_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(buttonPostFile);
            groupBox1.Controls.Add(textBoxApiAddress);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(comboBoxModel);
            groupBox1.Controls.Add(buttonRecTable);
            groupBox1.Controls.Add(buttonDownModels);
            groupBox1.Controls.Add(numericUpDowncpu_mem);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(numericUpDownThread);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(comboBoxJson);
            groupBox1.Controls.Add(numDowncpu_threads);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numDowngpu_id);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(comboBoxuse_gpu);
            groupBox1.Controls.Add(buttonGetBase64);
            groupBox1.Controls.Add(buttonInit);
            groupBox1.Controls.Add(buttonRec);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1069, 125);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "功能选项";
            // 
            // buttonPostFile
            // 
            buttonPostFile.Location = new Point(874, 87);
            buttonPostFile.Name = "buttonPostFile";
            buttonPostFile.Size = new Size(165, 28);
            buttonPostFile.TabIndex = 22;
            buttonPostFile.Text = "API接口测试";
            buttonPostFile.UseVisualStyleBackColor = true;
            buttonPostFile.Click += buttonPostFile_Click;
            // 
            // textBoxApiAddress
            // 
            textBoxApiAddress.Location = new Point(545, 89);
            textBoxApiAddress.Name = "textBoxApiAddress";
            textBoxApiAddress.Size = new Size(323, 23);
            textBoxApiAddress.TabIndex = 21;
            textBoxApiAddress.Text = "http://localhost:5000/OCRService/GetOCRFile";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(419, 92);
            label8.Name = "label8";
            label8.Size = new Size(114, 17);
            label8.TabIndex = 20;
            label8.Text = "WebApi接口地址：";
            label8.TextAlign = ContentAlignment.TopRight;
            label8.UseWaitCursor = true;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(18, 92);
            label7.Name = "label7";
            label7.Size = new Size(68, 17);
            label7.TabIndex = 19;
            label7.Text = "模型方案：";
            // 
            // comboBoxModel
            // 
            comboBoxModel.FormattingEnabled = true;
            comboBoxModel.Items.AddRange(new object[] { "PP-OCRv5_mobile", "ch_PP-OCRv4" });
            comboBoxModel.Location = new Point(92, 89);
            comboBoxModel.Name = "comboBoxModel";
            comboBoxModel.Size = new Size(321, 25);
            comboBoxModel.TabIndex = 18;
            comboBoxModel.SelectedIndexChanged += comboBoxModel_SelectedIndexChanged;
            // 
            // buttonRecTable
            // 
            buttonRecTable.Enabled = false;
            buttonRecTable.Location = new Point(748, 55);
            buttonRecTable.Name = "buttonRecTable";
            buttonRecTable.Size = new Size(120, 28);
            buttonRecTable.TabIndex = 17;
            buttonRecTable.Text = "OCR表格识别";
            buttonRecTable.UseVisualStyleBackColor = true;
            buttonRecTable.Click += buttonRecTable_Click;
            // 
            // buttonDownModels
            // 
            buttonDownModels.Location = new Point(919, 54);
            buttonDownModels.Name = "buttonDownModels";
            buttonDownModels.Size = new Size(120, 28);
            buttonDownModels.TabIndex = 16;
            buttonDownModels.Text = "下载OCR模型";
            buttonDownModels.UseVisualStyleBackColor = true;
            buttonDownModels.Click += buttonDownModels_Click;
            // 
            // numericUpDowncpu_mem
            // 
            numericUpDowncpu_mem.Location = new Point(545, 60);
            numericUpDowncpu_mem.Maximum = new decimal(new int[] { 8000, 0, 0, 0 });
            numericUpDowncpu_mem.Name = "numericUpDowncpu_mem";
            numericUpDowncpu_mem.Size = new Size(55, 23);
            numericUpDowncpu_mem.TabIndex = 15;
            numericUpDowncpu_mem.ValueChanged += numericUpDowncpu_mem_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(419, 62);
            label6.Name = "label6";
            label6.Size = new Size(120, 17);
            label6.TabIndex = 14;
            label6.Text = "内存占用上限(MB)：";
            label6.TextAlign = ContentAlignment.TopRight;
            label6.UseWaitCursor = true;
            // 
            // numericUpDownThread
            // 
            numericUpDownThread.Location = new Point(333, 59);
            numericUpDownThread.Name = "numericUpDownThread";
            numericUpDownThread.Size = new Size(80, 23);
            numericUpDownThread.TabIndex = 13;
            numericUpDownThread.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownThread.ValueChanged += numericUpDownThread_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(235, 61);
            label5.Name = "label5";
            label5.Size = new Size(92, 17);
            label5.TabIndex = 12;
            label5.Text = "模拟循环识别：";
            label5.TextAlign = ContentAlignment.TopRight;
            label5.UseWaitCursor = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(10, 62);
            label4.Name = "label4";
            label4.Size = new Size(76, 17);
            label4.TabIndex = 11;
            label4.Text = "输出JSON：";
            // 
            // comboBoxJson
            // 
            comboBoxJson.FormattingEnabled = true;
            comboBoxJson.Items.AddRange(new object[] { "只输出文字", "输出文字+JSON" });
            comboBoxJson.Location = new Point(92, 58);
            comboBoxJson.Name = "comboBoxJson";
            comboBoxJson.Size = new Size(129, 25);
            comboBoxJson.TabIndex = 10;
            comboBoxJson.SelectedIndexChanged += comboBoxJson_SelectedIndexChanged;
            // 
            // numDowncpu_threads
            // 
            numDowncpu_threads.Location = new Point(545, 27);
            numDowncpu_threads.Name = "numDowncpu_threads";
            numDowncpu_threads.Size = new Size(55, 23);
            numDowncpu_threads.TabIndex = 9;
            numDowncpu_threads.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numDowncpu_threads.ValueChanged += numDowncpu_threads_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(459, 33);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 8;
            label3.Text = "CPU线程数：";
            label3.TextAlign = ContentAlignment.TopRight;
            label3.UseWaitCursor = true;
            // 
            // numDowngpu_id
            // 
            numDowngpu_id.Location = new Point(333, 28);
            numDowngpu_id.Name = "numDowngpu_id";
            numDowngpu_id.Size = new Size(80, 23);
            numDowngpu_id.TabIndex = 7;
            numDowngpu_id.ValueChanged += numDowngpu_id_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(264, 31);
            label2.Name = "label2";
            label2.Size = new Size(63, 17);
            label2.TabIndex = 6;
            label2.Text = "GPU_ID：";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 31);
            label1.Name = "label1";
            label1.Size = new Size(69, 17);
            label1.TabIndex = 5;
            label1.Text = "启用GPU：";
            // 
            // comboBoxuse_gpu
            // 
            comboBoxuse_gpu.FormattingEnabled = true;
            comboBoxuse_gpu.Items.AddRange(new object[] { "使用CPU", "使用GPU" });
            comboBoxuse_gpu.Location = new Point(92, 27);
            comboBoxuse_gpu.Name = "comboBoxuse_gpu";
            comboBoxuse_gpu.Size = new Size(129, 25);
            comboBoxuse_gpu.TabIndex = 4;
            comboBoxuse_gpu.SelectedIndexChanged += comboBoxuse_gpu_SelectedIndexChanged;
            // 
            // pictureBoxImg
            // 
            pictureBoxImg.Dock = DockStyle.Fill;
            pictureBoxImg.Location = new Point(3, 19);
            pictureBoxImg.Name = "pictureBoxImg";
            pictureBoxImg.Size = new Size(510, 515);
            pictureBoxImg.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImg.TabIndex = 5;
            pictureBoxImg.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(pictureBoxImg);
            groupBox2.Location = new Point(12, 143);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(516, 537);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "图片";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1093, 683);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(textBoxResult);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PaddleOCR识别Demo V2.0.0--QQ群：475159576 https://github.com/PaddleOCRCore/PaddleOCRApi";
            Load += MainForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDowncpu_mem).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownThread).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).EndInit();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonInit;
        private Button buttonRec;
        private TextBox textBoxResult;
        private Button buttonGetBase64;
        private GroupBox groupBox1;
        private Label label1;
        private ComboBox comboBoxuse_gpu;
        private Label label2;
        private NumericUpDown numDowngpu_id;
        private NumericUpDown numDowncpu_threads;
        private Label label3;
        private PictureBox pictureBoxImg;
        private GroupBox groupBox2;
        private Label label4;
        private ComboBox comboBoxJson;
        private NumericUpDown numericUpDownThread;
        private Label label5;
        private NumericUpDown numericUpDowncpu_mem;
        private Label label6;
        private Button buttonDownModels;
        private Button buttonRecTable;
        private Label label7;
        private ComboBox comboBoxModel;
        private Label label8;
        private TextBox textBoxApiAddress;
        private Button buttonPostFile;
    }
}
