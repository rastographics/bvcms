using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace CmsWeb.Areas.People.Models.Person
{
    public class PictureResult : ActionResult
    {
        private int id;
        private int size;
        public PictureResult(int id, int s)
        {
            this.id = id;
            size = s;
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
            else if (id == -2)
            {
                context.HttpContext.Response.ContentType = "image/jpeg";
                context.HttpContext.Response.BinaryWrite(NoPic1());
            }
            else
            {
                var i = ImageData.DbUtil.Db.Images.SingleOrDefault(ii => ii.Id == id);
                if (i == null)
                {
                    context.HttpContext.Response.ContentType = "image/jpeg";
                    context.HttpContext.Response.BinaryWrite(size == 1 ? NoPic1() : NoPic2());
                }
                else
                {
                    context.HttpContext.Response.ContentType = i.Mimetype ?? "image/jpeg";
                    context.HttpContext.Response.BinaryWrite(i.Bits);
                }
            }
        }
        private static byte[] NoPic1()
        {
            var u = HttpRuntime.Cache["unknownimagesm"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/images/unknownsm.jpg"));
                HttpRuntime.Cache["unknownimagesm"] = u;
                HttpRuntime.Cache.Insert("unknownimagesm", u, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
            }
            return u;
        }
        private static byte[] NoPic2()
        {
            var u = HttpRuntime.Cache["unknownimage"] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContext.Current.Server.MapPath("/images/unknown.jpg"));
                HttpRuntime.Cache.Insert("unknownimagesm", u, null, DateTime.Now.AddMinutes(10), Cache.NoSlidingExpiration);
            }
            return u;
        }
    }
}