using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using CmsData;
using CmsData.View;
using UtilityExtensions;
using Dapper;
using System.Data;
using System.Web.Security;

namespace CmsWeb.Models
{
    public class DepositTotalsModel
    {
        public DateTime? Dt1 { get; set; }
        public DateTime? Dt2 { get; set; }

        public DepositDateTotal Total { get; set; }

        public DepositTotalsModel()
        {
            var today = Util.Now.Date;
            var first = new DateTime(today.Year, today.Month, 1);
            if (today.Day < 8)
                first = first.AddMonths(-1);
            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public IEnumerable<DepositDateTotal> FetchData()
        {
            var authorizedFundIds = DbUtil.Db.ContributionFunds.ScopedByRoleMembership().Select(f => f.FundId).JoinInts(",");
            var connection = new SqlConnection(Util.ConnectionString);
            var parameters = new DynamicParameters();
            parameters.Add("authorizedFunds", authorizedFundIds);
            parameters.Add("startDate", Dt1 ?? DateTime.MinValue);
            parameters.Add("endDate", Dt2 ?? DateTime.MaxValue);

            var reader = connection.ExecuteReader("dbo.GetDepositDateTotalsUsingAuthorizedFunds", parameters, commandTimeout: 1200, commandType: CommandType.StoredProcedure);
            var items = new List<DepositDateTotal>();

            while (reader.Read())
            {
                items.Add(new DepositDateTotal
                {
                    Count = reader.GetInt32(reader.GetOrdinal("Count")),
                    TotalContributions = reader.GetDecimal(reader.GetOrdinal("TotalContributions")),
                    TotalHeader = reader.GetDecimal(reader.GetOrdinal("TotalHeader"))
                });
            }

            Total = new DepositDateTotal
            {
                TotalHeader = items.Sum(x => x.TotalHeader),
                TotalContributions = items.Sum(x => x.TotalContributions),
                Count = items.Sum(x => x.Count)
            };

            return items.OrderBy(x => x.DepositDate);

            //var list = (from r in DbUtil.Db.ViewDepositDateTotals
            //            where Dt1 == null || r.DepositDate >= Dt1 
            //            where Dt2 == null || r.DepositDate <= Dt2
            //            orderby r.DepositDate
            //            select r).ToList();
            //Total = new DepositDateTotal()
            //{
            //    TotalHeader = list.Sum(vv => vv.TotalHeader),
            //    TotalContributions = list.Sum(vv => vv.TotalContributions),
            //    Count = list.Sum(vv => vv.Count ?? 0),
            //};
            //return list;
        }
    }
}
