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
        public ActionResult Attendance(int id)
        {
            var m = new SettingsAttendanceModel(id);
            return PartialView("Settings/Attendance", m);
        }

        [HttpPost]
        public ActionResult AttendanceHelpToggle(int id)
        {
            CurrentDatabase.ToggleUserPreference("ShowAttendanceHelp");
            var m = new SettingsAttendanceModel(id);
            return PartialView("Settings/Attendance", m);
        }

        [HttpPost]
        public ActionResult AttendanceEdit(int id)
        {
            var m = new SettingsAttendanceModel(id);
            return PartialView("Settings/AttendanceEdit", m);
        }

        [HttpPost]
        public ActionResult AttendanceUpdate(SettingsAttendanceModel m)
        {
            m.Update();
            m.UpdateSchedules();
            CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, m.Org.OrgSchedules);
            DbUtil.LogActivity($"Update SettingsAttendance {m.Org.OrganizationName}");
            return PartialView("Settings/Attendance", m);
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
