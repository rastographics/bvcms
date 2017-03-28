using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Hosting;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData;
using OfficeOpenXml;

namespace CmsWeb.Areas.Manage.Controllers
{
	[Authorize(Roles = "Admin")]
    [RouteArea("Manage", AreaPrefix= "UploadExcelIps"), Route("{action=index}")]
	public class UploadExcelIpsController : CmsStaffController
	{
		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Index(HttpPostedFileBase file, bool noupdate)
		{
			string host = Util.Host;
			var runningtotals = new UploadPeopleRun { Started = DateTime.Now, Count = 0, Processed = 0 };
			DbUtil.Db.UploadPeopleRuns.InsertOnSubmit(runningtotals);
			DbUtil.Db.SubmitChanges();
			var pid = Util.UserPeopleId;

		    var package = new ExcelPackage(file.InputStream);

            HostingEnvironment.QueueBackgroundWorkItem(ct => 
			{
				var db = DbUtil.Create(host);
				try
				{
					var m = new UploadExcelIpsModel(host, pid ?? 0, noupdate, testing: true);
					m.DoUpload(package);
					db.Dispose();
    				db = DbUtil.Create(host);

        			runningtotals = new UploadPeopleRun { Started = DateTime.Now, Count = 0, Processed = 0 };
        			db.UploadPeopleRuns.InsertOnSubmit(runningtotals);
        			db.SubmitChanges();

					m = new UploadExcelIpsModel(host, pid ?? 0, noupdate);
					m.DoUpload(package);
				}
				catch (Exception ex)
				{
					db.Dispose();
    				db = DbUtil.Create(host);

				    var q = from r in db.UploadPeopleRuns
				            where r.Id == db.UploadPeopleRuns.Max(rr => rr.Id)
				            select r;
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
				    var rt = q.Single();
					rt.Error = ex.Message.Truncate(200);
        			db.SubmitChanges();
				}
			});
			return Redirect("/UploadExcelIps/Progress");
		}

		[HttpGet]
		public ActionResult Progress()
		{
			var rt = DbUtil.Db.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
			return View(rt);
		}

		[HttpPost]
		public JsonResult Progress2()
		{
			var r = DbUtil.Db.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
			return Json(new {r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running});
		}

	}
}

