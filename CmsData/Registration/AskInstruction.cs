using System.ComponentModel;
using System.Text;
using System.Xml;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskInstruction : Ask
	{
	    public override string Help
	    {
	        get { return @"Displays the label text (can include HTML) on the registration page to provide some brief instruction."; }
	    }
        [DisplayName("Text")]
		public string Label { get; set; }
		public AskInstruction() : base("AskInstruction") { }
		public static AskInstruction Parse(Parser parser)
		{
			var r = new AskInstruction();
			parser.GetBool();
			r.Label = parser.GetLabel("Instruction");
			return r;
		}
		public override void Output(StringBuilder sb)
		{
			Settings.AddValueCk(0, sb, "AskInstruction", true);
			if (!Label.HasValue())
				Label = "Instruction";
			Settings.AddValueCk(1, sb, "Label", Label);
			sb.AppendLine();
		}
	    public override void WriteXml(XmlWriter writer)
	    {
            var w = new APIWriter(writer);
	        if (!Label.HasValue())
	            Label = "Instruction";
            w.AddCdata(Type, Label);
	    }
	}
}