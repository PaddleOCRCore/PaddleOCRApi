namespace WinFormsApp.UserControl
{
    public partial class PictureView : PictureBox
    {
        public PictureView()
        {
            InitializeComponent();
            PictureView_Load();
        }

        private bool isInitParent = false;
        private bool isMouseDown = false;
        private Point p1 = new Point();
        private Point p2 = new Point();

        public string imgPath
        {
            set
            {
                if (value != string.Empty)
                {
                    this.Size = this.Parent.Size;
                    this.Location = new Point(0, 0);
                    this.Image = ImageTools.LoadImage(value);
                    if (!isInitParent)
                    {
                        isInitParent = true;
                        this.Parent.MouseDoubleClick += PictureBox_MouseDoubleClick;
                    }
                }
            }
        }

        private void PictureView_Load()
        {
            this.Location = new Point(0, 0);
            this.Dock = DockStyle.None;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.MouseWheel += PictureBox_MouseWheel;
            this.MouseDown += PictureBox_MouseDown;
            this.MouseUp += PictureBox_MouseUp;
            this.MouseMove += PictureBox_MouseMove;
            this.MouseDoubleClick += PictureBox_MouseDoubleClick;
        }

        private void PictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            this.Size = this.Parent.Size;
            this.Location = new Point(0, 0);
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            int a = Control.MousePosition.X - p1.X;
            int b = Control.MousePosition.Y - p1.Y;
            if (isMouseDown)
                this.Location = new Point(p2.X + a, p2.Y + b);
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            isMouseDown = false;
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            isMouseDown = true;
            p1 = Control.MousePosition;
            p2 = this.Location;
        }

        private void PictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
            {
                return;
            }
            int x = e.Location.X;
            int y = e.Location.Y;
            int ow = this.Width;
            int oh = this.Height;
            int VX, VY; 
            int zoomStep = (this.Width > this.Height) ? this.Width / 10 : this.Height / 10;
            if (e.Delta > 0)
            {
                if (Math.Max(this.Width, this.Height) > Math.Max(this.Image.Width * 10, this.Image.Height * 10))
                    return;
                this.Width += zoomStep;
                this.Height += zoomStep;
            }
            if (e.Delta < 0)
            {
                if (Math.Min(this.Width, this.Height) < Math.Min(this.Image.Width / 10, this.Image.Height / 10))
                    return;

                this.Width -= zoomStep;
                this.Height -= zoomStep;
            }
            VX = (int)((double)x * (ow - this.Width) / ow);
            VY = (int)((double)y * (oh - this.Height) / oh);
            this.Location = new Point(this.Location.X + VX, this.Location.Y + VY);
        }
    }
}
