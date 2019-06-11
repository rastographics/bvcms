using CmsData;
using CmsData.Codes;
using CmsWeb.Code;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using CmsWeb.Areas.Reports.Models;
using System.Web.Mvc;
using UtilityExtensions;
using CmsWeb.Areas.Search.Models;

namespace CmsWeb.Areas.Reports.Models.Attendance
{
    public class MeetingAttendanceModel
    {
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public int Type { get; set; }
        public int Program { get; set; }
        public int Division { get; set; }
        public string Heading { get; set; }
        public bool EmptyRun { get; set; }

        private readonly List<int> DivisionIds;
        private readonly List<DateTime> weeks;

        private CMSDataContext CurrentDatabase { get; set; }

        public MeetingAttendanceModel(CMSDataContext db, string dt1, string dt2, string type, string program, string div)
        {
            CurrentDatabase = db;

            // dont run the report without parameters
            EmptyRun = !dt1.HasValue() || !dt2.HasValue();

            EndDt = dt2.ToDate() ?? DateTime.Today;
            StartDt = dt1.ToDate() ?? new DateTime(EndDt.Year, EndDt.Month, 1);
            
            if (type.HasValue())
            {
                Type = type.ToInt();
            }
            else
            {
                Type = 0;
            }
            Program = program.ToInt();
            Division = div.ToInt();

            if (Division == 0)
            {
                if (Program != 0)
                {
                    DivisionIds = (from d in CurrentDatabase.Divisions
                                   where d.ProgId == Program
                                   select d.Id).ToList();
                    Heading = (from p in CurrentDatabase.Programs
                               where p.Id == Program
                               select p.Name).FirstOrDefault();
                }
                else
                {
                    DivisionIds = (from d in CurrentDatabase.Divisions
                                   select d.Id).ToList();
                    DivisionIds.Add(0);
                }
            }
            else
            {
                DivisionIds = new List<int>() { Division };
                Heading = (from d in CurrentDatabase.Divisions
                           where d.Id == Division
                           select d.Name).FirstOrDefault();
            }
            weeks = CurrentDatabase.SundayDates(StartDt, EndDt).Select(w => w.Dt.Value).ToList();
            if (weeks.Count > 0 && weeks[0] >= StartDt) {
                StartDt = weeks[0];
            }
            if (weeks[weeks.Count - 1] != EndDt)
            {
                weeks.Add(EndDt);
            }
        }

        public SelectList Types()
        {
            var types = new CodeValueModel().MeetingCategories();
            if (CurrentDatabase.Setting("MeetingTypesReportIncludeEmpty"))
            {
                types = types.AddNotSpecified(9999,"(Empty Type)");
            }
            return new SelectList(types.OrderBy(t => t.Id), "Id", "Value", Type);
        }

        public SelectList Programs()
        {
            return new SelectList(new CodeValueModel().OrgDivTags(true),
                "Id", "Value", Program);
        }

        public SelectList Divisions()
        {
            return new SelectList(OrgSearchModel.DivisionIds(Program),
                "Value", "Text", Division);
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
                            if (wk < WeekDates.Count() - 1)
                            {
                                var week = WeekDates[wk];
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
                                            where m.Date < w
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
                var attends = meetings.SelectMany(m => m.Attendees).OrderBy(x => x).Distinct().ToList();
                a.UniquePeople = attends.Count;
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
                                            where m.Date < w
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
            public int UniquePeople { get; set; }
            public int TotalPeople { get; set; }
        }

        public class MeetInfo
        {
            public int Id { get; set; }
            public int OrgId { get; set; }
            public string OrgName { get; set; }
            public DateTime? Date { get; set; }
            public int Present { get; set; }
            public List<int> Attendees { get
                {
                    var rollsheet = RollsheetModel.RollList(Id, OrgId, Date.Value, false, false, false);
                    return rollsheet.Where(a => a.Attended == true).Select(a => a.PeopleId).ToList();
                }
            }
        }

        public List<TypeInfo> FetchInfo()
        {
            var categories = CurrentDatabase.MeetingCategories.AsEnumerable();
            CmsData.MeetingCategory[] EmptyCategory =
            {
                new CmsData.MeetingCategory
                {
                    Id = 9999,
                    Description = null
                }
            };
            if (CurrentDatabase.Setting("MeetingTypesReportIncludeEmpty"))
            {
                categories = EmptyCategory.Union(categories);
            }
            var q = from cat in categories
                            select new TypeInfo
                            {
                                Id = (int) cat.Id,
                                Name = cat.Description ?? "(Empty Type)",
                                WeekDates = weeks,
                                Orgs = (from org in CurrentDatabase.Organizations
                                        join meet in CurrentDatabase.Meetings on org.OrganizationId equals meet.OrganizationId
                                        join div in CurrentDatabase.DivOrgs on org.OrganizationId equals div.OrgId
                                        where org.OrganizationStatusId == OrgStatusCode.Active
                                        where ((meet.Description == null && cat.Description == null) || (meet.Description == cat.Description))
                                        where DivisionIds.Contains(div.DivId)
                                        select new OrgInfo
                                        {
                                            Id = org.OrganizationId,
                                            Name = org.OrganizationName,
                                            Meetings = (from m in CurrentDatabase.Meetings
                                                        where org.OrganizationId == m.OrganizationId
                                                        where ((m.Description == null && cat.Description == null) || (m.Description == cat.Description))
                                                        where m.MeetingDate >= StartDt
                                                        where m.MeetingDate < EndDt
                                                        select new MeetInfo
                                                        {
                                                            Id = m.MeetingId,
                                                            Date = m.MeetingDate,
                                                            OrgId = org.OrganizationId,
                                                            OrgName = org.OrganizationName,
                                                            Present = m.NumPresent
                                                        })
                                                        .ToList()
                                        })
                                        .Where(org => org.Meetings.Count > 0)
                                        .OrderBy(org => org.Name)
                                        .GroupBy(org => org.Id)
                                        .Select(org => org.FirstOrDefault())
                                        .ToList()
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
