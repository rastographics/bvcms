using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskDropdown : Ask
	{
	    public override string Help
	    {
	        get 
            { return @"
This will be presented as a dropdown selection.

* **Sub-Group** Enters them in a sub-group within the organization.
* **Fee** (optional) for the selection.
* **Limit** (optional) which limits the number of people allowed for a selection.
* **DateTime** (optional) which registers them in a meeting.
"; 
            }
	    }

	    public string Label { get; set; }
		public List<DropdownItem> list { get; set; }

		public AskDropdown()
			: base("AskDropdown")
		{
			list = new List<DropdownItem>();
		}
		public override void Output(StringBuilder sb)
		{
			if (list == null || list.Count == 0)
				return;
			Settings.AddValueNoCk(0, sb, "Dropdown", Label);
			foreach (var i in list)
				i.Output(sb);
			sb.AppendLine();
		}
		public static AskDropdown Parse(Parser parser)
		{
			var dd = new AskDropdown();
			dd.Label = parser.GetString("Dropdown");
			dd.list = new List<DropdownItem>();
			if (parser.curr.indent == 0)
				return dd;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var m = DropdownItem.Parse(parser, startindent);
				dd.list.Add(m);
			}
            var q = (from i in dd.list
                     group i by i.SmallGroup into g
                     where g.Count() > 1
                     select g.Key).ToList();
			if (q.Any())
				throw parser.GetException("Duplicate SmallGroup in Dropdown: {0}".Fmt(string.Join(",", q)));
			return dd;
		}
        public override List<string> SmallGroups()
        {
            var q = (from i in list
                     select i.SmallGroup).ToList();
            return q;
        }
		public DropdownItem SmallGroupChoice(List<string> choices)
		{
            var v = list.SingleOrDefault(i => choices.Contains(i.SmallGroup, StringComparer.OrdinalIgnoreCase));
		    return v;
		}
		public string SmallGroupDescription(List<string> choices)
		{
            var v = list.SingleOrDefault(i => choices.Contains(i.SmallGroup, StringComparer.OrdinalIgnoreCase));
            if(v != null)
    		    return v.Description;
		    return "";
		}

		public bool IsSmallGroupFilled(List<string> smallgroups, string sg)
		{
			string desc;
			return IsSmallGroupFilled(smallgroups, new []{sg}, out desc);
		}
		public bool IsSmallGroupFilled(IEnumerable<string> smallgroups, IEnumerable<string> sgs, out string desc)
		{
            var i = list.SingleOrDefault(dd => sgs.Contains(dd.SmallGroup, StringComparer.OrdinalIgnoreCase));
			desc = null;
			if (i == null)
				return false;
			desc = i.Description;
			return i.IsSmallGroupFilled(smallgroups);
		}
		public class DropdownItem
		{
		    public string Name { get; set; }
			public string Description { get; set; }
            [DisplayName("Sub-Group")]
			public string SmallGroup { get; set; }
			public decimal? Fee { get; set; }
			public int? Limit { get; set; }
			[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
			public DateTime? MeetingTime { get; set; }

            [DisplayName("DateTime")]
		    public string MeetingTimeString
		    {
		        get { return MeetingTime.ToString2("g"); }
		        set { MeetingTime = value.ToDate(); }
		    }

		    public DropdownItem()
			{
				
			}
			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, Description);
				Settings.AddValueCk(2, sb, "SmallGroup", SmallGroup);
				Settings.AddValueCk(2, sb, "Fee", Fee);
				Settings.AddValueCk(2, sb, "Limit", Limit);
				Settings.AddValueCk(2, sb, "Time", MeetingTime.ToString2("s"));
			}
			public static DropdownItem Parse(Parser parser, int startindent)
			{
				var i = new DropdownItem();
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("unexpected line in Dropdown");
				i.Description = parser.GetLine();
				i.SmallGroup = i.Description;
				if (parser.curr.indent <= startindent)
					return i;
				var ind = parser.curr.indent;
				while (parser.curr.indent == ind)
				{
					switch (parser.curr.kw)
					{
						case Parser.RegKeywords.SmallGroup:
							i.SmallGroup = parser.GetString(i.Description);
							break;
						case Parser.RegKeywords.Fee:
							i.Fee = parser.GetDecimal();
							break;
						case Parser.RegKeywords.Limit:
							i.Limit = parser.GetNullInt();
							break;
						case Parser.RegKeywords.Time:
							i.MeetingTime = parser.GetDateTime();
							break;
						default:
							throw parser.GetException("unexpected line in DropdownItem");
					}
				}
				return i;
			}
			public void AddToSmallGroup(CMSDataContext Db, OrganizationMember om, PythonEvents pe)
			{
				if (om == null)
					return;
				if (pe != null)
					pe.instance.AddToSmallGroup(SmallGroup, om);
				om.AddToGroup(Db, SmallGroup);
				if (MeetingTime.HasValue)
					Attend.MarkRegistered(Db, om.OrganizationId, om.PeopleId, MeetingTime.Value, 1);
			}
			public void RemoveFromSmallGroup(CMSDataContext Db, OrganizationMember om)
			{
				om.RemoveFromGroup(Db, SmallGroup);
			}
			public bool IsSmallGroupFilled(IEnumerable<string> smallgroups)
			{
				if (!(Limit > 0)) return false;
				var cnt = smallgroups.Count(mm => mm == SmallGroup);
				return cnt >= Limit;
			}
		}
	}
}