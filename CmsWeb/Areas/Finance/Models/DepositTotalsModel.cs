using CmsData;
using CmsData.View;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using UtilityExtensions;

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
            {
                first = first.AddMonths(-1);
            }

            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public IEnumerable<DepositDateTotal> FetchData()
        {
            var authorizedFunds = DbUtil.Db.ContributionFunds.ScopedByRoleMembership().Select(f => f.FundId).ToList();
            var authorizedFundsCsv = string.Join(",", authorizedFunds);

            var connection = new SqlConnection(Util.ConnectionString);
            var parameters = new DynamicParameters();
            parameters.Add("authorizedFunds", authorizedFundsCsv);
            parameters.Add("startDate", Dt1 ?? DateTime.MinValue);
            parameters.Add("endDate", Dt2 ?? DateTime.MaxValue);

            var reader = connection.ExecuteReader("dbo.GetDepositDateTotalsUsingAuthorizedFunds", parameters, commandTimeout: 1200, commandType: CommandType.StoredProcedure);
            var items = new List<DepositDateTotal>();

            while (reader.Read())
            {
                items.Add(new DepositDateTotal
                {
                    DepositDate = reader["DepositDate"].ToNullableDate(),
                    Count = reader["Count"].ToNullableInt(),
                    TotalContributions = reader["TotalContributions"].ToNullableDecimal(),
                    TotalHeader = reader["TotalHeader"].ToNullableDecimal()
                });
            }

            Total = new DepositDateTotal
            {
                TotalHeader = items.Sum(x => x.TotalHeader ?? 0.00m),
                TotalContributions = items.Sum(x => x.TotalContributions ?? 0.00m),
                Count = items.Sum(x => x.Count ?? 0)
            };

            return items.OrderBy(x => x.DepositDate);
        }
    }
}
