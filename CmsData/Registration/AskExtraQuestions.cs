using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskExtraQuestions : Ask
	{
	    public override string Help { get { return @"
These questions can be answered with text.

The Question should be fairly short (25 characters or less)
but long enough for you and your registrant to know what it refers to. 
Note, this will be used as a column header on an Excel spreadsheet.

If you need a long explanation assoicated with your question, put that in as an Instruction above the question.
"; } }
		public List<ExtraQuestion> list { get; private set; }

		public AskExtraQuestions()
			: base("AskExtraQuestions")
		{
			list = new List<ExtraQuestion>();
		}
		public override void Output(StringBuilder sb)
		{
			if (list.Count == 0)
				return;
			Settings.AddValueNoCk(0, sb, "ExtraQuestions", "");
			foreach (var q in list)
				q.Output(sb);
			sb.AppendLine();
		}
		public static AskExtraQuestions Parse(Parser parser)
		{
			var eq = new AskExtraQuestions();
			parser.lineno++;
			if (parser.curr.indent == 0)
				return eq;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var q = ExtraQuestion.Parse(parser, startindent);
				eq.list.Add(q);
			}
			return eq;
		}
        public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type);
	        foreach (var q in list)
                w.Add("ExtraQuestion", q);
	        w.End();
	    }
	    public new static AskExtraQuestions ReadXml(XElement e)
	    {
			var eq = new AskExtraQuestions();
	        foreach (var ee in e.Elements("ExtraQuestion"))
                eq.list.Add(ExtraQuestion.ReadXml(ee));
			return eq;
	    }
		public class ExtraQuestion
		{
			public string Name { get; set; }
			public string Question { get; set; }
			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, Question);
			}
			public static ExtraQuestion Parse(Parser parser, int startindent)
			{
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line");
				return new ExtraQuestion { Question = parser.GetLine() };
			}

		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static ExtraQuestion ReadXml(XElement e)
		    {
		        return new ExtraQuestion() { Question = e.Value };
		    }
		}
	}
}
