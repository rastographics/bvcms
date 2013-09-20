using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class CurrentEnrollments : PagedTableModel<OrganizationMember, OrgMemberInfo>
    {
        private int PeopleId;
        public CmsData.Person person { get; set; }
        public CurrentEnrollments(int id)
            : base("", "")
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
        }
        private IQueryable<OrganizationMember> enrollments;
        override public IQueryable<OrganizationMember> ModelList()
        {
            if (enrollments == null)
            {
                var limitvisibility = Util2.OrgMembersOnly || Util2.OrgLeadersOnly
                    || !HttpContext.Current.User.IsInRole("Access");
                var oids = new int[0];
                if (Util2.OrgLeadersOnly)
                    oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            	var roles = DbUtil.Db.CurrentRoles();
                enrollments = from om in DbUtil.Db.OrganizationMembers
							   let org = om.Organization
                               where om.PeopleId == PeopleId
                               where (om.Pending ?? false) == false
                               where oids.Contains(om.OrganizationId) || !(limitvisibility && om.Organization.SecurityTypeId == 3) 
							   where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                               select om;
            }
            return enrollments;
        }
        override public IEnumerable<OrgMemberInfo> ViewList()
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
        override public IQueryable<OrganizationMember> ApplySort()
        {
            var q = ModelList();
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
