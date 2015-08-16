using System.ComponentModel;
using System.Xml.Linq;
using CmsData.API;
using UtilityExtensions;

namespace CmsData.Registration
{
    public class AskHeader : Ask
    {
        public override string Help => @"
Displays the label text (can include HTML) on the registration page.
This can be used to separate sections.
";

        [DisplayName("Text/HTML")]
        public string Label { get; set; }
        public AskHeader() : base("AskHeader") { }
        public override void WriteXml(APIWriter w)
        {
            if (!Label.HasValue())
                Label = "Header";
            w.AddCdata(Type, Label);
        }
        public new static AskHeader ReadXml(XElement e)
        {
            var h = new AskHeader() { Label = e.Value };
            return h;
        }
    }
}