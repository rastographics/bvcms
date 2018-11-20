using CmsData;
using CmsWeb.Lifecycle;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin, Manager2")]
    [RouteArea("Manage", AreaPrefix = "Duplicates"), Route("{action}")]
    public class DuplicatesController : CmsStaffController
    {
        public DuplicatesController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public class DuplicateInfo
        {
            public Duplicate d { get; set; }
            public string name { get; set; }
            public bool samefamily { get; set; }
            public bool notdup { get; set; }
        }
        [Route("~/Duplicates")]
        public ActionResult Index()
        {
            var q = from d in CurrentDatabase.Duplicates
                    where CurrentDatabase.People.Any(pp => pp.PeopleId == d.Id1)
                    where CurrentDatabase.People.Any(pp => pp.PeopleId == d.Id2)
                    let name = CurrentDatabase.People.SingleOrDefault(pp => pp.PeopleId == d.Id1).Name
                    let notdup = CurrentDatabase.PeopleExtras.Any(ee => ee.Field == "notdup" && ee.PeopleId == d.Id1 && ee.IntValue == d.Id2)
                    let f1 = CurrentDatabase.People.Single(pp => pp.PeopleId == d.Id1)
                    let f2 = CurrentDatabase.People.Single(pp => pp.PeopleId == d.Id2)
                    let samefamily = f1.FamilyId == f2.FamilyId
                    orderby d.Id1
                    select new DuplicateInfo { d = d, name = name, samefamily = samefamily, notdup = notdup };
            return View(q);
        }

        [HttpGet]
        public ActionResult Find()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Find(string fromDate, string toDate)
        {
            var fdt = fromDate.ToDate();
            var tdt = toDate.ToDate();
            string host = Util.Host;
            var runningtotals = new DuplicatesRun
            {
                Started = DateTime.Now,
                Count = 0,
                Processed = 0,
                Found = 0
            };
            CurrentDatabase.DuplicatesRuns.InsertOnSubmit(runningtotals);
            CurrentDatabase.SubmitChanges();

            HostingEnvironment.QueueBackgroundWorkItem(ct =>
            {
                var db = DbUtil.Create(host);
                var rt = db.DuplicatesRuns.OrderByDescending(mm => mm.Id).First();
                db.ExecuteCommand("delete duplicate");
                var q = from p in db.People
                        where p.CreatedDate > fdt
                        where p.CreatedDate < tdt.Value.AddDays(1)
                        select p.PeopleId;
                rt.Count = q.Count();
                db.SubmitChanges();
                foreach (var p in q)
                {
                    var pids = db.FindPerson4(p);
                    rt.Processed++;
                    db.SubmitChanges();
                    if (!pids.Any())
                    {
                        continue;
                    }

                    foreach (var pid in pids)
                    {
                        if (pid.PeopleId != null)
                        {
                            db.InsertDuplicate(p, pid.PeopleId.Value);
                        }
                    }

                    rt.Found++;
                }
                rt.Completed = DateTime.Now;
                db.SubmitChanges();
            });
            return Redirect("/Duplicates/Progress");
        }
        [HttpGet]
        public ActionResult Progress()
        {
            var rt = CurrentDatabase.DuplicatesRuns.OrderByDescending(mm => mm.Id).First();
            return View(rt);
        }
        [HttpPost]
        public JsonResult Progress2()
        {
            var r = CurrentDatabase.DuplicatesRuns.OrderByDescending(mm => mm.Id).First();
            return Json(new { r.Count, r.Error, r.Processed, r.Found, Completed = r.Completed.ToString(), r.Running });
        }
    }
}
