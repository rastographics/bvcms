using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskSize : Ask
	{
	    public override string Help => @"
Display a dropdown of custom sizes. With each size you can:

* Associate a Fee
* Put in a Sub-Group
* Adds an extra item to the sizes to indicate they will use last year's shirt.
";
        public bool TargetExtraValue { get; set; }
	    public decimal? Fee { get; set; }
		public string Label { get; set; }
		public bool AllowLastYear { get; set; }
		public List<Size> list { get; set; }
		public AskSize() : base("AskSize")
		{
		    list = new List<Size>();
		}
	    public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type)
                .AttrIfTrue("TargetExtraValue", TargetExtraValue)
	            .Attr("Fee", Fee)
	            .Attr("AllowLastYear", AllowLastYear)
	            .Add("Label", Label ?? "Size");
			foreach (var g in list)
                g.WriteXml(w);
	        w.End();
	    }
		public new static AskSize ReadXml(XElement e)
		{
		    var r = new AskSize
		    {
                TargetExtraValue = e.Attribute("TargetExtraValue").ToBool(),
		        Label = e.Element("Label")?.Value,
		        Fee = e.Attribute("Fee").ToDecimal(),
		        AllowLastYear = e.Attribute("AllowLastYear").ToBool(),
		        list = new List<Size>()
		    };
		    foreach (var ee in e.Elements("Size"))
		        r.list.Add(Size.ReadXml(ee));
            // todo: check duplicates
			return r;
		}
        public override List<string> SmallGroups()
        {
            var q = (from i in list
                     select i.SmallGroup).ToList();
            return q;
        }

		public partial class Size
		{
			public string Name { get; set; }
			public string Description { get; set; }
            [DisplayName("Sub-Group")]
			public string SmallGroup { get; set; }
		    public void WriteXml(APIWriter w)
		    {
		        w.Start("Size")
		            .Add("Description", Description)
		            .Add("SmallGroup", SmallGroup)
		            .End();
		    }

		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static Size ReadXml(XElement e)
		    {
				var i = new Size();
		        i.Description = e.Element("Description")?.Value;
		        i.SmallGroup = (e.Element("SmallGroup")?.Value ?? i.Description)?.Trim();
				return i;
		    }
		}
	}
}
