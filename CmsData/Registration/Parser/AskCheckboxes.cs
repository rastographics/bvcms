using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public void Output(StringBuilder sb, AskCheckboxes ask)
        {
            if (ask.list.Count == 0)
                return;
            AddValueNoCk(0, sb, "Checkboxes", ask.Label);
            AddValueCk(1, sb, "Minimum", ask.Minimum);
            AddValueCk(1, sb, "Maximum", ask.Maximum);
            AddValueCk(1, sb, "Columns", ask.Columns);
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
        public AskCheckboxes ParseAskCheckboxes()
        {
            var cb = new AskCheckboxes();
            cb.Label = GetString("CheckBoxes");
            cb.Minimum = GetInt(RegKeywords.Minimum);
            cb.Maximum = GetInt(RegKeywords.Maximum);
            cb.Columns = GetInt(RegKeywords.Columns) ?? 1;
            cb.list = new List<AskCheckboxes.CheckboxItem>();
            if (curr.indent == 0)
                return cb;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                var i = new AskCheckboxes.CheckboxItem();
                if (curr.kw != Parser.RegKeywords.None)
                    throw GetException("unexpected line in CheckBoxes");
                i.Description = GetLine();
                i.SmallGroup = i.Description;
                if (curr.indent <= startindent)
                {
                    cb.list.Add(i);
                    continue;
                }
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    switch (curr.kw)
                    {
                        case Parser.RegKeywords.SmallGroup:
                            i.SmallGroup = GetString(i.Description);
                            break;
                        case Parser.RegKeywords.Fee:
                            i.Fee = GetDecimal();
                            break;
                        case Parser.RegKeywords.Limit:
                            i.Limit = GetNullInt();
                            break;
                        case Parser.RegKeywords.Time:
                            i.MeetingTime = GetDateTime();
                            break;
                        default:
                            throw GetException("unexpected line in CheckboxItem");
                    }
                }
                cb.list.Add(i);
            }
            var q = (from i in cb.list
                     where i.SmallGroup != "nocheckbox"
                     where i.SmallGroup != "comment"
                     group i by i.SmallGroup into g
                     where g.Count() > 1
                     select g.Key).ToList();
            if (q.Any())
                throw GetException($"Duplicate SmallGroup in Checkboxes: {string.Join(",", q)}");
            return cb;
        }
    }
}
