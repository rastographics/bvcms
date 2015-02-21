using System.Text;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskRequest : Ask
	{
	    public override string Help
	    {
	        get 
            { return @"
Displays a text box for entering things like roomate/teacher/coach request.
You can put a label on this text box to clarify what you are asking.
"; 
            }
	    }
		public string Label { get; set; }
		public AskRequest() : base("AskRequest") { }
		public static AskRequest Parse(Parser parser)
		{
			var r = new AskRequest();
			parser.GetBool();
			r.Label = parser.GetLabel("Request");
			return r;
		}
		public override void Output(StringBuilder sb)
		{
			Settings.AddValueCk(0, sb, "AskRequest", true);
			if (!Label.HasValue())
				Label = "Request";
			Settings.AddValueCk(1, sb, "Label", Label);
			sb.AppendLine();
		}
	}
}