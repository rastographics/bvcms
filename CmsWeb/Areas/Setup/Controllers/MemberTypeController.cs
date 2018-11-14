using CmsData;
using CmsWeb.Lifecycle;
using Dapper;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Setup.Controllers
{
    [Authorize(Roles = "Admin")]
    [RouteArea("Setup", AreaPrefix = "MemberType"), Route("{action=index}/{id?}")]
    public class MemberTypeController : CmsStaffController
    {
        public MemberTypeController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Index()
        {
            var q = from mt in CurrentDatabase.MemberTypes
                    select new MemberTypeInfo
                    {
                        Id = mt.Id,
                        Code = mt.Code,
                        Description = mt.Description,
                        AttendType = mt.AttendType.Description,
                        AttendTypeId = mt.AttendanceTypeId,
                        Hardwired = mt.Hardwired
                    };
            return View(q);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create(int? id)
        {
            if (!id.HasValue)
            {
                return Content("need an integer id");
            }

            if (!CurrentDatabase.MemberTypes.Any(mt => mt.Id == id))
            {
                var m = new MemberType { Id = id.Value };
                CurrentDatabase.MemberTypes.InsertOnSubmit(m);
                CurrentDatabase.SubmitChanges();
            }

            return Redirect($"/MemberType/#{id}");
        }

        [HttpPost]
        public ActionResult Move(int fromid, int toid)
        {
            CurrentDatabase.ExecuteCommand("UPDATE dbo.OrganizationMembers SET MemberTypeId = {0} WHERE MemberTypeId = {1}", toid, fromid);
            CurrentDatabase.ExecuteCommand("UPDATE dbo.EnrollmentTransaction SET MemberTypeId = {0} WHERE MemberTypeId = {1}", toid, fromid);
            CurrentDatabase.ExecuteCommand("UPDATE dbo.Attend SET MemberTypeId = {0} WHERE MemberTypeId = {1}", toid, fromid);

            TempData["SuccessMessage"] = "Member type and associated members were successfully migrated.";
            return Redirect("/MemberType/");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult Edit(string id, string value)
        {
            var iid = id.Substring(1).ToInt();
            var mt = CurrentDatabase.MemberTypes.SingleOrDefault(m => m.Id == iid);
            if (id.StartsWith("v"))
            {
                mt.Description = value;
            }
            else if (id.StartsWith("c"))
            {
                mt.Code = value;
            }

            CurrentDatabase.SubmitChanges();
            var c = new ContentResult();
            c.Content = value;
            return c;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ContentResult EditAttendType(string id, string value)
        {
            var iid = id.Substring(1).ToInt();
            var mt = CurrentDatabase.MemberTypes.SingleOrDefault(m => m.Id == iid);
            mt.AttendanceTypeId = value.ToInt();
            CurrentDatabase.SubmitChanges();
            return Content(mt.AttendType.Description);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete(string id)
        {
            var iid = id.Substring(1).ToInt();
            var mt = CurrentDatabase.MemberTypes.SingleOrDefault(m => m.Id == iid);
            if (mt == null)
            {
                return new EmptyResult();
            }

            var IsUsed = (from m in CurrentDatabase.MemberTypes
                          where m.Id == mt.Id
                          let mta = m.Attends.Any()
                          let mto = m.OrganizationMembers.Any()
                          let mte = m.EnrollmentTransactions.Any()
                          select (mta || mto || mte)).SingleOrDefault();
            if (IsUsed)
            {
                return Content("used");
            }

            CurrentDatabase.MemberTypes.DeleteOnSubmit(mt);
            CurrentDatabase.SubmitChanges();
            return Content("done");
        }

        public JsonResult AttendTypeCodes()
        {
            var q = from c in CurrentDatabase.AttendTypes
                    select new
                    {
                        value = c.Id.ToString(),
                        text = c.Description
                    };

            return Json(q.ToList(), JsonRequestBehavior.AllowGet);
        }

        public class MemberTypeInfo
        {
            public int Id { get; set; }
            public string Code { get; set; }
            public string Description { get; set; }
            public string AttendType { get; set; }
            public int? AttendTypeId { get; set; }
            public bool? Hardwired { get; set; }
        }
        [HttpGet, Route("~/MemberTypeCodes")]
        public ActionResult Codes()
        {
            var sql = "SELECT Id, Description FROM lookup.MemberType";
            var q = CurrentDatabase.Connection.Query(sql);
            var sb = new StringBuilder();
            foreach (var r in q)
            {
                sb.AppendLine($"{r.Id}[{r.Description}],");
            }

            return Content(sb.ToString(), "text/plain");
        }
    }
}
