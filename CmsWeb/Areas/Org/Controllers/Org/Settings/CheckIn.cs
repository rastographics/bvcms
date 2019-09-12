using System;
using System.Data.Linq;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrgController
    {
        [HttpPost]
        public ActionResult CheckIn(int id)
        {
            var m = new SettingsCheckInModel(id, CurrentDatabase);
            return PartialView("Settings/CheckIn", m);
        }

        [HttpPost]
        public ActionResult CheckInHelpToggle(int id)
        {
            CurrentDatabase.ToggleUserPreference("ShowCheckInHelp");
            var m = new SettingsCheckInModel(id, CurrentDatabase);
            return PartialView("Settings/CheckIn", m);
        }
        
        [HttpPost]
        public ActionResult CheckInUpdate(SettingsCheckInModel m)
        {
            m.CurrentDatabase = CurrentDatabase;
            m.Update();
            return PartialView("Settings/CheckIn", m);
        }
    }
}
