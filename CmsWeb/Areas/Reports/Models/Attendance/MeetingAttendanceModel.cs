using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models.Attendance
{
    public class MeetingAttendanceModel
    {
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public int Type { get; set; }
        private readonly List<DateTime> weeks;

        private CMSDataContext CurrentDatabase { get; set; }

        public MeetingAttendanceModel(CMSDataContext db, DateTime dt1, DateTime dt2, string type)
        {
            CurrentDatabase = db;
            StartDt = dt1;
            EndDt = dt2;
            Type = type.ToInt();
            weeks = CurrentDatabase.SundayDates(StartDt, EndDt).Select(w => w.Dt.Value).ToList();
        }

        public SelectList Types()
        {
            return new SelectList(new CodeValueModel().MeetingCategories(),
                "Id", "Value", Type);
        }

        public class ColInfo
        {
            public string Heading { get; set; }
            public DateTime StartDt { get; set; }
            public DateTime EndDt { get; set; }
        }

        public class TypeInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            private List<ColInfo> cols;

            public List<ColInfo> Cols
            {
                get
                {
                    if (cols == null)
                    {
                        cols = new List<ColInfo>();
                        for (int wk = 0; wk < WeekDates.Count(); wk++)
                        {
                            var week = WeekDates[wk];
                            if (wk < WeekDates.Count() - 1)
                            {
                                var ci = new ColInfo();
                                ci.Heading = "Week of " + week.Month + "/" + week.Day;
                                ci.StartDt = week;
                                ci.EndDt = WeekDates[wk + 1];
                                cols.Add(ci);
                            }
                        }
                    }
                    return cols;
                }
            }

            public List<DateTime> WeekDates { get; set; }

            public List<OrgInfo> Orgs;

            public List<WeekInfo> Weeks
            {
                get
                {
                    var q = from w in WeekDates
                            select new WeekInfo
                            {
                                Sunday = w,
                                Meetings = (from d in Orgs
                                            from m in d.Meetings
                                            where m.Date >= w
                                            where m.Date <= w
                                            select m).ToList()
                            };
                    return q.Where(w => w.Meetings.Sum(m => m.Present) > 0).ToList();
                }
            }

            public Cell Total()
            {
                var a = new Cell();
                var meetings = (from d in Orgs
                                from m in d.Meetings
                                select m).ToList();
                a.TotalMeetings = meetings.Count;
                a.TotalPeople = meetings.Sum(mm => mm.Present);
                return a;
            }
        }

        public class WeekInfo
        {
            public DateTime Sunday { get; set; }
            public List<MeetInfo> Meetings;
        }

        public class OrgInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
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
                                            where m.Date >= w
                                            where m.Date <= w
                                            select m).ToList()
                            };
                    return q.Where(w => w.Meetings.Sum(m => m.Present) > 0).ToList();
                }
            }
            
            public Cell Column(ColInfo c)
            {
                var a = new Cell();
                var q = (from m in Meetings
                         where m.Date >= c.StartDt
                         where m.Date < c.EndDt
                         select m).ToList();
                a.TotalMeetings = q.Count;
                a.TotalPeople = q.Sum(mm => mm.Present);
                return a;
            }

            public Cell Total()
            {
                var a = new Cell();
                a.TotalMeetings = Meetings.Count;
                a.TotalPeople = Meetings.Sum(mm => mm.Present);
                return a;
            }
        }

        public class Cell
        {
            public int TotalMeetings { get; set; }
            public int TotalPeople { get; set; }
        }

        public class MeetInfo
        {
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTime? Date { get; set; }
            public int Present { get; set; }
        }

        public List<TypeInfo> FetchInfo()
        {
            var q = from cat in CurrentDatabase.MeetingCategories
                            select new TypeInfo
                            {
                                Id = (int) cat.Id,
                                Name = cat.Description,
                                WeekDates = weeks,
                                Orgs = (from org in CurrentDatabase.Organizations
                                        join meet in CurrentDatabase.Meetings on org.OrganizationId equals meet.OrganizationId
                                        where org.OrganizationStatusId == OrgStatusCode.Active
                                        where meet.Description == cat.Description
                                        select new OrgInfo
                                        {
                                            Id = org.OrganizationId,
                                            Name = org.OrganizationName,
                                            Meetings = (from m in CurrentDatabase.Meetings
                                                        where org.OrganizationId == m.OrganizationId
                                                        where m.MeetingDate >= StartDt
                                                        where m.MeetingDate < EndDt
                                                        select new MeetInfo
                                                        {
                                                            Date = m.MeetingDate,
                                                            OrgId = org.OrganizationId,
                                                            OrgName = org.OrganizationName,
                                                            Present = m.MaxCount ?? 0
                                                        }).ToList()
                                        })
                                        .Where(org => org.Meetings.Count > 0)
                                        .OrderBy(org => org.Name)
                                        .GroupBy(org => org.Id)
                                        .Select(org => org.FirstOrDefault()).ToList()
                            };
            if (Type != 0)
            {
                q = q.Where(cat => cat.Id == Type);
            }
            var list = q.ToList();
            return list;
        }
    }
}
