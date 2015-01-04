using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CmsData;
using MoreLinq;
using UtilityExtensions;
using System.Web.Mvc;
using CmsData.Codes;

namespace CmsWeb.Models
{
    public class PromotionModel
    {
        public PromotionModel()
        {
            
        }
        public PromotionModel(int id)
            :this()
        {
            PromotionId = id;
        }
        private int? _PromotionId;
        public int? PromotionId
        {
            get
            {
                if (_PromotionId != null)
                    return _PromotionId;
                _PromotionId = DbUtil.Db.UserPreference("PromotionId", "0").ToInt2();
                return _PromotionId;
            }
            set
            {
                _PromotionId = value;
                DbUtil.Db.SetUserPreference("PromotionId", value);
            }
        }

        private Promotion _Promotion;
        public Promotion Promotion
        {
            get
            {
                if (_Promotion == null)
                {
                    _Promotion = DbUtil.Db.Promotions.SingleOrDefault(p => p.Id == PromotionId);
                    if (_Promotion == null)
                        return new Promotion { FromDivId = 0 };
                }
                return _Promotion;
            }
        }
        private int? _ScheduleId;
        public int? ScheduleId
        {
            get
            {
                if (_ScheduleId != null)
                    return _ScheduleId;
                _ScheduleId = DbUtil.Db.UserPreference("ScheduleId", "0").ToInt2();
                return _ScheduleId;
            }
            set
            {
                _ScheduleId = value;
                DbUtil.Db.SetUserPreference("ScheduleId", value);
            }
        }

        public int TargetClassId { get; set; }

        private bool? _FilterUnassigned;
        public bool FilterUnassigned
        {
            get
            {
                if (_FilterUnassigned != null)
                    return _FilterUnassigned.Value;
                _FilterUnassigned = DbUtil.Db.UserPreference("FilterUnassigned", "false").ToBool();
                return _FilterUnassigned.Value;
            }
            set
            {
                _FilterUnassigned = value;
                DbUtil.Db.SetUserPreference("FilterUnassigned", value);
            }
        }

        private bool? _NormalMembersOnly;
        public bool NormalMembersOnly
        {
            get
            {
                if (_NormalMembersOnly != null)
                    return _NormalMembersOnly.Value;
                _NormalMembersOnly = DbUtil.Db.UserPreference("NormalMembersOnly", "false").ToBool();
                return _NormalMembersOnly.Value;
            }
            set
            {
                _NormalMembersOnly = value;
                DbUtil.Db.SetUserPreference("NormalMembersOnly", value);
            }
        }

        private string[] _Selected;
        public string[] selected
        {
            get
            {
                if (_Selected == null)
                    _Selected = new string[0];
                return _Selected;
            }
            set
            {
                _Selected = value;
            }
        }

        private string _Sort = "Mixed";
        public string Sort
        {
            get { return _Sort; }
            set { _Sort = value; }
        }
        private string _Dir = "asc";
        public string Dir
        {
            get { return _Dir; }
            set { _Dir = value; }
        }

        public IEnumerable<PromoteInfo> FetchStudents()
        {
            var fromdiv = Promotion.FromDivId;
            var todiv = Promotion.ToDivId;

            var q = from om in DbUtil.Db.OrganizationMembers
                    let sc = om.Organization.OrgSchedules.FirstOrDefault() // SCHED
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where ScheduleId == 0 || om.Organization.OrgSchedules.Any(os => os.ScheduleId == ScheduleId)
                    where (om.Pending ?? false) == false
                    where !NormalMembersOnly || om.MemberTypeId == MemberTypeCode.Member
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                       op.Pending == true
                       && op.PeopleId == om.PeopleId
                       && op.Organization.DivOrgs.Any(dd => dd.DivId == todiv))
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
                        Hash = om.Person.HashNum.Value,
                    };
            if (Dir == "asc")
                switch (Sort)
                {
                    case "Mixed":
                        q = q.OrderBy(i => i.Hash);
                        break;
                    case "Name":
                        q = q.OrderBy(i => i.Name2);
                        break;
                    case "CurrentClass":
                        q = q.OrderBy(i => i.CurrOrgName);
                        break;
                    case "PendingClass":
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
            else
                switch (Sort)
                {
                    case "Mixed":
                        q = q.OrderByDescending(i => i.Hash);
                        break;
                    case "Name":
                        q = q.OrderByDescending(i => i.Name2);
                        break;
                    case "CurrentClass":
                        q = q.OrderByDescending(i => i.CurrOrgName);
                        break;
                    case "PendingClass":
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

            return q;
        }

        public void AssignPending()
        {
            var t = DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == TargetClassId);
            if (t == null)
                return;
            var fromdiv = Promotion.FromDivId;
            var todiv = Promotion.ToDivId;

            foreach (var i in selected)
            {
                var a = i.Split(',');
                var q = from om in DbUtil.Db.OrganizationMembers
                        where om.Pending == true
                        where om.PeopleId == a[0].ToInt()
                        where om.Organization.DivOrgs.Any(dd => dd.DivId == todiv)
                        where ScheduleId == 0 || om.Organization.OrgSchedules.Any(os => os.ScheduleId == ScheduleId)
                        select om;
                // get them out of the class they will be going to first
                foreach (var pc in q)
                {
                    pc.Drop(DbUtil.Db);
                    DbUtil.Db.SubmitChanges();
                }
                // this is their membership where they are currently a member
                var fom = DbUtil.Db.OrganizationMembers.Single(m => m.OrganizationId == a[1].ToInt() && m.PeopleId == a[0].ToInt());
                // now put them in the to class as pending member
                var tom = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                    t.OrganizationId,
                    a[0].ToInt(),
                    fom.MemberTypeId, // keep their existing membertype
                    Util.Now,
                    null, 
                    pending: true);
                // todo: store the from orgid in tom record and use that do do promotion with
            }
			DbUtil.Db.UpdateMainFellowship(t.OrganizationId);
        }
        public DataTable Export()
        {
            var fromdiv = Promotion.FromDivId;
            var todiv = Promotion.ToDivId;
            var q = from om in DbUtil.Db.OrganizationMembers
                    where om.Organization.DivOrgs.Any(d => d.DivId == fromdiv)
                    where (om.Pending ?? false) == false
                    where om.MemberTypeId == MemberTypeCode.Member
                    let pc = DbUtil.Db.OrganizationMembers.FirstOrDefault(op =>
                       op.Pending == true
                       && op.PeopleId == om.PeopleId
                       && op.Organization.DivOrgs.Any(dd => dd.DivId == todiv))
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
                        Text = "{0} - {1}".Fmt(p.Sort, p.Description),
                        Value = p.Id.ToString(),
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
                    group o by new { sc.ScheduleId, sc.MeetingTime } into g
                    orderby g.Key.ScheduleId
                    select new SelectListItem
                    {
                        Value = g.Key.ScheduleId.ToString(),
                        Text = DbUtil.Db.GetScheduleDesc(g.Key.MeetingTime)
                    };

            var list = q.ToList();
            if (list.Count == 0)
                list.Insert(0, new SelectListItem { Text = "(Select Promotion First)", Value = "0", Selected = true });
            else
                list.Insert(0, new SelectListItem { Text = "(Select Schedule)", Value = "0", Selected = true });
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
                        Text = CmsData.Organization.FormatOrgName(o.OrganizationName, pt != null ? pt.Person.Name : o.LeaderName, o.Location),
                        Time = sc.MeetingTime,
                        Value = o.OrganizationId.ToString(),
                    };
            var list = q.ToList();
            var qq = from i in list
                     select new SelectListItem
                     {
                         Text = i.Text + ", {0:t}".Fmt(i.Time.Value),
                         Value = i.Value
                     };
            return qq;
        }
    }
}
