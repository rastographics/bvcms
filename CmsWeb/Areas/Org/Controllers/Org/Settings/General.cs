using System;
using System.Collections.Specialized;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using eSpace;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult General(int id)
        {
            var m = new SettingsGeneralModel(CurrentDatabase, id);
            return PartialView("Settings/General", m);
        }

        [HttpPost]
        public ActionResult GeneralHelpToggle(int id)
        {
            CurrentDatabase.ToggleUserPreference("ShowGeneralHelp");
            var m = new SettingsGeneralModel(CurrentDatabase, id);
            return PartialView("Settings/General", m);
        }

        [HttpPost]
        public ActionResult GeneralEdit(int id)
        {
            var m = new SettingsGeneralModel(CurrentDatabase, id);
            return PartialView("Settings/GeneralEdit", m);
        }

        [HttpPost]
        public ActionResult GeneralUpdate(SettingsGeneralModel m)
        {
            if (!m.Org.LimitToRole.HasValue())
            {
                m.Org.LimitToRole = null;
            }
            DbUtil.LogActivity($"Update SettingsGeneral {m.Org.OrganizationName}", orgid: m.Id);
            if (ModelState.IsValid)
            {
                m.Update(User.IsInRole("Admin"));
                return View("Settings/General", m);
            }
            return PartialView("Settings/GeneralEdit", m);
        }

        [HttpGet]
        [OutputCache(Duration = 60)]
        public ActionResult EspaceSearch(string q)
        {
            var espace = new eSpaceClient
            {
                Username = CurrentDatabase.Setting("eSpaceUserName", ""),
                Password = CurrentDatabase.Setting("eSpacePassword", "")
            };
            var list = espace.Event.List(new NameValueCollection {
                { "eventName", q },
                { "topX", "10" },
                { "startDate", DateTime.Now.ToString("d") }
            });
            return PartialView("Other/eSpaceResults", list);
        }
    }
}
