using System;
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
            Joins = getJoins(xdoc.Root.Element("Joins"));
            Columns = getColumns(xdoc.Root.Element("Columns"));
            SpecialColumns = getColumns(xdoc.Root.Element("SpecialColumns"));
        }

        private Dictionary<string, string> getJoins(XElement ele)
        {
            if (ele == null)
                return new Dictionary<string, string>();
            var ret = ele.Elements().ToDictionary(j => j.Attribute("name").Value, j => j.Value);
            return ret;
        }
        private Dictionary<string, CustomColumn> getColumns(XElement ele)
        {
            if (ele == null)
                return new Dictionary<string, CustomColumn>();
            var ret = ele.Elements().Select(c => new CustomColumn
                {
                    Column = c.Attribute("name").Value,
                    Join = (string)c.Attribute("join"),
                    Select = c.Value.Trim(),
                }).ToDictionary(cc => cc.Column, cc => cc);
            return ret;
        }
    }
    public class CustomColumn
    {
        public string Column { get; set; }
        public string Join { get; set; }
        public string Select { get; set; }
    }
}