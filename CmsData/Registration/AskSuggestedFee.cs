using System.Xml.Linq;
using CmsData.API;

namespace CmsData.Registration
{
	public class AskSuggestedFee : Ask
	{
	    public override string Help => @"Allows the final fee to be adjusted to any amount (including zero).";
	    public string Label { get; set; }
		public AskSuggestedFee() : base("AskSuggestedFee") { }
	    public override void WriteXml(APIWriter w)
	    {
	        w.Start(Type)
                .AddText(Label ?? "Suggested Amount");
	        w.End();
	    }
        public new static AskSuggestedFee ReadXml(XElement e)
        {
            return new AskSuggestedFee {
                Label = e.Value
            };
        }
	}
}
