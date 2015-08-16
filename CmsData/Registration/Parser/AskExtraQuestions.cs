using System.Text;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public void Output(StringBuilder sb, AskExtraQuestions ask)
		{
			if (ask.list.Count == 0)
				return;
			AddValueNoCk(0, sb, "ExtraQuestions", "");
			foreach (var q in ask.list)
				AddValueCk(1, sb, q.Question);
			sb.AppendLine();
		}
		public AskExtraQuestions ParseAskExtraQuestions()
		{
			var eq = new AskExtraQuestions();
			lineno++;
			if (curr.indent == 0)
				return eq;
			var startindent = curr.indent;
			while (curr.indent == startindent)
			{
				if (curr.kw != Parser.RegKeywords.None)
					throw GetException("unexpected line");
				var q = new AskExtraQuestions.ExtraQuestion { Question = GetLine() };
				eq.list.Add(q);
			}
			return eq;
		}
	}
}
