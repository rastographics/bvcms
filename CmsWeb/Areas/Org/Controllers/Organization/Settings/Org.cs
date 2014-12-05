using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult SettingsOrg(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/Org", m);
        }
        [HttpPost]
        public ActionResult SettingsOrgEdit(OrganizationModel m)
        {
            return PartialView("Settings/OrgEdit",  m);
        }
        [HttpPost]
        public ActionResult SettingsOrgUpdate(OrganizationModel m)
        {
            if (!m.Org.LimitToRole.HasValue())
                m.Org.LimitToRole = null;
            DbUtil.LogActivity("Update SettingsOrg {0}".Fmt(m.Org.OrganizationName));
            if (ModelState.IsValid)
            {
                m.UpdateSchedules();
                DbUtil.Db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, m.Org.OrgSchedules);
                return View("Settings/Org", m);
            }
            return PartialView("Settings/OrgEdit", m);
        }
    }
}