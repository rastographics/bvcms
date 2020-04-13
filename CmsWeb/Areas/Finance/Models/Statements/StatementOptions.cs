using System.Drawing.Printing;
using TuesPechkin;

namespace CmsWeb.Areas.Finance.Models.Report
{
    public class StatementOptions
    {
        public bool CombinedTaxSummary { get; set; }

        public HeaderOptions Header { get; set; } = new HeaderOptions
        {
            RightText = "",
            FontName = "Helvetica",
            FontSize = 10.0,
            CenterText = "",
            ContentSpacing = 0.0,
            HtmlUrl = "",
            LeftText = "",
            UseLineSeparator = false
        };

        public FooterOptions Footer { get; set; } = new FooterOptions
        {
            RightText = "Page [page] of [topage]",
            FontName = "Helvetica",
            FontSize = 10.0,
            CenterText = "",
            ContentSpacing = 0.0,
            HtmlUrl = "",
            LeftText = "",
            UseLineSeparator = false
        };

        public MarginOptions Margins { get; set; } = new MarginOptions
        {
            All = 0.5,
            Unit = Unit.Inches,
            Bottom = null,
            Left = null,
            Right = null,
            Top = null
        };

        public PaperKind PaperSize { get; set; }
    }
}
