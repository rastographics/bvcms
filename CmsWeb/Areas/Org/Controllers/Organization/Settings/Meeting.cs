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
        public ActionResult SettingsMeetings(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/Meetings", m);
        }
        [HttpPost]
        public ActionResult SettingsMeetingsEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/MeetingsEdit", m);
        }
        [HttpPost]
        public ActionResult SettingsMeetingsUpdate(int id)
        {
            var m = new OrganizationModel(id);
            m.Schedules.Clear();

            UpdateModel(m);
            m.UpdateSchedules();
            DbUtil.Db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, m.Org.OrgSchedules);
            DbUtil.LogActivity("Update SettingsMeetings {0}".Fmt(m.Org.OrganizationName));
            return PartialView("Settings/Meetings", m);
        }
        [HttpPost]
        public ActionResult NewSchedule()
        {
            var s = new ScheduleInfo(
                new OrgSchedule
                {
                    SchedDay = 0,
                    SchedTime = DateTime.Parse("8:00 AM"),
                    AttendCreditId = 1
                });
            return PartialView("EditorTemplates/ScheduleInfo", s);
        }

    }
}