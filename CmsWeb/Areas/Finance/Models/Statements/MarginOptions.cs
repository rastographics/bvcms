using TuesPechkin;

namespace CmsWeb.Areas.Finance.Models.Report
{
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
}
