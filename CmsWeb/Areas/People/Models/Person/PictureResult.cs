using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class PictureResult : ActionResult
    {
        private readonly int id;
        private readonly bool portrait;
        private readonly bool tiny;
        private readonly bool nodefault;
        private int? w, h;
        private readonly string mode;
        public PictureResult(int id, int? w = null, int? h = null, bool portrait = false, bool tiny = false, bool nodefault = false, string mode = "max")
        {
            this.id = id;
            this.portrait = portrait;
            this.tiny = tiny;
            this.nodefault = nodefault;
            this.w = w;
            this.h = h;
            this.mode = mode;
        }
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddMinutes(10));
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);

            if (id == -2)
            {
                context.HttpContext.Response.ContentType = "image/png";
                context.HttpContext.Response.BinaryWrite(NoPic2());
            }
            else if (id == -1)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoPic1());
            }
			else if (id == -3)
			{
				context.HttpContext.Response.ContentType = "image/jpeg";
				context.HttpContext.Response.BinaryWrite(NoPic3());
			}
            else if (id == -4)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoMalePic());
            }
            else if (id == -5)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoFemalePic());
            }
            else if (id == -6)
            {
                context.HttpContext.Response.ContentType = "image/png";
                context.HttpContext.Response.BinaryWrite(NoPic2Sm());
            }
            else if (id == -7)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoMalePicSm());
            }
            else if (id == -8)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoFemalePicSm());
            }
            else
            {
                ImageData.Image i = null;
#if DEBUG
                var idb = DbUtil.CheckImageDatabaseExists(Util.CmsHost.Replace("CMS_", "CMSi_"));
                if(idb == DbUtil.CheckDatabaseResult.DatabaseExists)
#endif
                try
                {
                    i = ImageData.CurrentDatabase.Images.SingleOrDefault(ii => ii.Id == id);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                }
                if (i == null || i.Secure == true)
                {
                    if (nodefault)
                        return;
                    if (portrait)
                    {
                        context.HttpContext.Response.ContentType = "image/jpeg";
                        context.HttpContext.Response.BinaryWrite(tiny ? NoPic1() : NoPic2());
                    }
                    else
                        context.HttpContext.Response.ContentType = "image/png";
                        context.HttpContext.Response.BinaryWrite(NoPic());
                }
                else
                {
                    if (w.HasValue && h.HasValue)
                    {
                        context.HttpContext.Response.ContentType = "image/jpeg";
                        var ri = FetchResizedImage(i, w.Value, h.Value, mode);
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
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/unknown.png"));
                HttpRuntime.Cache.Insert("unknownimage", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        private static byte[] NoPic2Sm()
        {
            var u = HttpRuntime.Cache["unknownimagesm"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/unknown_sm.png"));
                HttpRuntime.Cache.Insert("unknownimagesm", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        private static byte[] NoMalePic()
        {
            var u = HttpRuntime.Cache["nomalepic"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/male.png"));
                HttpRuntime.Cache.Insert("nomalepic", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        private static byte[] NoMalePicSm()
        {
            var u = HttpRuntime.Cache["nomalepicsm"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/male_sm.png"));
                HttpRuntime.Cache.Insert("nomalepicsm", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        private static byte[] NoFemalePic()
        {
            var u = HttpRuntime.Cache["nofemalepic"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/female.png"));
                HttpRuntime.Cache.Insert("nofemalepic", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        private static byte[] NoFemalePicSm()
        {
            var u = HttpRuntime.Cache["nofemalepicsm"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/female_sm.png"));
                HttpRuntime.Cache.Insert("nofemalepicsm", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

		private static byte[] NoPic3()
		{
			var u = HttpRuntime.Cache["sgfimage"] as byte[];
			if (u == null)
			{
				u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/images/sgfunknown.jpg"));
				HttpRuntime.Cache.Insert("sgfimage", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
			}
			return u;
		}

        private static byte[] NoPic()
        {
            var u = HttpRuntime.Cache["imagenotfound"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/Content/touchpoint/img/image_not_found.png"));
                HttpRuntime.Cache.Insert("imagenotfound", u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        public byte[] FetchResizedImage(ImageData.Image img, int w, int h, string mode = "max")
        {
            return ImageData.Image.ResizeFromBits(img.Bits, w, h, mode);
        }
        
    }
}