using System.Xml.Serialization;

namespace CmsData.Classes.ExtraValues
{
    public class Code
    {
        [XmlAttribute] public string Metadata { get; set; }
        [XmlText] public string Text { get; set; }
    }
}
