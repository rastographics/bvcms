using CmsData.View;
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
    }
}
