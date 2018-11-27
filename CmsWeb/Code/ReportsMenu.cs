using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class ReportsMenuModel
    {
        private static XDocument CustomReportsMenu
        {
            get
            {

                //var db = Db;
#if DEBUG2
                var s = Resource1.ReportsMenuCustom;
#else
                var s = HttpRuntime.Cache[DbUtil.Db.Host + "CustomReportsMenu"] as string;
                if (s == null)
                {
                    s = DbUtil.Db.ContentText("CustomReportsMenu", defaultValue: "<ReportsMenu><Column1/><Column2/></ReportsMenu>");
                    HttpRuntime.Cache.Insert(DbUtil.Db.Host + "CustomReportsMenu", s, null,
                        DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
                }
                if (!s.HasValue())
                {
                    return null;
                }
#endif
                var xdoc = XDocument.Parse(s);
                return xdoc;
            }
        }
        private static List<CmsData.View.CustomMenuRole> CustomMenuRoles
        {
            get
            {

                //var db = Db;
                var s = HttpRuntime.Cache[DbUtil.Db.Host + "CustomMenuRoles"] as List<CmsData.View.CustomMenuRole>;
                if (s == null)
                {
                    s = DbUtil.Db.ViewCustomMenuRoles.ToList();
                    HttpRuntime.Cache.Insert(DbUtil.Db.Host + "CustomReportsMenu", s, null,
                        DateTime.Now.AddMinutes(Util.IsDebug() ? 0 : 1), Cache.NoSlidingExpiration);
                }
                return s;
            }
        }

        private static XDocument ReportsMenu
        {
            get { return XDocument.Parse(Resource1.ReportsMenu); }
        }

        public static string Items
        {
            get
            {
                var xdoc = ReportsMenu;
                if (xdoc?.Root == null)
                {
                    return null;
                }

                return ReportItems(xdoc, "/ReportsMenu");
            }
        }

        public static string CustomItems1
        {
            get
            {
                var xdoc = CustomReportsMenu;
                if (xdoc?.Root == null)
                {
                    return null;
                }

                return ReportItems(xdoc, "/ReportsMenu/Column1");
            }
        }
        public static string CustomItems2
        {
            get
            {
                var xdoc = CustomReportsMenu;
                if (xdoc?.Root == null)
                {
                    return null;
                }

                return ReportItems(xdoc, "/ReportsMenu/Column2");
            }
        }

        private static string ReportItems(XDocument xdoc, string path)
        {
            var listroles = CustomMenuRoles;
            var userroles = DbUtil.Db.CurrentUser.Roles;
            var sb = new StringBuilder();
            foreach (var e in xdoc.XPathSelectElements(path).Elements())
            {
                var link = e.Attribute("link")?.Value;
                var roles = new List<string>();
                var excludedRoles = new List<string>();

                var aroles = e.Attribute("roles")?.Value;
                if (aroles != null && aroles.HasValue())
                {
                    roles.AddRange(aroles.Split(','));
                }

                var eroles = e.Attribute("excludedRoles")?.Value;
                if (eroles != null && eroles.HasValue())
                {
                    excludedRoles.AddRange(eroles.Split(','));
                }

                var rroles = listroles.FirstOrDefault(vv => vv.Link == link)?.Role;
                if (rroles != null && rroles.HasValue())
                {
                    roles.AddRange(rroles.Split(','));
                }

                if (roles.Count > 0)
                {
                    if (!roles.Any(rr => userroles.Contains(rr)))
                    {
                        continue;
                    }
                }

                if (excludedRoles.Count > 0)
                {
                    if (excludedRoles.Any(rr => userroles.Contains(rr)))
                    {
                        continue;
                    }
                }

                var tb = new TagBuilder("li");
                switch (e.Name.LocalName)
                {
                    case "Report":
                        var a = new TagBuilder("a");
                        a.MergeAttribute("href", e.Attribute("link").Value);
                        var t = e.Attribute("target");
                        if (t != null)
                        {
                            a.MergeAttribute("target", t.Value);
                        }

                        a.SetInnerText(e.Value);
                        tb.InnerHtml = a.ToString();
                        break;
                    case "Header":
                        tb.AddCssClass("dropdown-header");
                        tb.AddCssClass("dropdown-sub-header");
                        tb.SetInnerText(e.Value);
                        break;
                    case "Space":
                        tb.AddCssClass("divider");
                        break;
                }
                sb.AppendLine(tb.ToString());
            }
            return sb.ToString();
        }
    }
}
