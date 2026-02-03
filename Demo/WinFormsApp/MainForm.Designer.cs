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
            splitContainerOCR = new SplitContainer();
            groupBoxOCRImage = new GroupBox();
            pictureBoxImg = new WinFormsApp.UserControl.PictureView();
            textBoxResult = new TextBox();
            groupBox1 = new GroupBox();
            chkUseTensorRT = new CheckBox();
            chkJson = new CheckBox();
            chkUseGpu = new CheckBox();
            buttonRecPDF = new Button();
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
            numDowncpu_threads = new NumericUpDown();
            label3 = new Label();
            numDowngpu_id = new NumericUpDown();
            label2 = new Label();
            buttonGetBase64 = new Button();
            buttonInit = new Button();
            buttonRecClipboard = new Button();
            buttonRec = new Button();
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
            ((System.ComponentModel.ISupportInitialize)splitContainerOCR).BeginInit();
            splitContainerOCR.Panel1.SuspendLayout();
            splitContainerOCR.Panel2.SuspendLayout();
            splitContainerOCR.SuspendLayout();
            groupBoxOCRImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDowncpu_mem).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownThread).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).BeginInit();
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
            tabPageOCR.Controls.Add(splitContainerOCR);
            tabPageOCR.Controls.Add(groupBox1);
            tabPageOCR.Location = new Point(4, 26);
            tabPageOCR.Name = "tabPageOCR";
            tabPageOCR.Padding = new Padding(3);
            tabPageOCR.Size = new Size(1084, 662);
            tabPageOCR.TabIndex = 0;
            tabPageOCR.Text = "文本识别";
            tabPageOCR.UseVisualStyleBackColor = true;
            // 
            // splitContainerOCR
            // 
            splitContainerOCR.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainerOCR.Location = new Point(6, 137);
            splitContainerOCR.Name = "splitContainerOCR";
            // 
            // splitContainerOCR.Panel1
            // 
            splitContainerOCR.Panel1.Controls.Add(groupBoxOCRImage);
            // 
            // splitContainerOCR.Panel2
            // 
            splitContainerOCR.Panel2.Controls.Add(textBoxResult);
            splitContainerOCR.Size = new Size(1072, 519);
            splitContainerOCR.SplitterDistance = 518;
            splitContainerOCR.TabIndex = 8;
            // 
            // groupBoxOCRImage
            // 
            groupBoxOCRImage.Controls.Add(pictureBoxImg);
            groupBoxOCRImage.Dock = DockStyle.Fill;
            groupBoxOCRImage.Location = new Point(0, 0);
            groupBoxOCRImage.Name = "groupBoxOCRImage";
            groupBoxOCRImage.Size = new Size(518, 519);
            groupBoxOCRImage.TabIndex = 7;
            groupBoxOCRImage.TabStop = false;
            groupBoxOCRImage.Text = "图片";
            // 
            // pictureBoxImg
            // 
            pictureBoxImg.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBoxImg.Location = new Point(3, 19);
            pictureBoxImg.Name = "pictureBoxImg";
            pictureBoxImg.Size = new Size(512, 497);
            pictureBoxImg.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImg.TabIndex = 0;
            pictureBoxImg.TabStop = false;
            // 
            // textBoxResult
            // 
            textBoxResult.Dock = DockStyle.Fill;
            textBoxResult.Location = new Point(0, 0);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ScrollBars = ScrollBars.Both;
            textBoxResult.Size = new Size(550, 519);
            textBoxResult.TabIndex = 2;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(chkUseTensorRT);
            groupBox1.Controls.Add(chkJson);
            groupBox1.Controls.Add(chkUseGpu);
            groupBox1.Controls.Add(buttonRecPDF);
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
            groupBox1.Controls.Add(numDowncpu_threads);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numDowngpu_id);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(buttonGetBase64);
            groupBox1.Controls.Add(buttonInit);
            groupBox1.Controls.Add(buttonRecClipboard);
            groupBox1.Controls.Add(buttonRec);
            groupBox1.Location = new Point(6, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1072, 125);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "OCR文本识别功能选项";
            // 
            // chkUseTensorRT
            // 
            chkUseTensorRT.AutoSize = true;
            chkUseTensorRT.Enabled = false;
            chkUseTensorRT.Location = new Point(119, 30);
            chkUseTensorRT.Name = "chkUseTensorRT";
            chkUseTensorRT.Size = new Size(82, 21);
            chkUseTensorRT.TabIndex = 29;
            chkUseTensorRT.Text = "TensorRT";
            chkUseTensorRT.UseVisualStyleBackColor = true;
            chkUseTensorRT.CheckedChanged += chkUseTensorRT_CheckedChanged;
            // 
            // chkJson
            // 
            chkJson.AutoSize = true;
            chkJson.Location = new Point(10, 59);
            chkJson.Name = "chkJson";
            chkJson.Size = new Size(83, 21);
            chkJson.TabIndex = 28;
            chkJson.Text = "输出JSON";
            chkJson.UseVisualStyleBackColor = true;
            chkJson.CheckedChanged += chkJson_CheckedChanged;
            // 
            // chkUseGpu
            // 
            chkUseGpu.AutoSize = true;
            chkUseGpu.Location = new Point(10, 30);
            chkUseGpu.Name = "chkUseGpu";
            chkUseGpu.Size = new Size(76, 21);
            chkUseGpu.TabIndex = 27;
            chkUseGpu.Text = "启用GPU";
            chkUseGpu.UseVisualStyleBackColor = true;
            chkUseGpu.CheckedChanged += chkUseGpu_CheckedChanged;
            // 
            // buttonRecPDF
            // 
            buttonRecPDF.Enabled = false;
            buttonRecPDF.Location = new Point(825, 24);
            buttonRecPDF.Name = "buttonRecPDF";
            buttonRecPDF.Size = new Size(109, 25);
            buttonRecPDF.TabIndex = 26;
            buttonRecPDF.Text = "PDF识别";
            buttonRecPDF.UseVisualStyleBackColor = true;
            buttonRecPDF.Click += buttonRecPDF_Click;
            // 
            // chkReturnWordBox
            // 
            chkReturnWordBox.AutoSize = true;
            chkReturnWordBox.Location = new Point(119, 58);
            chkReturnWordBox.Name = "chkReturnWordBox";
            chkReturnWordBox.RightToLeft = RightToLeft.No;
            chkReturnWordBox.Size = new Size(99, 21);
            chkReturnWordBox.TabIndex = 25;
            chkReturnWordBox.Text = "生成单字坐标";
            chkReturnWordBox.UseVisualStyleBackColor = true;
            // 
            // buttonFreeEngine
            // 
            buttonFreeEngine.Enabled = false;
            buttonFreeEngine.Location = new Point(613, 57);
            buttonFreeEngine.Name = "buttonFreeEngine";
            buttonFreeEngine.Size = new Size(80, 27);
            buttonFreeEngine.TabIndex = 24;
            buttonFreeEngine.Text = "释放OCR";
            buttonFreeEngine.UseVisualStyleBackColor = true;
            buttonFreeEngine.Click += buttonFreeEngine_Click;
            // 
            // buttonPostFile
            // 
            buttonPostFile.Location = new Point(946, 86);
            buttonPostFile.Name = "buttonPostFile";
            buttonPostFile.Size = new Size(120, 28);
            buttonPostFile.TabIndex = 22;
            buttonPostFile.Text = "API接口测试";
            buttonPostFile.UseVisualStyleBackColor = true;
            buttonPostFile.Click += buttonPostFile_Click;
            // 
            // textBoxApiAddress
            // 
            textBoxApiAddress.Location = new Point(613, 89);
            textBoxApiAddress.Name = "textBoxApiAddress";
            textBoxApiAddress.Size = new Size(321, 23);
            textBoxApiAddress.TabIndex = 21;
            textBoxApiAddress.Text = "http://localhost:5000/OCRService/GetOCRFile";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(493, 92);
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
            label7.Location = new Point(8, 90);
            label7.Name = "label7";
            label7.Size = new Size(68, 17);
            label7.TabIndex = 19;
            label7.Text = "模型方案：";
            // 
            // comboBoxModel
            // 
            comboBoxModel.FormattingEnabled = true;
            comboBoxModel.Items.AddRange(new object[] { "PP-OCRv5_mobile", "PP-OCRv5_server", "PP-OCRv4_mobile" });
            comboBoxModel.Location = new Point(82, 87);
            comboBoxModel.Name = "comboBoxModel";
            comboBoxModel.Size = new Size(129, 25);
            comboBoxModel.TabIndex = 18;
            comboBoxModel.SelectedIndexChanged += comboBoxModel_SelectedIndexChanged;
            // 
            // buttonRecTable
            // 
            buttonRecTable.Enabled = false;
            buttonRecTable.Location = new Point(825, 58);
            buttonRecTable.Name = "buttonRecTable";
            buttonRecTable.Size = new Size(109, 25);
            buttonRecTable.TabIndex = 17;
            buttonRecTable.Text = "OCR表格识别";
            buttonRecTable.UseVisualStyleBackColor = true;
            buttonRecTable.Click += buttonRecTable_Click;
            // 
            // buttonDownModels
            // 
            buttonDownModels.Location = new Point(946, 54);
            buttonDownModels.Name = "buttonDownModels";
            buttonDownModels.Size = new Size(120, 28);
            buttonDownModels.TabIndex = 16;
            buttonDownModels.Text = "下载OCR模型";
            buttonDownModels.UseVisualStyleBackColor = true;
            buttonDownModels.Click += buttonDownModels_Click;
            // 
            // numericUpDowncpu_mem
            // 
            numericUpDowncpu_mem.Location = new Point(495, 59);
            numericUpDowncpu_mem.Maximum = new decimal(new int[] { 8000, 0, 0, 0 });
            numericUpDowncpu_mem.Name = "numericUpDowncpu_mem";
            numericUpDowncpu_mem.Size = new Size(55, 23);
            numericUpDowncpu_mem.TabIndex = 15;
            numericUpDowncpu_mem.ValueChanged += numericUpDowncpu_mem_ValueChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(369, 61);
            label6.Name = "label6";
            label6.Size = new Size(120, 17);
            label6.TabIndex = 14;
            label6.Text = "内存占用上限(MB)：";
            label6.TextAlign = ContentAlignment.TopRight;
            label6.UseWaitCursor = true;
            // 
            // numericUpDownThread
            // 
            numericUpDownThread.Location = new Point(495, 30);
            numericUpDownThread.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            numericUpDownThread.Name = "numericUpDownThread";
            numericUpDownThread.Size = new Size(55, 23);
            numericUpDownThread.TabIndex = 13;
            numericUpDownThread.Value = new decimal(new int[] { 1, 0, 0, 0 });
            numericUpDownThread.ValueChanged += numericUpDownThread_ValueChanged;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(397, 33);
            label5.Name = "label5";
            label5.Size = new Size(92, 17);
            label5.TabIndex = 12;
            label5.Text = "模拟循环识别：";
            label5.TextAlign = ContentAlignment.TopRight;
            label5.UseWaitCursor = true;
            // 
            // numDowncpu_threads
            // 
            numDowncpu_threads.Location = new Point(303, 27);
            numDowncpu_threads.Name = "numDowncpu_threads";
            numDowncpu_threads.Size = new Size(60, 23);
            numDowncpu_threads.TabIndex = 9;
            numDowncpu_threads.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numDowncpu_threads.ValueChanged += numDowncpu_threads_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(229, 30);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 8;
            label3.Text = "CPU线程数：";
            label3.TextAlign = ContentAlignment.TopRight;
            label3.UseWaitCursor = true;
            // 
            // numDowngpu_id
            // 
            numDowngpu_id.Location = new Point(303, 58);
            numDowngpu_id.Name = "numDowngpu_id";
            numDowngpu_id.Size = new Size(60, 23);
            numDowngpu_id.TabIndex = 7;
            numDowngpu_id.ValueChanged += numDowngpu_id_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(235, 61);
            label2.Name = "label2";
            label2.Size = new Size(63, 17);
            label2.TabIndex = 6;
            label2.Text = "GPU_ID：";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // buttonGetBase64
            // 
            buttonGetBase64.Location = new Point(946, 22);
            buttonGetBase64.Name = "buttonGetBase64";
            buttonGetBase64.Size = new Size(120, 28);
            buttonGetBase64.TabIndex = 3;
            buttonGetBase64.Text = "获取图片Base64";
            buttonGetBase64.UseVisualStyleBackColor = true;
            buttonGetBase64.Click += buttonGetBase64_Click;
            // 
            // buttonInit
            // 
            buttonInit.BackColor = Color.FromArgb(0, 192, 0);
            buttonInit.ForeColor = Color.Transparent;
            buttonInit.Location = new Point(613, 24);
            buttonInit.Name = "buttonInit";
            buttonInit.Size = new Size(80, 27);
            buttonInit.TabIndex = 0;
            buttonInit.Text = "初始化OCR";
            buttonInit.UseVisualStyleBackColor = false;
            buttonInit.Click += buttonInit_Click;
            // 
            // buttonRecClipboard
            // 
            buttonRecClipboard.Enabled = false;
            buttonRecClipboard.Location = new Point(706, 56);
            buttonRecClipboard.Name = "buttonRecClipboard";
            buttonRecClipboard.Size = new Size(110, 27);
            buttonRecClipboard.TabIndex = 1;
            buttonRecClipboard.Text = "剪贴板识别";
            buttonRecClipboard.UseVisualStyleBackColor = true;
            buttonRecClipboard.Click += buttonRecClipboard_Click;
            // 
            // buttonRec
            // 
            buttonRec.Enabled = false;
            buttonRec.Location = new Point(706, 23);
            buttonRec.Name = "buttonRec";
            buttonRec.Size = new Size(110, 27);
            buttonRec.TabIndex = 1;
            buttonRec.Text = "OCR文本识别";
            buttonRec.UseVisualStyleBackColor = true;
            buttonRec.Click += buttonRec_Click;
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
            Text = "PaddleOCR识别Demo V4.1.1--QQ群：475159576 https://github.com/PaddleOCRCore/PaddleOCRApi";
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
            splitContainerOCR.Panel1.ResumeLayout(false);
            splitContainerOCR.Panel2.ResumeLayout(false);
            splitContainerOCR.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerOCR).EndInit();
            splitContainerOCR.ResumeLayout(false);
            groupBoxOCRImage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numericUpDowncpu_mem).EndInit();
            ((System.ComponentModel.ISupportInitialize)numericUpDownThread).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).EndInit();
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
        private NumericUpDown numDowncpu_threads;
        private Label label3;
        private NumericUpDown numDowngpu_id;
        private Label label2;
        private Button buttonGetBase64;
        private Button buttonInit;
        private Button buttonRec;
        private TextBox textBoxResult;
        private GroupBox groupBoxOCRImage;
        private UserControl.PictureView pictureBoxImg;
        private TabControl tabControlMain;
        private Button btnUVDocFreeEngine;
        private CheckBox chkReturnWordBox;
        private SplitContainer splitContainerOCR;
        private Button buttonRecClipboard;
        private Button buttonRecPDF;
        private CheckBox chkUseGpu;
        private CheckBox chkJson;
        private CheckBox chkUseTensorRT;
    }
}
