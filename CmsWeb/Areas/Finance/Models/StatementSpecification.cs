using System.Collections.Generic;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class StatementSpecification
    {
        public string Description { get; set; }
        public string Header { get; set; }
        public string Notice { get; set; }
        public List<int> Funds { get; set; }
    }
}
