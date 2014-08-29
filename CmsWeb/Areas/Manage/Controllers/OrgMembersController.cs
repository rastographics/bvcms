using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

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
        public ActionResult Move()
        {
            var m = new OrgMembersModel();
            UpdateModel(m);
            m.Move();
            return View("List", m);
        }
        [HttpPost]
        public ActionResult EmailNotices()
        {
            var m = new OrgMembersModel();
            UpdateModel(m);
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
        public ActionResult List()
        {
            var m = new OrgMembersModel();
            UpdateModel(m);
            m.ValidateIds();
            DbUtil.Db.SetUserPreference("OrgMembersModelIds", "{0}.{1}.{2}".Fmt(m.ProgId,m.DivId,m.SourceId));
            DbUtil.DbDispose();
            DbUtil.Db.SetNoLock();
            return View(m);
        }
        [HttpPost]
        public ActionResult ResetMoved()
        {
            var m = new OrgMembersModel();
            UpdateModel(m);
            m.ResetMoved();
            return View("List", m);
        }
    }
}
