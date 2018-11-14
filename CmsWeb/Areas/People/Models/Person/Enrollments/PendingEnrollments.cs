using CmsData;
using CmsData.View;
using CmsWeb.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.WebPages;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class PendingEnrollments : PagedTableModel<InvolvementCurrent, OrgMemberInfo>
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

        public List<string> OrgTypesFilter
        {
            get
            {
                if (_orgTypesFilter == null)
                {
                    var isInAccess = WebPageContext.Current?.Page?.User?.IsInRole("Access") ?? false;
                    var isInOrgLeadersOnly = WebPageContext.Current?.Page?.User?.IsInRole("OrgLeadersOnly") ?? false;
                    string defaultFilter = null;

                    if (isInAccess && !isInOrgLeadersOnly)
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

        public PendingEnrollments()
            : base("", "", true)
        { }

        public IQueryable<InvolvementCurrent> DefineModelList(bool useOrgFilter)
        {
            var roles = DbUtil.Db.CurrentRoles();
            return from om in DbUtil.Db.InvolvementCurrent(PeopleId, Util.UserId)
                   where om.Pending.Value
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
            return q.OrderBy(m => m.OrgTypeSort).ThenBy(m => m.Name);
        }

        public override IEnumerable<OrgMemberInfo> DefineViewList(IQueryable<InvolvementCurrent> q)
        {
            return from om in q
                   select new OrgMemberInfo
                   {
                       OrgId = om.OrganizationId,
                       PeopleId = om.PeopleId,
                       Name = om.Name,
                       Location = om.Location,
                       LeaderName = om.LeaderName,
                       MeetingTime = om.MeetingTime,
                       LeaderId = om.LeaderId,
                       EnrollDate = om.EnrollDate,
                       MemberType = om.MemberType,
                       DivisionName = om.ProgramName + "/" + om.DivisionName,
                       IsLeaderAttendanceType = false
                   };
        }
    }
}
