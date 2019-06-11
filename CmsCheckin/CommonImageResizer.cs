using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CmsCheckin
{
    public delegate void WorkCompleteDelegate(object sender, bool complete);

    public partial class CommonImageResizer : UserControl
    {
        // Used for saving the edited image
        private Dictionary<string, ImageCodecInfo> _codecs = new Dictionary<string, ImageCodecInfo>();
        private EncoderParameters _encoderParams = new EncoderParameters();

        private Image _editedImage = null;

        public int RequiredWidth { get; set; }
        public int RequiredHeight { get; set; }

        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        // The starting image
        protected internal Image BaseImage
        {
            get
            {
                return _image;
            }
            set
            {
                try
                {
                    SuspendRefresh = true;
                    _image = value;
                    tbResize.Value = 100;
                    SetSizes(true);
                    SetCropValues(true);
                }
                finally
                {
                    btnSave.Enabled = true;
                    tbResize.Enabled = true;
                    SuspendRefresh = false;
                    RefreshForm();
                }
            }
        }
        private Image _image = null;

        // The in-process image
        protected Bitmap DrawnImage { get; set; }

        // X coordinate of the top left point of the yellow crop box
        protected int CropBoxX { get; set; }

        // Y coordinate of the top left point of the yellow crop box
        protected int CropBoxY { get; set; }

        protected int MaxCanvasWidth
        {
            get
            {
                int retVal = DrawnImage.Width;
                if (retVal > pctCamera.Width)
                    retVal = pctCamera.Width;
                return retVal;
            }
        }

        protected int MaxCanvasHeight
        {
            get
            {
                int retVal = DrawnImage.Height;
                if (retVal > pctCamera.Height)
                    retVal = pctCamera.Height;
                return retVal;
            }
        }

        protected bool SuspendRefresh { get; set; }

        public CommonImageResizer()
        {
            InitializeComponent();

            CropBoxX = -1;
            CropBoxY = -1;

            // Save in full quality
            _encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            // Find the codecs for the supported formats, set the open and save dialog filters
            string displayFilters = string.Empty;
            int codecCount = 0;
            int jpegIndex = 0;
            List<Guid> imageFormats = new List<Guid>(new Guid[] { ImageFormat.Bmp.Guid, ImageFormat.Gif.Guid, ImageFormat.Jpeg.Guid, ImageFormat.Png.Guid, ImageFormat.Tiff.Guid });
            foreach (ImageCodecInfo codec in ImageCodecInfo.GetImageEncoders())
            {
                if (imageFormats.Contains(codec.FormatID))
                {
                    codecCount++;
                    if (codec.FormatDescription.Equals("JPEG"))
                        jpegIndex = codecCount;

                    string fileFilterLeft = codec.FormatDescription + " files (";
                    string fileFilterRight = "|";
                    foreach (string extension in codec.FilenameExtension.Split(new char[] { ';' }))
                    {
                        string ext = extension.ToLower();
                        fileFilterLeft += ext + ",";
                        fileFilterRight += ext + ";";
                        _codecs.Add(ext.Replace("*", string.Empty), codec);
                    }
                    displayFilters += fileFilterLeft.Substring(0, fileFilterLeft.Length - 1) + ")" + fileFilterRight.Substring(0, fileFilterRight.Length - 1) + "|";
                }
            }
        }

        // Allows developer to set starting image and required size of resulting image
        public CommonImageResizer(Image baseImage, int requiredWidth, int requiredHeight)
        {
            BaseImage = baseImage;
            if (requiredWidth > 0)
                RequiredWidth = requiredWidth;
            if (requiredHeight > 0)
                RequiredHeight = requiredHeight;
        }

        // Removes all images for control reuse
        public void Reset()
        {
            _editedImage = null;
            DrawnImage = null;
            BaseImage = null;
            RefreshForm();
        }

        public Image GetEditedImage()
        {
            return _editedImage;
        }

        // Draw the canvas, the image and crop box
        private void ImageResizer_Paint(object sender, PaintEventArgs e)
        {
            // Draws alternating shaded rectangles so user can differentiate canvas from image.
            bool xGrayBox = true;
            int backgroundX = 0;
            while (backgroundX < pctCamera.Width)
            {
                int backgroundY = 0;
                bool yGrayBox = xGrayBox;
                while (backgroundY < pctCamera.Height)
                {
                    int recWidth = (int)((backgroundX + 50 > pctCamera.Width) ? pctCamera.Width - backgroundX : 50);
                    int recHeight = (int)((backgroundY + 50 > pctCamera.Height) ? pctCamera.Height - backgroundY : 50);
                    e.Graphics.FillRectangle(((Brush)(yGrayBox ? Brushes.LightGray : Brushes.Gainsboro)), backgroundX, backgroundY, recWidth + 2, recHeight + 2);
                    backgroundY += 50;
                    yGrayBox = !yGrayBox;
                }
                backgroundX += 50;
                xGrayBox = !xGrayBox;
            }

            if (!SuspendRefresh && DrawnImage != null)
            {

                OffsetX = DrawnImage.Width / 2 - pctCamera.Width / 2;
                OffsetY = DrawnImage.Height / 2 - pctCamera.Height / 2;
                e.Graphics.DrawImage(DrawnImage, 0, 0,
                    new Rectangle(OffsetX, OffsetY, pctCamera.Width, pctCamera.Height),
                    GraphicsUnit.Pixel);

                // Draw the crop rectangle with both yellow and black so it is easily visible no matter the image.
                e.Graphics.DrawRectangle(Pens.Yellow, CropBoxX, CropBoxY, (float)nudCropWidth.Value, (float)nudCropHeight.Value);
                e.Graphics.DrawRectangle(Pens.Black, CropBoxX - 1, CropBoxY - 1, (float)nudCropWidth.Value + 2, (float)nudCropHeight.Value + 2);
            }
        }

        // Keep all stored values up to date
        protected void SetSizes(bool adjustCropSize)
        {
            if (BaseImage != null)
            {
                DrawnImage = new Bitmap(BaseImage,
                    (int)Math.Ceiling(BaseImage.Width * 0.01 * tbResize.Value),
                    (int)Math.Ceiling(BaseImage.Height * 0.01 * tbResize.Value));

                //SetCropValues(adjustCropSize);

            }
            RefreshForm();
        }

        // Ensure the crop box size meets the required sizes, if any.
        protected void SetCropValues(bool adjustCropSize)
        {
            if (RequiredWidth < 1)
            {
                nudCropWidth.Maximum = MaxCanvasWidth;
                if (adjustCropSize)
                    nudCropWidth.Value = 300;
            }
            else
            {
                nudCropWidth.Maximum = RequiredWidth;
                if (adjustCropSize)
                    nudCropWidth.Value = RequiredWidth;
            }
            if (RequiredHeight < 1)
            {
                nudCropHeight.Maximum = MaxCanvasHeight;
                if (adjustCropSize)
                    nudCropHeight.Value = 400;
            }
            else
            {
                nudCropHeight.Maximum = RequiredHeight;
                if (adjustCropSize)
                    nudCropHeight.Value = RequiredHeight;
            }

            if (CropBoxX == -1)
                CropBoxX = (int)(MaxCanvasWidth / 2) - (int)(nudCropWidth.Value / 2);
            if (CropBoxY == -1)
                CropBoxY = (int)(MaxCanvasHeight / 2) - (int)(nudCropHeight.Value / 2);
            VerifyCropValues();
        }

        // Ensure the crop box stays with in the bounds of the drawn image
        protected void VerifyCropValues()
        {
            bool toggle = false;
            try
            {
                // Suspend repainting so the crop box only redraws once
                if (!SuspendRefresh)
                {
                    SuspendRefresh = true;
                    toggle = true;
                }
            }
            finally
            {
                if (toggle)
                    SuspendRefresh = false;
            }
            RefreshForm();
        }

        // Update image size label and repaint
        protected void RefreshForm()
        {
            if (!SuspendRefresh)
            {
                Refresh();
                Invalidate();
            }
        }

        // Save the resized or cropped image
        public Image SaveImage()
        {
            try
            {
                // if( cropped, create a new graphics object to draw the new image on, otherwise just save the current image
                _editedImage = new Bitmap((int)nudCropWidth.Value, (int)nudCropHeight.Value);
                Graphics g = Graphics.FromImage(_editedImage);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                int drawnImageCropBoxX = CropBoxX - pctCamera.Location.X + OffsetX;
                int drawnImageCropBoxY = CropBoxY - pctCamera.Location.Y + OffsetY;

                g.DrawImage(DrawnImage, 0, 0, 
                    new Rectangle(drawnImageCropBoxX, drawnImageCropBoxY, 
                        (int)nudCropWidth.Value, 
                        (int)nudCropHeight.Value), 
                        GraphicsUnit.Pixel);
                return _editedImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return null;
        }

        // Controls the zoom factor.
        private void tbResize_ValueChanged(object sender, EventArgs e)
        {
            SetSizes(false);
            //lblZoomFactor.Text = tbResize.Value.ToString() + "%";
        }

        private void picZoomOut_Click(object sender, EventArgs e)
        {
            if (tbResize.Value > tbResize.Minimum)
                tbResize.Value -= 1;
        }

        private void picZoomIn_Click(object sender, EventArgs e)
        {
            if (tbResize.Value < tbResize.Maximum)
                tbResize.Value += 1;
        }

        // Toggles the crop box.
        private void chkCrop_CheckedChanged(object sender, EventArgs e)
        {
            SetSizes(true);
            SetCropValues(true);
        }

        // Update stored values when user resizes control
        private void EasyImageResizerControl_SizeChanged(object sender, EventArgs e)
        {
            SetSizes(true);
        }

        bool mousedown = false;
        // Allows user to reposition the crop box.
        private void EasyImageResizerControl_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            EasyImageResizerControl_MouseMove(sender, e);
        }
        // Allows user to reposition the crop box.
        private void EasyImageResizerControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown == false)
                return;
            // Ignore clicks outside the canvas.
            if (e.X < pctCamera.Width + 1 && e.Y < pctCamera.Height + 1)
            {
                CropBoxX = e.X - (int)(nudCropWidth.Value / 2);
                CropBoxY = e.Y - (int)(nudCropHeight.Value / 2);
                RefreshForm();
            }
        }
        // Allows user to reposition the crop box.
        private void EasyImageResizerControl_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }
        // Simple scrolling.
        private void ImagedScrolled(object sender, ScrollEventArgs e)
        {
            RefreshForm();
        }

        // Ensure the crop box draws correctly
        private void nudCrop_ValueChanged(object sender, EventArgs e)
        {
            VerifyCropValues();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
