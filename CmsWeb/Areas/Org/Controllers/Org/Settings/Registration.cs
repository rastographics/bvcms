using System;
using System.Linq;
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
        public ActionResult Registration(int id)
        {
            var m = new SettingsRegistrationModel(id, CurrentDatabase);
            return PartialView("Registration/Registration", m);
        }

        [HttpPost]
        public ActionResult RegistrationHelpToggle(int id)
        {
            CurrentDatabase.ToggleUserPreference("ShowRegistrationHelp");
            var m = new SettingsRegistrationModel(id, CurrentDatabase);
            return PartialView("Registration/Registration", m);
        }

        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult RegistrationEdit(int id)
        {
            var m = new SettingsRegistrationModel(id, CurrentDatabase);
            return PartialView("Registration/RegistrationEdit", m);
        }

        [HttpPost]
        public ActionResult RegistrationUpdate(SettingsRegistrationModel m)
        {
            DbUtil.LogActivity($"Update Registration {m.Org.OrganizationName}");
            try
            {
                m.Update();
                if (!m.Org.NotifyIds.HasValue() && m.Org.RegistrationTypeId > 0)
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("Registration/Registration", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("Registration/RegistrationEdit", m);
            }
        }

        [HttpPost]
        public ActionResult NewAgeGroup()
        {
            return PartialView("EditorTemplates/AgeGroup", new Settings.AgeGroup());
        }

        public ActionResult OrgPickList(int id)
        {
            if (Util.SessionTimedOut())
                return Content("<script type='text/javascript'>window.onload = function() { parent.location = '/'; }</script>");
            Response.NoCache();
            CurrentDatabase.SetCurrentOrgId(id);
            var o = CurrentDatabase.LoadOrganizationById(id);
            Session["orgPickList"] = (o.OrgPickList ?? "").Split(',').Select(oo => oo.ToInt()).ToList();
            return Redirect("/SearchOrgs/" + id);
        }

        [HttpPost]
        public ActionResult UpdateOrgIds(int id, string list)
        {
            CurrentDatabase.SetCurrentOrgId(id);
            var m = new SettingsRegistrationModel(id, CurrentDatabase);
            m.Org.OrgPickList = list;
            CurrentDatabase.SubmitChanges();
            return PartialView("DisplayTemplates/OrgPickList", m);
        }
    }
}
