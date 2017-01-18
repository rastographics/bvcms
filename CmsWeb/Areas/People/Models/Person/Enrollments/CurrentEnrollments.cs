using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using CmsData;
using CmsWeb.Models;
using MoreLinq;
using UtilityExtensions;
using CmsData.Classes.RoleChecker;

namespace CmsWeb.Areas.People.Models
{
    public class CurrentEnrollments : PagedTableModel<OrganizationMember, OrgMemberInfo>
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
                    var isInAccess = WebPageContext.Current?.Page?.User?.IsInRole("Access") ?? false;
                    var isInOrgLeadersOnly = WebPageContext.Current?.Page?.User?.IsInRole("OrgLeadersOnly") ?? false;
                    string defaultFilter = null;

                    if(isInAccess && !isInOrgLeadersOnly)
                        defaultFilter = DbUtil.Db.Setting("UX-DefaultAcccessInvolvementOrgTypeFilter", "");
                    else
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
                return DefineModelList(false).Select(x => x.Organization.OrganizationType.Description).Distinct().Where(x => !excludedTypes.Contains(x));
            }
        } 

        public CurrentEnrollments()
            : base("default", "asc", true)
        {}

        public IQueryable<OrganizationMember> DefineModelList(bool useOrgFilter)
        {
            var limitvisibility = Util2.OrgLeadersOnly || !HttpContext.Current.User.IsInRole("Access");
            var oids = new int[0];
            if (Util2.OrgLeadersOnly)
                oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            var roles = DbUtil.Db.CurrentRoles();

            var modelList = from om in DbUtil.Db.OrganizationMembers
                            let org = om.Organization
                            where om.PeopleId == PeopleId
                            where (om.Pending ?? false) == false
                            where oids.Contains(om.OrganizationId) || !(limitvisibility && om.Organization.SecurityTypeId == 3)
                            where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                            where (!useOrgFilter || !OrgTypesFilter.Any() || OrgTypesFilter.Contains(om.Organization.OrganizationType.Description))
                            select om;

            return modelList;
        } 

        override public IQueryable<OrganizationMember> DefineModelList()
        {
            return DefineModelList(true);
        }
        override public IQueryable<OrganizationMember> DefineModelSort(IQueryable<OrganizationMember> q)
        {
            switch (SortExpression)
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
            var viewList = from om in q
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
                       HasDirectory = (om.Organization.PublishDirectory ?? 0) > 0,
                       IsLeaderAttendanceType = (om.MemberType.AttendanceTypeId ?? 0) == 10
                   };

            if (RoleChecker.HasSetting(SettingName.ShowChildOrgsOnInvolvementTabs, false))
            {
                var viewListAsList = viewList.ToList();
                var parentIds = viewListAsList.Select(x => x.OrgId).ToList();

                var childGroups = from org in DbUtil.Db.Organizations
                                   where org.ParentOrgId.HasValue && parentIds.Contains(org.ParentOrgId.Value)
                                   group new OrgMemberInfo
                                   {
                                       OrgId = org.OrganizationId,
                                       Name = org.OrganizationName,
                                       Location = org.Location,
                                       LeaderName = org.LeaderName,
                                       MeetingTime = org.OrgSchedules.FirstOrDefault().MeetingTime,
                                       LeaderId = org.LeaderId,
                                       DivisionName = org.Division.Name,
                                       ProgramName = org.Division.Program.Name,
                                       OrgType = org.OrganizationType.Description ?? "Other",
                                       HasDirectory = (org.PublishDirectory ?? 0) > 0
                                   } by org.ParentOrgId into childGroup
                                   select childGroup;

                foreach (var group in viewListAsList)
                {
                    var childOrgs = childGroups.FirstOrDefault(x => x.Key == group.OrgId)?.ToList();
                    if (childOrgs != null)
                    {
                        childOrgs.ForEach(x => x.IsLeaderAttendanceType = group.IsLeaderAttendanceType);
                        group.ChildOrgs = childOrgs;
                    }
                }

                return viewListAsList;
            }

            return viewList;
        }
    }
}
