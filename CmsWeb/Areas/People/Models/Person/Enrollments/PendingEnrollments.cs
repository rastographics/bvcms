using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class PendingEnrollments : PagedTableModel<OrganizationMember, OrgMemberInfo>
    {
        public int? PeopleId { get; set; }
        public Person Person
        {
            get
            {
                if (_person == null && PeopleId.HasValue)
                    _person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                return _person;
            }
        }
        private Person _person;

        public List<string> OrgTypesFilter
        {
            get
            {
                if (_orgTypesFilter == null)
                {
                    var isInAccess = WebPageContext.Current.Page.User.IsInRole("Access");
                    string defaultFilter = null;

                    if (isInAccess)
                        defaultFilter = DbUtil.Db.Setting("UX-DefaultAcccessInvolvementOrgTypeFilter", null);

                    if (defaultFilter == null)
                        defaultFilter = DbUtil.Db.Setting("UX-DefaultInvolvementOrgTypeFilter", "");

                    _orgTypesFilter = string.IsNullOrEmpty(defaultFilter) ?
                        new List<string>() : defaultFilter.Split(',').Select(x => x.Trim()).ToList();
                }
                return _orgTypesFilter;
            }
            set
            {
                if (value.Any() && !string.IsNullOrWhiteSpace(value[0]))
                    _orgTypesFilter = value[0].Split(',').Select(x => x.Trim()).ToList();
                else
                    _orgTypesFilter = new List<string>();
            }
        }
        private List<string> _orgTypesFilter;

        public IEnumerable<string> OrgTypes
        {
            get
            {
                var excludedTypes =
                    DbUtil.Db.Setting("UX-ExcludeFromInvolvementOrgTypeFilter", "").Split(',').Select(x => x.Trim());
                return DbUtil.Db.OrganizationTypes.Where(x => !excludedTypes.Contains(x.Description)).Select(x => x.Description);
            }
        }

        public PendingEnrollments() 
            : base ("", "", true)
        {}

        override public IQueryable<OrganizationMember> DefineModelList()
        {
            var roles = DbUtil.Db.CurrentRoles();
            return from o in DbUtil.Db.Organizations
                   from om in o.OrganizationMembers
                   where om.PeopleId == PeopleId && om.Pending.Value == true
                   where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                   where (!OrgTypesFilter.Any() || OrgTypesFilter.Contains(om.Organization.OrganizationType.Description))
                   select om;
        }

        override public IQueryable<OrganizationMember> DefineModelSort(IQueryable<OrganizationMember> q)
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
