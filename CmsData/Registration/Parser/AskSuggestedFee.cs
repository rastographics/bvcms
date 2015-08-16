using System.Text;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public AskSuggestedFee ParseAskSuggestedFee()
		{
			var r = new AskSuggestedFee();
			GetBool();
			r.Label = GetLabel("Suggested Amount");
			return r;
		}
		public void Output(StringBuilder sb, AskSuggestedFee ask)
		{
            AddValueCk(0, sb, "AskSuggestedFee", true);
			AddValueCk(1, sb, "Label", ask.Label ?? "Suggested Amount");
			sb.AppendLine();
		}
	}
}