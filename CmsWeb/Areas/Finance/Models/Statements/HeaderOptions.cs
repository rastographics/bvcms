using TuesPechkin;

namespace CmsWeb.Areas.Finance.Models.Report
{
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
}
