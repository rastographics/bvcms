using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.View;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult Main(int id)
        {
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            m.OrgId = id;
            m.OrgMain.Divisions = GetOrgDivisions(id);
            return PartialView("Settings/Main", m.OrgMain);
        }

        [HttpPost]
        public ActionResult MainEdit(int id)
        {
            var m = OrganizationModel.Create(CurrentDatabase, CurrentUser);
            m.OrgId = id;
            m.OrgMain.Divisions = GetOrgDivisions(id);
            return PartialView("Settings/MainEdit", m.OrgMain);
        }

        [HttpPost]
        public ActionResult MainUpdate(OrgMain m)
        {
            if (!m.OrganizationName.HasValue())
                m.OrganizationName = $"Organization needs a name ({Util.UserFullName})";
            m.Update();
            m.Divisions = GetOrgDivisions(m.Org.OrganizationId);
            DbUtil.LogActivity($"Update OrgMain {m.OrganizationName}");
            return PartialView("Settings/Main", m);
        }

        [HttpPost]
        public ActionResult DivisionList(int id)
        {
            return PartialView("Other/DivisionList", GetOrgDivisions(id));
        }

        private IEnumerable<SearchDivision> GetOrgDivisions(int? id)
        {
            var q = from d in CurrentDatabase.SearchDivisions(id, null)
                    where d.IsChecked == true
                    orderby d.IsMain descending, d.IsChecked descending, d.Program, d.Division
                    select d;
            return q.AsEnumerable();
        }
    }
}
