using System;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class QueryFunctions
    {
        public DateTime LastSunday
        {
            get
            {
                if (lastSunday.HasValue)
                    return lastSunday.Value;
                var q = from m in db.Meetings
                        where m.MeetingDate.Value.Date.DayOfWeek == 0
                        where m.MaxCount > 0
                        where m.MeetingDate < Util.Now
                        orderby m.MeetingDate descending
                        select m.MeetingDate.Value.Date;
                var dt = q.FirstOrDefault();
                if (dt == DateTime.MinValue) //Sunday Date equal/before today
                    dt = Util.Today.AddDays(-(int) Util.Today.DayOfWeek);
                lastSunday = dt;
                return dt;
            }
        }

        public int AttendanceTypeCountDateRange(int progid, int divid, int orgid, string attendtype, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);

            string ids;
            var re = new Regex(@"\A(\d+(,\s*)?)+\z");
            if (re.IsMatch(attendtype)) // all comma separated digits?
                ids = attendtype.Replace(" ", ""); // remove whitespace
            else // match on strings = description
            {
                var a = attendtype.Split(',').Select(aa => aa.Trim());
                var qt = from t in db.AttendTypes select t;
                if (attendtype != "All")
                    qt = qt.Where(t => a.Contains(t.Description));
                ids = string.Join(",", qt.Select(t => t.Id));
            }
            var q = db.AttendanceTypeAsOf(start, enddt, progid, divid, orgid, 0, ids);
            return q.Count();
        }

        public int AttendCountAsOf(object startdt, object enddt, bool guestonly, int progid, int divid, int orgid)
        {
            var dates = GetStartEndDates(startdt, enddt);
            return db.AttendedAsOf(progid, divid, orgid, dates.Start, dates.End, guestonly).Count();
        }

        public int AttendMemberTypeCountAsOf(object startdt, object enddt, string membertypes, string notmembertypes,
            int progid, int divid, int orgid)
        {
            var dates = GetStartEndDates(startdt, enddt);

            var memberids = GetMemberTypeIds(membertypes);
            var notmemberids = GetMemberTypeIds(notmembertypes);
            return db.AttendMemberTypeAsOf(dates.Start, dates.End, progid, divid, orgid, memberids, notmemberids).Count();
        }

        public int LastWeekAttendance(int progid, int divid, int starthour, int endhour)
        {
            var dt = LastSunday;
            var dt1 = dt.AddHours(starthour);
            var dt2 = dt.AddHours(endhour);

            var q = from p in db.Programs
                    where progid == 0 || p.Id == progid
                    from pd in p.ProgDivs
                    where divid == 0 || pd.DivId == divid
                    select (from dg in pd.Division.DivOrgs
                            from m in dg.Organization.Meetings
                            where m.MeetingDate >= dt1
                            where m.MeetingDate < dt2
                            select m.MaxCount).Sum() ?? 0;
            return q.Sum();
        }

        public int MeetingCount(int days, int progid, int divid, int orgid)
        {
            var dt = Util.Now.AddDays(-days);
            var q = from m in db.Meetings
                    where m.MeetingDate >= dt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }

        public int MeetingCountDateHours(int progid, int divid, int orgid, object startdt, int hours)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddHours(hours);
            var q = from m in db.Meetings
                    where m.MeetingDate >= start
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            return q.Count();
        }

        public int NumPresent(int days, int progid, int divid, int orgid)
        {
            var dt = Util.Now.AddDays(-days);
            var q = from m in db.Meetings
                    where m.MeetingDate >= dt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            if (!q.Any())
                return 0;
            return q.Sum(mm => mm.MaxCount ?? 0);
        }

        public int NumPresentDateRange(int progid, int divid, int orgid, object startdt, int days)
        {
            var start = startdt.ToDate();
            if (start == null)
                throw new Exception("bad date: " + startdt);
            var enddt = start.Value.AddDays(days);

            var q = from m in db.Meetings
                    where m.MeetingDate >= start
                    where m.MeetingDate < enddt
                    where orgid == 0 || m.OrganizationId == orgid
                    where divid == 0 || m.Organization.DivOrgs.Any(t => t.DivId == divid)
                    where progid == 0 || m.Organization.DivOrgs.Any(t => t.Division.ProgDivs.Any(d => d.ProgId == progid))
                    select m;
            if (!q.Any())
                return 0;
            return q.Sum(mm => mm.MaxCount ?? 0);
        }

        private string GetMemberTypeIds(string membertypes)
        {
            string ids;
            var re = new Regex(@"\A(\d+(,\s*)?)+\z");
            if (re.IsMatch(membertypes)) // all comma separated digits?
                ids = membertypes.Replace(" ", ""); // remove whitespace
            else // match on strings = description
            {
                var a = membertypes.Split(',').Select(aa => aa.Trim());

                var qt = from t in db.MemberTypes select t;
                if (membertypes != "All")
                    qt = qt.Where(t => a.Contains(t.Description));

                ids = string.Join(",", qt.Select(t => t.Id));
            }
            return ids;
        }

        private static StartEndDates GetStartEndDates(object startdt, object enddt)
        {
            var d1 = startdt.ToDate();
            if (d1 == null)
                throw new ArgumentException("bad start date: " + startdt);

            var d2 = enddt.ToDate();
            if (d2 == null)
                throw new ArgumentException("bad start date: " + startdt);
            if (d2 == d2.Value.Date)
                d2 = d2.Value.AddHours(24);
            return new StartEndDates {Start = d1.Value, End = d2.Value};
        }

        private class StartEndDates
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }
    }
}
