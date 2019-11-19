using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class PythonModel
    {
        public DateTime DateTime => Util.Now;
        public int DayOfWeek => Util.Now.DayOfWeek.ToInt();
        public string ScheduledTime { get; private set; }
        public string FormatDate(DateTime dt) => dt.ToShortDateString();

        public string AddDays(object dt1, int days)
        {
            var dt = dt1.ToDate();
            if (dt.HasValue)
                return dt.Value.AddDays(days).ToShortDateString();
            return "1/1/1";
        }

        public DateTime DateAddDays(object dt, int days)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.AddDays(days);
        }
        public int DateDiffDays(object dt1, object dt2)
        {
            var d1 = dt1.ToDate();
            var d2 = dt2.ToDate();
            if (!d1.HasValue)
                throw new Exception("bad date: " + dt1);
            if (!d2.HasValue)
                throw new Exception("bad date: " + dt2);
            return d2.Value.Subtract(d1.Value).Days;
        }

        public DateTime DateAddHours(object dt, int hours)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.AddHours(hours);
        }

        public DateTime MostRecentAttendedSunday(int progid)
        {
            var q = from m in db.Meetings
                where m.MeetingDate.Value.Date.DayOfWeek == 0
                where m.MaxCount > 0
                where
                progid == 0 || m.Organization.DivOrgs.Any(dd => dd.Division.ProgDivs.Any(pp => pp.ProgId == progid))
                where m.MeetingDate < Util.Now
                orderby m.MeetingDate descending
                select m.MeetingDate.Value.Date;
            var dt = q.FirstOrDefault();
            if (dt == DateTime.MinValue) //Sunday Date equal/before today
                dt = Util.Today.AddDays(-(int) Util.Now.DayOfWeek);
            return dt;
        }

        public DateTime? ParseDate(string dt)
        {
            return dt.ToDate();
        }

        public void SetToday(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            Util.Today = dt2.Value;
        }
        public void ResetToday()
        {
            Util.ResetToday();
        }

        public DateTime SundayForDate(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.Sunday();
        }

        public DateTime SundayForWeek(int year, int week)
        {
            return Util.SundayForWeek(year, week);
        }

        public int WeekNumber(object dt)
        {
            var dt2 = dt.ToDate();
            if (dt2 == null)
                throw new Exception("bad date: " + dt);
            return dt2.Value.GetWeekNumber();
        }

        public int WeekOfMonth(object dt)
        {
            var sunday = SundayForDate(dt);
            return sunday.WeekOfMonth();
        }
    }
}