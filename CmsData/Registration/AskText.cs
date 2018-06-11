using System.Collections.Generic;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskText : Ask
	{
	    public override string Help => @"
These questions can be answered with text on multiple lines.

The Question should be fairly short (25 characters or so)
but long enough for you and your registrant to know what it refers to. 
Note, this will be used as a column header on an Excel spreadsheet.

If you need a long explanation assoicated with your question, put that in as an Instruction above the question.
";
        public bool TargetExtraValue { get; set; }
	    public List<AskExtraQuestions.ExtraQuestion> list { get; private set; }

		public AskText()
			: base("AskText")
		{
			list = new List<AskExtraQuestions.ExtraQuestion>();
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
	    public new static AskText ReadXml(XElement e)
	    {
	        var t = new AskText {
                TargetExtraValue = e.Attribute("TargetExtraValue").ToBool(),
	        };
            foreach(var ee in e.Elements("Question"))
                t.list.Add(AskExtraQuestions.ExtraQuestion.ReadXml(ee));
	        return t;
	    }
	}
}
