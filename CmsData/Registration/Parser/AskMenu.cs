using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
    public partial class Parser 
    {
        public void Output(StringBuilder sb, AskMenu ask)
        {
            if (ask.list.Count == 0)
                return;
            AddValueNoCk(0, sb, "MenuItems", ask.Label);
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
        public AskMenu ParseAskMenu()
        {
            var mi = new AskMenu();
            mi.Label = GetString("Menu");
            mi.list = new List<AskMenu.MenuItem>();
            if (curr.indent == 0)
                return mi;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                var m = new AskMenu.MenuItem();
                if (curr.kw != Parser.RegKeywords.None)
                    throw GetException("unexpected line in MenuItem");
                m.Description = GetLine();
                m.SmallGroup = m.Description;
                if (curr.indent <= startindent)
                {
                    mi.list.Add(m);
                    continue;
                }
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    switch (curr.kw)
                    {
                        case Parser.RegKeywords.SmallGroup:
                            m.SmallGroup = GetString(m.Description);
                            break;
                        case Parser.RegKeywords.Fee:
                            m.Fee = GetDecimal();
                            break;
                        case Parser.RegKeywords.Limit:
                            m.Limit = GetNullInt();
                            break;
                        case Parser.RegKeywords.Time:
                            m.MeetingTime = GetDateTime();
                            break;
                        default:
                            throw GetException("unexpected line in MenuItem");
                    }
                }
                mi.list.Add(m);
            }
            var q = (from i in mi.list
                     group i by i.SmallGroup into g
                     where g.Count() > 1
                     select g.Key).ToList();
            if (q.Any())
                throw GetException($"Duplicate SmallGroup in MenuItems: {string.Join(",", q)}");
            return mi;
        }
    }
}