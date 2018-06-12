using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
	public class AskRequest : Ask
	{
	    public override string Help => @"
Displays a text box for entering things like roomate/teacher/coach request.
You can put a label on this text box to clarify what you are asking.
";
	    public string Label { get; set; }
		public AskRequest() : base("AskRequest") { }
	    public override void WriteXml(APIWriter w)
	    {
	        if (!Label.HasValue())
	            Label = "Request";
            w.Add("AskRequest", Label);
	    }
        public new static AskRequest ReadXml(XElement e)
        {
            return new AskRequest {
                Label = e.Value,
            };
        }
	}
}
