using System.Collections.Generic;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskExtraQuestions : Ask
	{
	    public override string Help => @"
These questions can be answered with text.

The Question should be fairly short (25 characters or less)
but long enough for you and your registrant to know what it refers to. 
Note, this will be used as a column header on an Excel spreadsheet.

If you need a long explanation assoicated with your question, put that in as an Instruction above the question.
";
        public bool TargetExtraValue { get; set; }
	    public List<ExtraQuestion> list { get; private set; }

		public AskExtraQuestions()
			: base("AskExtraQuestions")
		{
			list = new List<ExtraQuestion>();
		}
        public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type)
                .AttrIfTrue("TargetExtraValue", TargetExtraValue);
	        foreach (var q in list)
                w.Add("Question", q.Question);
	        w.End();
	    }
	    public new static AskExtraQuestions ReadXml(XElement e)
	    {
	        var eq = new AskExtraQuestions {
                TargetExtraValue = e.Attribute("TargetExtraValue").ToBool(),
            };
	        foreach (var ee in e.Elements("Question"))
                eq.list.Add(ExtraQuestion.ReadXml(ee));
			return eq;
	    }
		public partial class ExtraQuestion
		{
			public string Name { get; set; }
			public string Question { get; set; }

		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static ExtraQuestion ReadXml(XElement e)
		    {
		        return new ExtraQuestion() { Question = e.Value };
		    }
		}
	}
}
