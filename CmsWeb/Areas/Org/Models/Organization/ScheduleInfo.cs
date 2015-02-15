using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsWeb.Code;
using UtilityExtensions;
using CmsData;

namespace CmsWeb.Models
{
    public class ScheduleInfo
    {
        private OrgSchedule sc;

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

        public int Id
        {
            get { return sc.Id; }
            set { sc.Id = value; }
        }

        public CodeInfo SchedDay { get; set; }
//        public int SchedDay
//        {
//            get { return sc.SchedDay ?? 0; }
//            set { sc.SchedDay = value; }
//        }

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

        public CodeInfo AttendCredit { get; set; }
//        public int AttendCreditId
//        {
//            get { return sc.AttendCreditId ?? 1; }
//            set { sc.AttendCreditId = value; }
//        }



        public string DisplayAttendCredit
        {
            get
            {
                return (from i in CodeValueModel.AttendCredits()
                        where i.Id == AttendCredit.Value.ToInt()
                        select i.Value).Single();
            }
        }

        public string DisplayDay
        {
            get
            {
                return (from i in CodeValueModel.DaysOfWeek()
                        where i.Value == SchedDay.ToString()
                        select i.Text).SingleOrDefault();
            }
        }

        public string Display
        {
            get
            {
                if (sc == null)
                    return "None";
                return "{0}, {1}, {2}".Fmt(DisplayDay, Time, DisplayAttendCredit);
            }
        }

        public string ValuePrev
        {
            get
            {
                var dt = PrevMeetingTime;
                return "{0},{1},{2}".Fmt(dt.Date.ToShortDateString(), dt.ToShortTimeString(), AttendCredit);
            }
        }
        public string ValueNext
        {
            get
            {
                var dt = NextMeetingTime;
                return "{0},{1},{2}".Fmt(dt.Date.ToShortDateString(), dt.ToShortTimeString(), AttendCredit);
            }
        }

        public DateTime PrevMeetingTime
        {
            get
            {
                DateTime dt = Util.Now.Date;
                if (sc.SchedDay < 9)
                {
                    dt = Util.Now.Date.Sunday().AddDays(sc.SchedDay ?? 0);
                    if (dt > Util.Now.Date)
                        dt = dt.AddDays(-7);
                }
                return dt.Add(Time.ToDate().Value.TimeOfDay);
            }
        }

        public DateTime NextMeetingTime
        {
            get { return PrevMeetingTime.AddDays(7); }
        }
    }
}