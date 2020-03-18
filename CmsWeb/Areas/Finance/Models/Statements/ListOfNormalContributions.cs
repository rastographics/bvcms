using CmsData.View;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class ListOfNormalContributions : List<NormalContribution>
    {
        public ListOfNormalContributions(List<NormalContribution> list)
        {
            AddRange(list);
        }

        public decimal Total => this.Sum(c => c.ContributionAmount ?? 0);

        public void Combine(ListOfNormalContributions list)
        {
            foreach (var item in list)
            {
                var found = this.FirstOrDefault(c => c.FundName == item.FundName);
                if (found != null)
                {
                    found.ContributionAmount += item.ContributionAmount;
                }
                else
                {
                    Add(item);
                }
            }
        }
    }
}
