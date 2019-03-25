using CmsData;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using System.Web.Mvc;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "OrgChildren"), Route("{action}")]
    public class OrgChildrenController : CmsStaffController
    {
        public OrgChildrenController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
            var o = CurrentDatabase.LoadOrganizationById(ChildOrg);
            if (Checked)
            {
                o.ParentOrgId = ParentOrg;
            }
            else
            {
                o.ParentOrgId = null;
            }

            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }
    }
}
