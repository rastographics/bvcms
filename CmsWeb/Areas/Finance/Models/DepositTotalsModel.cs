using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using CmsData.View;
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
                first = first.AddMonths(-1);
            Dt1 = first;
            Dt2 = first.AddMonths(1).AddDays(-1);
        }

        public IEnumerable<DepositDateTotal> FetchData()
        {
            var list = (from r in DbUtil.Db.ViewDepositDateTotals
                        where Dt1 == null || r.DepositDate >= Dt1 
                        where Dt2 == null || r.DepositDate <= Dt2
                        orderby r.DepositDate
                        select r).ToList();
            Total = new DepositDateTotal()
            {
                TotalHeader = list.Sum(vv => vv.TotalHeader),
                TotalContributions = list.Sum(vv => vv.TotalContributions),
                Count = list.Sum(vv => vv.Count ?? 0),
            };
            return list;
        }
    }
}