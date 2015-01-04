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

        public int SchedDay
        {
            get { return sc.SchedDay ?? 0; }
            set { sc.SchedDay = value; }
        }

        public string Time
        {
            get
            {
                if (!sc.SchedTime.HasValue)
                    return "08:00 AM";
                return sc.SchedTime.ToString2("t");
            }
            set { sc.SchedTime = value.ToDate(); }
        }

        public int AttendCreditId
        {
            get { return sc.AttendCreditId ?? 1; }
            set { sc.AttendCreditId = value; }
        }

        public SelectList DaysOfWeek()
        {
            return new SelectList(new[]
                {
                    new {Text = "Sun", Value = "0"},
                    new {Text = "Mon", Value = "1"},
                    new {Text = "Tue", Value = "2"},
                    new {Text = "Wed", Value = "3"},
                    new {Text = "Thu", Value = "4"},
                    new {Text = "Fri", Value = "5"},
                    new {Text = "Sat", Value = "6"},
                    new {Text = "Any", Value = "10"},
                }, "Value", "Text", SchedDay.ToString());
        }

        public SelectList AttendCredits()
        {
            return new SelectList(CodeValueModel.AttendCredits(),
                                  "Id", "Value", AttendCreditId.ToString());
        }

        public string DisplayAttendCredit
        {
            get
            {
                return (from i in CodeValueModel.AttendCredits()
                        where i.Id == AttendCreditId
                        select i.Value).Single();
            }
        }

        public string DisplayDay
        {
            get
            {
                return (from i in DaysOfWeek()
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
                return "{0},{1},{2}".Fmt(dt.Date.ToShortDateString(), dt.ToShortTimeString(), AttendCreditId);
            }
        }
        public string ValueNext
        {
            get
            {
                var dt = NextMeetingTime;
                return "{0},{1},{2}".Fmt(dt.Date.ToShortDateString(), dt.ToShortTimeString(), AttendCreditId);
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