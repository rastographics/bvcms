using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskSuggestedFee : Ask
	{
	    public override string Help { get { return @"Allows the final fee to be adjusted to any amount (including zero)."; } }
		public string Label { get; set; }
		public AskSuggestedFee() : base("AskSuggestedFee") { }
		public static AskSuggestedFee Parse(Parser parser)
		{
			var r = new AskSuggestedFee();
			parser.GetBool();
			r.Label = parser.GetLabel("Suggested Amount");
			return r;
		}
		public override void Output(StringBuilder sb)
		{
            Settings.AddValueCk(0, sb, "AskSuggestedFee", true);
			Settings.AddValueCk(1, sb, "Label", Label ?? "Suggested Amount");
			sb.AppendLine();
		}
	}
}