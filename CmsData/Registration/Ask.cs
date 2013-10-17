using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class Ask
	{
		public string Type { get; set; }
		public string Name { get; set; }
		public int UniqueId { get; set; }

		public Ask(string type)
		{
			Type = type;
		}
		public static Ask ParseAsk(Parser parser)
		{
			var r = new Ask(parser.curr.kw.ToString());
			parser.GetBool();
			return r;
		}
		public virtual void Output(StringBuilder sb)
		{
			Settings.AddValueCk(0, sb, Type, true);
		}
        public virtual List<string> SmallGroups()
        {
            return new List<string>();
        }

    }
}
