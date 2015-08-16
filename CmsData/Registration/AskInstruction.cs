using System.ComponentModel;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskInstruction : Ask
	{
	    public override string Help => @"Displays the label text (can include HTML) on the registration page to provide some brief instruction.";

	    [DisplayName("Text")]
		public string Label { get; set; }
		public AskInstruction() : base("AskInstruction") { }
	    public override void WriteXml(APIWriter w)
	    {
	        if (!Label.HasValue())
	            Label = "Instruction";
            w.AddCdata(Type, Label);
	    }
        public new static AskInstruction ReadXml(XElement e)
        {
            return new AskInstruction() { Label = e.Value };
        }
	}
}