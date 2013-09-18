using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models.Person
{
    public class PendingEnrollments
    {
        readonly int PeopleId;
        public CmsData.Person person { get; set; }
        public PendingEnrollments(int id)
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
        }
        public IEnumerable<OrgMemberInfo> Fetch()
        {
            var dt = Util.Now;
            var roles = DbUtil.Db.CurrentRoles();
            var q = from o in DbUtil.Db.Organizations
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    from om in o.OrganizationMembers
                    where om.PeopleId == PeopleId && om.Pending.Value == true
				    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    let leader = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == o.LeaderId)
                    orderby o.OrganizationName
                    select new OrgMemberInfo
                    {
                        OrgId = o.OrganizationId,
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
            return q;
        }
    }
}
