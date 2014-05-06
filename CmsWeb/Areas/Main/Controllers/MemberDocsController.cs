using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Main.Controllers
{
	[Authorize(Roles="Admin,Membership")]
    [RouteArea("Main", AreaPrefix="MemberDocs")]
	public class MemberDocsController : CmsController
	{
        [Route("~/MemberDocs/{id:int}")]
		public ActionResult Index(int id)
		{
			var m = new MemberDocs { PeopleId = id };
			return View(m);
		}
		[HttpPost]
		public ActionResult Delete(int id)
		{
			var m = DbUtil.Db.MemberDocForms.SingleOrDefault(mm => mm.Id == id);
			ImageData.Image.DeleteOnSubmit(m.SmallId);
			ImageData.Image.DeleteOnSubmit(m.MediumId);
			ImageData.Image.DeleteOnSubmit(m.LargeId);

			DbUtil.Db.MemberDocForms.DeleteOnSubmit(m);
			DbUtil.Db.SubmitChanges();
			return Content("/MemberDocs/" + m.PeopleId);
		}
		[HttpGet]
        [Route("~/MemberDocs/Image/{id:int}")]
		public ActionResult Image(int id, string size)
		{
			var mdf = DbUtil.Db.MemberDocForms.SingleOrDefault(ff => ff.Id == id);
			return View(mdf);
		}
		[HttpPost]
		public ActionResult Upload(int id, HttpPostedFileBase file)
		{
			var m = new MemberDocs { PeopleId = id };
		    try
		    {
    			var mdf = new MemberDocForm 
    			{ 
    				PeopleId = id, 
    				DocDate = Util.Now,
    				UploaderId = Util2.CurrentPeopleId,
                    Name = System.IO.Path.GetFileName(file.FileName).Truncate(100)
    			};
                DbUtil.Db.MemberDocForms.InsertOnSubmit(mdf);
                var bits = new byte[file.ContentLength];
                file.InputStream.Read(bits, 0, bits.Length);
                var mimetype = file.ContentType.ToLower();
                switch (mimetype)
                {
                    case "image/jpeg":
                    case "image/pjpeg":
                    case "image/gif":
                    case "image/png":
                        mdf.IsDocument = false;
                            mdf.SmallId = ImageData.Image.NewImageFromBits(bits, 165, 220).Id;
                            mdf.MediumId = ImageData.Image.NewImageFromBits(bits, 675, 900).Id;
                            mdf.LargeId = ImageData.Image.NewImageFromBits(bits).Id;
                        break;
                    case "text/plain":
                    case "application/pdf":
                    case "application/msword":
                    case "application/vnd.ms-excel":
                        mdf.MediumId = ImageData.Image.NewImageFromBits(bits, mimetype).Id;
                        mdf.SmallId = mdf.MediumId;
                        mdf.LargeId = mdf.MediumId;
                        mdf.IsDocument = true;
                        break;
                    default:
    					throw new FormatException("file type not supported: " + mimetype);
                }
                DbUtil.Db.SubmitChanges();
                DbUtil.LogActivity("Uploading MemberDoc for {0}".Fmt(mdf.Person.Name));
		    }
		    catch (Exception ex)
		    {
                ModelState.AddModelError("ImageFile", ex.Message);
    			return View("Index", m);
		    }
			return View("Index", m);
		}
        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Edit(string id, string value)
        {
            var iid = id.Substring(2).ToInt();
            var f = DbUtil.Db.MemberDocForms.Single(m => m.Id == iid);
            if (id.StartsWith("n"))
                f.Name = value.Truncate(100);
            DbUtil.Db.SubmitChanges();
            var c = new ContentResult();
            c.Content = value;
            return c;
        }
	}
}
