using CmsData;
using CmsData.Codes;
using MoreLinq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class PromotionModel
    {
        private bool? _filterUnassigned;
        private bool? _normalMembersOnly;
        private Promotion _promotion;
        private int? _promotionId;
        private int? _scheduleId;
        private string[] _selected;

        public PromotionModel()
        {
        }

        public PromotionModel(int id)
            : this()
        {
            PromotionId = id;
        }

        public int? PromotionId
        {
            get
            {
                if (_promotionId != null)
                {
                    return _promotionId;
                }

                _promotionId = DbUtil.Db.UserPreference("PromotionId", "0").ToInt2();
                return _promotionId;
            }
            set
            {
                _promotionId = value;
                DbUtil.Db.SetUserPreference("PromotionId", value);
            }
        }

        public Promotion Promotion
        {
            get
            {
                if (_promotion == null)
                {
                    _promotion = DbUtil.Db.Promotions.SingleOrDefault(p => p.Id == PromotionId);
                    if (_promotion == null)
                    {
                        return new Promotion { FromDivId = 0 };
                    }
                }
                return _promotion;
            }
        }

        public int? ScheduleId
        {
            get
            {
                if (_scheduleId != null)
                {
                    return _scheduleId;
                }

                _scheduleId = DbUtil.Db.UserPreference("ScheduleId", "0").ToInt2();
                return _scheduleId;
            }
            set
            {
                _scheduleId = value;
                DbUtil.Db.SetUserPreference("ScheduleId", value);
            }
        }

        public int TargetClassId { get; set; }

        public bool FilterUnassigned
        {
            get
            {
                if (_filterUnassigned != null)
                {
                    return _filterUnassigned.Value;
                }

                _filterUnassigned = DbUtil.Db.UserPreference("FilterUnassigned", "false").ToBool();
                return _filterUnassigned.Value;
            }
            set
            {
                _filterUnassigned = value;
                DbUtil.Db.SetUserPreference("FilterUnassigned", value);
            }
        }

        public bool NormalMembersOnly
        {
            get
            {
                if (_normalMembersOnly != null)
                {
                    return _normalMembersOnly.Value;
                }

                _normalMembersOnly = DbUtil.Db.UserPreference("NormalMembersOnly", "false").ToBool();
                return _normalMembersOnly.Value;
            }
            set
            {
                _normalMembersOnly = value;
                DbUtil.Db.SetUserPreference("NormalMembersOnly", value);
            }
        }

        public string[] selected
        {
            get
            {
                if (_selected == null)
                {
                    _selected = new string[0];
                }

                return _selected;
            }
            set { _selected = value; }
        }

        public string Sort { get; set; } = "Mixed";
        public string Dir { get; set; } = "asc";

        public IEnumerable<PromoteInfo> FetchStudents()
        {
            var fromdiv = Promotion.FromDivId;

            var q = from om in DbUtil.Db.OrganizationMembers
                    let sc = om.Organization.OrgSchedules.FirstOrDefault() // SCHED
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where ScheduleId == 0 || om.Organization.OrgSchedules.Any(os => os.ScheduleId == ScheduleId)
                    where (om.Pending ?? false) == false
                    where !NormalMembersOnly || om.MemberTypeId == MemberTypeCode.Member
                    let pcid = om.OrgMemberExtras.Where(vv => vv.Field == "PromotingTo").Select(vv => vv.IntValue).SingleOrDefault()
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                        op.Pending == true
                        && op.PeopleId == om.PeopleId
                        && op.OrganizationId == pcid)
                    let pt = pc.Organization.OrganizationMembers.FirstOrDefault(om2 =>
                        om2.Pending == true
                        && om2.MemberTypeId == MemberTypeCode.Teacher)
                    let psc = pc.Organization.OrgSchedules.FirstOrDefault() // SCHED
                    where !FilterUnassigned || pc == null
                    select new PromoteInfo
                    {
                        IsSelected = selected.Contains(om.PeopleId + "," + om.OrganizationId),
                        PeopleId = om.PeopleId,
                        Name = om.Person.Name,
                        Name2 = om.Person.Name2,
                        AttendPct = om.AttendPct,
                        BDay = om.Person.BirthDay,
                        BMon = om.Person.BirthMonth,
                        BYear = om.Person.BirthYear,
                        CurrClassId = om.OrganizationId,
                        CurrOrgName = om.Organization.OrganizationName,
                        CurrLeader = om.Organization.LeaderName,
                        CurrLoc = om.Organization.Location,
                        CurrSchedule = sc.MeetingTime.ToString2("t"),
                        Gender = om.Person.GenderId == 1 ? "M" : "F",
                        PendingClassId = pc == null ? (int?)null : pc.OrganizationId,
                        PendingOrgName = pc == null ? "" : pc.Organization.OrganizationName,
                        PendingLeader = pc == null ? "" : (pt != null ? pt.Person.Name : pc.Organization.LeaderName),
                        PendingLoc = pc == null ? "" : pc.Organization.Location,
                        PendingSchedule = psc.MeetingTime.ToString2("t"),
                        Hash = om.Person.HashNum.Value
                    };
            if (Dir == "asc")
            {
                switch (Sort)
                {
                    case "Mixed":
                        q = q.OrderBy(i => i.Hash);
                        break;
                    case "Name":
                        q = q.OrderBy(i => i.Name2);
                        break;
                    case "Current Class":
                        q = q.OrderBy(i => i.CurrOrgName);
                        break;
                    case "Pending Class":
                        q = q.OrderBy(i => i.PendingOrgName);
                        break;
                    case "Attendance":
                        q = q.OrderBy(i => i.AttendPct);
                        break;
                    case "Gender":
                        q = q.OrderBy(i => i.Gender);
                        break;
                    case "Birthday":
                        q = from i in q
                            orderby i.BYear, i.BMon, i.BDay
                            select i;
                        break;
                }
            }
            else
            {
                switch (Sort)
                {
                    case "Mixed":
                        q = q.OrderByDescending(i => i.Hash);
                        break;
                    case "Name":
                        q = q.OrderByDescending(i => i.Name2);
                        break;
                    case "Current Class":
                        q = q.OrderByDescending(i => i.CurrOrgName);
                        break;
                    case "Pending Class":
                        q = q.OrderByDescending(i => i.PendingOrgName);
                        break;
                    case "Attendance":
                        q = q.OrderByDescending(i => i.AttendPct);
                        break;
                    case "Gender":
                        q = q.OrderByDescending(i => i.Gender);
                        break;
                    case "Birthday":
                        q = from i in q
                            orderby i.BYear descending, i.BMon descending, i.BDay descending
                            select i;
                        break;
                }
            }

            return q;
        }

        public void AssignPending()
        {
            if (TargetClassId == 0)
            {
                RemoveAssignments();
                return;
            }
            var t = DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == TargetClassId);
            if (t == null)
            {
                return;
            }

            foreach (var i in selected)
            {
                var a = i.Split(',');
                var foid = a[1].ToInt();
                var pid = a[0].ToInt();

                // this is their membership where they are currently a member
                var fom = DbUtil.Db.OrganizationMembers.Single(m => m.OrganizationId == foid && m.PeopleId == pid);

                // drop pending in previously assigned to org
                var prevtoid = fom.GetExtra(DbUtil.Db, "PromotingTo").ToInt();
                var prevto = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == prevtoid && m.PeopleId == pid && m.Pending == true);
                prevto?.Drop(DbUtil.Db);
                DbUtil.Db.SubmitChanges();

                // now put them in the to class as pending member
                if (t.OrganizationId != fom.OrganizationId) // prevent promoting into the same class as they are currently in
                {
                    OrganizationMember.InsertOrgMembers(DbUtil.Db,
                        t.OrganizationId,
                        a[0].ToInt(),
                        fom.MemberTypeId, // keep their Existing membertype
                        Util.Now,
                        null, true);
                    // record where they will be going
                    fom.AddEditExtraInt("PromotingTo", t.OrganizationId);
                    DbUtil.Db.SubmitChanges();
                }
            }
            DbUtil.Db.UpdateMainFellowship(t.OrganizationId);
        }

        private void RemoveAssignments()
        {
            foreach (var i in selected)
            {
                var a = i.Split(',');
                var foid = a[1].ToInt();
                var pid = a[0].ToInt();

                // this is their membership where they are currently a member
                var fom = DbUtil.Db.OrganizationMembers.Single(m => m.OrganizationId == foid && m.PeopleId == pid);

                // drop pending in previously assigned to org
                var prevtoid = fom.GetExtra(DbUtil.Db, "PromotingTo").ToInt();
                var prevto = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.OrganizationId == prevtoid && m.PeopleId == pid && m.Pending == true);
                prevto?.Drop(DbUtil.Db);
                fom.RemoveExtraValue(DbUtil.Db, "PromotingTo");
                DbUtil.Db.SubmitChanges();
            }
        }

        public DataTable Export()
        {
            var fromdiv = Promotion.FromDivId;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where (om.Pending ?? false) == false
                    where om.MemberTypeId == MemberTypeCode.Member
                    let pcid = om.OrgMemberExtras.Where(vv => vv.Field == "PromotingTo").Select(vv => vv.IntValue).SingleOrDefault()
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                        op.Pending == true
                        && op.PeopleId == om.PeopleId
                        && op.OrganizationId == pcid)
                    let sc = pc.Organization.OrgSchedules.FirstOrDefault() // SCHED
                    let tm = sc.SchedTime.Value
                    let pt = pc.Organization.OrganizationMembers.FirstOrDefault(om2 =>
                        om2.Pending == true
                        && om2.MemberTypeId == MemberTypeCode.Teacher)
                    let ploc = pc.Organization.PendingLoc
                    where pc != null
                    select new
                    {
                        om.PeopleId,
                        Title = om.Person.TitleCode,
                        FirstName = om.Person.PreferredName,
                        om.Person.LastName,
                        Address = om.Person.PrimaryAddress,
                        Address2 = om.Person.PrimaryAddress2,
                        City = om.Person.PrimaryCity,
                        State = om.Person.PrimaryState,
                        Zip = om.Person.PrimaryZip.FmtZip(),
                        Email = om.Person.EmailAddress,
                        MemberType = om.MemberType.Description,
                        Location = (ploc == null || ploc == "") ? pc.Organization.Location : ploc,
                        Leader = pt != null ? pt.Person.Name : pc.Organization.LeaderName,
                        OrgName = pc.Organization.OrganizationName,
                        Schedule = tm.Hour + ":" + tm.Minute.ToString().PadLeft(2, '0'),
                        HomePhone = om.Person.HomePhone.FmtFone(),
                        CellPhone = om.Person.CellPhone.FmtFone(),
                        Parent1 = om.Person.Family.HeadOfHousehold.Name,
                        CellPhone1 = om.Person.Family.HeadOfHousehold.CellPhone.FmtFone(),
                        Parent2 = om.Person.Family.HeadOfHouseholdSpouse.Name,
                        CellPhone2 = om.Person.Family.HeadOfHouseholdSpouse.CellPhone.FmtFone(),
                        AttendPct = om.AttendPct.ToString2("N1"),
                        om.AttendStr
                    };
            return q.ToDataTable();
        }

        public IEnumerable<SelectListItem> Promotions()
        {
            var q = from p in DbUtil.Db.Promotions
                    orderby p.Sort, p.Description
                    select new SelectListItem
                    {
                        Text = $"{p.Sort} - {p.Description}",
                        Value = p.Id.ToString()
                    };
            var list = q.ToList();
            list.Insert(0, new SelectListItem { Text = "(Select Promotion)", Value = "0", Selected = true });
            return list;
        }

        public IEnumerable<SelectListItem> Schedules()
        {
            var q = from o in DbUtil.Db.Organizations
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where o.DivOrgs.Any(dd => dd.DivId == Promotion.FromDivId)
                    group o by new { sc.ScheduleId, sc.MeetingTime }
                    into g
                    orderby g.Key.ScheduleId
                    select new SelectListItem
                    {
                        Value = g.Key.ScheduleId.ToString(),
                        Text = DbUtil.Db.GetScheduleDesc(g.Key.MeetingTime)
                    };

            var list = q.ToList();
            if (list.Count == 0)
            {
                list.Insert(0, new SelectListItem { Text = "(Select Promotion First)", Value = "0", Selected = true });
            }
            else
            {
                list.Insert(0, new SelectListItem { Text = "(Select Schedule)", Value = "0", Selected = true });
            }

            return list;
        }

        public IEnumerable<SelectListItem> TargetClasses()
        {
            var todiv = Promotion.ToDivId;
            var roles = DbUtil.Db.CurrentRoles();
            var q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where o.DivOrgs.Any(dd => dd.DivId == todiv)
                    where sc.ScheduleId == ScheduleId || ScheduleId == 0
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    where sc != null
                    orderby o.OrganizationName
                    let pt = o.OrganizationMembers.FirstOrDefault(om2 =>
                        om2.Pending == true
                        && om2.MemberTypeId == MemberTypeCode.Teacher)
                    select new
                    {
                        Text = Organization.FormatOrgName(o.OrganizationName, pt != null ? pt.Person.Name : o.LeaderName, o.Location),
                        Time = sc.MeetingTime,
                        Value = o.OrganizationId.ToString()
                    };
            var list = (from i in q.ToList()
                        select new SelectListItem
                        {
                            Text = i.Text + $", {i.Time.Value:t}",
                            Value = i.Value
                        }).ToList();
            list.Add(new SelectListItem() { Text = "(Remove Assignment)", Value = "0" });
            return list;
        }
    }
}
