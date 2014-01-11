using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmsData.ExtraValue
{
    // A View is a place for a set of Extravalues to be seen in the UI, like Standard, Entry, Family etc.
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