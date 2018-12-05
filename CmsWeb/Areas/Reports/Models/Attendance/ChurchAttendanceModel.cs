using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class ChurchAttendanceModel
    {
        public DateTime Sunday { get; set; }
        public ChurchAttendanceModel() { }
        public ChurchAttendanceModel(DateTime sunday)
        {
            Sunday = sunday;
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
            public int progid { get; set; }
            public string Name { get; set; }
            public IEnumerable<DivInfo> Divs { get; set; }
            public string RptGroup { get; set; }

            private List<ColInfo> _cols;
            public List<ColInfo> Cols
            {
                get
                {
                    if (_cols == null)
                    {
                        _cols = new List<ColInfo>();
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
                            _cols.Add(ci);
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
                    return _cols;
                }
            }

            private int? _line;
            public int Line
            {
                get
                {
                    if (!_line.HasValue)
                    {
                        _line = Regex.Match(RptGroup, @"\A\d+").Value.ToInt();
                    }

                    return _line.Value;
                }
            }
            public DateTime? Dt1 { get; set; }
            public DateTime? Dt2 { get; set; }
        }
        public class DivInfo
        {
            public int DivId { get; set; }
            public string Name { get; set; }
            public bool NoDisplayZero { get; set; }
            public IEnumerable<MeetInfo> Meetings { get; set; }
        }
        public class MeetInfo
        {
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTime date { get; set; }
            public int Present { get; set; }
            public int Visitors { get; set; }
            public int OutTowners { get; set; }
        }
        public IEnumerable<ProgInfo> FetchInfo()
        {
            var q = from p in DbUtil.Db.Programs
                    where p.RptGroup != null && p.RptGroup.Length > 0
                    from pd in p.ProgDivs
                    where pd.Division.ReportLine > 0
                    group pd by p into g
                    orderby g.Key.RptGroup
                    let dt1 = Sunday.AddHours((double)g.Key.StartHoursOffset)
                    let dt2 = Sunday.AddHours((double)g.Key.EndHoursOffset)
                    select new ProgInfo
                    {
                        progid = g.Key.Id,
                        Name = g.Key.Name,
                        RptGroup = g.Key.RptGroup,
                        Dt1 = dt1,
                        Dt2 = dt2,
                        Divs = from pd in g
                               orderby pd.Division.ReportLine
                               select new DivInfo
                               {
                                   DivId = pd.DivId,
                                   Name = pd.Division.Name,
                                   NoDisplayZero = pd.Division.NoDisplayZero ?? false,
                                   Meetings = from dg in pd.Division.DivOrgs
                                              from m in dg.Organization.Meetings
                                              where m.MeetingDate >= dt1
                                              where m.MeetingDate < dt2
                                              select new MeetInfo
                                              {
                                                  date = m.MeetingDate.Value,
                                                  OrgId = m.OrganizationId,
                                                  OrgName = m.Organization.OrganizationName,
                                                  Present = m.MaxCount ?? 0,
                                                  Visitors = m.NumNewVisit + m.NumRepeatVst,
                                                  OutTowners = m.NumOutTown ?? 0
                                              }
                               },
                    };
            return q;
        }
        public static DateTime MostRecentAttendedSunday()
        {
            var q = from m in DbUtil.Db.Meetings
                    where m.MeetingDate.Value.Date.DayOfWeek == 0
                    where m.MaxCount > 0
                    where m.MeetingDate < Util.Now
                    orderby m.MeetingDate descending
                    select m.MeetingDate.Value.Date;
            var dt = q.FirstOrDefault();
            if (dt == DateTime.MinValue) //Sunday Date equal/before today
            {
                dt = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            }

            return dt;
        }
    }
}
