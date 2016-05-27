using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.Manage.Controllers
{
    [Authorize(Roles="Edit")]
    [RouteArea("Manage", AreaPrefix= "OrgMembers"), Route("{action=index}/{id?}")]
    public class OrgMembersController : CmsStaffController
    {
        [HttpGet]
        public ActionResult Index()
        {
            var m = new OrgMembersModel();
            m.FetchSavedIds();
            return View(m);
        }

        [HttpPost]
        public ActionResult Move(OrgMembersModel m)
        {
            if (m.TargetId == 0)
                return Content("!Target required");
            m.Move();
            return View("List", m);
        }
        [HttpPost]
        public ActionResult EmailNotices(OrgMembersModel m)
        {
            m.SendMovedNotices();
            return View("List", m);
        }
        public ActionResult GradeList(int id)
        {
            var m = new OrgMembersModel();
            UpdateModel(m);
            return m.ToExcel(id);
        }

        [HttpPost]
        public ActionResult List(OrgMembersModel m)
        {
            m.ValidateIds();
            DbUtil.Db.SetUserPreference("OrgMembersModelIds", $"{m.ProgId}.{m.SourceDivId}.{m.SourceId}");
            DbUtil.DbDispose();
            DbUtil.Db.SetNoLock();
            return View(m);
        }
        [HttpPost]
        public ActionResult ResetMoved(OrgMembersModel m)
        {
            m.ResetMoved();
            return View("List", m);
        }
    }
}
