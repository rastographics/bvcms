using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsData.Codes;
using NPOI.SS.Formula.Functions;
using UtilityExtensions;

namespace CmsWeb.Models.PersonPage
{
    public class PersonEnrollmentsModel
    {
        private int PeopleId;
        public Person person { get; set; }
        public PagerModel2 Pager { get; set; }
        public PersonEnrollmentsModel(int id)
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
            Pager = new PagerModel2(Count);
        }
        private IQueryable<OrganizationMember> _enrollments;
        private IQueryable<OrganizationMember> FetchEnrollments()
        {
            if (_enrollments == null)
            {
                var mydata = !HttpContext.Current.User.IsInRole("Access");
                var limitvisibility = Util2.OrgMembersOnly || Util2.OrgLeadersOnly || mydata;
                var oids = new int[0];
                if (Util2.OrgLeadersOnly)
                    oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            	var roles = DbUtil.Db.CurrentRoles();
                _enrollments = from om in DbUtil.Db.OrganizationMembers
							   let org = om.Organization
                               where om.PeopleId == PeopleId
                               where !mydata || om.MemberTypeId != MemberTypeCode.Prospect
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
                     let orgtype = om.MemberTypeId == MemberTypeCode.Prospect ? "Prospect" : om.Organization.OrganizationType.Description ?? "Other"
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
                         DivisionName = om.Organization.Division.Program.Name + "/" + om.Organization.Division.Name,
						 OrgType = orgtype,
                         HasDirectory = (om.Organization.PublishDirectory ?? 0) > 0
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
