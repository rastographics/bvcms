using CmsData;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class ChurchAttendance2Model
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }

        private readonly List<DateTime> weeks;
        public ChurchAttendance2Model() { }
        public ChurchAttendance2Model(DateTime? dt1, DateTime? dt2, string skipWeeks)
        {
            Dt1 = dt1;
            Dt2 = dt2;
            weeks = DbUtil.Db.SundayDates(Dt1, Dt2).Select(w => w.Dt.Value).ToList();
            if (!skipWeeks.HasValue())
            {
                return;
            }

            foreach (var wk in skipWeeks.Split(','))
            {
                var dt = wk.ToDate();
                if (dt.HasValue)
                {
                    weeks.Remove(dt.Value);
                }
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
            private List<ColInfo> cols;

            public List<ColInfo> Cols
            {
                get
                {
                    if (cols == null)
                    {
                        cols = new List<ColInfo>();
                        Regex re = null;
                        if (RptGroup.TrimEnd().EndsWith(")"))
                        {
                            re = new Regex(@"(?<re>\d+:\d+ [AP]M)");
                        }
                        else
                        {
                            re = new Regex(@"\((?<re>[^)]*)\)=(?<na>[^,)]*)|(?<re>\d+:\d+ [AP]M)");
                        }

                        var m = re.Match(RptGroup);
                        while (m.Success)
                        {
                            var ci = new ColInfo();
                            cols.Add(ci);
                            var a = m.Groups["re"].Value.Split('|');
                            if (m.Groups["na"].Value.HasValue())
                            {
                                ci.Heading = m.Groups["na"].Value;
                            }
                            else
                            {
                                ci.Heading = m.Groups[1].Value;
                            }

                            foreach (var s in a)
                            {
                                var dt = DateTime.Parse(s);
                                ci.Times.Add(dt.TimeOfDay);
                            }
                            m = m.NextMatch();
                        }
                    }
                    return cols;
                }
            }

            private int? line;

            public int Line
            {
                get
                {
                    if (!line.HasValue)
                    {
                        line = Regex.Match(RptGroup, @"\A\d+").Value.ToInt();
                    }

                    return line.Value;
                }
            }

            public List<DateTime> WeekDates { get; set; }

            public List<DivInfo> Divs;

            public List<WeekInfo> Weeks
            {
                get
                {
                    var q = from w in WeekDates
                            select new WeekInfo
                            {
                                Sunday = w,
                                Meetings = (from d in Divs
                                            from m in d.Meetings
                                            where m.Date >= w.AddHours((double)(StartHour ?? 0))
                                            where m.Date <= w.AddHours((double)(EndHour ?? 0))
                                            select m).ToList()
                            };
                    return q.Where(w => w.Meetings.Sum(m => m.Present) > 0).ToList();
                }
            }

            public Average Total()
            {
                var a = new Average();
                var q = (from w in Weeks
                         from m in w.Meetings
                         group m by w.Sunday
                         into g
                         select g.Sum(mm => mm.Present)).ToList();
                a.Avg = !q.Any() ? 0 : q.Average();
                a.TotalPeople = q.Sum();
                a.TotalMeetings = q.Count;
                return a;
            }

            public Average Column(ColInfo c)
            {
                var a = new Average();
                var q = (from w in Weeks
                         from m in w.Meetings
                         where c.Times.Contains(m.Date.TimeOfDay)
                         group m by w.Sunday
                         into g
                         select g.Sum(mm => mm.Present)).ToList();
                a.Avg = !q.Any() ? 0 : q.Average();
                a.TotalMeetings = q.Count; //q.Where(g => g > 0).Count();
                a.TotalPeople = q.Sum();
                return a;
            }
        }

        public class WeekInfo
        {
            public DateTime Sunday { get; set; }
            public List<MeetInfo> Meetings;
        }

        public class DivInfo
        {
            public ProgInfo Prog { get; set; }
            public int DivId { get; set; }
            public string Name { get; set; }
            public int Line { get; set; }
            public List<MeetInfo> Meetings;
            public List<DateTime> WeekDates { get; set; }

            public List<WeekInfo> Weeks
            {
                get
                {
                    var q = from w in WeekDates
                            select new WeekInfo
                            {
                                Sunday = w,
                                Meetings = (from m in Meetings
                                            where m.Date >= w.AddHours((double)(Prog.StartHour ?? 0))
                                            where m.Date <= w.AddHours((double)(Prog.EndHour ?? 0))
                                            select m).ToList()
                            };
                    return q.Where(w => w.Meetings.Sum(m => m.Present) > 0).ToList();
                }
            }

            public Average Total()
            {
                var a = new Average();
                var q = (from w in Weeks
                         from m in w.Meetings
                         group m by w.Sunday
                         into g
                         select g.Sum(mm => mm.Present)).ToList();
                a.Avg = q.Any() ? q.Average() : 0;
                a.TotalMeetings = q.Count;
                a.TotalPeople = q.Sum();
                return a;
            }

            public Average Column(ColInfo c)
            {
                var a = new Average();
                var q = (from w in Weeks
                         from m in w.Meetings
                         where c.Times.Contains(m.Date.TimeOfDay)
                         group m by w.Sunday
                         into g
                         select g.Sum(mm => mm.Present)).ToList();
                a.Avg = q.Any() ? q.Average() : 0;
                a.TotalPeople = q.Sum();
                a.TotalMeetings = q.Count();
                return a;
            }
        }

        public class Average
        {
            public double Avg { get; set; }
            public int TotalMeetings { get; set; }
            public int TotalPeople { get; set; }
        }

        public class MeetInfo
        {
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTime Date { get; set; }
            public int Present { get; set; }
        }

        public List<ProgInfo> FetchInfo()
        {
            string sql = $@"
WITH progs AS (
	SELECT * FROM dbo.Program
	WHERE LEN(RptGroup) > 0
),
progdivs AS (
	SELECT 
		p.Id ProgId
		,p.Name Prog
		,d.Id DivId
		,d.Name Div
		,p.RptGroup
		,d.ReportLine
		,p.StartHoursOffset
		,p.EndHoursOffset
	FROM dbo.Division d
	JOIN dbo.ProgDiv pd ON pd.DivId = d.Id 
	JOIN progs p ON p.Id = pd.ProgId
	WHERE d.ReportLine >= 0
),
meetings AS (
	SELECT m.MeetingId
          ,m.MaxCount
          ,m.MeetingDate
		  ,pd.ProgId
          ,pd.Prog
          ,pd.DivId
          ,pd.Div
          ,pd.RptGroup
          ,pd.ReportLine
          ,pd.StartHoursOffset
          ,pd.EndHoursOffset
          ,o.OrganizationId
		  ,o.OrganizationName

	FROM dbo.Meetings m
	JOIN dbo.Organizations o ON o.OrganizationId = m.OrganizationId
	JOIN dbo.DivOrg dd ON dd.OrgId = o.OrganizationId
	JOIN progdivs pd ON pd.DivId = dd.DivId
	WHERE m.MeetingDate >= @Dt1
	AND CONVERT(DATE, m.MeetingDate) <= @Dt2
	AND m.MaxCount > 0
)
SELECT * FROM meetings
ORDER BY RptGroup, ReportLine
";
            var j = DbUtil.Db.Connection.Query<Data>(sql, new { Dt1, Dt2 }).ToList();
            var q = from prog in j
                    group prog by prog.RptGroup
                    into g
                    orderby g.Key
                    let p = g.First()
                    select new ProgInfo
                    {
                        Name = p.Prog,
                        RptGroup = p.RptGroup,
                        StartHour = p.StartHoursOffset,
                        EndHour = p.EndHoursOffset,
                        Divs = (from div in g
                                group div by div.ReportLine
                                into lines
                                let d1 = lines.First()
                                select new DivInfo
                                {
                                    DivId = d1.DivId,
                                    Name = d1.Div,
                                    Meetings = (from m in lines
                                                select new MeetInfo
                                                {
                                                    Date = m.MeetingDate,
                                                    OrgId = m.OrgId,
                                                    OrgName = m.OrganizationName,
                                                    Present = m.MaxCount
                                                }).ToList()
                                }).ToList()
                    };

            var list = q.ToList();
            foreach (var p in list)
            {
                p.WeekDates = weeks;
                foreach (var d in p.Divs)
                {
                    d.Prog = p;
                    d.WeekDates = weeks;
                }
            }
            return list;
        }

        public class Data
        {
            public int MeetingId { get; set; }
            public int MaxCount { get; set; }
            public DateTime MeetingDate { get; set; }
            public int ProgId { get; set; }
            public string Prog { get; set; }
            public int DivId { get; set; }
            public string Div { get; set; }
            public string RptGroup { get; set; }
            public string ReportLine { get; set; }
            public int StartHoursOffset { get; set; }
            public int EndHoursOffset { get; set; }
            public int OrgId { get; set; }
            public string OrganizationName { get; set; }
        }
    }
}
