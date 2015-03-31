using System;
using System.Linq;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org2.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.Org2.Controllers
{
    public partial class Org2Controller
    {
        [HttpPost]
        public ActionResult Registration(int id)
        {
            var m = new SettingsRegistrationModel(id);
            return PartialView("Registration/Registration", m);
        }
        [HttpPost]
        public ActionResult RegistrationHelpToggle(int id)
        {
            DbUtil.Db.ToggleUserPreference("ShowRegistrationHelp");
            var m = new SettingsRegistrationModel(id);
            return PartialView("Registration/Registration", m);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult RegistrationEdit(int id)
        {
            var m = new SettingsRegistrationModel(id);
            return PartialView("Registration/RegistrationEdit", m);
        }
        [HttpPost]
        public ActionResult RegistrationUpdate(SettingsRegistrationModel m)
        {
            DbUtil.LogActivity("Update Registration {0}".Fmt(m.Org.OrganizationName));
            try
            {
                m.Update();
                if (!m.Org.NotifyIds.HasValue())
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
            DbUtil.Db.SetCurrentOrgId(id);
            var o = DbUtil.Db.LoadOrganizationById(id);
            Session["orgPickList"] = (o.OrgPickList ?? "").Split(',').Select(oo => oo.ToInt()).ToList();
            return Redirect("/SearchOrgs/" + id);
        }
        [HttpPost]
        public ActionResult UpdateOrgIds(int id, string list)
        {
            DbUtil.Db.SetCurrentOrgId(id);
            var m = new SettingsRegistrationModel(id);
            m.Org.OrgPickList = list;
            DbUtil.Db.SubmitChanges();
            return PartialView("DisplayTemplates/OrgPickList", m);
        }
        private static Settings getRegSettings(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var m = new Settings(org.RegSetting, DbUtil.Db, id);
            return m;
        }
    }
}