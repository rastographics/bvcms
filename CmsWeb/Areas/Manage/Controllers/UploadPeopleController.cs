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
            CurrentDatabase.UploadPeopleRuns.InsertOnSubmit(runningtotals);
            CurrentDatabase.SubmitChanges();
            var pid = Util.UserPeopleId;

            var package = new ExcelPackage(file.InputStream);

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var Db = DbUtil.Create(host);
                try
                {
                    var m = new UploadPeopleModel(host, pid ?? 0, noupdate, testing: true);
                    m.DoUpload(package);
                    CurrentDatabase.Dispose();
                    Db = DbUtil.Create(host);

                    runningtotals = new UploadPeopleRun { Started = DateTime.Now, Count = 0, Processed = 0 };
                    CurrentDatabase.UploadPeopleRuns.InsertOnSubmit(runningtotals);
                    CurrentDatabase.SubmitChanges();

                    m = new UploadPeopleModel(host, pid ?? 0, noupdate);
                    m.DoUpload(package);
                }
                catch (Exception ex)
                {
                    CurrentDatabase.Dispose();
                    Db = DbUtil.Create(host);

                    var q = from r in CurrentDatabase.UploadPeopleRuns
                            where r.Id == CurrentDatabase.UploadPeopleRuns.Max(rr => rr.Id)
                            select r;
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(ex));
                    var rt = q.Single();
                    rt.Error = ex.Message.Truncate(200);
                    CurrentDatabase.SubmitChanges();
                }
            });
            return Redirect("/UploadPeople/Progress");
        }

        [HttpGet]
        public ActionResult Progress()
        {
            var rt = CurrentDatabase.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            return View(rt);
        }

        [HttpPost]
        public JsonResult Progress2()
        {
            var r = CurrentDatabase.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            return Json(new { r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running });
        }

    }
}

