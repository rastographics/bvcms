using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class AskCheckboxes : Ask
    {
        public const string Help = @"
This is a group of checkboxes where you can check more than one.
You can specify a minumum number they must check.
And you can specify a maximum number they can check.

For each checkbox, you can specify the following:

* **SmallGroup** (required)
* **Fee** (optional) for the selection.
* **Limit** (optional) which limits the number of people allowed for a selection.
* **DateTime** (optional) which registers them in a meeting.
";
        public string Label { get; set; }
        public int? Minimum { get; set; }
        public int? Maximum { get; set; }
        public int? Columns { get; set; }
        public List<CheckboxItem> list { get; set; }

        public AskCheckboxes()
            : base("AskCheckboxes")
        {
            list = new List<CheckboxItem>();
        }

        public override void Output(StringBuilder sb)
        {
            if (list.Count == 0)
                return;
            Settings.AddValueNoCk(0, sb, "Checkboxes", Label);
            Settings.AddValueCk(1, sb, "Minimum", Minimum);
            Settings.AddValueCk(1, sb, "Maximum", Maximum);
            Settings.AddValueCk(1, sb, "Columns", Columns);
            foreach (var i in list)
                i.Output(sb);
            sb.AppendLine();
        }
        public static AskCheckboxes Parse(Parser parser)
        {
            var cb = new AskCheckboxes();
            cb.Label = parser.GetString("CheckBoxes");
            cb.Minimum = parser.GetInt(Parser.RegKeywords.Minimum);
            cb.Maximum = parser.GetInt(Parser.RegKeywords.Maximum);
            cb.Columns = parser.GetInt(Parser.RegKeywords.Columns) ?? 1;
            cb.list = new List<CheckboxItem>();
            if (parser.curr.indent == 0)
                return cb;
            var startindent = parser.curr.indent;
            while (parser.curr.indent == startindent)
            {
                var m = CheckboxItem.Parse(parser, startindent);
                cb.list.Add(m);
            }
            var q = (from i in cb.list
                     where i.SmallGroup != "nocheckbox"
                     where i.SmallGroup != "comment"
                     group i by i.SmallGroup into g
                     where g.Count() > 1
                     select g.Key).ToList();
            if (q.Any())
                throw parser.GetException("Duplicate SmallGroup in Checkboxes: {0}".Fmt(string.Join(",", q)));
            return cb;
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
            var q = from i in items
                    join c in list on i equals c.SmallGroup
                    select c;
            return q;
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
        public class CheckboxItem
        {
            public override string ToString()
            {
                return "{0}: {1}|{2} (limit={3},fee={4})".Fmt(Name, Description, SmallGroup, Limit, Fee);
            }
            public string Name { get; set; }
            public string Description { get; set; }
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

            public void Output(StringBuilder sb)
            {
                Settings.AddValueCk(1, sb, Description);
                Settings.AddValueCk(2, sb, "SmallGroup", SmallGroup);
                Settings.AddValueCk(2, sb, "Fee", Fee);
                Settings.AddValueCk(2, sb, "Limit", Limit);
                Settings.AddValueCk(2, sb, "Time", MeetingTime.ToString2("s"));
            }
            public static CheckboxItem Parse(Parser parser, int startindent)
            {
                var i = new CheckboxItem();
                if (parser.curr.kw != Parser.RegKeywords.None)
                    throw parser.GetException("unexpected line in CheckBoxes");
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
                            throw parser.GetException("unexpected line in CheckboxItem");
                    }
                }
                return i;
            }
            public void AddToSmallGroup(CMSDataContext Db, OrganizationMember om, PythonEvents pe)
            {
                if (om == null)
                    return;
                if (pe != null)
                {
                    pe.instance.AddToSmallGroup(SmallGroup, om);
                    om.Person.LogChanges(Db, om.PeopleId);
                }
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