using System;
using System.Collections.Generic;
using System.Linq;
using CmsData.Codes;
using UtilityExtensions;

namespace CmsData
{
    public partial class QueryFunctions
    {
        class Funds
        {
            public Funds(object funds)
            {
                var strFunds = funds as string;
                Fundids = new List<int>();
                ExFundIds = new List<int>();
                if (strFunds != null)
                {
                    Fundids = (from f in strFunds.Split(',')
                               let i = f.ToInt()
                               where i > 0
                               select i).ToList();
                    ExFundIds = (from f in strFunds.Split(',')
                                 let i = f.ToInt()
                                 where i < 0
                                 select -i).ToList();
                }
            }
            public readonly List<int> Fundids;
            public readonly List<int> ExFundIds;
        }
        public int ContributionCount(int days1, int days2, object funds)
        {
            var f = new Funds(funds);
            var dt1 = DateTime.Now.AddDays(-days1);
            var dt2 = DateTime.Now.AddDays(-days2);
            var typs = new[] { 6, 7 };

            var q = from c in db.Contributions
                    where c.ContributionDate >= dt1
                    where days2 == 0 || c.ContributionDate <= dt2
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where c.ContributionAmount > 0
                    where f.Fundids.Count == 0 || f.Fundids.Contains(c.FundId)
                    where f.ExFundIds.Count == 0 || !f.ExFundIds.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Count();
        }

        public int ContributionCount(int days, object funds)
        {
            var f = new Funds(funds);
            var dt = DateTime.Now.AddDays(-days);
            var typs = new[] { 6, 7 };
            var q = from c in db.Contributions
                    where c.ContributionDate >= dt
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where c.ContributionAmount > 0
                    where f.Fundids.Count == 0 || f.Fundids.Contains(c.FundId)
                    where f.ExFundIds.Count == 0 || !f.ExFundIds.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Count();
        }

        public double ContributionTotals(int days1, int days2, object funds)
        {
            var f = new Funds(funds);

            var dt1 = DateTime.Now.AddDays(-days1);
            var dt2 = DateTime.Now.AddDays(-days2);
            var typs = new[] { 6, 7 };
            var q = from c in db.Contributions
                    where c.ContributionDate >= dt1
                    where days2 == 0 || c.ContributionDate <= dt2
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where f.Fundids.Count == 0 || f.Fundids.Contains(c.FundId)
                    where f.ExFundIds.Count == 0 || !f.ExFundIds.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return Convert.ToDouble(q.Sum(c => c.ContributionAmount) ?? 0);
        }
    }
}