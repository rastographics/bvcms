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
    public class PreviousEnrollments : PagedTableModel<InvolvementPreviou, OrgMemberInfo>
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

        public PreviousEnrollments()
            : base("default", "asc", true)
        { }

        public IQueryable<InvolvementPreviou> DefineModelList(bool useOrgFilter)
        {
            var limitvisibility = Util2.OrgLeadersOnly || !HttpContextFactory.Current.User.IsInRole("Access");
            var roles = DbUtil.Db.CurrentRoles();
            return from etd in DbUtil.Db.InvolvementPrevious(PeopleId, Util.UserId)
                   where etd.TransactionStatus == false
                   where etd.PeopleId == PeopleId
                   where etd.TransactionTypeId >= 4
                   where !(limitvisibility && etd.SecurityTypeId == 3)
                   where etd.LimitToRole == null || roles.Contains(etd.LimitToRole)
                   where (!useOrgFilter || !OrgTypesFilter.Any() || OrgTypesFilter.Contains(etd.OrgType))
                   select etd;
        }

        public override IQueryable<InvolvementPreviou> DefineModelList()
        {
            return DefineModelList(true);
        }

        public override IEnumerable<OrgMemberInfo> DefineViewList(IQueryable<InvolvementPreviou> q)
        {
            var q2 = from om in q
                     select new OrgMemberInfo
                     {
                         OrgId = om.OrganizationId,
                         PeopleId = om.PeopleId,
                         Name = om.Name,
                         MemberType = om.MemberType,
                         EnrollDate = om.EnrollDate,
                         DropDate = om.TransactionDate,
                         AttendPct = om.AttendancePercentage,
                         DivisionName = om.ProgramName + "/" + om.DivisionName,
                         OrgType = om.OrgType,
                         IsLeaderAttendanceType = false
                     };
            return q2;
        }
        public override IQueryable<InvolvementPreviou> DefineModelSort(IQueryable<InvolvementPreviou> q)
        {
            switch (SortExpression)
            {
                case "Enroll Date desc":
                    return from om in q
                           orderby om.FirstTransactionDate
                           select om;
                case "Enroll Date":
                    return from om in q
                           orderby om.FirstTransactionDate descending
                           select om;
                case "Drop Date desc":
                    return from om in q
                           orderby om.TransactionDate
                           select om;
                case "Drop Date":
                    return from om in q
                           orderby om.TransactionDate descending
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
    }
}
