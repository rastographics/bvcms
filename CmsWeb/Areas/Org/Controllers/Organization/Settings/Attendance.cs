using System;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsWeb.Models;

namespace CmsWeb.Areas.Org.Controllers
{
    public partial class OrganizationController
    {
        [HttpPost]
        public ActionResult Attendance(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/Attendance", m);
        }
        [HttpPost]
        public ActionResult AttendanceEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView("Settings/AttendanceEdit", m);
        }
        [HttpPost]
        public ActionResult AttendanceUpdate(int id)
        {
            var m = new OrganizationModel(id);
            m.Schedules.Clear();

            UpdateModel(m);
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