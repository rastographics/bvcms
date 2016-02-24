using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CmsWeb.Areas.Reports.Models
{
    public class CustomColumnsModel
    {
        public Dictionary<string, CustomColumn> Columns { get; set; }
        public Dictionary<string, CustomColumn> SpecialColumns { get; set; }
        public Dictionary<string, string> Joins { get; set; }

        public CustomColumnsModel()
        {
            var xdoc = XDocument.Parse(Resource1.CustomColumns);
            if (xdoc.Root == null)
                return;
            Joins = GetJoins(xdoc.Root.Element("Joins"));
            Columns = GetColumns(xdoc.Root.Element("Columns"));
            SpecialColumns = GetColumns(xdoc.Root.Element("SpecialColumns"));
        }

        private static Dictionary<string, string> GetJoins(XContainer ele)
        {
            if (ele == null)
                return new Dictionary<string, string>();
            var ret = ele.Elements().ToDictionary(j => j.Attribute("name").Value, j => j.Value);
            return ret;
        }

        private static Dictionary<string, CustomColumn> GetColumns(XContainer ele)
        {
            if (ele == null)
                return new Dictionary<string, CustomColumn>();
            var ret = ele.Elements().Select(c => new CustomColumn
                {
                    Column = c.Attribute("name").Value,
                    Join = (string)c.Attribute("join"),
                    Select = c.Value.Trim(),
                    Context = (string)c.Attribute("context")
                }).ToDictionary(cc => cc.Column, cc => cc);
            return ret;
        }
    }
    public class CustomColumn
    {
        public string Column { get; set; }
        public string Join { get; set; }
        public string Select { get; set; }
        public string Context { get; set; }
    }
}