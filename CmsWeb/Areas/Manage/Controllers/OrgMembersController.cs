using CmsWeb.Lifecycle;
using CmsWeb.Models;
using System.Web.Mvc;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles = "ManageOrgMembers")]
    [RouteArea("Manage", AreaPrefix = "OrgMembers"), Route("{action=index}/{id?}")]
    public class OrgMembersController : CmsStaffController
    {
        public OrgMembersController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            var m = new OrgMembersModel(CurrentDatabase);
            m.FetchSavedIds();
            return View(m);
        }

        [HttpPost]
        public ActionResult EmailNotices(OrgMembersModel m)
        {
            m.CurrentDatabase = CurrentDatabase;
            m.SendMovedNotices();
            return View("List", m);
        }

        public ActionResult GradeList(int id)
        {
            var m = new OrgMembersModel(CurrentDatabase);
            UpdateModel(m);
            return m.ToExcel(id);
        }

        [HttpPost]
        public ActionResult List(OrgMembersModel m)
        {
            m.CurrentDatabase = CurrentDatabase;
            m.ValidateIds();
            CurrentDatabase.SetUserPreference("OrgMembersModelIds", $"{m.ProgId}.{m.SourceDivId}.{m.SourceId}");
            CurrentDatabase.SetNoLock();
            return View(m);
        }
        [HttpPost]
        public ActionResult ResetMoved(OrgMembersModel m)
        {
            m.CurrentDatabase = CurrentDatabase;
            m.ResetMoved();
            return View("List", m);
        }
    }
}
