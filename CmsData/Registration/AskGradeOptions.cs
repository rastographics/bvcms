using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskGradeOptions : Ask
	{
	    public override string Help => @"This allows you to specify the grade being registered for.";
	    public string Label { get; set; }
		public List<GradeOption> list { get; set; }

		public AskGradeOptions()
			: base("AskGradeOptions")
		{
			list = new List<GradeOption>();
		}
	    public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type)
	            .Add("Label", Label);
			foreach (var g in list)
                g.WriteXml(w);
	        w.End();
	    }
	    public new static AskGradeOptions ReadXml(XElement e)
	    {
			var go = new AskGradeOptions {
	            Label = e.Element("Label")?.Value
            };
	        foreach (var ele in e.Elements("GradeOption"))
	            go.list.Add(GradeOption.ReadXml(ele));
	        return go;
	    }
		public partial class GradeOption
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public int Code { get; set; }
		    public void WriteXml(APIWriter w)
		    {
		        w.Start("GradeOption")
		            .Attr("Code", Code)
		            .AddText(Description)
		            .End();
		    }
		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static GradeOption ReadXml(XElement e)
		    {
		        var option = new GradeOption
		        {
		            Description = e.Value,
		            Code = e.Attribute("Code").ToInt2() ?? 0
		        };
		        return option;
		    }
		}
	}
}
