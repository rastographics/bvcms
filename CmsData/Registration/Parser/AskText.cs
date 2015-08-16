using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CmsData.API;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
	public partial class Parser
	{
		public void Output(StringBuilder sb, AskText ask)
		{
			if (ask.list.Count == 0)
				return;
			AddValueNoCk(0, sb, "Text", "");
			foreach (var q in ask.list)
				AddValueCk(1, sb, q.Question);
			sb.AppendLine();
		}
		public AskText ParseAskText()
		{
			var tx = new AskText();
			lineno++;
			if (curr.indent == 0)
				return tx;
			var startindent = curr.indent;
			while (curr.indent == startindent)
			{
				if (curr.kw != RegKeywords.None)
					throw GetException("unexpected line");
				var q = new AskExtraQuestions.ExtraQuestion { Question = GetLine() };
				tx.list.Add(q);
			}
			return tx;
		}
	}
}
