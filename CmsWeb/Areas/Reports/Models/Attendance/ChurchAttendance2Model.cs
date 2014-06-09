using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using System.Collections;
using UtilityExtensions;
using System.Text.RegularExpressions;

namespace CmsWeb.Areas.Reports.Models
{
    public class ChurchAttendance2Model
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }

        private List<DateTime> weeks;

        public ChurchAttendance2Model(DateTime? dt1, DateTime? dt2, string SkipWeeks)
        {
            Dt1 = dt1;
            Dt2 = dt2;
            weeks = DbUtil.Db.SundayDates(Dt1, Dt2).Select(w => w.Dt.Value).ToList();
            if (SkipWeeks.HasValue())
                foreach (var wk in SkipWeeks.Split(','))
                {
                    var dt = wk.ToDate();
                    if (dt.HasValue)
                        weeks.Remove(dt.Value);
                }
        }

        public class ColInfo
        {
            public string Heading { get; set; }
            public List<TimeSpan> Times { get; set; }
            public ColInfo()
            {
                Times = new List<TimeSpan>();
            }
        }

        public class ProgInfo
        {
            public string Name { get; set; }
            public string RptGroup { get; set; }
            public decimal? StartHour { get; set; }
            public decimal? EndHour { get; set; }
            List<ColInfo> _cols;
            public List<ColInfo> Cols
            {
                get
                {
                    if (_cols == null)
                    {
                        _cols = new List<ColInfo>();
                        Regex re = null;
                        if (RptGroup.TrimEnd().EndsWith(")"))
                            re = new Regex(@"(?<re>\d+:\d+ [AP]M)");
                        else
                            re = new Regex(@"\((?<re>[^)]*)\)=(?<na>[^,)]*)|(?<re>\d+:\d+ [AP]M)");
                        var m = re.Match(RptGroup);
                        while (m.Success)
                        {
                            var ci = new ColInfo();
                            _cols.Add(ci);
                            var a = m.Groups["re"].Value.Split('|');
                            if (m.Groups["na"].Value.HasValue())
                                ci.Heading = m.Groups["na"].Value;
                            else
                                ci.Heading = m.Groups[1].Value;
                            foreach (var s in a)
                            {
                                var dt = DateTime.Parse(s);
                                ci.Times.Add(dt.TimeOfDay);
                            }
                            m = m.NextMatch();
                        }
                    }
                    return _cols;
                }
            }
            int? _line;
            public int Line
            {
                get
                {
                    if (!_line.HasValue)
                        _line = Regex.Match(RptGroup, @"\A\d+").Value.ToInt();
                    return _line.Value;
                }
            }
            public List<DateTime> weeks { get; set; }

            public IEnumerable<DivInfo> Divs;

            public IEnumerable<WeekInfo> Weeks
            {
                get
                {
                    var q = from w in weeks
                            select new WeekInfo
                            {
                                Sunday = w,
                                Meetings = from d in Divs
                                           from m in d.Meetings
                                           where m.date >= w.AddHours((double)StartHour.Value)
                                           where m.date <= w.AddHours((double)EndHour.Value)
                                           select m
                            };
                    return q.Where(w => w.Meetings.Sum(m => m.Present) > 0);
                }
            }

            public Average Total()
            {
                var a = new Average();
                var q = from w in Weeks
                        from m in w.Meetings
                        group m by w.Sunday into g
                        select g.Sum(mm => mm.Present);
                if (q.Count() == 0)
                    a.avg = 0;
                else
                    a.avg = q.Average();
                a.totalpeople = q.Sum();
                a.totalmeetings = q.Count();
                return a;
            }
            public Average Column(ColInfo c)
            {
                var a = new Average();
                var q = from w in Weeks
                        from m in w.Meetings
                        where c.Times.Contains(m.date.TimeOfDay)
                        group m by w.Sunday into g
                        select g.Sum(mm => mm.Present);
                if (q.Count() == 0)
                    a.avg = 0;
                else
                    a.avg = q.Average();
                a.totalmeetings = q.Count();//q.Where(g => g > 0).Count();
                a.totalpeople = q.Sum();
                return a;
            }
        }
        public class WeekInfo
        {
            public DateTime Sunday { get; set; }
            public IEnumerable<MeetInfo> Meetings;
        }
        public class DivInfo
        {
            public ProgInfo Prog { get; set; }
            public int DivId { get; set; }
            public string Name { get; set; }
            public int Line { get; set; }
            public IEnumerable<MeetInfo> Meetings;
            public List<DateTime> weeks { get; set; }
            public IEnumerable<WeekInfo> Weeks
            {
                get
                {
                    var q = from w in weeks
                            select new WeekInfo
                            {
                                Sunday = w,
                                Meetings = from m in Meetings
                                           where m.date >= w.AddHours((double)Prog.StartHour.Value)
                                           where m.date <= w.AddHours((double)Prog.EndHour.Value)
                                           select m
                            };
                    return q.Where(w => w.Meetings.Sum(m => m.Present) > 0);
                }
            }
            public Average Total()
            {
                var a = new Average();
                var q = from w in Weeks
                        from m in w.Meetings
                        group m by w.Sunday into g
                        select g.Sum(mm => mm.Present);
                if (q.Count() == 0)
                    a.avg = 0;
                else
                    a.avg = q.Average();
                a.totalmeetings = q.Count();
                a.totalpeople = q.Sum();
                return a;
            }
            public Average Column(ColInfo c)
            {
                var a = new Average();
                var q = from w in Weeks
                        from m in w.Meetings
                        where c.Times.Contains(m.date.TimeOfDay)
                        group m by w.Sunday into g
                        select g.Sum(mm => mm.Present);
                if (q.Count() == 0)
                    a.avg = 0;
                else
                    a.avg = q.Average();
                a.totalpeople = q.Sum();
                a.totalmeetings = q.Count();
                return a;
            }
        }

        public class Average
        {
            public double avg { get; set; }
            public int totalmeetings { get; set; }
            public int totalpeople { get; set; }
        }
        public class MeetInfo
        {
            public IEnumerable<DivInfo> Divs { get; set; }
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTime date { get; set; }
            public int Present { get; set; }
        }
        public IEnumerable<ProgInfo> FetchInfo()
        {
            var q = from p in DbUtil.Db.Programs
                    where p.RptGroup != null && p.RptGroup.Length > 0
                    from pd in p.ProgDivs
                    where pd.Division.ReportLine > 0
                    group pd by p into g
                    orderby g.Key.RptGroup
                    select new ProgInfo
                    {
                        Name = g.Key.Name,
                        RptGroup = g.Key.RptGroup,
                        StartHour = g.Key.StartHoursOffset,
                        EndHour = g.Key.EndHoursOffset,
                        Divs = from pd in g
                               orderby pd.Division.ReportLine
                               select new DivInfo
                               {
                                   DivId = pd.DivId,
                                   Name = pd.Division.Name,
                                   Meetings = from dg in pd.Division.DivOrgs
                                              from m in dg.Organization.Meetings
                                              where m.MeetingDate
                                                >= Dt1.Value.AddDays((double)-6)
                                              where m.MeetingDate
                                                <= Dt2.Value.AddDays((double)6)
                                              select new MeetInfo
                                              {
                                                  date = m.MeetingDate.Value,
                                                  OrgId = m.OrganizationId,
                                                  OrgName = m.Organization.OrganizationName,
                                                  Present = m.MaxCount ?? 0,
                                              }
                               },
                    };
            var list = q.ToList();
            foreach (var p in list)
            {
                p.weeks = weeks;
                foreach (var d in p.Divs)
                {
                    d.Prog = p;
                    d.weeks = weeks;
                }
            }
            return list;
        }
    }
}
