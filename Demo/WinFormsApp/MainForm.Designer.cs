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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            imageList1 = new ImageList(components);
            tabPageImageCorrection = new TabPage();
            splitContainerUVDoc = new SplitContainer();
            groupBoxUVDocOriginal = new GroupBox();
            pictureBoxOriginal = new PictureBox();
            groupBoxUVDocOutput = new GroupBox();
            pictureBoxOutput = new PictureBox();
            groupBoxUVDocControl = new GroupBox();
            chkUVDocUseTensorRT = new CheckBox();
            btnUVDocUpload = new Button();
            btnUVDocInitialize = new Button();
            btnUVDocProcess = new Button();
            btnUVDocFreeEngine = new Button();
            btnUVDocSave = new Button();
            chkUVDocUseGpu = new CheckBox();
            lblUVDocCpuThreads = new Label();
            numUVDocCpuThreads = new NumericUpDown();
            lblUVDocGpuId = new Label();
            numUVDocGpuId = new NumericUpDown();
            lblUVDocGpuMem = new Label();
            numUVDocGpuMem = new NumericUpDown();
            lblUVDocStatus = new Label();
            tabPageOCR = new TabPage();
            groupBox1 = new GroupBox();
            chkReturnWordBox = new CheckBox();
            buttonFreeEngine = new Button();
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
            buttonGetBase64 = new Button();
            buttonInit = new Button();
            buttonRec = new Button();
            textBoxResult = new TextBox();
            groupBoxOCRImage = new GroupBox();
            pictureBoxImg = new WinFormsApp.UserControl.PictureView();
            tabControlMain = new TabControl();
            tabPageImageCorrection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerUVDoc).BeginInit();
            splitContainerUVDoc.Panel1.SuspendLayout();
            splitContainerUVDoc.Panel2.SuspendLayout();
            splitContainerUVDoc.SuspendLayout();
            groupBoxUVDocOriginal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOriginal).BeginInit();
            groupBoxUVDocOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOutput).BeginInit();
            groupBoxUVDocControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numUVDocCpuThreads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numUVDocGpuId).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numUVDocGpuMem).BeginInit();
            tabPageOCR.SuspendLayout();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDowncpu_mem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownThread).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).BeginInit();
            groupBoxOCRImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).BeginInit();
            tabControlMain.SuspendLayout();
            SuspendLayout();
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // tabPageImageCorrection
            // 
            tabPageImageCorrection.Controls.Add(splitContainerUVDoc);
            tabPageImageCorrection.Controls.Add(groupBoxUVDocControl);
            tabPageImageCorrection.Controls.Add(lblUVDocStatus);
            tabPageImageCorrection.Location = new Point(4, 26);
            tabPageImageCorrection.Name = "tabPageImageCorrection";
            tabPageImageCorrection.Padding = new Padding(3);
            tabPageImageCorrection.Size = new Size(1084, 662);
            tabPageImageCorrection.TabIndex = 1;
            tabPageImageCorrection.Text = "图像矫正";
            tabPageImageCorrection.UseVisualStyleBackColor = true;
            // 
            // splitContainerUVDoc
            // 
            splitContainerUVDoc.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerUVDoc.Location = new Point(6, 69);
            splitContainerUVDoc.Name = "splitContainerUVDoc";
            // 
            // splitContainerUVDoc.Panel1
            // 
            splitContainerUVDoc.Panel1.Controls.Add(groupBoxUVDocOriginal);
            // 
            // splitContainerUVDoc.Panel2
            // 
            splitContainerUVDoc.Panel2.Controls.Add(groupBoxUVDocOutput);
            splitContainerUVDoc.Size = new Size(1072, 551);
            splitContainerUVDoc.SplitterDistance = 530;
            splitContainerUVDoc.TabIndex = 2;
            // 
            // groupBoxUVDocOriginal
            // 
            groupBoxUVDocOriginal.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxUVDocOriginal.Controls.Add(pictureBoxOriginal);
            groupBoxUVDocOriginal.Location = new Point(0, 3);
            groupBoxUVDocOriginal.Name = "groupBoxUVDocOriginal";
            groupBoxUVDocOriginal.Size = new Size(530, 548);
            groupBoxUVDocOriginal.TabIndex = 1;
            groupBoxUVDocOriginal.TabStop = false;
            groupBoxUVDocOriginal.Text = "原始图像";
            // 
            // pictureBoxOriginal
            // 
            pictureBoxOriginal.BackColor = Color.FromArgb(240, 240, 240);
            pictureBoxOriginal.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxOriginal.Dock = DockStyle.Fill;
            pictureBoxOriginal.Location = new Point(3, 19);
            pictureBoxOriginal.Name = "pictureBoxOriginal";
            pictureBoxOriginal.Size = new Size(524, 526);
            pictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOriginal.TabIndex = 0;
            pictureBoxOriginal.TabStop = false;
            // 
            // groupBoxUVDocOutput
            // 
            groupBoxUVDocOutput.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxUVDocOutput.Controls.Add(pictureBoxOutput);
            groupBoxUVDocOutput.Location = new Point(0, 3);
            groupBoxUVDocOutput.Name = "groupBoxUVDocOutput";
            groupBoxUVDocOutput.Size = new Size(538, 548);
            groupBoxUVDocOutput.TabIndex = 2;
            groupBoxUVDocOutput.TabStop = false;
            groupBoxUVDocOutput.Text = "矫正后图像";
            // 
            // pictureBoxOutput
            // 
            pictureBoxOutput.BackColor = Color.FromArgb(240, 240, 240);
            pictureBoxOutput.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxOutput.Dock = DockStyle.Fill;
            pictureBoxOutput.Location = new Point(3, 19);
            pictureBoxOutput.Name = "pictureBoxOutput";
            pictureBoxOutput.Size = new Size(532, 526);
            pictureBoxOutput.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOutput.TabIndex = 0;
            pictureBoxOutput.TabStop = false;
            // 
            // groupBoxUVDocControl
            // 
            groupBoxUVDocControl.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBoxUVDocControl.Controls.Add(chkUVDocUseTensorRT);
            groupBoxUVDocControl.Controls.Add(btnUVDocUpload);
            groupBoxUVDocControl.Controls.Add(btnUVDocInitialize);
            groupBoxUVDocControl.Controls.Add(btnUVDocProcess);
            groupBoxUVDocControl.Controls.Add(btnUVDocFreeEngine);
            groupBoxUVDocControl.Controls.Add(btnUVDocSave);
            groupBoxUVDocControl.Controls.Add(chkUVDocUseGpu);
            groupBoxUVDocControl.Controls.Add(lblUVDocCpuThreads);
            groupBoxUVDocControl.Controls.Add(numUVDocCpuThreads);
            groupBoxUVDocControl.Controls.Add(lblUVDocGpuId);
            groupBoxUVDocControl.Controls.Add(numUVDocGpuId);
            groupBoxUVDocControl.Controls.Add(lblUVDocGpuMem);
            groupBoxUVDocControl.Controls.Add(numUVDocGpuMem);
            groupBoxUVDocControl.Location = new Point(6, 3);
            groupBoxUVDocControl.Name = "groupBoxUVDocControl";
            groupBoxUVDocControl.Size = new Size(1072, 60);
            groupBoxUVDocControl.TabIndex = 0;
            groupBoxUVDocControl.TabStop = false;
            groupBoxUVDocControl.Text = "图像矫正控制";
            // 
            // chkUVDocUseTensorRT
            // 
            chkUVDocUseTensorRT.AutoSize = true;
            chkUVDocUseTensorRT.Enabled = false;
            chkUVDocUseTensorRT.Location = new Point(472, 26);
            chkUVDocUseTensorRT.Name = "chkUVDocUseTensorRT";
            chkUVDocUseTensorRT.Size = new Size(82, 21);
            chkUVDocUseTensorRT.TabIndex = 11;
            chkUVDocUseTensorRT.Text = "TensorRT";
            chkUVDocUseTensorRT.UseVisualStyleBackColor = true;
            // 
            // btnUVDocUpload
            // 
            btnUVDocUpload.Enabled = false;
            btnUVDocUpload.Location = new Point(660, 22);
            btnUVDocUpload.Name = "btnUVDocUpload";
            btnUVDocUpload.Size = new Size(95, 28);
            btnUVDocUpload.TabIndex = 1;
            btnUVDocUpload.Text = "上传图像";
            btnUVDocUpload.UseVisualStyleBackColor = true;
            btnUVDocUpload.Click += btnUVDocUpload_Click;
            // 
            // btnUVDocInitialize
            // 
            btnUVDocInitialize.Location = new Point(559, 22);
            btnUVDocInitialize.Name = "btnUVDocInitialize";
            btnUVDocInitialize.Size = new Size(95, 28);
            btnUVDocInitialize.TabIndex = 0;
            btnUVDocInitialize.Text = "初始化模型";
            btnUVDocInitialize.UseVisualStyleBackColor = true;
            btnUVDocInitialize.Click += btnUVDocInitialize_Click;
            // 
            // btnUVDocProcess
            // 
            btnUVDocProcess.Enabled = false;
            btnUVDocProcess.Location = new Point(761, 22);
            btnUVDocProcess.Name = "btnUVDocProcess";
            btnUVDocProcess.Size = new Size(95, 28);
            btnUVDocProcess.TabIndex = 2;
            btnUVDocProcess.Text = "开始矫正";
            btnUVDocProcess.UseVisualStyleBackColor = true;
            btnUVDocProcess.Click += btnUVDocProcess_Click;
            // 
            // btnUVDocFreeEngine
            // 
            btnUVDocFreeEngine.Enabled = false;
            btnUVDocFreeEngine.Location = new Point(963, 22);
            btnUVDocFreeEngine.Name = "btnUVDocFreeEngine";
            btnUVDocFreeEngine.Size = new Size(95, 28);
            btnUVDocFreeEngine.TabIndex = 3;
            btnUVDocFreeEngine.Text = "释放模型";
            btnUVDocFreeEngine.UseVisualStyleBackColor = true;
            btnUVDocFreeEngine.Click += btnUVDocFreeEngine_Click;
            // 
            // btnUVDocSave
            // 
            btnUVDocSave.Enabled = false;
            btnUVDocSave.Location = new Point(862, 22);
            btnUVDocSave.Name = "btnUVDocSave";
            btnUVDocSave.Size = new Size(95, 28);
            btnUVDocSave.TabIndex = 3;
            btnUVDocSave.Text = "保存结果";
            btnUVDocSave.UseVisualStyleBackColor = true;
            btnUVDocSave.Click += btnUVDocSave_Click;
            // 
            // chkUVDocUseGpu
            // 
            chkUVDocUseGpu.AutoSize = true;
            chkUVDocUseGpu.Location = new Point(7, 26);
            chkUVDocUseGpu.Name = "chkUVDocUseGpu";
            chkUVDocUseGpu.Size = new Size(76, 21);
            chkUVDocUseGpu.TabIndex = 4;
            chkUVDocUseGpu.Text = "启用GPU";
            chkUVDocUseGpu.UseVisualStyleBackColor = true;
            chkUVDocUseGpu.CheckedChanged += chkUVDocUseGpu_CheckedChanged;
            // 
            // lblUVDocCpuThreads
            // 
            lblUVDocCpuThreads.AutoSize = true;
            lblUVDocCpuThreads.Location = new Point(86, 27);
            lblUVDocCpuThreads.Name = "lblUVDocCpuThreads";
            lblUVDocCpuThreads.Size = new Size(59, 17);
            lblUVDocCpuThreads.TabIndex = 5;
            lblUVDocCpuThreads.Text = "CPU线程:";
            // 
            // numUVDocCpuThreads
            // 
            numUVDocCpuThreads.Location = new Point(146, 25);
            numUVDocCpuThreads.Maximum = new decimal(new int[] { 512, 0, 0, 0 });
            numUVDocCpuThreads.Name = "numUVDocCpuThreads";
            numUVDocCpuThreads.Size = new Size(50, 23);
            numUVDocCpuThreads.TabIndex = 6;
            numUVDocCpuThreads.Value = new decimal(new int[] { 4, 0, 0, 0 });
            // 
            // lblUVDocGpuId
            // 
            lblUVDocGpuId.AutoSize = true;
            lblUVDocGpuId.Location = new Point(202, 27);
            lblUVDocGpuId.Name = "lblUVDocGpuId";
            lblUVDocGpuId.Size = new Size(53, 17);
            lblUVDocGpuId.TabIndex = 7;
            lblUVDocGpuId.Text = "GPU ID:";
            // 
            // numUVDocGpuId
            // 
            numUVDocGpuId.Enabled = false;
            numUVDocGpuId.Location = new Point(260, 25);
            numUVDocGpuId.Name = "numUVDocGpuId";
            numUVDocGpuId.Size = new Size(44, 23);
            numUVDocGpuId.TabIndex = 8;
            // 
            // lblUVDocGpuMem
            // 
            lblUVDocGpuMem.AutoSize = true;
            lblUVDocGpuMem.Location = new Point(310, 27);
            lblUVDocGpuMem.Name = "lblUVDocGpuMem";
            lblUVDocGpuMem.Size = new Size(88, 17);
            lblUVDocGpuMem.TabIndex = 9;
            lblUVDocGpuMem.Text = "GPU内存(MB):";
            // 
            // numUVDocGpuMem
            // 
            numUVDocGpuMem.Enabled = false;
            numUVDocGpuMem.Location = new Point(404, 25);
            numUVDocGpuMem.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
            numUVDocGpuMem.Name = "numUVDocGpuMem";
            numUVDocGpuMem.Size = new Size(55, 23);
            numUVDocGpuMem.TabIndex = 10;
            numUVDocGpuMem.Value = new decimal(new int[] { 2000, 0, 0, 0 });
            // 
            // lblUVDocStatus
            // 
            lblUVDocStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblUVDocStatus.Location = new Point(6, 623);
            lblUVDocStatus.Name = "lblUVDocStatus";
            lblUVDocStatus.Size = new Size(1072, 35);
            lblUVDocStatus.TabIndex = 3;
            lblUVDocStatus.Text = "状态: 就绪";
            lblUVDocStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabPageOCR
            // 
            tabPageOCR.Controls.Add(groupBox1);
            tabPageOCR.Controls.Add(textBoxResult);
            tabPageOCR.Controls.Add(groupBoxOCRImage);
            tabPageOCR.Location = new Point(4, 26);
            tabPageOCR.Name = "tabPageOCR";
            tabPageOCR.Padding = new Padding(3);
            tabPageOCR.Size = new Size(1084, 662);
            tabPageOCR.TabIndex = 0;
            tabPageOCR.Text = "文本识别";
            tabPageOCR.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(chkReturnWordBox);
            groupBox1.Controls.Add(buttonFreeEngine);
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
            groupBox1.Location = new Point(6, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1072, 125);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "OCR文本识别功能选项";
            // 
            // chkReturnWordBox
            // 
            chkReturnWordBox.AutoSize = true;
            chkReturnWordBox.Location = new Point(266, 91);
            chkReturnWordBox.Name = "chkReturnWordBox";
            chkReturnWordBox.Size = new Size(147, 21);
            chkReturnWordBox.TabIndex = 25;
            chkReturnWordBox.Text = "是否输出单字坐标json";
            chkReturnWordBox.UseVisualStyleBackColor = true;
            // 
            // buttonFreeEngine
            // 
            buttonFreeEngine.Enabled = false;
            buttonFreeEngine.Location = new Point(825, 23);
            buttonFreeEngine.Name = "buttonFreeEngine";
            buttonFreeEngine.Size = new Size(87, 60);
            buttonFreeEngine.TabIndex = 24;
            buttonFreeEngine.Text = "释放OCR";
            buttonFreeEngine.UseVisualStyleBackColor = true;
            buttonFreeEngine.Click += buttonFreeEngine_Click;
            // 
            // buttonPostFile
            // 
            buttonPostFile.Location = new Point(919, 87);
            buttonPostFile.Name = "buttonPostFile";
            buttonPostFile.Size = new Size(120, 28);
            buttonPostFile.TabIndex = 22;
            buttonPostFile.Text = "API接口测试";
            buttonPostFile.UseVisualStyleBackColor = true;
            buttonPostFile.Click += buttonPostFile_Click;
            // 
            // textBoxApiAddress
            // 
            textBoxApiAddress.Location = new Point(545, 89);
            textBoxApiAddress.Name = "textBoxApiAddress";
            textBoxApiAddress.Size = new Size(367, 23);
            textBoxApiAddress.TabIndex = 21;
            textBoxApiAddress.Text = "http://localhost:5000/OCRService/GetOCRFile";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(425, 92);
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
            comboBoxModel.Items.AddRange(new object[] { "PP-OCRv5_mobile", "PP-OCRv5_server", "PP-OCRv4_mobile" });
            comboBoxModel.Location = new Point(92, 89);
            comboBoxModel.Name = "comboBoxModel";
            comboBoxModel.Size = new Size(129, 25);
            comboBoxModel.TabIndex = 18;
            comboBoxModel.SelectedIndexChanged += comboBoxModel_SelectedIndexChanged;
            // 
            // buttonRecTable
            // 
            buttonRecTable.Enabled = false;
            buttonRecTable.Location = new Point(699, 56);
            buttonRecTable.Name = "buttonRecTable";
            buttonRecTable.Size = new Size(120, 28);
            buttonRecTable.TabIndex = 17;
            buttonRecTable.Text = "OCR表格识别";
            buttonRecTable.UseVisualStyleBackColor = true;
            buttonRecTable.Click += buttonRecTable_Click;
            // 
            // buttonDownModels
            // 
            buttonDownModels.Location = new Point(919, 55);
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
            numericUpDownThread.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
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
            // buttonGetBase64
            // 
            buttonGetBase64.Location = new Point(919, 23);
            buttonGetBase64.Name = "buttonGetBase64";
            buttonGetBase64.Size = new Size(120, 28);
            buttonGetBase64.TabIndex = 3;
            buttonGetBase64.Text = "获取图片Base64";
            buttonGetBase64.UseVisualStyleBackColor = true;
            buttonGetBase64.Click += buttonGetBase64_Click;
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
            buttonRec.Location = new Point(699, 23);
            buttonRec.Name = "buttonRec";
            buttonRec.Size = new Size(120, 28);
            buttonRec.TabIndex = 1;
            buttonRec.Text = "OCR文本识别";
            buttonRec.UseVisualStyleBackColor = true;
            buttonRec.Click += buttonRec_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxResult.Location = new Point(512, 137);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ScrollBars = ScrollBars.Both;
            textBoxResult.Size = new Size(566, 519);
            textBoxResult.TabIndex = 2;
            // 
            // groupBoxOCRImage
            // 
            groupBoxOCRImage.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBoxOCRImage.Controls.Add(pictureBoxImg);
            groupBoxOCRImage.Location = new Point(6, 137);
            groupBoxOCRImage.Name = "groupBoxOCRImage";
            groupBoxOCRImage.Size = new Size(500, 519);
            groupBoxOCRImage.TabIndex = 7;
            groupBoxOCRImage.TabStop = false;
            groupBoxOCRImage.Text = "图片";
            // 
            // pictureBoxImg
            // 
            pictureBoxImg.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxImg.Location = new Point(6, 22);
            pictureBoxImg.Name = "pictureBoxImg";
            pictureBoxImg.Size = new Size(488, 491);
            pictureBoxImg.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImg.TabIndex = 0;
            pictureBoxImg.TabStop = false;
            // 
            // tabControlMain
            // 
            tabControlMain.Controls.Add(tabPageOCR);
            tabControlMain.Controls.Add(tabPageImageCorrection);
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.Location = new Point(0, 0);
            tabControlMain.Name = "tabControlMain";
            tabControlMain.SelectedIndex = 0;
            tabControlMain.Size = new Size(1092, 692);
            tabControlMain.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1092, 692);
            Controls.Add(tabControlMain);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PaddleOCR识别Demo V4.0.0--QQ群：475159576 https://github.com/PaddleOCRCore/PaddleOCRApi";
            Load += MainForm_Load;
            tabPageImageCorrection.ResumeLayout(false);
            splitContainerUVDoc.Panel1.ResumeLayout(false);
            splitContainerUVDoc.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerUVDoc).EndInit();
            splitContainerUVDoc.ResumeLayout(false);
            groupBoxUVDocOriginal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxOriginal).EndInit();
            groupBoxUVDocOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxOutput).EndInit();
            groupBoxUVDocControl.ResumeLayout(false);
            groupBoxUVDocControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numUVDocCpuThreads).EndInit();
            ((System.ComponentModel.ISupportInitialize)numUVDocGpuId).EndInit();
            ((System.ComponentModel.ISupportInitialize)numUVDocGpuMem).EndInit();
            tabPageOCR.ResumeLayout(false);
            tabPageOCR.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDowncpu_mem).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownThread).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).EndInit();
            groupBoxOCRImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).EndInit();
            tabControlMain.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ImageList imageList1;
        private TabPage tabPageImageCorrection;
        private SplitContainer splitContainerUVDoc;
        private GroupBox groupBoxUVDocOriginal;
        private PictureBox pictureBoxOriginal;
        private GroupBox groupBoxUVDocOutput;
        private PictureBox pictureBoxOutput;
        private GroupBox groupBoxUVDocControl;
        private CheckBox chkUVDocUseTensorRT;
        private Button btnUVDocUpload;
        private Button btnUVDocInitialize;
        private Button btnUVDocProcess;
        private Button btnUVDocSave;
        private CheckBox chkUVDocUseGpu;
        private Label lblUVDocCpuThreads;
        private NumericUpDown numUVDocCpuThreads;
        private Label lblUVDocGpuId;
        private NumericUpDown numUVDocGpuId;
        private Label lblUVDocGpuMem;
        private NumericUpDown numUVDocGpuMem;
        private Label lblUVDocStatus;
        private TabPage tabPageOCR;
        private GroupBox groupBox1;
        private Button buttonFreeEngine;
        private Button buttonPostFile;
        private TextBox textBoxApiAddress;
        private Label label8;
        private Label label7;
        private ComboBox comboBoxModel;
        private Button buttonRecTable;
        private Button buttonDownModels;
        private NumericUpDown numericUpDowncpu_mem;
        private Label label6;
        private NumericUpDown numericUpDownThread;
        private Label label5;
        private Label label4;
        private ComboBox comboBoxJson;
        private NumericUpDown numDowncpu_threads;
        private Label label3;
        private NumericUpDown numDowngpu_id;
        private Label label2;
        private Label label1;
        private ComboBox comboBoxuse_gpu;
        private Button buttonGetBase64;
        private Button buttonInit;
        private Button buttonRec;
        private TextBox textBoxResult;
        private GroupBox groupBoxOCRImage;
        private UserControl.PictureView pictureBoxImg;
        private TabControl tabControlMain;
        private Button btnUVDocFreeEngine;
        private CheckBox chkReturnWordBox;
    }
}
