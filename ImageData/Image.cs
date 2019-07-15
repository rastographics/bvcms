using System;
using System.Linq;
using System.Data.Linq;
using System.IO;
using ImageResizer;
using Drawing = System.Drawing;
using System.Drawing.Imaging;
using UtilityExtensions;
using System.Text.RegularExpressions;

namespace ImageData
{
    public partial class Image
    {
        public static void Delete(int? id)
        {
            if (id.HasValue)
                ImageData.DbUtil.Db.ExecuteCommand("DELETE dbo.Image WHERE Id = {0}", id);
        }

        public static Image ImageFromId(int? id)
        {
            return DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == id);
        }

        public static Image NewImageFromBits(byte[] bits, int w, int h)
        {
            var i = new Image();
            i.LoadResizeFromBits(bits, w, h);
            DbUtil.Db.Images.InsertOnSubmit(i);
            DbUtil.Db.SubmitChanges();
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
                Mode =  mode == "pad" ? FitMode.Pad : 
                        mode == "max" ? FitMode.Max : 
                        mode == "crop" ? FitMode.Crop : 
                        FitMode.None
            };
            return resizeCropSettings;
        }

        public static Image NewTextFromString(string s)
        {
            var i = new Image();
            i.Mimetype = "text/plain";
            i.Bits = System.Text.Encoding.ASCII.GetBytes(s);
            i.Length = i.Bits.Length;
            DbUtil.Db.Images.InsertOnSubmit(i);
            DbUtil.Db.SubmitChanges();
            return i;
        }
        public void SetText(string s)
        {
            Mimetype = "text/plain";
            Bits = System.Text.Encoding.ASCII.GetBytes(s);
            Length = Bits.Length;
        }
        public static Image NewTextFromBits(byte[] bits)
        {
            var i = new Image();
            i.Mimetype = "text/plain";
            i.Bits = bits;
            i.Length = i.Bits.Length;
            DbUtil.Db.Images.InsertOnSubmit(i);
            DbUtil.Db.SubmitChanges();
            return i;
        }
        public static Image NewImageFromBits(byte[] bits)
        {
            var i = new Image();
            i.LoadImageFromBits(bits);
            DbUtil.Db.Images.InsertOnSubmit(i);
            DbUtil.Db.SubmitChanges();
            return i;
        }

        public static Image UpdateImageFromBits(int imageID, byte[] bits)
        {
            var i = from t in DbUtil.Db.Images
                    where t.Id == imageID
                    select t;

            var ii = i.FirstOrDefault();
            if (ii != null)
                ii.LoadImageFromBits(bits);
            DbUtil.Db.SubmitChanges();
            return ii;
        }

        public double Ratio()
        {
            var istream = new MemoryStream(Bits);
            var img1 = Drawing.Image.FromStream(istream);
            return Convert.ToDouble(img1.Width)/img1.Height;
        }
        private void LoadImageFromBits(byte[] bits)
        {
            var istream = new MemoryStream(bits);
            var img1 = Drawing.Image.FromStream(istream);
            var img2 = new Drawing.Bitmap(img1, img1.Width, img1.Height);
            var ostream = new MemoryStream();
            img2.Save(ostream, ImageFormat.Jpeg);
            Mimetype = "image/jpeg";
            Bits = ostream.GetBuffer();
            Length = Bits.Length;
            img1.Dispose();
            img2.Dispose();
            istream.Close();
            ostream.Close();
        }
        public static Image NewImageFromBits(byte[] bits, string type)
        {
            var image = CreateImageFromType(bits, type);            
            DbUtil.Db.Images.InsertOnSubmit(image);
            DbUtil.Db.SubmitChanges();
            return image;
        }
        public static Image CreateImageFromType(byte[] bits, string type)
        {
            var i = new Image();
            i.LoadFromBits(bits, type);
            return i;
        }
        public Image CreateNewTinyImage()
        {
            var i = new Image();
            i.LoadResizeFromBits(Bits, 50, 50);
            DbUtil.Db.Images.InsertOnSubmit(i);
            DbUtil.Db.SubmitChanges();
            return i;
        }
        private void LoadFromBits(byte[] bits, string type)
        {
            Bits = bits;
            Length = Bits.Length;
            Mimetype = type;
        }
        public static void DeleteOnSubmit(int? imageid)
        {
            var i = DbUtil.Db.Images.SingleOrDefault(img => img.Id == imageid);
            if (i == null)
                return;
            DbUtil.Db.Images.DeleteOnSubmit(i);
        }
        public bool HasMedical() // special function
        {
            var line = Medical();
            if (!line.HasValue())
                return false;
            if (line.ToLower().Contains("none"))
                return false;
            if (line.ToLower().Contains("n/a"))
                return false;
            if (line.ToLower().Contains("nka"))
                return false;
            return line.HasValue();
        }
        public string Medical() // special function
        {
            if (Mimetype != "text/plain")
                return null;
            var t = System.Text.ASCIIEncoding.ASCII.GetString(Bits);
            var q = from li in t.SplitStr("\r\n")
                    where li.StartsWith("Medical:")
                    select li;
            if (q.Count() == 0)
                return null;
            var a = q.First().Split(':');
            return a[1].Trim();
        }
        public bool InterestedInCoaching() // special function
        {
            if (Mimetype != "text/plain")
                return false;
            var t = System.Text.ASCIIEncoding.ASCII.GetString(Bits);
            var q = from li in t.SplitStr("\r\n")
                    where li.StartsWith("<tr><td>Coaching:")
                    select li;
            if (q.Count() == 0)
                return false;
            var s = q.First();
            return Regex.IsMatch(s, @"\A(?:<tr><td>.*</td><td>(1|true)</td></tr>)\Z", RegexOptions.IgnoreCase);
        }
        public static string Content(int id)
        {
            var img = DbUtil.Db.Images.SingleOrDefault(i => i.Id == id);
            if (img == null || img.Mimetype != "text/plain")
                return null;
            return System.Text.ASCIIEncoding.ASCII.GetString(img.Bits);
        }
        public override string ToString()
        {
            if (this.Mimetype != "text/plain")
                return null;
            return System.Text.ASCIIEncoding.ASCII.GetString(Bits);
        }
        public Drawing.Bitmap GetBitmap(int? w = null, int? h = null)
        {
            byte[] bits;
            if (w.HasValue && h.HasValue)
                bits = ResizeFromBits(Bits, w.Value, h.Value);
            else
                bits = Bits;
            var istream = new MemoryStream(bits);
            var img = Drawing.Image.FromStream(istream);
            var bmp = new Drawing.Bitmap(img, img.Width, img.Height);
            return bmp;
        }
    }
}
