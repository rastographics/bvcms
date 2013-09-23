using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class PendingEnrollments : PagedTableModel<OrganizationMember, OrgMemberInfo>
    {
        readonly int PeopleId;
        public CmsData.Person person { get; set; }
        public PendingEnrollments(int id)
            : base("", "")
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
        }

        public override IQueryable<OrganizationMember> DefineModelList()
        {
            var roles = DbUtil.Db.CurrentRoles();
            return from o in DbUtil.Db.Organizations
                   from om in o.OrganizationMembers
                   where om.PeopleId == PeopleId && om.Pending.Value == true
                   where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                   select om;
        }

        public override IQueryable<OrganizationMember> DefineModelSort(IQueryable<OrganizationMember> q)
        {
            return q.OrderBy(m => m.Organization.OrganizationName);
        }

        public override IEnumerable<OrgMemberInfo> DefineViewList(IQueryable<OrganizationMember> q)
        {
            return from om in q
                   let sc = om.Organization.OrgSchedules.FirstOrDefault() // SCHED
                   let o = om.Organization
                   let leader = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == om.Organization.LeaderId)
                   select new OrgMemberInfo
                   {
                       OrgId = om.OrganizationId,
                       PeopleId = om.PeopleId,
                       Name = o.OrganizationName,
                       Location = o.Location,
                       LeaderName = leader.Name,
                       MeetingTime = sc.MeetingTime,
                       LeaderId = o.LeaderId,
                       EnrollDate = om.EnrollmentDate,
                       MemberType = om.MemberType.Description,
                       DivisionName = om.Organization.Division.Program.Name + "/" + om.Organization.Division.Name,
                   };
        }
    }
}
