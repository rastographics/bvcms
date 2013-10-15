using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmsWeb.Models.ExtraValues
{
    public class View
    {
        [XmlElement("Value")] public List<Value> Values { get; set; }
        [XmlAttribute] public string Table { get; set; }
        [XmlAttribute] public string Location { get; set; }

        public View()
        {
            Values = new List<Value>();
        }
    }
}