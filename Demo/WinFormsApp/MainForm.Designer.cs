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
            button1 = new Button();
            textBoxResult = new TextBox();
            button2 = new Button();
            SuspendLayout();
            // 
            // buttonInit
            // 
            buttonInit.Location = new Point(28, 22);
            buttonInit.Name = "buttonInit";
            buttonInit.Size = new Size(120, 53);
            buttonInit.TabIndex = 0;
            buttonInit.Text = "初始化";
            buttonInit.UseVisualStyleBackColor = true;
            buttonInit.Click += buttonInit_Click;
            // 
            // button1
            // 
            button1.Location = new Point(188, 22);
            button1.Name = "button1";
            button1.Size = new Size(120, 53);
            button1.TabIndex = 1;
            button1.Text = "OCR识别";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBoxResult
            // 
            textBoxResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBoxResult.Location = new Point(2, 91);
            textBoxResult.Multiline = true;
            textBoxResult.Name = "textBoxResult";
            textBoxResult.ScrollBars = ScrollBars.Both;
            textBoxResult.Size = new Size(796, 356);
            textBoxResult.TabIndex = 2;
            // 
            // button2
            // 
            button2.Location = new Point(349, 22);
            button2.Name = "button2";
            button2.Size = new Size(120, 53);
            button2.TabIndex = 3;
            button2.Text = "获取图片Base64";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button2);
            Controls.Add(textBoxResult);
            Controls.Add(button1);
            Controls.Add(buttonInit);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "居民身份证OCR识别Demo";
            Load += MainForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button buttonInit;
        private Button button1;
        private TextBox textBoxResult;
        private Button button2;
    }
}
