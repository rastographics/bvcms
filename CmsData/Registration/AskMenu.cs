using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml;
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

        public override void Output(StringBuilder sb)
        {
            if (list.Count == 0)
                return;
            Settings.AddValueNoCk(0, sb, "MenuItems", Label);
            foreach (var i in list)
                i.Output(sb);
            sb.AppendLine();
        }

        public static AskMenu Parse(Parser parser)
        {
            var mi = new AskMenu();
            mi.Label = parser.GetString("Menu");
            mi.list = new List<MenuItem>();
            if (parser.curr.indent == 0)
                return mi;
            var startindent = parser.curr.indent;
            while (parser.curr.indent == startindent)
            {
                var m = MenuItem.Parse(parser, startindent);
                mi.list.Add(m);
            }
            var q = (from i in mi.list
                     group i by i.SmallGroup into g
                     where g.Count() > 1
                     select g.Key).ToList();
            if (q.Any())
                throw parser.GetException($"Duplicate SmallGroup in MenuItems: {string.Join(",", q)}");
            return mi;
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

        public class MenuItem
        {
            public string Name { get; set; }
            public string Description { get; set; }
            [DisplayName("Sub-Group")]
            public string SmallGroup { get; set; }
            public decimal? Fee { get; set; }
            public int? Limit { get; set; }

            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
            public DateTime? MeetingTime { get; set; }

            public void Output(StringBuilder sb)
            {
                Settings.AddValueCk(1, sb, Description);
                Settings.AddValueCk(2, sb, "SmallGroup", SmallGroup);
                Settings.AddValueCk(2, sb, "Fee", Fee);
                Settings.AddValueCk(2, sb, "Limit", Limit);
                Settings.AddValueCk(2, sb, "Time", MeetingTime.ToString2("s"));
            }

            public static MenuItem Parse(Parser parser, int startindent)
            {
                var menuitem = new MenuItem();
                if (parser.curr.kw != Parser.RegKeywords.None)
                    throw parser.GetException("unexpected line in MenuItem");
                menuitem.Description = parser.GetLine();
                menuitem.SmallGroup = menuitem.Description;
                if (parser.curr.indent <= startindent)
                    return menuitem;
                var ind = parser.curr.indent;
                while (parser.curr.indent == ind)
                {
                    switch (parser.curr.kw)
                    {
                        case Parser.RegKeywords.SmallGroup:
                            menuitem.SmallGroup = parser.GetString(menuitem.Description);
                            break;
                        case Parser.RegKeywords.Fee:
                            menuitem.Fee = parser.GetDecimal();
                            break;
                        case Parser.RegKeywords.Limit:
                            menuitem.Limit = parser.GetNullInt();
                            break;
                        case Parser.RegKeywords.Time:
                            menuitem.MeetingTime = parser.GetDateTime();
                            break;
                        default:
                            throw parser.GetException("unexpected line in MenuItem");
                    }
                }
                return menuitem;
            }

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
		            Fee = e.Attribute("Fee")?.Value.ToDecimal(),
		            Limit = e.Attribute("Limit")?.Value.ToInt2(),
		            MeetingTime = e.Attribute("Time")?.Value.ToDate()
		        };
		        mi.SmallGroup = e.Element("SmallGroup")?.Value ?? mi.Description;
		        return mi;
		    }
        }

	    public override void WriteXml(APIWriter w)
	    {
			if (list.Count == 0)
				return;
	        w.Start(Type);
            w.Add("Label", Label);
			foreach (var g in list)
                g.WriteXml(w);
	        w.End();
	    }

	    public new static AskMenu ReadXml(XElement ele)
	    {
			var m = new AskMenu();
	        m.Label = ele.Element("Label")?.Value;
			m.list = new List<MenuItem>();
            foreach(var ee in ele.Elements("MenuItem"))
                m.list.Add(MenuItem.ReadXml(ee));
	        return m;
	        // todo: check duplicates
	    }
    }
}