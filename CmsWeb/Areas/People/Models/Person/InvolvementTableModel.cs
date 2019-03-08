using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class InvolvementTableColumn
    {
        public string Page { get; set; }
        public string Field { get; set; }
        public string Label { get; set; }
        public bool Sortable { get; set; }
        public List<string> Roles { get; set; }
    }

    public class InvolvementTableColumnSet
    {
        public List<InvolvementTableColumn> DefaultColumns { get; set; }
        public Dictionary<string, List<InvolvementTableColumn>> OrgTypeColumns { get; set; }

        public InvolvementTableColumnSet()
        {
            DefaultColumns = new List<InvolvementTableColumn>();
            OrgTypeColumns = new Dictionary<string, List<InvolvementTableColumn>>();
        }

        public bool HasOrgTypeColumns
        {
            get { return OrgTypeColumns.Any(); }
        }

        public List<InvolvementTableColumn> GetColumnsForOrgType(string orgtype, string sort)
        {
            if (!OrgTypeColumns.ContainsKey(orgtype) || sort != "default")
            {
                return DefaultColumns;
            }

            var columnsToRemove = (from column in OrgTypeColumns[orgtype]
                                   where column.Roles != null
                                   from role in column.Roles
                                   where !HttpContextFactory.Current.User.IsInRole(role)
                                   select column).ToList();

            return OrgTypeColumns[orgtype].Where(x => !columnsToRemove.Contains(x)).ToList();
        }
    }

    public class InvolvementTableModel
    {
        public InvolvementTableModel() { }
        public static List<InvolvementTableColumn> GetColumns(string page)
        {
            var columnset = GetColumnSet(page);
            return columnset.DefaultColumns;
        }

        public static InvolvementTableColumnSet GetColumnSet(string page)
        {
            string customTextName, defaultXml;
            switch (page)
            {
                default:
                    customTextName = "InvolvementTableCurrent";
                    defaultXml = Resource1.InvolvementTableCurrent;
                    break;
                case "Pending":
                    customTextName = "InvolvementTablePending";
                    defaultXml = Resource1.InvolvementTablePending;
                    break;
                case "Previous":
                    customTextName = "InvolvementTablePrevious";
                    defaultXml = Resource1.InvolvementTablePrevious;
                    break;
            }

            //var db = Db;

            var s = HttpRuntime.Cache[DbUtil.Db.Host + customTextName] as string;
            if (s == null)
            {
                s = DbUtil.Db.ContentText(customTextName, defaultXml);
                HttpRuntime.Cache.Insert(DbUtil.Db.Host + customTextName, s, null,
                    DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
            }
            if (!s.HasValue())
            {
                return null;
            }

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Parse(s);
            }
            catch (Exception)
            {
                // If the above fails for any reason, fall back on the default
                xdoc = XDocument.Parse(defaultXml);
            }

            if (xdoc?.Root == null)
            {
                return new InvolvementTableColumnSet();
            }

            var set = new InvolvementTableColumnSet();
            foreach (var d in xdoc.XPathSelectElements("/InvolvementTable").Elements())
            {
                if (d.Name.LocalName.ToLower() == "columns")
                {
                    var list = new List<InvolvementTableColumn>();

                    foreach (var e in d.DescendantsAndSelf())
                    {
                        if (e.Name.LocalName.ToLower() == "column")
                        {
                            var column = new InvolvementTableColumn();
                            column.Field = e.Attribute("field")?.Value;
                            column.Label = e.Attribute("label")?.Value ?? column.Field;
                            column.Page = page;

                            bool sortable;
                            bool.TryParse(e.Attribute("sortable")?.Value ?? "false", out sortable);
                            column.Sortable = sortable;
                            column.Roles = e.Attribute("role")?.Value.Split(',').ToList();

                            list.Add(column);
                        }
                    }

                    var orgtype = d.Attribute("orgtype")?.Value;
                    if (orgtype != null)
                    {
                        set.OrgTypeColumns.Add(orgtype, list);
                    }
                    else
                    {
                        set.DefaultColumns = list;
                    }
                }
            }
            return set;
        }
    }
}
