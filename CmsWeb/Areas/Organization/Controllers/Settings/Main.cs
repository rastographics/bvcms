using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org2.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org2.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult Main(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/Main", m.OrgMain);
        }
        [HttpPost]
        public ActionResult MainEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/MainEdit", m.OrgMain);
        }
        [HttpPost]
        public ActionResult MainUpdate(OrgMain m)
        {
            m.Update();
            DbUtil.LogActivity("Update OrgMain {0}".Fmt(m.OrganizationName));
            return PartialView("Settings/Main", m);
        }

        [HttpPost]
        public ActionResult DivisionList(int id)
        {
            return View("Other/DivisionList", OrganizationModel.Divisions(id));
        }
    }
}