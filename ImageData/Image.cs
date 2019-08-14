using ImageResizer;
using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UtilityExtensions;
using Drawing = System.Drawing;

namespace ImageData
{
    public partial class Image
    {
        public static void Delete(int? id)
        {
            if (id.HasValue)
            {
                CMSImageDataContext.Create(HttpContextFactory.Current).ExecuteCommand("DELETE dbo.Image WHERE Id = {0}", id);
            }
        }

        public static Image ImageFromId(int? id)
        {
            return CMSImageDataContext.Create(HttpContextFactory.Current).Images.SingleOrDefault(ii => ii.Id == id);
        }

        public static Image NewImageFromBits(byte[] bits, int w, int h, CMSImageDataContext db)
        {
            var i = new Image();
            i.LoadResizeFromBits(bits, w, h);
            InsertImage(db, i);
            return i;
        }

        public static Image NewImageFromImage(Image i, int w, int h)
        {
            var i2 = new Image();
            i2.LoadResizeFromBits(i.Bits, w, h);
            return i2;
        }

        private void LoadResizeFromBits(byte[] bits, int w, int h)
        {
            Bits = ResizeFromBits(bits, w, h);
        }

        public static byte[] ResizeFromBits(byte[] bitsin, int w, int h, string mode = "max")
        {
            var resizeCropSettings = ResizeCropSettings(w, h, mode);
            byte[] bits = null;
            using (var ostream = new MemoryStream())
            {
                ImageBuilder.Current.Build(bitsin, ostream, resizeCropSettings);
                bits = ostream.ToArray();
            }
            return bits;
        }

        public Stream ResizeToStream(int w, int h, string mode = "max")
        {
            var resizeCropSettings = ResizeCropSettings(w, h, mode);
            var ostream = new MemoryStream();
            ImageBuilder.Current.Build(Bits, ostream, resizeCropSettings);
            ostream.Position = 0;
            return ostream;
        }

        public Stream ResizeToStream(string instructions)
        {
            var settings = new ResizeSettings(instructions);
            var ostream = new MemoryStream();
            ImageBuilder.Current.Build(Bits, ostream, settings);
            ostream.Position = 0;
            return ostream;
        }

        static readonly byte[] onepixelgif = { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x1, 0x0, 0x1, 0x0, 0x80, 0x0, 0x0, 0xff, 0xff, 0xff, 0x0, 0x0, 0x0, 0x2c, 0x0, 0x0, 0x0, 0x0, 0x1, 0x0, 0x1, 0x0, 0x0, 0x2, 0x2, 0x44, 0x1, 0x0, 0x3b };
        public static Stream BlankImage(int w, int h)
        {
            var resizeCropSettings = ResizeCropSettings(w, h, "pad");
            var ostream = new MemoryStream();
            ImageBuilder.Current.Build(onepixelgif, ostream, resizeCropSettings);
            ostream.Position = 0;
            return ostream;
        }

        private static ResizeSettings ResizeCropSettings(int w, int h, string mode)
        {
            var resizeCropSettings = new ResizeSettings
            {
                Format = "jpg",
                Width = w,
                Height = h,
                Scale = ScaleMode.UpscaleCanvas,
                Mode = mode == "pad" ? FitMode.Pad :
                        mode == "max" ? FitMode.Max :
                        mode == "crop" ? FitMode.Crop :
                        FitMode.None
            };
            return resizeCropSettings;
        }

        public static Image NewTextFromString(string s, CMSImageDataContext db)
        {
            var i = new Image();
            i.Mimetype = "text/plain";
            i.Bits = Encoding.ASCII.GetBytes(s);
            i.Length = i.Bits.Length;
            InsertImage(db, i);
            return i;
        }

        public void SetText(string s)
        {
            Mimetype = "text/plain";
            Bits = Encoding.ASCII.GetBytes(s);
            Length = Bits.Length;
        }

        public static Image NewTextFromBits(byte[] bits, CMSImageDataContext db)
        {
            var i = new Image();
            i.Mimetype = "text/plain";
            i.Bits = bits;
            i.Length = i.Bits.Length;
            InsertImage(db, i);
            return i;
        }

        public static Image NewImageFromBits(byte[] bits, CMSImageDataContext db)
        {
            var i = new Image();
            i.LoadImageFromBits(bits);
            InsertImage(db, i);
            return i;
        }
        
        public double Ratio()
        {
            var istream = new MemoryStream(Bits);
            var img1 = Drawing.Image.FromStream(istream);
            return Convert.ToDouble(img1.Width) / img1.Height;
        }

        internal void LoadImageFromBits(byte[] bits)
        {
            using (var istream = new MemoryStream(bits))
            {
                using (var img1 = Drawing.Image.FromStream(istream))
                {
                    using (var img2 = new Drawing.Bitmap(img1, img1.Width, img1.Height))
                    {
                        using (var ostream = new MemoryStream())
                        {
                            img2.Save(ostream, ImageFormat.Jpeg);
                            Mimetype = "image/jpeg";
                            Bits = ostream.GetBuffer();
                            Length = Bits.Length;
                        }
                    }
                }
            }
        }

        public static Image NewImageFromBits(byte[] bits, string type, CMSImageDataContext db)
        {
            var i = new Image();
            i.LoadFromBits(bits, type);
            InsertImage(db, i);
            return i;
        }

        private static void InsertImage(CMSImageDataContext db, Image i)
        {
            db.Images.InsertOnSubmit(i);
            db.SubmitChanges();
        }

        public Image CreateNewTinyImage(CMSImageDataContext db)
        {
            var i = new Image();
            i.LoadResizeFromBits(Bits, 50, 50);
            InsertImage(db, i);
            return i;
        }

        private void LoadFromBits(byte[] bits, string type)
        {
            Bits = bits;
            Length = Bits.Length;
            Mimetype = type;
        }

        public bool HasMedical() // special function
        {
            var line = Medical();
            if (!line.HasValue())
            {
                return false;
            }

            if (line.ToLower().Contains("none"))
            {
                return false;
            }

            if (line.ToLower().Contains("n/a"))
            {
                return false;
            }

            if (line.ToLower().Contains("nka"))
            {
                return false;
            }

            return line.HasValue();
        }

        public string Medical() // special function
        {
            if (Mimetype != "text/plain")
            {
                return null;
            }

            var t = Encoding.ASCII.GetString(Bits);
            var q = from li in t.SplitStr("\r\n")
                    where li.StartsWith("Medical:")
                    select li;
            if (q.Count() == 0)
            {
                return null;
            }

            var a = q.First().Split(':');
            return a[1].Trim();
        }

        public bool InterestedInCoaching() // special function
        {
            if (Mimetype != "text/plain")
            {
                return false;
            }

            var t = Encoding.ASCII.GetString(Bits);
            var q = from li in t.SplitStr("\r\n")
                    where li.StartsWith("<tr><td>Coaching:")
                    select li;
            if (q.Count() == 0)
            {
                return false;
            }

            var s = q.First();
            return Regex.IsMatch(s, @"\A(?:<tr><td>.*</td><td>(1|true)</td></tr>)\Z", RegexOptions.IgnoreCase);
        }

        public override string ToString()
        {
            if (this.Mimetype != "text/plain")
            {
                return null;
            }

            return Encoding.ASCII.GetString(Bits);
        }

        public Drawing.Bitmap GetBitmap(int? w = null, int? h = null)
        {
            byte[] bits;
            if (w.HasValue && h.HasValue)
            {
                bits = ResizeFromBits(Bits, w.Value, h.Value);
            }
            else
            {
                bits = Bits;
            }

            var istream = new MemoryStream(bits);
            var img = Drawing.Image.FromStream(istream);
            var bmp = new Drawing.Bitmap(img, img.Width, img.Height);
            return bmp;
        }
    }
}
