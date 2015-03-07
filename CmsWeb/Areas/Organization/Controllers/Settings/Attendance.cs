using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org2.Models;
using CmsWeb.Code;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org2.Controllers
{
    public partial class OrganizationController
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
            DbUtil.Db.ToggleUserPreference("ShowAttendanceHelp");
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
            DbUtil.Db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, m.Org.OrgSchedules);
            DbUtil.LogActivity("Update SettingsAttendance {0}".Fmt(m.Org.OrganizationName));
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