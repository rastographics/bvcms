using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using OfficeOpenXml;
using System;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Developer,UploadPeople")]
    [RouteArea("Manage", AreaPrefix = "UploadPeople"), Route("{action=index}")]
    public class UploadPeopleController : CmsStaffController
    {
        public UploadPeopleController(RequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            ViewData["text"] = "";
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
                var Db = DbUtil.Create(host);
                try
                {
                    var m = new UploadPeopleModel(host, pid ?? 0, noupdate, testing: true);
                    m.DoUpload(package);
                    Db.Dispose();
                    Db = DbUtil.Create(host);

                    runningtotals = new UploadPeopleRun { Started = DateTime.Now, Count = 0, Processed = 0 };
                    Db.UploadPeopleRuns.InsertOnSubmit(runningtotals);
                    Db.SubmitChanges();

                    m = new UploadPeopleModel(host, pid ?? 0, noupdate);
                    m.DoUpload(package);
                }
                catch (Exception ex)
                {
                    Db.Dispose();
                    Db = DbUtil.Create(host);

                    var q = from r in Db.UploadPeopleRuns
                            where r.Id == Db.UploadPeopleRuns.Max(rr => rr.Id)
                            select r;
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    var rt = q.Single();
                    rt.Error = ex.Message.Truncate(200);
                    Db.SubmitChanges();
                }
            });
            return Redirect("/UploadPeople/Progress");
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
            return Json(new { r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running });
        }

    }
}

