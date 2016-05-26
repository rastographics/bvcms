using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;
using CmsData;
using CmsWeb.Models;

namespace CmsWeb.Areas.People.Models
{
    public class PreviousEnrollments : PagedTableModel<EnrollmentTransaction, OrgMemberInfo>
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

        public PreviousEnrollments()
            : base("default", "asc", true)
        {}

        override public IQueryable<EnrollmentTransaction> DefineModelList()
        {
            var limitvisibility = Util2.OrgLeadersOnly || !HttpContext.Current.User.IsInRole("Access");
            var roles = DbUtil.Db.CurrentRoles();
            return from etd in DbUtil.Db.EnrollmentTransactions
                   let org = etd.Organization
                   where etd.TransactionStatus == false
                   where etd.PeopleId == PeopleId
                   where etd.TransactionTypeId >= 4
                   where !(limitvisibility && etd.Organization.SecurityTypeId == 3)
                   where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                   where (!OrgTypesFilter.Any() || OrgTypesFilter.Contains(org.OrganizationType.Description))
                   select etd;
        }
        public override IEnumerable<OrgMemberInfo> DefineViewList(IQueryable<EnrollmentTransaction> q)
        {
            var q2 = from om in q
                     select new OrgMemberInfo
                     {
                         OrgId = om.OrganizationId,
                         PeopleId = om.PeopleId,
                         Name = om.OrganizationName,
                         MemberType = om.MemberType.Description,
                         EnrollDate = om.FirstTransaction.TransactionDate,
                         DropDate = om.TransactionDate,
                         AttendPct = om.AttendancePercentage,
                         DivisionName = om.Organization.Division.Program.Name + "/" + om.Organization.Division.Name,
                         OrgType = om.Organization.OrganizationType.Description ?? "Other"
                     };
            return q2;
        }
        override public IQueryable<EnrollmentTransaction> DefineModelSort(IQueryable<EnrollmentTransaction> q)
        {
            switch (SortExpression)
            {
                case "Enroll Date desc":
                    return from om in q
                           orderby om.FirstTransaction.TransactionDate
                           select om;
                case "Enroll Date":
                    return from om in q
                           orderby om.FirstTransaction.TransactionDate descending 
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
    }
}
