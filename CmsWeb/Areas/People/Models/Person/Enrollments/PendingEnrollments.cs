using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
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

        private IQueryable<OrganizationMember> members;

        public override IQueryable<OrganizationMember> ModelList()
        {
            if (members != null)
                return members;
            var roles = DbUtil.Db.CurrentRoles();
            return members = from o in DbUtil.Db.Organizations
                             from om in o.OrganizationMembers
                             where om.PeopleId == PeopleId && om.Pending.Value == true
                             where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                             select om;
        }

        public override IQueryable<OrganizationMember> ApplySort()
        {
            return ModelList().OrderBy(m => m.Organization.OrganizationName);
        }

        override public IEnumerable<OrgMemberInfo> ViewList()
        {
            return from om in ApplySort()
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
