using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class CurrentEnrollments : PagedTableModel<InvolvementCurrent, OrgMemberInfo>
    {
        public int? PeopleId { get; set; }
        public Person Person
        {
            get
            {
                if (_person == null && PeopleId.HasValue)
                {
                    _person = DbUtil.Db.LoadPersonById(PeopleId.Value);
                }

                return _person;
            }
        }
        private Person _person;

        private bool IsInAccess => WebPageContext.Current?.Page?.User?.IsInRole("Access") ?? false;
        private bool IsInOrgLeadersOnly => WebPageContext.Current?.Page?.User?.IsInRole("OrgLeadersOnly") ?? false;

        public List<string> OrgTypesFilter
        {
            get
            {
                if (_orgTypesFilter == null)
                {
                    string defaultFilter;
                    if (IsInAccess && !IsInOrgLeadersOnly)
                    {
                        defaultFilter = DbUtil.Db.Setting("UX-DefaultAccessInvolvementOrgTypeFilter", "");
                    }
                    else
                    {
                        defaultFilter = DbUtil.Db.Setting("UX-DefaultInvolvementOrgTypeFilter", "");
                    }

                    _orgTypesFilter = string.IsNullOrEmpty(defaultFilter) ?
                        new List<string>() : defaultFilter.Split(',').Select(x => x.Trim()).ToList();
                }
                return _orgTypesFilter;
            }
            set
            {
                if (value.Any() && !string.IsNullOrWhiteSpace(value[0]))
                {
                    _orgTypesFilter = value[0].Split(',').Select(x => x.Trim()).ToList();
                }
                else
                {
                    _orgTypesFilter = new List<string>();
                }
            }
        }
        private List<string> _orgTypesFilter;

        public IEnumerable<string> OrgTypes
        {
            get
            {
                var excludedTypes =
                    DbUtil.Db.Setting("UX-ExcludeFromInvolvementOrgTypeFilter", "").Split(',').Select(x => x.Trim());
                return DefineModelList(false).Select(x => x.OrgType).Distinct().Where(x => !excludedTypes.Contains(x));
            }
        }

        public CurrentEnrollments()
            : base("default", "asc", true)
        { }

        public IQueryable<InvolvementCurrent> DefineModelList(bool useOrgFilter)
        {
            var limitvisibility = Util2.OrgLeadersOnly || !HttpContextFactory.Current.User.IsInRole("Access");
            var oids = new int[0];
            if (Util2.OrgLeadersOnly)
            {
                oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
            }

            var roles = DbUtil.Db.CurrentRoles();

            return from om in DbUtil.Db.InvolvementCurrent(PeopleId, Util.UserId)
                   where (om.Pending ?? false) == false
                   where oids.Contains(om.OrganizationId) || !(limitvisibility && om.SecurityTypeId == 3)
                   where om.LimitToRole == null || roles.Contains(om.LimitToRole)
                   where (!useOrgFilter || !OrgTypesFilter.Any() || OrgTypesFilter.Contains(om.OrgType))
                   select om;
        }

        public override IQueryable<InvolvementCurrent> DefineModelList()
        {
            return DefineModelList(true);
        }

        public override IQueryable<InvolvementCurrent> DefineModelSort(IQueryable<InvolvementCurrent> q)
        {
            switch (SortExpression)
            {
                case "Enroll Date":
                    return from om in q
                           orderby om.EnrollDate descending // this is the natural order for date
                           select om;
                case "Enroll Date desc":
                    return from om in q
                           orderby om.EnrollDate
                           select om;
                case "Organization":
                    return from om in q
                           orderby om.Name
                           select om;
                case "Organization desc":
                    return from om in q
                           orderby om.Name descending
                           select om;
                case "default desc":
                    return from om in q
                           orderby om.OrgCode ?? "z" descending, om.Name descending
                           select om;
                default:
                    return from om in q
                           orderby om.OrgTypeSort, om.OrgCode ?? "z", om.Name
                           select om;
            }
        }

        public override IEnumerable<OrgMemberInfo> DefineViewList(IQueryable<InvolvementCurrent> q)
        {
            var viewList = from om in q
                           select new OrgMemberInfo
                           {
                               OrgId = om.OrganizationId,
                               PeopleId = om.PeopleId,
                               Name = om.Name,
                               Location = om.Location,
                               LeaderName = om.LeaderName,
                               MeetingTime = om.MeetingTime,
                               MemberType = om.MemberType,
                               LeaderId = om.LeaderId,
                               EnrollDate = om.EnrollDate,
                               AttendPct = om.AttendPct,
                               DivisionName = om.DivisionName,
                               ProgramName = om.ProgramName,
                               OrgType = om.OrgType,
                               HasDirectory = om.HasDirectory.GetValueOrDefault(),
                               IsLeaderAttendanceType = om.IsLeaderAttendanceType.GetValueOrDefault(),
                               IsProspect = om.IsProspect ?? false,
                               IsMissionTripOrg = om.IsMissionTripOrg ?? false
                           };

            if (DbUtil.Db.Setting("UX-ShowChildOrgsOnInvolvementTabs"))
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
