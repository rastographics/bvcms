using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class CurrentEnrollments : PagedTableModel<OrganizationMember, OrgMemberInfo>
    {
        private int PeopleId;
        public CmsData.Person person { get; set; }
        public CurrentEnrollments(int id, PagerModel2 pager)
            : base("", "", pager)
        {
            PeopleId = id;
            person = DbUtil.Db.LoadPersonById(id);
        }
        override public IQueryable<OrganizationMember> DefineModelList()
        {
            var limitvisibility = Util2.OrgMembersOnly || Util2.OrgLeadersOnly
                || !HttpContext.Current.User.IsInRole("Access");
            var oids = new int[0];
            if (Util2.OrgLeadersOnly)
                oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            var roles = DbUtil.Db.CurrentRoles();
            return from om in DbUtil.Db.OrganizationMembers
                   let org = om.Organization
                   where om.PeopleId == PeopleId
                   where (om.Pending ?? false) == false
                   where oids.Contains(om.OrganizationId) || !(limitvisibility && om.Organization.SecurityTypeId == 3)
                   where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                   select om;
        }
        override public IQueryable<OrganizationMember> DefineModelSort(IQueryable<OrganizationMember> q)
        {
            switch (Pager.SortExpression)
            {
                case "Enroll Date":
                    return from om in q
                        orderby om.EnrollmentDate descending // this is the natural order for date
                        select om;
                case "Enroll Date desc":
                    return from om in q
                        orderby om.EnrollmentDate
                        select om;
                case "Organization":
                    return from om in q
                        orderby om.Organization.OrganizationName
                        select om;
                case "Organization desc":
                    return from om in q
                        orderby om.Organization.OrganizationName descending
                        select om;
                case "default desc":
                    return from om in q
                        orderby om.Organization.OrganizationType.Code ?? "z" descending, om.Organization.OrganizationName descending 
                        select om;
                default:
                    return from om in q
                        orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
                        select om;
            }
        }

        public override IEnumerable<OrgMemberInfo> DefineViewList(IQueryable<OrganizationMember> q)
        {
            return from om in q
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
                       OrgType = om.Organization.OrganizationType.Description ?? "Other",
                       HasDirectory = (om.Organization.PublishDirectory ?? 0) > 0
                   };
        }
    }
}
