using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CmsData;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class AskDropdown : Ask
    {
        public override string Help => @"
This will be presented as a dropdown selection.

* **Sub-Group** Enters them in a sub-group within the organization.
* **Fee** (optional) for the selection.
* **Limit** (optional) which limits the number of people allowed for a selection.
* **DateTime** (optional) which registers them in a meeting.
";

        public bool TargetExtraValue { get; set; }
        public string Label { get; set; }

        public List<DropdownItem> list { get; set; }

        public AskDropdown()
            : base("AskDropdown")
        {
            list = new List<DropdownItem>();
        }
        public override void WriteXml(APIWriter w)
        {
            if (list.Count == 0)
                return;
            w.Start(Type)
                .Add("Label", Label)
                .AttrIfTrue("TargetExtraValue", TargetExtraValue);
            foreach (var i in list)
                i.WriteXml(w);
            w.End();
        }
		public new static AskDropdown ReadXml(XElement ele)
		{
		    var dd = new AskDropdown
		    {
                TargetExtraValue = ele.Attribute("TargetExtraValue").ToBool(),
		        Label = ele.Element("Label")?.Value,
		    };
		    foreach (var ee in ele.Elements("DropdownItem"))
                if(ee.Element("Description")?.Value != null)
    		        dd.list.Add(DropdownItem.ReadXml(ee));
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
            if (choices == null)
                return null;
            var v = list.Where(i => i.SmallGroup != "nocheckbox").SingleOrDefault(i => choices.Contains(i.SmallGroup, StringComparer.OrdinalIgnoreCase));
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

        public partial class DropdownItem
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

            public void AddToSmallGroup(CMSDataContext Db, OrganizationMember om, PythonModel pe)
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

		    public void WriteXml(APIWriter w)
		    {
		        w.Start("DropdownItem")
		            .Attr("Fee", Fee)
		            .Attr("Limit", Limit)
		            .Attr("Time", MeetingTime.ToString2("s"))
		            .Add("Description", Description)
		            .Add("SmallGroup", SmallGroup)
		            .End();
		    }

		    // ReSharper disable once MemberHidesStaticFromOuterClass
		    public static DropdownItem ReadXml(XElement ele)
		    {
		        var i = new DropdownItem
		        {
		            Description = ele.Element("Description")?.Value,
		            Fee = ele.Attribute("Fee").ToDecimal(),
		            Limit = ele.Attribute("Limit").ToInt2(),
		            MeetingTime = ele.Attribute("Time").ToDate()
		        };
		        i.SmallGroup = (ele.Element("SmallGroup")?.Value ?? i.Description)?.TrimEnd();
				return i;
		    }
        }
    }
}
