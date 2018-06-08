using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class AskMenu : Ask
    {
        public override string Help => @"
These will present a series of textboxes next to a label
allowing you to enter the number of items to purchase or select.
You can optionally associate a fee with one or more items.
";
        public string Label { get; set; }
        public List<MenuItem> list { get; set; }

        public AskMenu()
            : base("AskMenu")
        {
            list = new List<MenuItem>();
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
	    public new static AskMenu ReadXml(XElement ele)
	    {
	        var m = new AskMenu {
                Label = ele.Element("Label")?.Value,
			    list = new List<MenuItem>(),
	        };
            foreach(var ee in ele.Elements("MenuItem"))
                if(ee.Element("Description")?.Value != null)
                    m.list.Add(MenuItem.ReadXml(ee));
	        return m;
	        // todo: check duplicates
	    }
        public override List<string> SmallGroups()
        {
            var q = (from i in list
                     select i.SmallGroup).ToList();
            return q;
        }
        public class MenuItemChosen
        {
            public string sg { get; set; }
            public string desc { get; set; }
            public int number { get; set; }
            public decimal amt { get; set; }
        }

        public IEnumerable<MenuItemChosen> MenuItemsChosen(Dictionary<string, int?> items)
        {
            if (items == null)
                return new List<MenuItemChosen>();
            var q = from i in items
                    join m in list on i.Key equals m.SmallGroup
                    where i.Value.HasValue
                    select new MenuItemChosen
                    {
                        sg = m.SmallGroup,
                        number = i.Value ?? 0,
                        desc = m.Description,
                        amt = m.Fee ?? 0
                    };
            return q;
        }

        public partial class MenuItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            [DisplayName("Sub-Group")]
            public string SmallGroup { get; set; }
            public decimal? Fee { get; set; }
            public int? Limit { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
            public DateTime? MeetingTime { get; set; }

		    public void WriteXml(APIWriter w)
		    {
		        w.Start("MenuItem")
		            .Attr("Fee", Fee)
		            .Attr("Limit", Limit)
		            .Attr("Time", MeetingTime.ToString2("s"))
		            .Add("Description", Description)
		            .Add("SmallGroup", SmallGroup)
		            .End();
		    }
		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static MenuItem ReadXml(XElement e)
		    {
		        var mi = new MenuItem
		        {
		            Description = e.Element("Description")?.Value,
		            Fee = e.Attribute("Fee").ToDecimal(),
		            Limit = e.Attribute("Limit").ToInt2(),
		            MeetingTime = e.Attribute("Time").ToDate()
		        };
		        mi.SmallGroup = (e.Element("SmallGroup")?.Value ?? mi.Description)?.TrimEnd();
		        return mi;
		    }
        }
    }
}
