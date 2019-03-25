using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsData.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Manage.Models
{
    public class VolunteerCommitmentsModel
    {
        public string SmallGroup1 { get; set; }
        public string SmallGroup2 { get; set; }
        public bool SortByWeek { get; set; }
        public int WeekNumber { get; set; }
        public int PageNumber { get; set; }
        public int? CurMonth { get; set; }
        public int? CurYear { get; set; }
        public int? PrevYear { get; set; }

        public class NameId
        {
            public string Name { get; set; }
            public int PeopleId { get; set; }
            public int? Commitment { get; set; }
            public bool OtherCommitments { get; set; }
            public string CommitmentText
            {
                get
                {
                    return AttendCommitmentCode.Lookup(Commitment);
                }
            }
        }
        public string OrgName { get; set; }
        public int OrgId { get; set; }
        public bool IsLeader { get; set; }

        public SelectList SmallGroups()
        {
            var q = from m in DbUtil.Db.MemberTags
                    where m.OrgId == OrgId
                    where m.OrgMemMemTags.Any()
                    orderby m.Name
                    select m.Name;
            var list = q.ToList();
            list.Insert(0, "(not specified)");
            return new SelectList(list);
        }

        private List<VolunteerCalendar> volunteers;
        public IEnumerable<VolunteerCalendar> Volunteers()
        {
            if (volunteers != null)
            {
                return volunteers;
            }

            var q = from mm in DbUtil.Db.VolunteerCalendar(OrgId, SmallGroup1, SmallGroup2)
                    orderby mm.PeopleId
                    select mm;
            return volunteers = q.ToList();
        }
        public List<TimeSlots.TimeSlot> TimeSlots { get; set; }
        public DragDropInfo ddinfo { get; set; }

        public VolunteerCommitmentsModel() { }
        public VolunteerCommitmentsModel(int id)
        {
            OrgName = (from o in DbUtil.Db.Organizations where o.OrganizationId == id select o.OrganizationName).Single();
            OrgId = id;
            TimeSlots = (from ts in Regsettings.TimeSlots.list
                         orderby ts.DayOfWeek, ts.Time
                         select ts).ToList();

            IsLeader = OrganizationMember.VolunteerLeaderInOrg(DbUtil.Db, id);
        }

        public class Slot
        {
            public DateTime Time { get; set; }
            public long ticks => Time.Ticks;
            public DateTime Sunday { get; set; }
            public int Week { get; set; }
            public bool Disabled { get; set; }
            public DateTime DayHour { get; set; }
            public List<NameId> Persons { get; set; }
            public int Count
            {
                get { return Persons.Count(pp => AttendCommitmentCode.committed.Contains(pp.Commitment ?? 0)); }
            }
            public List<NameId> OrderedPersons()
            {
                return (from p in Persons
                        orderby CmsData.Codes.AttendCommitmentCode.Order(p.Commitment), p.Name
                        select p).ToList();
            }
            public int Limit { get; set; }
            public int MeetingId { get; set; }
        }

        public IEnumerable<List<Slot>> FetchSlotWeeks()
        {
            var evexweeks = Org.GetExtra(DbUtil.Db, "ExcludeWeeks") ?? "";
            var exweeks = evexweeks.Split(',').Select(ww => ww.ToInt()).ToArray();
            if (SortByWeek)
            {
                return from slot in FetchSlots()
                       where !exweeks.Contains(slot.Week) && slot.Week == WeekNumber
                       group slot by slot.Sunday into g
                       where g.Any(gg => gg.Time > DateTime.Today)
                       orderby g.Key.WeekOfMonth(), g.Key
                       select g.OrderBy(gg => gg.Time).ToList();
            }

            return from slot in FetchSlots()
                   where !exweeks.Contains(slot.Week)
                   group slot by slot.Sunday into g
                   where g.Any(gg => gg.Time > DateTime.Today)
                   orderby g.Key
                   select g.OrderBy(gg => gg.Time).ToList();
        }

        public List<List<Slot>> FetchSlotWeeksByMonth(int? curMonth)
        {
            var evexweeks = Org.GetExtra(DbUtil.Db, "ExcludeWeeks") ?? "";
            var exweeks = evexweeks.Split(',').Select(ww => ww.ToInt()).ToArray();
            if (SortByWeek)
            {
                var weekSorted = from slot in FetchSlots()
                                 where !exweeks.Contains(slot.Week) && slot.Week == WeekNumber
                                 group slot by slot.Sunday into g
                                 where g.Any(gg => gg.Time >= DateTime.Today)
                                 orderby g.Key.WeekOfMonth(), g.Key
                                 select g.OrderBy(gg => gg.Time).ToList();
                return weekSorted.ToList();
            }
            var monthWeeks = from slot in FetchSlotsByMonth(CurMonth)
                             where !exweeks.Contains(slot.Week)
                             group slot by slot.Sunday into g
                             where g.Any(gg => gg.Time >= DateTime.Today)
                             orderby g.Key.WeekOfMonth(), g.Key
                             select g.OrderBy(gg => gg.Time).ToList();
            return monthWeeks.ToList();
        }

        public IEnumerable<Slot> FetchSlotsByMonth(int? curMonth)
        {
            var mlist = (from m in DbUtil.Db.Meetings
                         where m.MeetingDate > Util.Now.Date
                         where m.OrganizationId == OrgId
                         orderby m.MeetingDate
                         select m).ToList();
            if (curMonth.IsNull() && mlist.Count > 0)
            {
                curMonth = mlist[0].MeetingDate.Value.Month;
            }

            //var mfinallist = mlist.Where(k => k.MeetingDate.Value.Month == curMonth).ToList();

            var alist = (from a in DbUtil.Db.AttendCommitments(OrgId)
                         orderby a.MeetingDate
                         select a).ToList();

            var list = new List<Slot>();

            DateTime calcSunday = Sunday;

            if (curMonth != Sunday.Month)
            {
                DateTime firstDayOfMonth = new DateTime(CurYear.ToInt(), CurMonth.ToInt(), 1);
                calcSunday = firstDayOfMonth.AddDays(7 - (int)firstDayOfMonth.DayOfWeek);
            }

            for (var sd = calcSunday; sd <= EndDt && sd.Month == curMonth; sd = sd.AddDays(7))
            {
                var dt = sd;
                {
                    var u = from ts in Regsettings.TimeSlots.list
                            orderby ts.Datetime()
                            let time = ts.Datetime(dt)
                            let meeting = mlist.SingleOrDefault(mm => mm.MeetingDate == time)
                            let needed = meeting != null ?
                                (from e in meeting.MeetingExtras
                                 where e.Field == "TotalVolunteersNeeded"
                                 select e.Data).SingleOrDefault().ToInt2() : null
                            let meetingid = meeting?.MeetingId ?? 0
                            select new Slot()
                            {
                                Time = time,
                                Sunday = dt,
                                Week = dt.WeekOfMonth(),
                                Disabled = time < DateTime.Now,
                                Limit = needed.ToInt2() ?? ts.Limit ?? 0,
                                Persons = (from a in alist
                                           where a.MeetingDate == time
                                           orderby a.Name2
                                           select new NameId
                                           {
                                               Name = a.Name2,
                                               PeopleId = a.PeopleId,
                                               Commitment = a.Commitment,
                                               OtherCommitments = a.Conflicts == true
                                           }).ToList(),
                                MeetingId = meetingid
                            };
                    list.AddRange(u);
                }
            }
            return list;
        }

        public IEnumerable<Slot> FetchSlots()
        {
            var mlist = (from m in DbUtil.Db.Meetings
                         where m.MeetingDate > Util.Now.Date
                         where m.OrganizationId == OrgId
                         orderby m.MeetingDate
                         select m).ToList();

            var alist = (from a in DbUtil.Db.AttendCommitments(OrgId)
                         orderby a.MeetingDate
                         select a).ToList();

            var list = new List<Slot>();
            for (var sd = Sunday; sd <= EndDt; sd = sd.AddDays(7))
            {
                var dt = sd;
                {
                    var u = from ts in Regsettings.TimeSlots.list
                            orderby ts.Datetime()
                            let time = ts.Datetime(dt)
                            let meeting = mlist.SingleOrDefault(mm => mm.MeetingDate == time)
                            let needed = meeting != null ?
                                    (from e in meeting.MeetingExtras
                                     where e.Field == "TotalVolunteersNeeded"
                                     select e.Data).SingleOrDefault().ToInt2() : null
                            let meetingid = meeting?.MeetingId ?? 0
                            select new Slot()
                            {
                                Time = time,
                                Sunday = dt,
                                Week = dt.WeekOfMonth(),
                                Disabled = time < DateTime.Now,
                                Limit = needed.ToInt2() ?? ts.Limit ?? 0,
                                Persons = (from a in alist
                                           where a.MeetingDate == time
                                           orderby a.Name2
                                           select new NameId
                                           {
                                               Name = a.Name2,
                                               PeopleId = a.PeopleId,
                                               Commitment = a.Commitment,
                                               OtherCommitments = a.Conflicts == true
                                           }).ToList(),
                                MeetingId = meetingid
                            };
                    list.AddRange(u);
                }
            }
            return list;
        }
        private Organization org;
        public Organization Org
        {
            get
            {
                return org ??
                    (org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == OrgId));
            }
        }
        private DateTime? endDt;
        public DateTime EndDt
        {
            get
            {
                if (!endDt.HasValue)
                {
                    var dt = Org.LastMeetingDate ?? DateTime.MinValue;
                    if (dt == DateTime.MinValue)
                    {
                        dt = DateTime.Today.AddMonths(7);
                    }

                    endDt = dt;
                }
                return endDt.Value;
            }
        }

        private DateTime? sunday;
        public DateTime Sunday
        {
            get
            {
                if (!sunday.HasValue)
                {
                    var dt = Org.FirstMeetingDate ?? DateTime.MinValue;
                    if (dt == DateTime.MinValue || dt < DateTime.Today)
                    {
                        dt = DateTime.Today;
                    }

                    sunday = dt.AddDays(-(int)dt.DayOfWeek);
                }
                return sunday.Value;
            }
        }
        private Settings regsettings;
        public Settings Regsettings => regsettings ?? (regsettings = DbUtil.Db.CreateRegistrationSettings(OrgId));

        public void ApplyDragDrop(
            string target,
            int? week,
            DateTime? time,
            DragDropInfo i)
        {
            List<int> volids = null;

            switch (i.source)
            {
                case "nocommits":
                    volids = (from p in Volunteers()
                              where (p.Commits ?? false) == false
                              select p.PeopleId).ToList();
                    break;
                case "commits":
                    volids = (from p in Volunteers()
                              where p.Commits ?? false
                              select p.PeopleId).ToList();
                    break;
                case "all":
                    volids = (from p in Volunteers()
                              select p.PeopleId).ToList();
                    break;
                case "registered":
                case "person":
                    volids = new List<int>() { i.pid.ToInt() };
                    break;
                default:
                    return;
            }

            if (target == "week")
            {
                var slots = (from s in FetchSlots()
                             where time != null && s.Time.TimeOfDay == time.Value.TimeOfDay
                             where time != null && s.Time.DayOfWeek == time.Value.DayOfWeek
                             where s.Week == week || week == 0
                             select s).ToList();
                foreach (var peopleId in volids)
                {
                    if (i.source == "registered")
                    {
                        DropFromAll(peopleId);
                    }

                    foreach (var s in slots)
                    {
                        Attend.MarkRegistered(DbUtil.Db, OrgId, peopleId, s.Time,
                            AttendCommitmentCode.Attending, AvoidRegrets: true);
                    }
                }
            }
            else if (target == "meeting")
            {
                foreach (var peopleId in volids)
                {
                    if (i.source == "registered")
                    {
                        if (i.mid != null)
                        {
                            DropFromMeeting(i.mid.Value, peopleId);
                        }
                    }

                    if (time != null)
                    {
                        Attend.MarkRegistered(DbUtil.Db, OrgId, peopleId, time.Value,
                            AttendCommitmentCode.Attending, AvoidRegrets: true);
                    }
                }

            }
            else if (target == "clear")
            {
                foreach (var peopleId in volids)
                {
                    if (i.source == "registered")
                    {
                        if (i.mid != null)
                        {
                            DropFromMeeting(i.mid.Value, peopleId);
                        }
                    }
                    else
                    {
                        DropFromAll(peopleId);
                    }
                }
            }
        }
        private void DropFromMeeting(int meetingid, int peopleid)
        {
            DbUtil.Db.ExecuteCommand(@"
DELETE dbo.SubRequest 
FROM dbo.SubRequest sr
JOIN dbo.Attend a ON a.AttendId = sr.AttendId
WHERE a.MeetingId = {0}
AND a.PeopleId = {1}
", meetingid, peopleid);
            DbUtil.Db.ExecuteCommand("DELETE dbo.Attend WHERE MeetingId = {0} AND PeopleId = {1}", meetingid, peopleid);
        }
        private void DropFromAll(int peopleid)
        {
            DbUtil.Db.ExecuteCommand(@"
DELETE dbo.SubRequest 
FROM dbo.SubRequest sr
JOIN dbo.Attend a ON a.AttendId = sr.AttendId
WHERE a.OrganizationId = {0}
AND a.MeetingDate > {1}
AND a.PeopleId = {2}
", OrgId, Sunday, peopleid);

            DbUtil.Db.ExecuteCommand("DELETE dbo.Attend WHERE OrganizationId = {0} AND MeetingDate > {1} AND PeopleId = {2} AND ISNULL(Commitment, 1) = 1", OrgId, Sunday, peopleid);
        }
    }
    public class DragDropInfo
    {
        public string source { get; set; }
        public int? pid { get; set; }
        public int? mid { get; set; }
    }
}
