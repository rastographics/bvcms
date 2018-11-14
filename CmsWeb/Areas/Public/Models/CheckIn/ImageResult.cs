using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class ImageResult : ActionResult
    {
        private readonly int Id;
        public ImageResult(int id)
        {
            Id = id;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            var image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == Id);
            if (image == null)
            {
                NoPic(context.HttpContext);
            }
            else
            {
                context.HttpContext.Response.ContentType = image.Mimetype;
                context.HttpContext.Response.BinaryWrite(image.Bits);
            }
        }

        private void NoPic(System.Web.HttpContextBase context)
        {
            var portrait = context.Request.QueryString["portrait"].ToInt2();
            if (portrait == 1)
            {
                context.Response.ContentType = "image/jpeg";
                var u = context.Cache["unknownimage"] as Byte[];
                if (u == null)
                {
                    u = File.ReadAllBytes(context.Server.MapPath("/Content/images/unknown.jpg"));
                    context.Cache["unknownimage"] = u;
                }
                context.Response.BinaryWrite(u);
            }
            else
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
}
