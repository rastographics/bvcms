using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CmsWeb.Areas.Reports.Models
{
    public class CustomColumn
    {
        public string Column { get; set; }
        public string Join { get; set; }
        public string Select { get; set; }
    }
    public class CustomColumnsModel
    {
        public Dictionary<string, CustomColumn> Columns { get; set; }
        public Dictionary<string, string> Joins { get; set; }

        public CustomColumnsModel()
        {
            Columns = new Dictionary<string, CustomColumn>();
            Joins = new Dictionary<string, string>();

            var ccdoc = XDocument.Parse(Resource1.CustomColumns);

            if (ccdoc.Root == null)
                return;
            var joinselement = ccdoc.Root.Element("Joins");
            if (joinselement == null)
                return;
            var columnselement = ccdoc.Root.Element("Columns");
            if (columnselement == null)
                return;

            Joins = joinselement.Elements().ToDictionary(j => j.Attribute("name").Value, j => j.Value);

            Columns = columnselement.Elements().Select(c => new CustomColumn
                {
                    Column = c.Attribute("name").Value,
                    Join = (string)c.Attribute("join"),
                    Select = c.Value.Trim(),
                }).ToDictionary(cc => cc.Column, cc => cc);
        }
    }
}