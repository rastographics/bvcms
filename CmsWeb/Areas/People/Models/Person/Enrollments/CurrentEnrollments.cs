using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class CurrentEnrollments
    {
        public class OrgMemberInfo
        {
            public int OrgId { get; set; }
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public string Location { get; set; }
            public string LeaderName { get; set; }
            public DateTime? MeetingTime { get; set; }
            public string Schedule { get { return "{0:ddd h:mm tt}".Fmt(MeetingTime); } }
            public string SchComma { get { return MeetingTime.HasValue ? ", " : ""; } }
            public string LocComma { get { return Location.HasValue() ? ", " : ""; } }
            public string MemberType { get; set; }
            public int? LeaderId { get; set; }
            public DateTime? EnrollDate { get; set; }
            public DateTime? DropDate { get; set; }
            public Decimal? AttendPct { get; set; }
            public string DivisionName { get; set; }
            public string ProgramName { get; set; }
        	public string OrgType { get; set; }
        	public string HasDirectory { get; set; }
        }
        private int PeopleId;
        public CmsData.Person person { get; set; }
        public CmsWeb.Models.PagerModel2 Pager { get; set; }
        public CurrentEnrollments(int id)
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
            Pager = new CmsWeb.Models.PagerModel2(Count);
        }
        private IQueryable<OrganizationMember> _enrollments;
        private IQueryable<OrganizationMember> FetchEnrollments()
        {
            if (_enrollments == null)
            {
                var limitvisibility = Util2.OrgMembersOnly || Util2.OrgLeadersOnly
                    || !HttpContext.Current.User.IsInRole("Access");
                var oids = new int[0];
                if (Util2.OrgLeadersOnly)
                    oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            	var roles = DbUtil.Db.CurrentRoles();
                _enrollments = from om in DbUtil.Db.OrganizationMembers
							   let org = om.Organization
                               where om.PeopleId == PeopleId
                               where (om.Pending ?? false) == false
                               where oids.Contains(om.OrganizationId) || !(limitvisibility && om.Organization.SecurityTypeId == 3) 
							   where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                               select om;
            }
            return _enrollments;
        }
        int? _count;
        public int Count()
        {
            if (!_count.HasValue)
                _count = FetchEnrollments().Count();
            return _count.Value;
        }
        public IEnumerable<OrgMemberInfo> Enrollments()
        {
            var q = ApplySort();
            q = q.Skip(Pager.StartRow).Take(Pager.PageSize);
            var q2 = from om in q
                     let sc = om.Organization.OrgSchedules.FirstOrDefault() // SCHED
                     select new OrgMemberInfo
                     {
                         OrgId = om.OrganizationId,
                         PeopleId = om.PeopleId,
                         Name = om.Organization.OrganizationName,
                         Location = om.Organization.Location,
                         LeaderName = om.Organization.LeaderName,
                         MeetingTime = sc.MeetingTime,
                         MemberType = om.MemberType.Description,
                         LeaderId = om.Organization.LeaderId,
                         EnrollDate = om.EnrollmentDate,
                         AttendPct = om.AttendPct,
                         DivisionName = om.Organization.Division.Name,
                         ProgramName = om.Organization.Division.Program.Name,
						 OrgType = om.Organization.OrganizationType.Description ?? "Other"
                     };
            return q2;
        }
        private IQueryable<OrganizationMember> ApplySort()
        {
            var q = FetchEnrollments();
            switch (Pager.SortExpression)
            {
                case "Enroll Date":
                case "Enroll Date desc":
					q = from om in q
						orderby om.Organization.OrganizationType.Code ?? "z", om.EnrollmentDate
						select om;
                    break;
				default:
					q = from om in q
						orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
						select om;
                    break;
            }
            return q;
        }

    }
}
