namespace PaddleVisionWinForm
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnUpload;
        private Button btnProcess;
        private Button btnSave;
        private Button btnInitialize;
        private PictureBox pictureBoxOriginal;
        private PictureBox pictureBoxOutput;
        private Label lblOriginal;
        private Label lblOutput;
        private Label lblStatus;
        private Panel panelTop;
        private Panel panelBottom;
        private SplitContainer splitContainer;
        private GroupBox groupBoxParameters;
        private CheckBox chkUseGpu;
        private CheckBox chkUseTensorRT;
        private NumericUpDown numCpuThreads;
        private NumericUpDown numGpuId;
        private NumericUpDown numGpuMem;
        private Label lblCpuThreads;
        private Label lblGpuId;
        private Label lblGpuMem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            btnUpload = new Button();
            btnProcess = new Button();
            btnSave = new Button();
            btnInitialize = new Button();
            pictureBoxOriginal = new PictureBox();
            pictureBoxOutput = new PictureBox();
            lblOriginal = new Label();
            lblOutput = new Label();
            lblStatus = new Label();
            panelTop = new Panel();
            groupBoxParameters = new GroupBox();
            chkUseGpu = new CheckBox();
            lblCpuThreads = new Label();
            numCpuThreads = new NumericUpDown();
            lblGpuId = new Label();
            numGpuId = new NumericUpDown();
            lblGpuMem = new Label();
            numGpuMem = new NumericUpDown();
            chkUseTensorRT = new CheckBox();
            panelBottom = new Panel();
            splitContainer = new SplitContainer();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOriginal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOutput).BeginInit();
            panelTop.SuspendLayout();
            groupBoxParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numCpuThreads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGpuId).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numGpuMem).BeginInit();
            panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            SuspendLayout();
            // 
            // btnUpload
            // 
            btnUpload.Enabled = false;
            btnUpload.Font = new Font("微软雅黑", 10F);
            btnUpload.Location = new Point(733, 35);
            btnUpload.Name = "btnUpload";
            btnUpload.Size = new Size(150, 40);
            btnUpload.TabIndex = 2;
            btnUpload.Text = "上传图像";
            btnUpload.UseVisualStyleBackColor = true;
            btnUpload.Click += btnUpload_Click;
            // 
            // btnProcess
            // 
            btnProcess.Enabled = false;
            btnProcess.Font = new Font("微软雅黑", 10F);
            btnProcess.Location = new Point(903, 35);
            btnProcess.Name = "btnProcess";
            btnProcess.Size = new Size(150, 40);
            btnProcess.TabIndex = 3;
            btnProcess.Text = "开始矫正";
            btnProcess.UseVisualStyleBackColor = true;
            btnProcess.Click += btnProcess_Click;
            // 
            // btnSave
            // 
            btnSave.Enabled = false;
            btnSave.Font = new Font("微软雅黑", 10F);
            btnSave.Location = new Point(1073, 35);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 40);
            btnSave.TabIndex = 4;
            btnSave.Text = "保存结果";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnInitialize
            // 
            btnInitialize.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            btnInitialize.Location = new Point(563, 35);
            btnInitialize.Name = "btnInitialize";
            btnInitialize.Size = new Size(150, 40);
            btnInitialize.TabIndex = 1;
            btnInitialize.Text = "初始化模型";
            btnInitialize.UseVisualStyleBackColor = true;
            btnInitialize.Click += btnInitialize_Click;
            // 
            // pictureBoxOriginal
            // 
            pictureBoxOriginal.BackColor = Color.FromArgb(240, 240, 240);
            pictureBoxOriginal.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxOriginal.Dock = DockStyle.Fill;
            pictureBoxOriginal.Location = new Point(0, 40);
            pictureBoxOriginal.Name = "pictureBoxOriginal";
            pictureBoxOriginal.Size = new Size(640, 562);
            pictureBoxOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOriginal.TabIndex = 1;
            pictureBoxOriginal.TabStop = false;
            // 
            // pictureBoxOutput
            // 
            pictureBoxOutput.BackColor = Color.FromArgb(240, 240, 240);
            pictureBoxOutput.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxOutput.Dock = DockStyle.Fill;
            pictureBoxOutput.Location = new Point(0, 40);
            pictureBoxOutput.Name = "pictureBoxOutput";
            pictureBoxOutput.Size = new Size(636, 562);
            pictureBoxOutput.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxOutput.TabIndex = 1;
            pictureBoxOutput.TabStop = false;
            // 
            // lblOriginal
            // 
            lblOriginal.Dock = DockStyle.Top;
            lblOriginal.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            lblOriginal.Location = new Point(0, 0);
            lblOriginal.Name = "lblOriginal";
            lblOriginal.Padding = new Padding(10, 10, 0, 5);
            lblOriginal.Size = new Size(640, 40);
            lblOriginal.TabIndex = 0;
            lblOriginal.Text = "原始图像";
            lblOriginal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblOutput
            // 
            lblOutput.Dock = DockStyle.Top;
            lblOutput.Font = new Font("微软雅黑", 10F, FontStyle.Bold);
            lblOutput.Location = new Point(0, 0);
            lblOutput.Name = "lblOutput";
            lblOutput.Padding = new Padding(10, 10, 0, 5);
            lblOutput.Size = new Size(636, 40);
            lblOutput.TabIndex = 0;
            lblOutput.Text = "矫正后图像";
            lblOutput.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblStatus
            // 
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.Font = new Font("微软雅黑", 9F);
            lblStatus.Location = new Point(0, 0);
            lblStatus.Name = "lblStatus";
            lblStatus.Padding = new Padding(10, 0, 0, 0);
            lblStatus.Size = new Size(1280, 40);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "状态: 请先设置参数并初始化模型";
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelTop
            // 
            panelTop.Controls.Add(groupBoxParameters);
            panelTop.Controls.Add(btnInitialize);
            panelTop.Controls.Add(btnUpload);
            panelTop.Controls.Add(btnProcess);
            panelTop.Controls.Add(btnSave);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(10);
            panelTop.Size = new Size(1280, 98);
            panelTop.TabIndex = 0;
            // 
            // groupBoxParameters
            // 
            groupBoxParameters.Controls.Add(chkUseGpu);
            groupBoxParameters.Controls.Add(lblCpuThreads);
            groupBoxParameters.Controls.Add(numCpuThreads);
            groupBoxParameters.Controls.Add(lblGpuId);
            groupBoxParameters.Controls.Add(numGpuId);
            groupBoxParameters.Controls.Add(lblGpuMem);
            groupBoxParameters.Controls.Add(numGpuMem);
            groupBoxParameters.Controls.Add(chkUseTensorRT);
            groupBoxParameters.Font = new Font("微软雅黑", 9F);
            groupBoxParameters.Location = new Point(20, 10);
            groupBoxParameters.Name = "groupBoxParameters";
            groupBoxParameters.Size = new Size(490, 80);
            groupBoxParameters.TabIndex = 0;
            groupBoxParameters.TabStop = false;
            groupBoxParameters.Text = "推理参数设置";
            // 
            // chkUseGpu
            // 
            chkUseGpu.AutoSize = true;
            chkUseGpu.Location = new Point(15, 25);
            chkUseGpu.Name = "chkUseGpu";
            chkUseGpu.Size = new Size(80, 21);
            chkUseGpu.TabIndex = 0;
            chkUseGpu.Text = "使用 GPU";
            chkUseGpu.UseVisualStyleBackColor = true;
            chkUseGpu.CheckedChanged += chkUseGpu_CheckedChanged;
            // 
            // lblCpuThreads
            // 
            lblCpuThreads.AutoSize = true;
            lblCpuThreads.Location = new Point(15, 53);
            lblCpuThreads.Name = "lblCpuThreads";
            lblCpuThreads.Size = new Size(75, 17);
            lblCpuThreads.TabIndex = 1;
            lblCpuThreads.Text = "CPU 线程数:";
            // 
            // numCpuThreads
            // 
            numCpuThreads.Location = new Point(100, 51);
            numCpuThreads.Maximum = new decimal(new int[] { 64, 0, 0, 0 });
            numCpuThreads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numCpuThreads.Name = "numCpuThreads";
            numCpuThreads.Size = new Size(70, 23);
            numCpuThreads.TabIndex = 2;
            numCpuThreads.Value = new decimal(new int[] { 10, 0, 0, 0 });
            // 
            // lblGpuId
            // 
            lblGpuId.AutoSize = true;
            lblGpuId.Location = new Point(200, 27);
            lblGpuId.Name = "lblGpuId";
            lblGpuId.Size = new Size(53, 17);
            lblGpuId.TabIndex = 3;
            lblGpuId.Text = "GPU ID:";
            // 
            // numGpuId
            // 
            numGpuId.Enabled = false;
            numGpuId.Location = new Point(260, 25);
            numGpuId.Maximum = new decimal(new int[] { 7, 0, 0, 0 });
            numGpuId.Name = "numGpuId";
            numGpuId.Size = new Size(70, 23);
            numGpuId.TabIndex = 4;
            // 
            // lblGpuMem
            // 
            lblGpuMem.AutoSize = true;
            lblGpuMem.Location = new Point(180, 53);
            lblGpuMem.Name = "lblGpuMem";
            lblGpuMem.Size = new Size(64, 17);
            lblGpuMem.TabIndex = 5;
            lblGpuMem.Text = "GPU 内存:";
            // 
            // numGpuMem
            // 
            numGpuMem.Enabled = false;
            numGpuMem.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numGpuMem.Location = new Point(260, 51);
            numGpuMem.Maximum = new decimal(new int[] { 16000, 0, 0, 0 });
            numGpuMem.Minimum = new decimal(new int[] { 500, 0, 0, 0 });
            numGpuMem.Name = "numGpuMem";
            numGpuMem.Size = new Size(70, 23);
            numGpuMem.TabIndex = 6;
            numGpuMem.Value = new decimal(new int[] { 2000, 0, 0, 0 });
            // 
            // chkUseTensorRT
            // 
            chkUseTensorRT.AutoSize = true;
            chkUseTensorRT.Enabled = false;
            chkUseTensorRT.Location = new Point(360, 25);
            chkUseTensorRT.Name = "chkUseTensorRT";
            chkUseTensorRT.Size = new Size(110, 21);
            chkUseTensorRT.TabIndex = 7;
            chkUseTensorRT.Text = "启用 TensorRT";
            chkUseTensorRT.UseVisualStyleBackColor = true;
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(lblStatus);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 700);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1280, 40);
            panelBottom.TabIndex = 2;
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.Location = new Point(0, 98);
            splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.Controls.Add(pictureBoxOriginal);
            splitContainer.Panel1.Controls.Add(lblOriginal);
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.Controls.Add(pictureBoxOutput);
            splitContainer.Panel2.Controls.Add(lblOutput);
            splitContainer.Size = new Size(1280, 602);
            splitContainer.SplitterDistance = 640;
            splitContainer.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 740);
            Controls.Add(splitContainer);
            Controls.Add(panelTop);
            Controls.Add(panelBottom);
            Font = new Font("微软雅黑", 9F);
            MinimumSize = new Size(1024, 600);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PaddleDocVision - 文本图像矫正工具";
            ((System.ComponentModel.ISupportInitialize)pictureBoxOriginal).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxOutput).EndInit();
            panelTop.ResumeLayout(false);
            groupBoxParameters.ResumeLayout(false);
            groupBoxParameters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numCpuThreads).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGpuId).EndInit();
            ((System.ComponentModel.ISupportInitialize)numGpuMem).EndInit();
            panelBottom.ResumeLayout(false);
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
