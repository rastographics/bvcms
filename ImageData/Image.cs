using System;
using System.Linq;
using System.Data.Linq;
using System.IO;
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
            if(id.HasValue)
                ImageData.DbUtil.Db.ExecuteCommand("DELETE dbo.Image WHERE Id = {0}", id);
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
            var istream = new MemoryStream(bits);
            var img1 = Drawing.Image.FromStream(istream);
            var ratio = Math.Min(w / (double)img1.Width, h / (double)img1.Height);
            if (ratio >= 1) // image is smaller than requested
                ratio = 1; // same size
            w = Convert.ToInt32(ratio * img1.Width);
            h = Convert.ToInt32(ratio * img1.Height);
            var img2 = new Drawing.Bitmap(img1, w, h);
            var ostream = new MemoryStream();
            img2.Save(ostream, ImageFormat.Jpeg);
            Bits = ostream.GetBuffer();
            Length = Bits.Length;
            img1.Dispose();
            img2.Dispose();
            istream.Close();
            ostream.Close();
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

		  public static Image UpdateImageFromBits( int imageID, byte[] bits )
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
            var i = new Image();
            i.LoadFromBits(bits, type);
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
    }
}
