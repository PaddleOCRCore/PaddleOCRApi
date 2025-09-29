namespace WinFormsApp.UserControl
{
    public partial class PictureView : PictureBox
    {
        public PictureView()
        {
            InitializeComponent();
            InitPictureView();
        }

        private bool parentEventAttached = false;
        private bool dragging = false;
        private Point dragStartScreen = Point.Empty;
        private Point dragStartLocation = Point.Empty;

        private void InitPictureView()
        {
            this.Location = new Point(0, 0);
            this.Dock = DockStyle.None;
            this.SizeMode = PictureBoxSizeMode.Zoom;
            this.MouseDown += PictureView_MouseDown;
            this.MouseUp += PictureView_MouseUp;
            this.MouseMove += PictureView_MouseMove;
            this.MouseDoubleClick += PictureView_MouseDoubleClick;
            this.MouseWheel += PictureView_MouseWheel;
        }

        public string ImgPath
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.Size = this.Parent?.Size ?? this.Size;
                    this.Location = new Point(0, 0);
                    this.Image = ImageTools.LoadImage(value);
                    if (!parentEventAttached && this.Parent != null)
                    {
                        parentEventAttached = true;
                        this.Parent.MouseDoubleClick += Parent_MouseDoubleClick;
                    }
                }
            }
        }

        private void Parent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ResetView();
        }

        private void PictureView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ResetView();
        }

        private void ResetView()
        {
            if (this.Image == null || this.Parent == null)
                return;
            this.Size = this.Parent.Size;
            this.Location = new Point(0, 0);
        }

        private void PictureView_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
                return;
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragStartScreen = Control.MousePosition;
                dragStartLocation = this.Location;
            }
        }

        private void PictureView_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
                return;
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
            }
        }

        private void PictureView_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.Image == null || !dragging)
                return;
            Point currentScreen = Control.MousePosition;
            int offsetX = currentScreen.X - dragStartScreen.X;
            int offsetY = currentScreen.Y - dragStartScreen.Y;
            this.Location = new Point(dragStartLocation.X + offsetX, dragStartLocation.Y + offsetY);
        }

        private void PictureView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this.Image == null)
                return;

            int zoomStep = Math.Max(this.Width, this.Height) / 10;
            if (zoomStep < 1) zoomStep = 1;

            int newWidth = this.Width;
            int newHeight = this.Height;

            if (e.Delta > 0)
            {
                newWidth += zoomStep;
                newHeight += zoomStep;
                if (newWidth > this.Image.Width * 10 || newHeight > this.Image.Height * 10)
                    return;
            }
            else if (e.Delta < 0)
            {
                newWidth -= zoomStep;
                newHeight -= zoomStep;
                if (newWidth < Math.Max(10, this.Image.Width / 10) || newHeight < Math.Max(10, this.Image.Height / 10))
                    return;
            }

            int oldWidth = this.Width;
            int oldHeight = this.Height;
            this.Size = new Size(newWidth, newHeight);

            // Adjust location to zoom around mouse pointer
            int mouseX = e.Location.X;
            int mouseY = e.Location.Y;
            int dx = (int)((double)mouseX * (oldWidth - newWidth) / oldWidth);
            int dy = (int)((double)mouseY * (oldHeight - newHeight) / oldHeight);
            this.Location = new Point(this.Location.X + dx, this.Location.Y + dy);
        }
    }
}
