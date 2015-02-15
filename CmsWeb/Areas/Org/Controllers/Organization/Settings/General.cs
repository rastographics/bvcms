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
        public ActionResult General(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/General", m);
        }
        [HttpPost]
        public ActionResult GeneralEdit(OrganizationModel m)
        {
            return PartialView("Settings/GeneralEdit",  m);
        }
        [HttpPost]
        public ActionResult GeneralUpdate(OrganizationModel m)
        {
            if (!m.Org.LimitToRole.HasValue())
                m.Org.LimitToRole = null;
            DbUtil.LogActivity("Update SettingsGeneral {0}".Fmt(m.Org.OrganizationName));
            if (ModelState.IsValid)
            {
                //m.UpdateSchedules();
                //DbUtil.Db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, m.Org.OrgSchedules);
                return View("Settings/General", m);
            }
            return PartialView("Settings/GeneralEdit", m);
        }
    }
}