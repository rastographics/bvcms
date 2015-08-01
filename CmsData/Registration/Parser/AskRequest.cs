using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public AskRequest ParseAskRequest()
		{
			var r = new AskRequest();
			GetBool();
			r.Label = GetLabel("Request");
			return r;
		}
		public void Output(StringBuilder sb, AskRequest ask)
		{
			AddValueCk(0, sb, "AskRequest", true);
			if (!ask.Label.HasValue())
				ask.Label = "Request";
			AddValueCk(1, sb, "Label", ask.Label);
			sb.AppendLine();
		}
	}
}