using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Areas.People.Models
{
    public class InvolvementTableColumn
    {
        public string Field { get; set; }
        public bool Sortable { get; set; }
    }

    public class InvolvementTableModel
    {
        private static XDocument TableDoc
        {
            get
            {

                var db = DbUtil.Db;

                var s = HttpRuntime.Cache[DbUtil.Db.Host + "InvolvementTables"] as string;
                if (s == null)
                {
                    s = db.ContentText("InvolvementTables", Resource1.ReportsMenuCustom);
                    HttpRuntime.Cache.Insert(db.Host + "InvolvementTables", s, null,
                        DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
                }
                if (!s.HasValue())
                    return null;

                var xdoc = XDocument.Parse(s);
                return xdoc;
            }
        }

        public static List<InvolvementTableColumn> Columns
        {
            get
            {
                var xdoc = TableDoc;
                if (xdoc?.Root == null)
                    return new List<InvolvementTableColumn>();

                var list = new List<InvolvementTableColumn>();
                foreach (var e in xdoc.XPathSelectElements("/InvolvementTable/Columns").Elements())
                {
                    if (e.Name.LocalName.ToLower() == "column")
                    {
                        var column = new InvolvementTableColumn();
                        column.Field = e.Attribute("field")?.Value;

                        bool sortable;
                        bool.TryParse(e.Attribute("sortable")?.Value ?? "false", out sortable);
                        column.Sortable = sortable;

                        list.Add(column);
                    }
                }
                return list;
            }
        }
    }
}