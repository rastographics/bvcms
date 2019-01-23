using System.Collections.Generic;
using System.ComponentModel;
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
        public bool TargetExtraValue { get; set; }
	    public List<YesNoQuestion> list { get; private set; }

		public AskYesNoQuestions()
			: base("AskYesNoQuestions")
		{
			list = new List<YesNoQuestion>();
		}
	    public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type)
	            .AttrIfTrue("TargetExtraValue", TargetExtraValue);
	        foreach (var q in list)
                q.WriteXml(w);
	        w.End();
	    }
	    public new static AskYesNoQuestions ReadXml(XElement e)
	    {
	        var yn = new AskYesNoQuestions {
                TargetExtraValue = e.Attribute("TargetExtraValue").ToBool(),
	        };
            foreach(var ee in e.Elements("YesNoQuestion"))
                yn.list.Add(YesNoQuestion.ReadXml(ee));
	        return yn;
	    }
		public partial class YesNoQuestion
		{
			public string Name { get; set; }
			public string Question { get; set; }
            [DisplayName("Sub-Group")]
			public string SmallGroup { get; set; }
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
		        var i = new YesNoQuestion();
		        i.Question = e.Element("Question")?.Value;
		        i.SmallGroup = (e.Element("SmallGroup")?.Value ?? i.Question)?.Trim();
		        return i;
		    }
		}
	}
}
