using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var strFunds = funds.ToString();
                Fundids = new List<int>();
                ExFundIds = new List<int>();
                if (strFunds.HasValue())
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
            var dt1 = Util.Now.AddDays(-days1);
            var dt2 = Util.Now.AddDays(-days2);
            var typs = new[] { 6, 7 };

            var q = from c in db.Contributions
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate.Value.Date >= dt1.Date
                    where days2 == 0 || c.ContributionDate.Value.Date <= dt2.Date
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where f.Fundids.Count == 0 || f.Fundids.Contains(c.FundId)
                    where f.ExFundIds.Count == 0 || !f.ExFundIds.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Count();
        }

        public int ContributionCount(int days, object funds)
        {
            var f = new Funds(funds);
            var dt = Util.Now.AddDays(-days);
            var typs = new[] { 6, 7 };
            var q = from c in db.Contributions
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate >= dt
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where f.Fundids.Count == 0 || f.Fundids.Contains(c.FundId)
                    where f.ExFundIds.Count == 0 || !f.ExFundIds.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return q.Count();
        }

        public double ContributionTotals(int days1, int days2, object funds)
        {
            var f = new Funds(funds);

            var dt1 = Util.Today.AddDays(-days1);
            var dt2 = Util.Today.AddDays(-days2);
            var typs = new[] { 6, 7 };
            var q = from c in db.Contributions
                    where c.ContributionStatusId == ContributionStatusCode.Recorded
                    where c.ContributionDate.Value.Date >= dt1.Date
                    where days2 == 0 || c.ContributionDate.Value.Date <= dt2.Date
                    where c.ContributionTypeId != ContributionTypeCode.Pledge
                    where f.Fundids.Count == 0 || f.Fundids.Contains(c.FundId)
                    where f.ExFundIds.Count == 0 || !f.ExFundIds.Contains(c.FundId)
                    where !typs.Contains(c.ContributionTypeId)
                    select c;
            return Convert.ToDouble(q.Sum(c => c.ContributionAmount) ?? 0);
        }
        public string DateRangeForContributionTotals(int days1, int days2)
        {
            var dt1 = Util.Today.AddDays(-days1);
            var dt2 = Util.Today.AddDays(-days2);
            return $"from {dt1.ToShortDateString()} up to {dt2.ToShortDateString()}";
        }
    }
}
