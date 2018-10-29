using System;
using System.ComponentModel;
using CmsData;
using CmsWeb.Code;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models
{
    public class ScheduleInfo
    {
        private readonly OrgSchedule sc;

        public ScheduleInfo(OrgSchedule sc)
        {
            this.sc = sc;
            SchedDay = new CodeInfo(sc.SchedDay, "SchedDay");
            AttendCredit = new CodeInfo(sc.AttendCreditId, "AttendCredit");
        }

        public ScheduleInfo()
        {
            sc = new OrgSchedule();
        }

        [DisplayName("Schedule Number")]
        public int Id
        {
            get { return sc.Id; }
            set { sc.Id = value; }
        }

        public DateTime Time
        {
            get
            {
                if (!sc.SchedTime.HasValue)
                    return DateTime.Today + new TimeSpan(8, 0, 0);
                return sc.SchedTime.Value;
            }
            set { sc.SchedTime = value; }
        }

        public CodeInfo SchedDay { get; set; }
        public CodeInfo AttendCredit { get; set; }

        public string Display
        {
            get
            {
                if (sc == null)
                    return "None";
                return $"{SchedDay}, {Time.FormatTime()}, {AttendCredit}";
            }
        }

        public string ValuePrev
        {
            get
            {
                var dt = PrevMeetingTime;
                return $"{dt.Date.ToShortDateString()} {dt.ToShortTimeString()},{AttendCredit.Value}";
            }
        }

        public string ValueNext
        {
            get
            {
                var dt = NextMeetingTime;
                return $"{dt.Date.ToShortDateString()} {dt.ToShortTimeString()},{AttendCredit.Value}";
            }
        }

        public DateTime PrevMeetingTime
        {
            get
            {
                var dt = Util.Now.Date;
                if (sc.SchedDay < 9)
                {
                    dt = Util.Now.Date.Sunday().AddDays(sc.SchedDay ?? 0);
                    if (dt > Util.Now.Date)
                        dt = dt.AddDays(-7);
                }
                return dt.Add(Time.TimeOfDay);
            }
        }

        public DateTime NextMeetingTime => PrevMeetingTime.AddDays(7);

        public OrgSchedule ToOrgSchedule()
        {
            return new OrgSchedule {
                Id = Id,
                AttendCreditId = AttendCredit.Value.ToInt(),
                SchedDay = SchedDay.Value.ToInt(),
                SchedTime = Time.ToDate()
            };
        }
    }
}
