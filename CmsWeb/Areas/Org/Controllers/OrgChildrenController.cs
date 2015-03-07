using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix="OrgChildren"), Route("{action}")]
    public class OrgChildrenController : CmsStaffController
    {
        [Route("~/OrgChildren/{id:int}")]
        public ActionResult Index(int id)
        {
            var m = new OrgChildrenModel { orgid = id };
            return View(m);
        }
        [HttpPost]
        public ActionResult Filter(OrgChildrenModel m)
        {
            return View("Rows", m);
        }
        [HttpPost]
        public ActionResult UpdateOrg(int ParentOrg, int ChildOrg, bool Checked)
        {
            var o = DbUtil.Db.LoadOrganizationById(ChildOrg);
            if (Checked)
                o.ParentOrgId = ParentOrg;
            else
                o.ParentOrgId = null;
            DbUtil.Db.SubmitChanges();
            return Content("ok");
        }
    }
}
