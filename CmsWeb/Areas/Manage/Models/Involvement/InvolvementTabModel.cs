using System.Collections.Generic;
using System.Xml.Linq;

namespace CmsWeb.Areas.Manage.Models.Involvement
{
    public class InvolvementTabModel
    {
        private const int COLUMN_MAX = 6;

        public InvolvementTabModel(string name)
        {
            Name = name;
            Types = new List<InvolvementType>();
        }

        public string Name { get; set; }

        public string Xml { get; private set; }

        public List<InvolvementType> Types { get; set; }

        public void ReadXml(string xml)
        {
            Xml = xml;

            var xDoc = XDocument.Parse(xml);
            foreach (XElement columns in xDoc.Descendants("Columns"))
            {
                var involvementType = new InvolvementType { Name = columns.Attribute("orgtype")?.Value };
                foreach (XElement column in columns.Descendants("Column"))
                {
                    involvementType.Columns.Add(new InvolvementColumn
                    {
                        Field = column.Attribute("field")?.Value,
                        Label = column.Attribute("label")?.Value,
                        Role = column.Attribute("role")?.Value,
                        Sortable = column.Attribute("sortable")?.Value == "true",
                    });
                }
                while (involvementType.Columns.Count < COLUMN_MAX)
                {
                    involvementType.Columns.Add(new InvolvementColumn());
                }
                Types.Add(involvementType);
            }
        }
    }
}
