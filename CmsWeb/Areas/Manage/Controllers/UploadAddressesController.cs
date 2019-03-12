using CmsData;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    public partial class BatchController
    {
        [HttpGet]
        public ActionResult UploadAddresses()
        {
            ViewData["text"] = "";
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UploadAddresses(string text)
        {
            string host = Util.Host;
            var pid = Util.UserPeopleId;
            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var db = DbUtil.Create(host);
                try
                {
                    var runningtotals = new UploadPeopleRun { Started = DateTime.Now, Count = 0, Processed = 0 };
                    db.UploadPeopleRuns.InsertOnSubmit(runningtotals);
                    db.SubmitChanges();

                    var m = new UploadAddressesModel(db, pid ?? 0);
                    m.DoUpload(text);
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
            return Redirect("/Batch/UploadAddressesProgress");
        }

        [HttpGet]
        public ActionResult UploadAddressesProgress()
        {
            var rt = CurrentDatabase.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            return View(rt);
        }

        [HttpPost]
        public JsonResult UploadAddressesProgress2()
        {
            var r = CurrentDatabase.UploadPeopleRuns.OrderByDescending(mm => mm.Id).First();
            return Json(new { r.Count, r.Error, r.Processed, Completed = r.Completed.ToString(), r.Running });
        }

    }
}

