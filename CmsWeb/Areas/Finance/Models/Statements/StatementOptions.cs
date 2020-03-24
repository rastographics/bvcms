using System.Drawing.Printing;
using TuesPechkin;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class StatementOptions
    {
        public bool CombinedTaxSummary { get; set; }
        public HeaderSettings Header { get; set; }
        public FooterSettings Footer { get; set; }
        public MarginSettings Margins { get; set; }
        public PaperKind PaperSize { get; set; }
    }
}
