using System.Text;
using System.Xml;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskTickets : Ask
	{
	    public override string Help
	    {
	        get 
            { return @"
This will ask for a number of items to purchase. 
The total will be the fee multipled by the number of items.
Good for things like number of lunches (so you can bring friends).
"; 
            }
	    }
		public string Label { get; set; }
		public AskTickets() : base("AskTickets") { }
		public static AskTickets Parse(Parser parser)
		{
			var r = new AskTickets();
			parser.GetBool();
			r.Label = parser.GetLabel("No. of Items");
			return r;
		}
		public override void Output(StringBuilder sb)
		{
            Settings.AddValueCk(0, sb, "AskTickets", true);
			if (!Label.HasValue())
				Label = "No. of Items";
			Settings.AddValueCk(1, sb, "Label", Label);
			sb.AppendLine();
		}
	    public override void WriteXml(APIWriter w)
	    {
	        w.Start(Type);
            w.AddText(Label ?? "No. of Items");
	        w.End();
	    }
	}
}