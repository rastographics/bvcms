using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
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
            if (!m.OrganizationName.HasValue())
                m.OrganizationName = $"Organization needs a name ({Util.UserFullName})";
            m.Update();
            DbUtil.LogActivity($"Update OrgMain {m.OrganizationName}");
            return PartialView("Settings/Main", m);
        }

        [HttpPost]
        public ActionResult DivisionList(int id)
        {
            return View("Other/DivisionList", OrganizationModel.Divisions(id));
        }
    }
}
