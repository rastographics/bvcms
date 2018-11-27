using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using MoreLinq;
using System;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Public
{
    [RouteArea("Public", AreaPrefix = "OrgContent"), Route("{action}/{id?}")]
    public class OrgContentController : CmsController
    {
        public OrgContentController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/OrgContent/{id:int}/{pid:int?}")]
        public ActionResult Index(int id, int? pid)
        {
            var o = OrgContentInfo.Get(id);
            if (o == null)
            {
                return Content("<h2>Not an Organization</h2>");
            }

            if (!Util.UserPeopleId.HasValue)
            {
                return Redirect("/OrgContent/Login/" + id);
            }

            if (o.TryRunPython(pid ?? Util.UserPeopleId.Value))
            {
                return View("ScriptResults", o);
            }

            var org = CurrentDatabase.LoadOrganizationById(o.OrgId);

            // Try to load a template specific to this org type
            var template = CurrentDatabase.ContentHtml($"OrgContent-{org.OrganizationType?.Description}", null);

            // Try to fall back on a standard template
            if (template == null)
            {
                template = CurrentDatabase.ContentHtml("OrgContent", null);
            }

            if (template != null)
            {
                template = template.Replace("{content}", o.Html ?? string.Empty)
                    .Replace("{location}", org.Location ?? string.Empty)
                    .Replace("{type}", org.OrganizationType?.Description ?? string.Empty)
                    .Replace("{division}", org.Division?.Name ?? string.Empty)
                    .Replace("{campus}", org.Campu?.Description ?? string.Empty)
                    .Replace("{orgid}", org.OrganizationId.ToString())
                    .Replace("{name}", org.OrganizationName ?? string.Empty);

                org.GetOrganizationExtras()
                    .ForEach(
                        ev =>
                        {
                            template = template.Replace($"{{{ev.Field}}}", ev.Data ?? ev.StrValue ?? string.Empty);
                        });

                if (template.Contains("{directory}"))
                {
                    ViewBag.qid = CurrentDatabase.NewOrgFilter(id).QueryId;
                }

                ViewBag.template = template;
                return View(o);
            }

            return View(o);
        }

        public ActionResult Edit(int id)
        {
            var o = OrgContentInfo.Get(id);
            if (o == null || o.Inactive || !Util.UserPeopleId.HasValue || !o.CanEdit)
            {
                return Redirect("/OrgContent/" + id);
            }

            return View(o);
        }

        [HttpPost]
        public ActionResult CKEditorUpload(int id, string CKEditorFuncNum)
        {
            var error = "";
            var file = Request.Files[0];
            var bits = new byte[file.ContentLength];
            file.InputStream.Read(bits, 0, bits.Length);
            var mimetype = file.ContentType.ToLower();
            var oc = new OrgContent { OrgId = id };
            switch (mimetype)
            {
                case "image/jpeg":
                case "image/png":
                case "image/pjpeg":
                case "image/gif":
                case "text/plain":
                case "application/pdf":
                case "application/msword":
                case "application/vnd.ms-excel":
                    try
                    {
                        oc.ImageId = ImageData.Image.NewImageFromBits(bits, mimetype).Id;
                        CurrentDatabase.OrgContents.InsertOnSubmit(oc);
                        CurrentDatabase.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        error = ex.Message;
                    }
                    break;
                default:
                    error = "invalid mime type " + mimetype;
                    break;
            }
            return Content(string.Format(
                "<script type='text/javascript'>window.parent.CKEDITOR.tools.callFunction( {0}, '/OrgContent/Display/{1}', '{2}' );</script>",
                CKEditorFuncNum, oc.Id, error));
        }

        public FileResult Display(int id)
        {
            var o = OrgContentInfo.GetOc(id);
            var image = o.image;
            return File(image.Bits, image.Mimetype);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Update(int id, string Html)
        {
            var o = OrgContentInfo.Get(id);
            o.Html = Html;
            ImageData.DbUtil.Db.SubmitChanges();
            return Redirect("/OrgContent/" + id);
        }
    }
}
