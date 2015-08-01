using System.Xml.Linq;
using CmsData.API;

namespace CmsData.Registration
{
	public class AskTickets : Ask
	{
	    public override string Help => @"
This will ask for a number of items to purchase. 
The total will be the fee multipled by the number of items.
Good for things like number of lunches (so you can bring friends).
";
	    public string Label { get; set; }
		public AskTickets() : base("AskTickets") { }
	    public override void WriteXml(APIWriter w)
	    {
	        w.Start(Type)
	            .AddText(Label ?? "No. of Items")
	            .End();
	    }
	    public new static AskTickets ReadXml(XElement e)
	    {
	        return new AskTickets {Label = e.Value};
	    }
	}
}