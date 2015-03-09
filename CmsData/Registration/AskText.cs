using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskText : Ask
	{
	    public override string Help { get { return @"These questions can be answered with text on multiple lines."; } }
		public List<AskExtraQuestions.ExtraQuestion> list { get; private set; }

		public AskText()
			: base("AskText")
		{
			list = new List<AskExtraQuestions.ExtraQuestion>();
		}
		public override void Output(StringBuilder sb)
		{
			if (list.Count == 0)
				return;
			Settings.AddValueNoCk(0, sb, "Text", "");
			foreach (var q in list)
				q.Output(sb);
			sb.AppendLine();
		}
		public static AskText Parse(Parser parser)
		{
			var tx = new AskText();
			parser.lineno++;
			if (parser.curr.indent == 0)
				return tx;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var q = AskExtraQuestions.ExtraQuestion.Parse(parser, startindent);
				tx.list.Add(q);
			}
			return tx;
		}
	}
}
