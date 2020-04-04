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

    public class MarginOptions
    {
        public double All { get; set; }
        public Unit Unit { get; set; }
        public int? Bottom { get; set; }
        public int? Left { get; set; }
        public int? Right { get; set; }
        public int? Top { get; set; }
        public MarginSettings Settings => new MarginSettings
        {
            All = All,
            Unit = Unit,
            Bottom = Bottom,
            Left = Left,
            Right = Right,
            Top = Top
        };
    }

    public class HeaderOptions
    {
        public string RightText { get; set; }
        public string FontName { get; set; }
        public double? FontSize { get; set; }
        public string CenterText { get; set; }
        public double? ContentSpacing { get; set; }
        public string HtmlUrl { get; set; }
        public string LeftText { get; set; }
        public bool UseLineSeparator { get; set; }
        public HeaderSettings Settings => new HeaderSettings
        {
            CenterText = CenterText,
            ContentSpacing = ContentSpacing,
            FontName = FontName,
            FontSize = FontSize,
            HtmlUrl = HtmlUrl,
            LeftText = LeftText,
            RightText = RightText,
            UseLineSeparator = UseLineSeparator
        };
    }

    public class FooterOptions
    {
        public string RightText { get; set; }
        public string FontName { get; set; }
        public double? FontSize { get; set; }
        public string CenterText { get; set; }
        public double? ContentSpacing { get; set; }
        public string HtmlUrl { get; set; }
        public string LeftText { get; set; }
        public bool UseLineSeparator { get; set; }
        public FooterSettings Settings => new FooterSettings
        {
            CenterText = CenterText,
            ContentSpacing = ContentSpacing,
            FontName = FontName,
            FontSize = FontSize,
            HtmlUrl = HtmlUrl,
            LeftText = LeftText,
            RightText = RightText,
            UseLineSeparator = UseLineSeparator
        };
    }
}
