using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CmsWeb.Areas.Manage.Models.Involvement
{
    public class InvolvementTabModel
    {
        private const int COLUMN_MAX = 6;

        public InvolvementTabModel()
        {
            Types = new List<InvolvementType>();
        }

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

        public string BuildXml()
        {
            var xDoc = new XDocument(
                new XElement("InvolvementTable",
                    Types.Select(type => new XElement("Columns",
                            type.Name != null ? new XAttribute("orgtype", type.Name) : null,
                            type.Columns.Select(column => !IsColumnNull(column) ? new XElement("Column",
                                column.Field != null ? new XAttribute("field", column.Field) : null,
                                column.Label != null ? new XAttribute("label", column.Label) : null,
                                column.Role != null ? new XAttribute("role", column.Role) : null,
                                column.Sortable != null && column.Sortable.Value ? new XAttribute("sortable", column.Sortable) : null) : null)
                            )
                        )
                    )
                );

            return xDoc.ToString();
        }

        private bool IsColumnNull(InvolvementColumn column)
        {
            return string.IsNullOrWhiteSpace(column.Field) &&
                   string.IsNullOrWhiteSpace(column.Label) &&
                   string.IsNullOrWhiteSpace(column.Role) &&
                   column.Sortable == null;
        }

        public IEnumerable<SelectListItem> TypeSelectList(string selectedValue)
        {
            var selectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Mailing List",
                    Value = "Mailing List"
                },
                new SelectListItem
                {
                    Text = "Leader Coach Group",
                    Value = "Leader Coach Group"
                },
                new SelectListItem
                {
                    Text = "Community Group",
                    Value = "Community Group"
                },
                new SelectListItem
                {
                    Text = "Registration",
                    Value = "Registration"
                },
                new SelectListItem
                {
                    Text = "Volunteer Group",
                    Value = "Volunteer Group"
                },
                new SelectListItem
                {
                    Text = "Children and Youth Group",
                    Value = "Children and Youth Group"
                }
            };

            var selected = selectList.FirstOrDefault(x => x.Value == selectedValue);
            if (selected != null)
            {
                selected.Selected = true;
            }

            return selectList;
        }

        public IEnumerable<SelectListItem> ColumnSelectList(string selectedValue)
        {
            var selectList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Text = "Organization",
                    Value = "Organization"
                },
                new SelectListItem
                {
                    Text = "Leader",
                    Value = "Leader"
                },
                new SelectListItem
                {
                    Text = "Enroll Date",
                    Value = "Enroll Date"
                },
                new SelectListItem
                {
                    Text = "MemberType",
                    Value = "MemberType"
                },
                new SelectListItem
                {
                    Text = "Unsubscribe",
                    Value = "Leave"
                },
                new SelectListItem
                {
                    Text = "View Calendar",
                    Value = "ViewCalendar"
                },
                new SelectListItem
                {
                    Text = "Location",
                    Value = "Location"
                },
                new SelectListItem
                {
                    Text = "Schedule",
                    Value = "Schedule"
                },
                new SelectListItem
                {
                    Text = "Drop Date",
                    Value = "Drop Date"
                },
                new SelectListItem
                {
                    Text = "Last Visit",
                    Value = "Last Visit"
                },
                new SelectListItem
                {
                    Text = "Health",
                    Value = "Health"
                },
                new SelectListItem
                {
                    Text = "AttendPct",
                    Value = "AttendPct"
                }
            };

            var selected = selectList.FirstOrDefault(x => x.Value == selectedValue);
            if (selected != null)
            {
                selected.Selected = true;
            }

            return selectList;
        }
    }
}
