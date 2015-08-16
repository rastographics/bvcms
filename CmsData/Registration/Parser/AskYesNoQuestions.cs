using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public void Output(StringBuilder sb, AskYesNoQuestions ask)
		{
			if (ask.list.Count == 0)
				return;
			AddValueNoCk(0, sb, "YesNoQuestions", "");
		    foreach (var q in ask.list)
		    {
				AddValueCk(1, sb, q.Question ?? "need a question here");
				AddValueNoCk(2, sb, "SmallGroup", q.SmallGroup ?? q.Question);
		    }
			sb.AppendLine();
		}
		public AskYesNoQuestions ParseAskYesNoQuestions()
		{
			var ynq = new AskYesNoQuestions();
			lineno++;
			if (curr.indent == 0)
				return ynq;
			var startindent = curr.indent;
			while (curr.indent == startindent)
			{
				var q = new AskYesNoQuestions.YesNoQuestion();
				if (curr.kw != Parser.RegKeywords.None)
					throw GetException("unexpected line");
				q.Question = GetLine();
				if (curr.indent <= startindent)
					throw GetException("Expected SmallGroup indented");
				if (curr.kw != Parser.RegKeywords.SmallGroup)
					throw GetException("Expected SmallGroup keyword");
				if (!curr.value.HasValue())
					throw GetException("Expected SmallGroup value");
				q.SmallGroup = GetString();
				ynq.list.Add(q);
			}
			return ynq;
		}
	}
}
