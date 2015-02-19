using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class OrgFees
	{
		public List<OrgFee> list { get; private set; }
		public bool HasValue { get { return list.Count > 0; } }

		public OrgFees()
		{
			list = new List<OrgFee>();
		}
		public void Output(StringBuilder sb)
		{
			if (list.Count == 0)
				return;
			Settings.AddValueNoCk(0, sb, "OrgFees", "");
			foreach (var i in list)
				i.Output(sb);
			sb.AppendLine();
		}
		public static OrgFees Parse(Parser parser)
		{
			var ff = new OrgFees();
			parser.lineno++;
			var startindent = parser.curr.indent;
			while (parser.curr.indent == startindent)
			{
				var f = OrgFee.Parse(parser, startindent);
				ff.list.Add(f);
			}
			return ff;
		}

		public class OrgFee
		{
			public int? OrgId { get; set; }
			public decimal? Fee { get; set; }
            public string Name { get; set; }
			public void Output(StringBuilder sb)
			{
				Settings.AddValueCk(1, sb, "{0}".Fmt(OrgId));
				Settings.AddValueCk(2, sb, "Fee", Fee);
			}
			public static OrgFee Parse(Parser parser, int startindent)
			{
				if (parser.curr.kw != Parser.RegKeywords.None)
					throw parser.GetException("expected orgid only");
				var oid = parser.GetInt();
				if (oid == 0)
				{
					parser.lineno--;
					throw parser.GetException("invalid orgid");
				}
				var f = new OrgFee { OrgId = oid };
				if (parser.curr.indent > startindent)
				{
					if (parser.curr.kw != Parser.RegKeywords.Fee)
						throw parser.GetException("expected fee");
					f.Fee = parser.GetDecimal();
				}
				return f;
			}
		}
	}
}