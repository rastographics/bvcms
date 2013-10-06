using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class PictureResult : ActionResult
    {
        private readonly int id;
        private readonly bool portrait;
        private readonly bool tiny;
        private int? w, h;
        public PictureResult(int id, int? w = null, int? h = null, bool portrait = false, bool tiny = false)
        {
            this.id = id;
            this.portrait = portrait;
            this.tiny = tiny;
            this.w = w;
            this.h = h;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddYears(1));
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
            if (id == -2)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoPic2());
            }
            else if (id == -1)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoPic1());
            }
            else
            {
                var i = ImageData.DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == id);
                if (i == null || i.Secure == true)
                {
                    if (portrait)
                    {
                        context.HttpContext.Response.ContentType = "image/jpeg";
                        context.HttpContext.Response.BinaryWrite(tiny ? NoPic1() : NoPic2());
                    }
                    else
                        NoPic(context.HttpContext);
                }
                else
                {
                    if (w.HasValue && h.HasValue)
                    {
                        context.HttpContext.Response.ContentType = "image/jpeg";
                        var ri = FetchResizedImage(i, w.Value, h.Value);
                        context.HttpContext.Response.BinaryWrite(ri);
                    }
                    else
                    {
                        context.HttpContext.Response.ContentType = i.Mimetype ?? "image/jpeg";
                        context.HttpContext.Response.BinaryWrite(i.Bits);
                    }
                }
            }
        }
        private static byte[] NoPic1()
        {
            var u = HttpRuntime.Cache["unknownimagesm"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/images/unknownsm.jpg"));
                HttpRuntime.Cache["unknownimagesm"] = u;
                HttpRuntime.Cache.Insert("unknownimagesm", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }
        private static byte[] NoPic2()
        {
            var u = HttpRuntime.Cache["unknownimage"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/images/unknown.jpg"));
                HttpRuntime.Cache.Insert("unknownimagesm", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }
        public byte[] FetchResizedImage(ImageData.Image img, int w, int h)
        {
            var istream = new MemoryStream(img.Bits);
            var img1 = System.Drawing.Image.FromStream(istream);
            var ratio = Math.Min(w / (double)img1.Width, h / (double)img1.Height);
            if (ratio >= 1) // image is smaller than requested
                ratio = 1; // same size
            w = Convert.ToInt32(ratio * img1.Width);
            h = Convert.ToInt32(ratio * img1.Height);
            var img2 = new System.Drawing.Bitmap(img1, w, h);
            var ostream = new MemoryStream();
            img2.Save(ostream, ImageFormat.Jpeg);
            var Bits = ostream.GetBuffer();
            var Length = Bits.Length;
            img1.Dispose();
            img2.Dispose();
            istream.Close();
            ostream.Close();
            return Bits;
        }
        void NoPic(HttpContextBase context)
        {
            var bmp = new Bitmap(200, 200, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(bmp);
            g.Clear(Color.Bisque);
            g.DrawString("No Image", new Font("Verdana", 22, FontStyle.Bold), SystemBrushes.WindowText, new PointF(2, 2));
            context.Response.ContentType = "image/gif";
            bmp.Save(context.Response.OutputStream, ImageFormat.Gif);
        }
    }
}