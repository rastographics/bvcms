using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using CmsData;
using CmsWeb.Areas.People.Models;
using MoreLinq;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Models.Org
{
    public class OrgSearchField
    {
        public string Field { get; set; }
        public bool Dropdown { get; set; }
        public string OrgType { get; set; }
        public List<SelectListItem> SelectList { get; set; }

        public string DivClass
        {
            get
            {
                if (string.IsNullOrEmpty(OrgType)) return "";
                return $"{OrgType.ToLower().Replace(" ", "")}-cell ev-orgtype-cell";
            }
        }
    }

    public class OrgSearchFieldsModel
    {
        public static List<OrgSearchField> GetFields()
        {
            string customTextName = "OrgSearchFields";
            var db = DbUtil.Db;
            var list = new List<OrgSearchField>();

            var s = HttpRuntime.Cache[DbUtil.Db.Host + customTextName] as string;
            if (s == null)
            {
                s = db.ContentText(customTextName, string.Empty);
                HttpRuntime.Cache.Insert(db.Host + customTextName, s, null,
                    DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
            }
            if (!s.HasValue() || s == string.Empty)
                return list; // Empty, but we already will display all the default fields

            XDocument xdoc;

            try
            {
                xdoc = XDocument.Parse(s);
            }
            catch (Exception)
            {
                return list; // Invalid XML, return empty list so just default list displayed
            }

            if (xdoc?.Root == null)
                return list;

            foreach (var e in xdoc.XPathSelectElements("/OrgSearch/Fields").Elements())
            {
                if (e.Name.LocalName.ToLower() == "field")
                {
                    var field = new OrgSearchField();
                    field.Field = e.Value;
                    field.OrgType = e.Attribute("orgtype")?.Value;

                    bool dropdown;
                    bool.TryParse(e.Attribute("dropdown")?.Value ?? "false", out dropdown);
                    field.Dropdown = dropdown;

                    if (dropdown)
                    {
                        var values =
                            DbUtil.Db.OrganizationExtras.Where(x => x.Field == field.Field)
                                .Select(x => x.Data ?? x.StrValue).Distinct().OrderBy(x => x);

                        var items = new List<SelectListItem>();
                        items.Add(new SelectListItem
                        {
                            Text = "(not specified)",
                            Value = ""
                        });

                        values.ForEach(x => items.Add(new SelectListItem
                        {
                            Text = x,
                            Value = x
                        }));

                        field.SelectList = items;
                    }

                    list.Add(field);
                }
            }
            return list;
        }
    }
}