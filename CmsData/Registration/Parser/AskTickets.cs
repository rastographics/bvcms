using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public AskTickets ParseAskTickets()
		{
			var r = new AskTickets();
			GetBool();
			r.Label = GetLabel("No. of Items");
			return r;
		}
		public void Output(StringBuilder sb, AskTickets ask)
		{
            AddValueCk(0, sb, "AskTickets", true);
			if (!ask.Label.HasValue())
				ask.Label = "No. of Items";
			AddValueCk(1, sb, "Label", ask.Label);
			sb.AppendLine();
		}
	}
}