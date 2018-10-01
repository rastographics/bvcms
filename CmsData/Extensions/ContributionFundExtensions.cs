using System.Collections.Generic;
using System.Linq;
using System.Web.Security;

namespace CmsData
{
    public static class ContributionFundExtensions
    {
        public static IQueryable<ContributionFund> ScopedByRoleMembership(this IQueryable<ContributionFund> contributionFunds)
        {
            return contributionFunds.ScopedByRoleMembership(Roles.GetRolesForUser());
        }

        public static IQueryable<ContributionFund> ScopedByRoleMembership(this IQueryable<ContributionFund> contributionFunds, string[] allowedRoles)
        {
            var financeRole = "Finance";
            var financeViewOnlyRole = "FinanceViewOnly";
            var fundManagerRole = "FundManager";

            if (allowedRoles != null)
            {
                if ((allowedRoles.Contains(financeRole) && !allowedRoles.Contains(fundManagerRole)) || (allowedRoles.Contains(financeViewOnlyRole) && !allowedRoles.Contains(fundManagerRole)))
                {
                    return contributionFunds;
                }

                if (allowedRoles.Contains(fundManagerRole))
                {
                    return contributionFunds.Where(f => f.FundManagerRoleId != 0)
                        .Join(DbUtil.Db.Roles, f => f.FundManagerRoleId, r => r.RoleId, (f, r) => new { role = r, fund = f })
                        .Where(r => allowedRoles.Contains(r.role.RoleName))
                        .Select(r => r.fund);
                }
            }

            return GetEmptyList();
        }

        private static IQueryable<ContributionFund> GetEmptyList()
        {
            return new List<ContributionFund>().AsQueryable();
        }
    }
}
