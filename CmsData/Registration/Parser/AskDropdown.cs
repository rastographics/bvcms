using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public void Output(StringBuilder sb, AskDropdown ask)
        {
            if (ask.list == null || ask.list.Count == 0)
                return;
            AddValueNoCk(0, sb, "Dropdown", ask.Label);
            foreach (var i in ask.list)
            {
                AddValueCk(1, sb, i.Description);
                AddValueCk(2, sb, "SmallGroup", i.SmallGroup);
                AddValueCk(2, sb, "Fee", i.Fee);
                AddValueCk(2, sb, "Limit", i.Limit);
                AddValueCk(2, sb, "Time", i.MeetingTime.ToString2("s"));
            }
            sb.AppendLine();
        }
        public AskDropdown ParseAskDropdown()
        {
            var dd = new AskDropdown();
            dd.Label = GetString("Dropdown");
            dd.list = new List<AskDropdown.DropdownItem>();
            if (curr.indent == 0)
                return dd;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                var i = new AskDropdown.DropdownItem();
                if (curr.kw != Parser.RegKeywords.None)
                    throw GetException("unexpected line in Dropdown");
                i.Description = GetLine();
                i.SmallGroup = i.Description;
                if (curr.indent <= startindent)
                {
                    dd.list.Add(i);
                    continue;
                }
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    switch (curr.kw)
                    {
                        case RegKeywords.SmallGroup:
                            i.SmallGroup = GetString(i.Description);
                            break;
                        case RegKeywords.Fee:
                            i.Fee = GetDecimal();
                            break;
                        case RegKeywords.Limit:
                            i.Limit = GetNullInt();
                            break;
                        case RegKeywords.Time:
                            i.MeetingTime = GetDateTime();
                            break;
                        default:
                            throw GetException("unexpected line in DropdownItem");
                    }
                }
                dd.list.Add(i);
            }
            var q = (from i in dd.list
                     group i by i.SmallGroup into g
                     where g.Count() > 1
                     select g.Key).ToList();
            if (q.Any())
                throw GetException($"Duplicate SmallGroup in Dropdown: {string.Join(",", q)}");
            return dd;
        }
    }
}