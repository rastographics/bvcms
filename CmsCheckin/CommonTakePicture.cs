using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CmsCheckin
{
    public partial class CommonTakePicture : Form
    {
        private Capture cam;
        IntPtr m_ip = IntPtr.Zero;
        const int VIDEODEVICE = 0; // zero based index of video capture device to use
        const int VIDEOWIDTH = 640; // Depends on video device caps
        const int VIDEOHEIGHT = 480; // Depends on video device caps
        const int VIDEOBITSPERPIXEL = 64; // BitsPerPixel values determined by device

        public CommonTakePicture()
        {
            InitializeComponent();

            try
            {
                cam = new Capture(VIDEODEVICE, VIDEOWIDTH, VIDEOHEIGHT, VIDEOBITSPERPIXEL, imageResizer1.pctCamera);
                imageResizer1.btnTakePicture.Click += new EventHandler(btnTakePicture_Click);
                imageResizer1.btnSave.Click += new EventHandler(btnUploadPicture_Click);
            }
            catch (Exception)
            {
                //Handle exception
            }
        }
        private void btnTakePicture_Click(object sender, EventArgs e)
        {
            const string STR_ShowCamera = "Show Camera";
            if (imageResizer1.btnTakePicture.Text == STR_ShowCamera)
            {
                imageResizer1.pctCamera.Visible = true;
                imageResizer1.btnTakePicture.Text = "Take Picture";
                return;
            }
            if (cam != null)
            {
                Cursor.Current = Cursors.WaitCursor;

                if (m_ip != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(m_ip);
                    m_ip = IntPtr.Zero;
                }

                m_ip = cam.Click();
                Bitmap b = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, m_ip);
                b.RotateFlip(RotateFlipType.RotateNoneFlipY);

                imageResizer1.BaseImage = b;
                imageResizer1.pctCamera.Visible = false;
                imageResizer1.btnTakePicture.Text = STR_ShowCamera;

                Cursor.Current = Cursors.Default;
            }

        }
        private static byte[] ConvertImageToByteArray(Image image, ImageFormat format)
        {
            byte[] b;
            try
            {

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, format);
                    b = ms.ToArray();
                }
            }
            catch (Exception) { throw; }
            return b;
        }

        private void btnUploadPicture_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            var bits = ConvertImageToByteArray(imageResizer1.SaveImage(), ImageFormat.Jpeg);
				var url = Program.settings.createURI("Checkin2/UploadImage/" + Program.PeopleId);
            var wc = Util.CreateWebClient();
            wc.UploadData(url, "POST", bits);
            this.Close();
        }

        private void TakePicture_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (cam != null)
                cam.Dispose();
        }

        private void Return_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
