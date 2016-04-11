using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Xml.Linq;
using CmsData;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class AskCheckboxes : Ask
    {
        public override string Help => @"
This is a group of checkboxes where you can check more than one.
You can specify a minumum number they must check.
And you can specify a maximum number they can check.
The Columns is only used when you need a grid of time slots and days (advanced).

For each checkbox, you can specify the following:

* **Sub-Group** (required)
* **Fee** (optional) for the selection.
* **Limit** (optional) which limits the number of people allowed for a selection.
* **DateTime** (optional) which registers them in a meeting.
";

        public string Label { get; set; }
        public bool HasLabel => Label.HasValue();
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public int? Columns { get; set; }
        public List<CheckboxItem> list { get; set; }

        public AskCheckboxes()
            : base("AskCheckboxes")
        {
            list = new List<CheckboxItem>();
        }
        public new static AskCheckboxes ReadXml(XElement ele)
        {
            var cb = new AskCheckboxes
            {
                Minimum = ele.Attribute("Minimum")?.Value.ToInt2(),
                Maximum = ele.Attribute("Maximum")?.Value.ToInt2(),
                Columns = ele.Attribute("Columns")?.Value.ToInt2(),
                Label = ele.Element("Label")?.Value,
            };
            foreach (var ee in ele.Elements("CheckboxItem"))
                cb.list.Add(CheckboxItem.ReadXml(ee));
            return cb;

        }
        public override void WriteXml(APIWriter w)
        {
            if (list.Count == 0)
                return;
            w.Start(Type)
                .Attr("Minimum", Minimum)
                .Attr("Maximum", Maximum)
                .Attr("Columns", Columns == 1 ? null : Columns)
                .AddCdata("Label", Label);
            foreach (var i in list)
                i.WriteXml(w);
            // todo: prevent duplicates
            w.End();
        }
        public override List<string> SmallGroups()
        {
            var q = (from i in list
                     where i.SmallGroup != "nocheckbox"
                     where i.SmallGroup != "comment"
                     select i.SmallGroup).ToList();
            return q;
        }
        public IEnumerable<CheckboxItem> CheckboxItemsChosen(IEnumerable<string> items)
        {
            try
            {
                var q = from i in items
                        join c in list on i equals c.SmallGroup
                        select c;
                return q;
            }
            catch (Exception)
            {
                return new List<CheckboxItem>();
            }
        }
        public bool IsSmallGroupFilled(List<string> smallgroups, string sg)
        {
            string desc;
            return IsSmallGroupFilled(smallgroups, sg, out desc);
        }
        public bool IsSmallGroupFilled(IEnumerable<string> smallgroups, string sg, out string desc)
        {
            var i = list.SingleOrDefault(dd => string.Compare(dd.SmallGroup, sg, StringComparison.OrdinalIgnoreCase) == 0);
            desc = null;
            if (i == null)
                return false;
            desc = i.Description;
            return i.IsSmallGroupFilled(smallgroups);
        }
        public partial class CheckboxItem
        {
            public override string ToString()
            {
                return $"{Name}: {Description}|{SmallGroup} (limit={Limit},fee={Fee})";
            }
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
            internal static CheckboxItem ReadXml(XElement ele)
            {
                var i = new CheckboxItem
                {
                    Description = ele.Element("Description")?.Value,
                    Fee = ele.Attribute("Fee")?.Value.ToDecimal(),
                    Limit = ele.Attribute("Limit")?.Value.ToInt2(),
                    MeetingTime = ele.Attribute("Time")?.Value.ToDate()
                };
                i.SmallGroup = (ele.Element("SmallGroup")?.Value ?? i.Description).TrimEnd();
                return i;
            }
            public void WriteXml(APIWriter w)
            {
                w.Start("CheckboxItem")
                    .Attr("Fee", Fee)
                    .Attr("Limit", Limit)
                    .Attr("Time", MeetingTime.ToString2("s"))
                    .Add("Description", Description)
                    .Add("SmallGroup", SmallGroup.trim())
                    .End();
            }
            public void AddToSmallGroup(CMSDataContext Db, OrganizationMember om, PythonModel pe)
            {
                if (om == null)
                    return;
                if (pe != null)
                {
                    pe.instance.AddToSmallGroup(SmallGroup.trim(), om);
                    om.Person.LogChanges(Db, om.PeopleId);
                }
                om.AddToGroup(Db, SmallGroup.trim());
                if (MeetingTime.HasValue)
                    Attend.MarkRegistered(Db, om.OrganizationId, om.PeopleId, MeetingTime.Value, 1);
            }
            public void RemoveFromSmallGroup(CMSDataContext Db, OrganizationMember om)
            {
                om.RemoveFromGroup(Db, SmallGroup.trim());
            }
            public bool IsSmallGroupFilled(IEnumerable<string> smallgroups)
            {
                if (!(Limit > 0)) return false;
                var cnt = smallgroups.Count(mm => mm.trim() == SmallGroup.trim());
                return cnt >= Limit;
            }
        }
    }
}
