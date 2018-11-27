using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public class VolunteerModel
    {
        private DateTime? _endDt;
        private Organization _org;
        private Person _person;
        private DateTime? _sunday;
        private Settings setting;

        public VolunteerModel(int orgId, int peopleId, bool leader = false)
        {
            OrgId = orgId;
            PeopleId = peopleId;
            dtlock = DateTime.Now.AddDays(Setting.TimeSlots.TimeSlotLockDays ?? 0);
            IsLeader = leader;
            SendEmail = leader == false;
        }

        public VolunteerModel()
        {
        }

        public int OrgId { get; set; }
        public int PeopleId { get; set; }
        public bool IsLeader { get; set; }
        public bool SendEmail { get; set; }
        public DateTime[] Commit { get; set; }
        public DateTime dtlock { get; set; }

        public Organization Org
        {
            get
            {
                return _org ??
                       (_org = DbUtil.Db.Organizations.Single(oo => oo.OrganizationId == OrgId));
            }
        }

        public Person Person
        {
            get { return _person ?? (_person = DbUtil.Db.People.Single(pp => pp.PeopleId == PeopleId)); }
        }

        public DateTime EndDt
        {
            get
            {
                if (!_endDt.HasValue)
                {
                    var dt = Org.LastMeetingDate ?? DateTime.MinValue;
                    if (dt == DateTime.MinValue)
                    {
                        dt = DateTime.Today.AddMonths(7);
                    }

                    _endDt = dt;
                }
                return _endDt.Value;
            }
        }

        public DateTime Sunday
        {
            get
            {
                if (!_sunday.HasValue)
                {
                    var dt = Org.FirstMeetingDate ?? DateTime.MinValue;
                    if (dt == DateTime.MinValue || dt < DateTime.Today)
                    {
                        dt = DateTime.Today;
                    }

                    _sunday = dt.AddDays(-(int)dt.DayOfWeek);
                }
                return _sunday.Value;
            }
        }

        public string Instructions => $@"
<div class=""instructions login"">{Setting.InstructionLogin}</div>
<div class=""instructions select"">{Setting.InstructionSelect}</div>
<div class=""instructions find"">{Setting.InstructionFind}</div>
<div class=""instructions options"">{Setting.InstructionOptions}</div>
<div class=""instructions special"">{Setting.InstructionSpecial}</div>
<div class=""instructions submit"">{Setting.InstructionSubmit}</div>
<div class=""instructions sorry"">{Setting.InstructionSorry}</div>
";
        public Settings Setting => setting ?? (setting = DbUtil.Db.CreateRegistrationSettings(OrgId));

        public IEnumerable<List<Slot>> FetchSlotWeeks()
        {
            var evexweeks = Org.GetExtra(DbUtil.Db, "ExcludeWeeks") ?? "";
            var exweeks = evexweeks.Split(',').Select(ww => ww.ToInt()).ToArray();
            return from slot in FetchSlots()
                   where !exweeks.Contains(slot.Week)
                   group slot by slot.Sunday
                   into g
                   where g.Any(gg => gg.Time > DateTime.Today)
                   orderby g.Key
                   select g.OrderBy(gg => gg.Time).ToList();
        }

        public List<DateInfo> Meetings()
        {
            return (from a in DbUtil.Db.Attends
                    where a.OrganizationId == OrgId
                    where a.MeetingDate >= Sunday
                    where a.MeetingDate <= EndDt.AddHours(24)
                    where AttendCommitmentCode.committed.Contains(a.Commitment ?? 0)
                    group a by a.MeetingDate
                    into g
                    let attend = (from aa in g
                                  where aa.PeopleId == PeopleId
                                  select aa).SingleOrDefault()
                    select new DateInfo
                    {
                        attend = attend,
                        MeetingDate = g.Key,
                        count = g.Count(),
                        iscommitted = attend != null
                    }).ToList();
        }

        public IEnumerable<Slot> FetchSlots()
        {
            var list = new List<Slot>();
            var sunday = Sunday;
            var meetings = Meetings();
            for (; sunday <= EndDt; sunday = sunday.AddDays(7))
            {
                var dt = sunday;
                {
                    var q = from ts in Setting.TimeSlots.list
                            orderby ts.Datetime()
                            let time = ts.Datetime(dt)
                            let meeting = meetings.SingleOrDefault(cc => cc.MeetingDate == time)
                            let count = meeting?.count ?? 0
                            select new Slot
                            {
                                Description = ts.Description,
                                AttendId = meeting != null ? (meeting.attend?.AttendId ?? 0) : 0,
                                Checked = meeting != null && meeting.iscommitted,
                                Time = time,
                                Sunday = dt,
                                Month = dt.Month,
                                Week = dt.WeekOfMonth(),
                                Year = dt.Year,
                                Full = meeting != null && meeting.count >= ts.Limit,
                                Need = (ts.Limit ?? 0) - count,
                                Disabled = time < DateTime.Now
                            };
                    list.AddRange(q);
                }
            }
            return list;
        }

        public void UpdateCommitments()
        {
            var commitments = (from m in Meetings()
                               where m.iscommitted
                               select m.MeetingDate).ToList();

            if (Commit == null)
            {
                Commit = new DateTime[] { };
            }

            var decommits = from currcommit in commitments
                            join newcommit in Commit on currcommit equals newcommit into j
                            from newcommit in j.DefaultIfEmpty(DateTime.MinValue)
                            where newcommit == DateTime.MinValue
                            select currcommit;

            var commits = from newcommit in Commit
                          join currcommit in commitments on newcommit equals currcommit into j
                          from currcommit in j.DefaultIfEmpty(DateTime.MinValue)
                          where currcommit == DateTime.MinValue
                          select newcommit;

            int? mid = null;
            foreach (var currcommit in decommits)
            {
                mid = Attend.MarkRegistered(DbUtil.Db, OrgId, PeopleId, currcommit, AttendCommitmentCode.Regrets);
            }

            foreach (var newcommit in commits)
            {
                mid = Attend.MarkRegistered(DbUtil.Db, OrgId, PeopleId, newcommit, AttendCommitmentCode.Attending);
            }

            if (mid.HasValue)
            {
                var slots = FetchSlots();
                Meeting.AddEditExtraData(DbUtil.Db, mid.Value, "Description", slots.First().Description);
            }
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(mm => mm.PeopleId == PeopleId && mm.OrganizationId == OrgId);
            if (om == null)
            {
                OrganizationMember.InsertOrgMembers(DbUtil.Db,
                    OrgId, PeopleId, MemberTypeCode.Member, DateTime.Now, null, false);
            }
        }

        public string Summary(CmsController controller)
        {
            var q = from i in FetchSlots()
                    where i.Checked
                    select i;
            return !q.Any() ? "no commitments"
                : ViewExtensions2.RenderPartialViewToString(controller, "ManageVolunteer/VolunteerSlotsSummary", q);
        }

        public SelectList Volunteers()
        {
            var q = from m in DbUtil.Db.OrganizationMembers
                    where m.OrganizationId == OrgId
                    where m.MemberTypeId != MemberTypeCode.InActive
                    orderby m.Person.Name2
                    select new { m.PeopleId, m.Person.Name };
            return new SelectList(q, "PeopleId", "Name", PeopleId);
        }

        public class DateInfo
        {
            public Attend attend { get; set; }
            public DateTime MeetingDate { get; set; }
            public int count { get; set; }
            public bool iscommitted { get; set; }
        }

        public class Slot
        {
            public int AttendId { get; set; }
            public string Description { get; set; }
            public DateTime Time { get; set; }
            public DateTime Sunday { get; set; }
            public int Year { get; set; }
            public int Month { get; set; }
            public int Week { get; set; }
            public bool Full { get; set; }
            public int? Need { get; set; }
            public bool Checked { get; set; }
            public bool Disabled { get; set; }
            public string CHECKED => Checked ? "checked=\"checked\"" : "";
            public string DISABLED => Disabled ? "disabled=\"true\"" : "";
        }
    }
}
