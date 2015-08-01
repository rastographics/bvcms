using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
	public partial class Parser 
	{
		public AskInstruction ParseAskInstruction()
		{
			var r = new AskInstruction();
			GetBool();
			r.Label = GetLabel("Instruction");
			return r;
		}
		public void Output(StringBuilder sb, AskInstruction ask)
		{
			AddValueCk(0, sb, "AskInstruction", true);
			if (!ask.Label.HasValue())
				ask.Label = "Instruction";
			AddValueCk(1, sb, "Label", ask.Label);
			sb.AppendLine();
		}
	}
}