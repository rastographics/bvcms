using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskYesNoQuestions : Ask
	{
	    public override string Help => @"
These are questions that will force a yes or no answer. 
The results will be in sub-groups with a Yes- or No- prepended to the name, so make sure to keep the question text very short.

If you need a longer explanation, use InstructionalText above the question so you can keep it short
";
	    public List<YesNoQuestion> list { get; private set; }

		public AskYesNoQuestions()
			: base("AskYesNoQuestions")
		{
			list = new List<YesNoQuestion>();
		}
		public override void Output(StringBuilder sb)
		{
			if (list.Count == 0)
				return;
			Settings.AddValueNoCk(0, sb, "YesNoQuestions", "");
			foreach (var q in list)
				q.Output(sb);
			sb.AppendLine();
		}
		public static AskYesNoQuestions Parse(Parser parser)
		{
			var ynq = new AskYesNoQuestions();
			parser.lineno++;
			if (parser.curr.indent == 0)
				return ynq;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var q = YesNoQuestion.Parse(parser, startindent);
				ynq.list.Add(q);
			}
			return ynq;
		}
	    public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type);
	        foreach (var q in list)
                q.WriteXml(w);
	        w.End();
	    }
	    public new static AskYesNoQuestions ReadXml(XElement e)
	    {
	        var yn = new AskYesNoQuestions();
            foreach(var ee in e.Elements("YesNoQuestion"))
                yn.list.Add(YesNoQuestion.ReadXml(ee));
	        return yn;
	    }
        public override List<string> SmallGroups()
        {
            var q = (from i in list
                     select i.SmallGroup).ToList();
            return q;
        }
		public class YesNoQuestion
		{
			public string Name { get; set; }
			public string Question { get; set; }
			public string SmallGroup { get; set; }
			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, Question ?? "need a question here");
				Settings.AddValueNoCk(2, sb, "SmallGroup", SmallGroup ?? Question);
			}
			public static YesNoQuestion Parse(Parser parser, int startindent)
			{
				var q = new YesNoQuestion();
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line");
				q.Question = parser.GetLine();
				if (parser.curr.indent <= startindent)
					throw parser.GetException("Expected SmallGroup indented");
				if (parser.curr.kw != Parser.RegKeywords.SmallGroup)
					throw parser.GetException("Expected SmallGroup keyword");
				if (!parser.curr.value.HasValue())
					throw parser.GetException("Expected SmallGroup value");
				q.SmallGroup = parser.GetString();
				return q;
			}
		    public void WriteXml(APIWriter w)
		    {
		        w.Start("YesNoQuestion")
		            .Add("Question", Question ?? "need a question here")
		            .Add("SmallGroup", SmallGroup)
		            .End();
		    }
		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static YesNoQuestion ReadXml(XElement e)
		    {
		        return new YesNoQuestion
		        {
		            Question = e.Element("Question")?.Value,
		            SmallGroup = e.Element("SmallGroup")?.Value
		        };
		    }
		}
	}
}
