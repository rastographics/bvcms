using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CmsData.API;
using CmsData.Registration;
using UtilityExtensions;

namespace RegistrationSettingsParser
{
    public partial class Parser
    {
        public AskHeader ParseAskHeader()
        {
            var r = new AskHeader();
            GetBool();
            r.Label = GetLabel("Header");
            return r;
        }
        public void Output(StringBuilder sb, AskHeader ask)
        {
            AddValueCk(0, sb, "AskHeader", true);
            if (!ask.Label.HasValue())
                ask.Label = "Header";
            AddValueCk(1, sb, "Label", ask.Label);
            sb.AppendLine();
        }
    }
}