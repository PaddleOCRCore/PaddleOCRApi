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
            button2 = new Button();
            groupBox1 = new GroupBox();
            numDowncpu_threads = new NumericUpDown();
            label3 = new Label();
            numDowngpu_id = new NumericUpDown();
            label2 = new Label();
            label1 = new Label();
            comboBoxuse_gpu = new ComboBox();
            pictureBoxImg = new PictureBox();
            groupBox2 = new GroupBox();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numDowncpu_threads).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDowngpu_id).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxImg).BeginInit();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // buttonInit
            // 
            buttonInit.Location = new Point(486, 15);
            buttonInit.Name = "buttonInit";
            buttonInit.Size = new Size(120, 36);
            buttonInit.TabIndex = 0;
            buttonInit.Text = "初始化OCR";
            buttonInit.UseVisualStyleBackColor = true;
            buttonInit.Click += buttonInit_Click;
            // 
            // buttonRec
            // 
            buttonRec.Location = new Point(612, 15);
            buttonRec.Name = "buttonRec";
            buttonRec.Size = new Size(120, 36);
            buttonRec.TabIndex = 1;
            buttonRec.Text = "OCR识别";
            buttonRec.UseVisualStyleBackColor = true;
            buttonRec.Click += buttonRec_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            textBoxResult.Location = new Point(534, 94);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ScrollBars = ScrollBars.Both;
            textBoxResult.Size = new Size(547, 586);
            textBoxResult.TabIndex = 2;
            // 
            // button2
            // 
            button2.Location = new Point(949, 17);
            button2.Name = "button2";
            button2.Size = new Size(120, 36);
            button2.TabIndex = 3;
            button2.Text = "获取图片Base64";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(numDowncpu_threads);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(numDowngpu_id);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(comboBoxuse_gpu);
            groupBox1.Controls.Add(button2);
            groupBox1.Controls.Add(buttonInit);
            groupBox1.Controls.Add(buttonRec);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1069, 66);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "功能选项";
            // 
            // numDowncpu_threads
            // 
            numDowncpu_threads.Location = new Point(419, 22);
            numDowncpu_threads.Name = "numDowncpu_threads";
            numDowncpu_threads.Size = new Size(55, 23);
            numDowncpu_threads.TabIndex = 9;
            numDowncpu_threads.Value = new decimal(new int[] { 30, 0, 0, 0 });
            numDowncpu_threads.ValueChanged += numDowncpu_threads_ValueChanged;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(333, 24);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 8;
            label3.Text = "CPU线程数：";
            label3.TextAlign = ContentAlignment.TopRight;
            label3.UseWaitCursor = true;
            // 
            // numDowngpu_id
            // 
            numDowngpu_id.Location = new Point(271, 22);
            numDowngpu_id.Name = "numDowngpu_id";
            numDowngpu_id.Size = new Size(54, 23);
            numDowngpu_id.TabIndex = 7;
            numDowngpu_id.ValueChanged += numDowngpu_id_ValueChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(202, 27);
            label2.Name = "label2";
            label2.Size = new Size(63, 17);
            label2.TabIndex = 6;
            label2.Text = "GPU_ID：";
            label2.TextAlign = ContentAlignment.TopRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 26);
            label1.Name = "label1";
            label1.Size = new Size(69, 17);
            label1.TabIndex = 5;
            label1.Text = "启用GPU：";
            // 
            // comboBoxuse_gpu
            // 
            comboBoxuse_gpu.FormattingEnabled = true;
            comboBoxuse_gpu.Items.AddRange(new object[] { "使用CPU", "使用GPU" });
            comboBoxuse_gpu.Location = new Point(92, 22);
            comboBoxuse_gpu.Name = "comboBoxuse_gpu";
            comboBoxuse_gpu.Size = new Size(101, 25);
            comboBoxuse_gpu.TabIndex = 4;
            comboBoxuse_gpu.SelectedIndexChanged += comboBoxuse_gpu_SelectedIndexChanged;
            // 
            // pictureBoxImg
            // 
            pictureBoxImg.Dock = DockStyle.Fill;
            pictureBoxImg.Location = new Point(3, 19);
            pictureBoxImg.Name = "pictureBoxImg";
            pictureBoxImg.Size = new Size(510, 574);
            pictureBoxImg.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxImg.TabIndex = 5;
            pictureBoxImg.TabStop = false;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(pictureBoxImg);
            groupBox2.Location = new Point(12, 84);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(516, 596);
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
            Text = "PaddleOCR识别Demo";
            Load += MainForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
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
        private Button button2;
        private GroupBox groupBox1;
        private Label label1;
        private ComboBox comboBoxuse_gpu;
        private Label label2;
        private NumericUpDown numDowngpu_id;
        private NumericUpDown numDowncpu_threads;
        private Label label3;
        private PictureBox pictureBoxImg;
        private GroupBox groupBox2;
    }
}
