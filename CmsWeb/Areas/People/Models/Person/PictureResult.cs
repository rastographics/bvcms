using CmsData;
using CmsWeb.Membership.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Text;
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
        private readonly bool nodefault;
        private int? w, h;
        private readonly string mode;
        private readonly bool shouldBePublic;
        private readonly bool preview;

        public PictureResult(int id, int? w = null, int? h = null, bool portrait = false, bool tiny = false, bool nodefault = false, string mode = "max", bool shouldBePublic = false, bool preview = false)
        {
            this.id = id;
            this.portrait = portrait;
            this.tiny = tiny;
            this.nodefault = nodefault;
            this.w = w;
            this.h = h;
            this.mode = mode;
            this.preview = preview;
            this.shouldBePublic = shouldBePublic;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Clear();
            context.HttpContext.Response.Cache.SetExpires(DateTime.Now.AddMinutes(10));
            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);

            switch (id)
            {
                case 0:
                    WritePng(context, NoPic());
                    break;
                case -1:
                    WriteJpeg(context, NoPic1());
                    break;
                case -2:
                    WritePng(context, NoPic2());
                    break;
                case -3:
                    WriteJpeg(context, NoPic3());
                    break;
                case -4:
                    WriteJpeg(context, NoMalePic());
                    break;
                case -5:
                    WriteJpeg(context, NoFemalePic());
                    break;
                case -6:
                    WritePng(context, NoPic2Sm());
                    break;
                case -7:
                    WriteJpeg(context, NoMalePicSm());
                    break;
                case -8:
                    WriteJpeg(context, NoFemalePicSm());
                    break;
                default:
                    WriteDbImage(context);
                    break;
            }
        }

        private void WriteDbImage(ControllerContext context)
        {
            ImageData.Image image = null;
            try
            {
                if (GrantPermission(id))
                {
                    using (var db = ImageData.CMSImageDataContext.Create(context.HttpContext))
                    {
                        image = db.Images.SingleOrDefault(ii => ii.Id == id);
                    }
                }
                else
                {
                    (new HttpUnauthorizedResult()).ExecuteResult(context);
                    return;
                }
            }
            catch { }

            if (shouldBePublic && image?.IsPublic == false)
            {
                WritePng(context, NoPic());
            }
            else if (image?.Secure == true)
            {
                if (nodefault)
                {
                    return;
                }

                if (portrait)
                {
                    WriteJpeg(context, tiny ? NoPic1() : NoPic2());
                }
                else
                {
                    WritePng(context, NoPic());
                }
            }
            else
            {
                if (w.HasValue && h.HasValue)
                {
                    var ri = FetchResizedImage(image, w.Value, h.Value, mode);
                    WriteJpeg(context, ri);
                }
                else
                {
                    WriteImage(context, image.Mimetype ?? "image/jpeg", image.Bits);
                }
            }
        }

        private static void WriteJpeg(ControllerContext context, byte[] bytes)
        {
            WriteImage(context, "image/jpeg", bytes);
        }

        private static void WritePng(ControllerContext context, byte[] bytes)
        {
            WriteImage(context, "image/png", bytes);
        }

        private static void WriteImage(ControllerContext context, string mimeType, byte[] bytes)
        {
            context.HttpContext.Response.ContentType = mimeType;
            context.HttpContext.Response.BinaryWrite(bytes);
        }

        private bool GrantPermission(int id)
        {
            var user = HttpContextFactory.Current.User;
            var overrideUser = false;
            if (HttpContextFactory.Current.Request.Cookies.AllKeys.Contains("Authorization")) //For web-based checkin
            {
                var auth = HttpContextFactory.Current.Request.Cookies["Authorization"]?.Value?.Substring(6);
                var authHeader = Encoding.ASCII.GetString(Convert.FromBase64String(auth));
                var tokens = authHeader.Split(new[] { ':' }, 2);
                if (tokens.Length > 1)
                {
                    if (Membership.CMSMembershipProvider.provider.ValidateUser(tokens[0], tokens[1]))
                    {
                        overrideUser = true;
                    }
                }
            }
            using (var cms = CMSDataContext.Create(HttpContextFactory.Current))
            {
                if (shouldBePublic && overrideUser)
                {
                    return true;
                }
                else if (portrait)
                {
                    var secured = !overrideUser && cms.Setting("SecureProfilePictures", true);
                    if (secured && !user.Identity.IsAuthenticated)
                    {
                        return false;
                    }
                    return cms.People.Any(p =>
                       p.Picture.LargeId == id
                    || p.Picture.MediumId == id
                    || p.Picture.SmallId == id
                    || p.Picture.ThumbId == id
                    || p.Family.Picture.LargeId == id
                    || p.Family.Picture.MediumId == id
                    || p.Family.Picture.SmallId == id
                    || p.Family.Picture.ThumbId == id);
                }
                else if (user.Identity.IsAuthenticated)
                {
                    if (preview)
                    {
                        return cms.Contents.Any(m => m.ThumbID == id);
                    }
                    if (cms.MemberDocForms.Any(m => m.LargeId == id || m.MediumId == id || m.SmallId == id)
                        && user.InAnyRole("Membership", "MemberDocs"))
                    {
                        return true;
                    }
                    if (cms.VolunteerForms.Any(m => m.LargeId == id || m.MediumId == id || m.SmallId == id)
                        && user.InAnyRole("ViewVolunteerApplication", "ApplicationReview"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static byte[] GetImageFromCache(string imagename, string path)
        {
            var u = HttpRuntime.Cache[imagename] as byte[];
            if (u == null)
            {
                u = File.ReadAllBytes(HttpContextFactory.Current.Server.MapPath(path));
                HttpRuntime.Cache.Insert(imagename, u, null, DateTime.Now.AddMinutes(100), Cache.NoSlidingExpiration);
            }
            return u;
        }

        private static byte[] NoPic1()
        {
            return GetImageFromCache("unknownimagesm", "/Content/images/unknownsm.jpg");
        }

        private static byte[] NoPic2()
        {
            return GetImageFromCache("unknownimage", "/Content/touchpoint/img/unknown.png");
        }

        private static byte[] NoPic2Sm()
        {
            return GetImageFromCache("unknown_sm", "/Content/touchpoint/img/unknown_sm.png");
        }

        private static byte[] NoMalePic()
        {
            return GetImageFromCache("nomalepic", "/Content/touchpoint/img/male.png");
        }

        private static byte[] NoMalePicSm()
        {
            return GetImageFromCache("nomalepicsm", "/Content/touchpoint/img/male_sm.png");
        }

        private static byte[] NoFemalePic()
        {
            return GetImageFromCache("nofemalepic", "/Content/touchpoint/img/female.png");
        }

        private static byte[] NoFemalePicSm()
        {
            return GetImageFromCache("nofemalepicsm", "/Content/touchpoint/img/female_sm.png");
        }

        private static byte[] NoPic3()
        {
            return GetImageFromCache("sgfimage", "/Content/images/sgfunknown.jpg");
        }

        private static byte[] NoPic()
        {
            return GetImageFromCache("imagenotfound", "/Content/touchpoint/img/image_not_found.png");
        }

        public byte[] FetchResizedImage(ImageData.Image img, int w, int h, string mode = "max")
        {
            return ImageData.Image.ResizeFromBits(img.Bits, w, h, mode);
        }
    }
}
