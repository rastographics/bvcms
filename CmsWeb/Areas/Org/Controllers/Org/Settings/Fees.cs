using System;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult Fees(int id)
        {
            var m = new SettingsFeesModel(id);
            return PartialView("Registration/Fees", m);
        }

        [HttpPost]
        public ActionResult FeesHelpToggle(int id)
        {
            CurrentDatabase.ToggleUserPreference("ShowFeesHelp");
            var m = new SettingsFeesModel(id);
            return PartialView("Registration/Fees", m);
        }

        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult FeesEdit(int id)
        {
            var m = new SettingsFeesModel(id);
            return PartialView("Registration/FeesEdit", m);
        }

        [HttpPost]
        public ActionResult FeesUpdate(SettingsFeesModel m)
        {
            if (!ModelState.IsValid)
                return PartialView("Registration/FeesEdit", m);
            DbUtil.LogActivity($"Update Fees {m.Org.OrganizationName}");
            try
            {
                m.Update();
                if (!m.Org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("Registration/Fees", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("Registration/FeesEdit", m);
            }
        }

        [HttpPost]
        public ActionResult NewOrgFee(string id)
        {
            return PartialView("EditorTemplates/OrgFee", new Settings.OrgFee());
        }
    }
}
