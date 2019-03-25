using CmsData;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Elmah;
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
    [RouteArea("Manage", AreaPrefix = "UploadPeople")]
    [Route("{action=index}")]
    public class UploadPeopleController : CmsStaffController
    {
        public UploadPeopleController(IRequestManager requestManager) : base(requestManager)
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
            var host = Util.Host;
            var pid = Util.UserPeopleId;

            var package = new ExcelPackage(file.InputStream);

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                try
                {
                    using (var testdb = DbUtil.Create(host))
                    {
                        var testrun = ProcessImport(testdb, noupdate, host, pid, package, true);
                    }

                    using (var realdb = DbUtil.Create(host))
                    {
                        var realrun = ProcessImport(realdb, noupdate, host, pid, package, false);
                    }
                }
                catch (Exception ex)
                {
                    var db = DbUtil.Create(host);

                    var q = from r in db.UploadPeopleRuns
                            where r.Id == db.UploadPeopleRuns.Max(rr => rr.Id)
                            select r;

                    var rt = q.Single();
                    rt.Error = ex.Message.Truncate(200);

                    db.SubmitChanges();

                    ErrorLog.GetDefault(null).Log(new Error(ex));
                }
            });

            return Redirect("/UploadExcelIps/Progress");
        }

        private UploadPeopleRun ProcessImport(CMSDataContext db, bool noupdate, string host, int? pid, ExcelPackage package, bool testing)
        {
            var rt = new UploadPeopleRun { Started = DateTime.Now, Count = 0, Processed = 0 };
            db.UploadPeopleRuns.InsertOnSubmit(rt);
            db.SubmitChanges();

            var upload = new UploadPeopleModel(db, host, pid ?? 0, noupdate, testing);
            upload.DoUpload(package);

            return rt;
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
