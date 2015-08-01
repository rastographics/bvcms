using System.Collections.Generic;
using System.Linq;
using System.Text;
using CmsData.Registration;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public AskSize ParseAskSize()
        {
            var r = new AskSize();
            lineno++;
            r.Label = GetLabel("Size");
            if (curr.kw == RegKeywords.Fee)
                r.Fee = GetDecimal();
            if (curr.kw == RegKeywords.AllowLastYear)
                r.AllowLastYear = GetBool();
            r.list = new List<AskSize.Size>();
            if (curr.indent == 0)
                return r;
            var startindent = curr.indent;
            while (curr.indent == startindent)
            {
                var i = new AskSize.Size();
                if (curr.kw != RegKeywords.None)
                    throw GetException("unexpected line in Size");
                i.Description = GetLine();
                i.SmallGroup = i.Description;
                if (curr.indent <= startindent)
                {
                    r.list.Add(i);
                    continue;
                }
                var ind = curr.indent;
                while (curr.indent == ind)
                {
                    if (curr.kw != Parser.RegKeywords.SmallGroup)
                        throw GetException("unexpected line in Size");
                    i.SmallGroup = GetString(i.Description);
                }
                r.list.Add(i);
            }
            var q = r.list.GroupBy(mi => mi.SmallGroup).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            if (q.Any())
                throw GetException("Duplicate SmallGroup in Sizes: " + string.Join(",", q));
            return r;
        }

        public void Output(StringBuilder sb, AskSize ask)
        {
            AddValueNoCk(0, sb, "AskSize", "");
            AddValueCk(1, sb, "Label", ask.Label ?? "Size");
            AddValueCk(1, sb, "Fee", ask.Fee);
            AddValueCk(1, sb, "AllowLastYear", ask.AllowLastYear);
            foreach (var q in ask.list)
            {
                AddValueCk(1, sb, q.Description);
                AddValueCk(2, sb, "SmallGroup", q.SmallGroup ?? q.Description);
            }
            sb.AppendLine();
        }
		public List<AskSize.Size> ParseShirtSizes()
		{
			lineno++;
			var list = new List<AskSize.Size>();
			if (curr.indent == 0)
				return list;
			var startindent = curr.indent;
			while (curr.indent == startindent)
			{
				var shirtsize = new AskSize.Size();
				if (curr.kw != RegKeywords.None)
					throw GetException("unexpected line");
				shirtsize.Description = GetLine();
				shirtsize.SmallGroup = shirtsize.Description;
				if (curr.indent > startindent)
				{
					if (curr.kw != RegKeywords.SmallGroup)
						throw GetException("expected SmallGroup keyword");
					shirtsize.SmallGroup = GetString(shirtsize.SmallGroup);
				}
				list.Add(shirtsize);
			}
			return list;
		}
    }
}