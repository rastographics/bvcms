using CmsData;
using CmsData.API;
using CmsData.Codes;
using CmsData.ExtraValue;
using CmsData.View;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.Reports.ViewModels;
using CmsWeb.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using UtilityExtensions;
using UtilityExtensions.Extensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class CustomReportsModel
    {
        private Organization _currentOrganization;
        private readonly CMSDataContext db;
        private readonly CustomColumnsModel mc;
        private readonly int? orgid;
        private readonly Guid queryid;
        public readonly string Report;

        [Obsolete("A constructor that passes the db context is preferred")]
        public CustomReportsModel()
        {
            // didn't supply the context, so go ahead and set it here to avoid multiple calls
            db = DbUtil.Db;
        }

        public CustomReportsModel(CMSDataContext db, int? orgId = null)
        {
            this.db = db;
            this.orgid = orgId;
            mc = new CustomColumnsModel();
        }

        public CustomReportsModel(CMSDataContext db, string report, Guid? id = null, int? orgId = null)
            : this(db, orgId)
        {
            Report = report;
            if (id != null)
            {
                queryid = id.Value;
            }

            if (orgid == null)
            {
                orgid = db.CurrentSessionOrgId;
            }
        }

        public class ReportItem
        {
            public ReportItem(string report, string type, string @class, string url)
            {
                Report = report;
                Type = type;
                Class = @class ?? "ViewReport";
                Url = url;
            }

            public string Report { get; set; }
            public string Type { get; set; }
            public string Class { get; set; }
            public string Url { get; set; }

            public override string ToString()
            {
                return Report;
            }

            public string Display => Report.SpaceCamelCase();
        }

        public Organization Org => _currentOrganization ?? (_currentOrganization = db.LoadOrganizationById(orgid));
        public string ExcelUrl => $"/Reports/CustomExcel/{Report}/{queryid}";
        public string EditUrl => GetEditUrl(Report, queryid, orgid);
        public string NewUrl(int? oid, Guid qid) => $"/Reports/EditCustomReport/{qid}{(oid > 0 ? $"?orgid={oid}" : "")}";
        public string DeleteUrl => $"/Reports/DeleteCustomReport/{Report}";
        public string Name => Report.SpaceCamelCase();

        public static string GetEditUrl(string report, Guid queryid, int? orgid) => $"/Reports/EditCustomReport/{report}/{queryid}{(orgid > 0 ? $"?orgid={orgid}" : "")}";

        private bool? isorg;

        public bool IsOrg
        {
            get
            {
                if (isorg.HasValue)
                {
                    return isorg.Value;
                }

                Sql(); // side effect of setting isorg
                return (bool)(isorg = isorg ?? false);
            }
        }

        public string Name2 => IsOrg ? Org?.FullName : "";

        private List<ReportItem> list;
        public List<ReportItem> List => list ?? (list = ReportList());

        private List<ReportItem> ReportList()
        {
            if (list != null)
            {
                return list;
            }

            var currentUserRoles = db.CurrentUser.Roles;
            var li = db.ViewCustomScriptRoles.ToList();
            var q = from e in li
                    let roles = (e.Role ?? "").Split(',').ToList()
                    where e.Role == null || roles.Any(rr => currentUserRoles.Contains(rr))
                    where e.Name != null
                    where e.ShowOnOrgId == null || e.ShowOnOrgId == orgid
                    select new ReportItem(e.Name, e.Type, e.ClassX, e.Url);
            list = new List<ReportItem>();
            foreach (var r in q.Where(r => !list.Contains(r)))
            {
                list.Add(r);
            }

            return list;
        }

        public List<ReportItem> CustomList()
        {
            return List.Where(vv => vv.Type == "Custom").ToList();
        }

        public List<ReportItem> UrlList()
        {
            return List.Where(vv => vv.Type == "URL").ToList();
        }

        public List<ReportItem> PythonList()
        {
            return List.Where(vv => vv.Type == "PyScript").ToList();
        }

        public List<ReportItem> SqlList()
        {
            return List.Where(vv => vv.Type == "SqlReport").ToList();
        }

        public List<ReportItem> OrgSearchSqlList()
        {
            return List.Where(vv => vv.Type == "OrgSearchSqlReport").ToList();
        }

        public string Table()
        {
            var cs = GetConnectionString();
            var cn = new SqlConnection(cs);
            var p = Parameters();
            var sql = Sql();
            if (sql.Contains("@userid"))
            {
                p.Add("@userid", Util.UserId);
            }

            if (sql.Contains("pagebreak"))
            {
                return PythonModel.PageBreakTables(db, sql, p);
            }

            var rd = cn.ExecuteReader(sql, p);
            return GridResult.Table(rd, Name2, 2000);
        }

        public EpplusResult Result()
        {
            var cs = GetConnectionString();
            var cn = new SqlConnection(cs);
            var p = Parameters();
            var sql = Sql();
            if (sql.Contains("@userid"))
            {
                p.Add("@userid", Util.UserId);
            }

            return cn.ExecuteReader(sql, p).ToExcel(Report + ".xlsx");
        }

        public int? Qtagid { get; set; }
        private DynamicParameters Parameters(string query = null)
        {
            var q = query != null
                ? db.PeopleQuery2(query)
                : db.PeopleQuery(queryid);
            var tag = db.PopulateSpecialTag(q, DbUtil.TagTypeId_Query);
            Qtagid = tag.Id;
            var p = new DynamicParameters();
            p.Add("@qtagid", Qtagid);
            return p;
        }

        public EpplusResult Result(string savedQuery)
        {
            string cs = GetConnectionString();
            var cn = new SqlConnection(cs);
            var p = Parameters(savedQuery);
            var sql = Sql();
            if (sql.Contains("@userid"))
            {
                p.Add("@userid", Util.UserId);
            }

            return cn.ExecuteReader(sql, p).ToExcel(Report + ".xlsx");
        }

        private string GetConnectionString()
        {
            return db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
        }

        public string Sql()
        {
            XDocument xdoc;
            if (Report == "AllColumns")
            {
                xdoc = StandardColumns(includeRoot: true);
            }
            else
            {
                var body = GetCustomReportsContent();
                if (string.IsNullOrEmpty(body))
                {
                    throw new Exception("missing CustomReports");
                }

                xdoc = XDocument.Parse(body);
            }

            if (xdoc.Root == null)
            {
                throw new Exception("missing xml root");
            }

            var r = (from e in xdoc.Root.Elements("Report")
                     where (string)e.Attribute("name") == Report || Report == "AllColumns"
                     select e).SingleOrDefault();
            if (r == null)
            {
                throw new Exception("no report");
            }

            var sb = new StringBuilder($"SELECT\n");

            Dictionary<string, StatusFlagList> flags = null;
            var comma = "";
            var joins = new List<string>();
            foreach (var e in r.Elements("Column"))
            {
                if ((string)e.Attribute("disabled") == "true")
                {
                    continue;
                }

                var name = e.Attribute("name").Value;
                if (name == "StatusFlag")
                {
                    var cc = mc.SpecialColumns[name];
                    if (flags == null)
                    {
                        flags = db.ViewStatusFlagLists.Where(ff => ff.RoleName == null).ToDictionary(ff => ff.Flag, ff => ff);
                    }

                    var flag = (string)e.Attribute("flag");
                    if (!flag.HasValue())
                    {
                        throw new Exception("missing flag on column " + cc.Column);
                    }

                    if (!flags.ContainsKey(flag))
                    {
                        throw new Exception($"missing flag '{flag}' on column {cc.Column}");
                    }

                    var sel = cc.Select.Replace("{flag}", flag);
                    var desc = (string)e.Attribute("description");
                    if (!desc.HasValue())
                    {
                        desc = flags[flag].Name;
                    }

                    sel = sel.Replace("{desc}", desc);
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, sel, DblQuotes(desc));
                }
                else if (name == "SmallGroup")
                {
                    var cc = mc.SpecialColumns[name];
                    var oid = (string)e.Attribute("orgid");
                    if (!oid.HasValue())
                    {
                        throw new Exception("missing orgid on column " + cc.Column);
                    }

                    isorg = true;
                    var sel = cc.Select.Replace("{orgid}", oid);
                    var smallgroup = (string)e.Attribute("smallgroup");
                    if (!smallgroup.HasValue())
                    {
                        continue;
                    }

                    sel = sel.Replace("{smallgroup}", DblQuotes(smallgroup));
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, sel, DblQuotes(smallgroup));
                }
                else if (name.StartsWith("OrgMember"))
                {
                    var cc = mc.SpecialColumns[name];
                    var oid = (string)e.Attribute("orgid");
                    if (!oid.HasValue())
                    {
                        throw new Exception("missing orgid on column " + cc.Column);
                    }

                    isorg = true;
                    if (!joins.Contains(cc.Join))
                    {
                        mc.Joins[cc.Join] = mc.Joins[cc.Join].Replace("{orgid}", oid);
                        joins.Add(cc.Join);
                    }
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, cc.Select, DblQuotes(name));
                }
                else if (name.StartsWith("Amount") && Regex.IsMatch(name, @"\AAmount(Tot|Paid|Due)\z"))
                {
                    var cc = mc.SpecialColumns[name];
                    var oid = (string)e.Attribute("orgid");
                    if (!oid.HasValue())
                    {
                        throw new Exception("missing orgid on column " + cc.Column);
                    }

                    isorg = true;

                    if (!joins.Contains(cc.Join))
                    {
                        mc.Joins[cc.Join] = mc.Joins[cc.Join].Replace("{orgid}", oid);
                        joins.Add(cc.Join);
                    }
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, cc.Select, DblQuotes(name));
                }
                else if (name.StartsWith("ExtraValue") && Regex.IsMatch(name, @"\AExtraValue(Code|Date|Text|Int|Bit)\z"))
                {
                    var cc = mc.SpecialColumns[name];
                    var field = (string)e.Attribute("field");
                    if (!field.HasValue())
                    {
                        throw new Exception("missing field on column " + cc.Column);
                    }

                    var sel = cc.Select.Replace("{field}", DblQuotes(field));
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, sel, DblQuotes(field));
                }
                else
                {
                    if (!mc.Columns.ContainsKey(name))
                    {
                        throw new Exception($"missing column named '{name}'");
                    }

                    var cc = mc.Columns[name];
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, cc.Select, DblQuotes(cc.Column));
                    if (cc.Join.HasValue())
                    {
                        if (!joins.Contains(cc.Join))
                        {
                            joins.Add(cc.Join);
                        }
                    }
                }
                comma = ",";
            }
            sb.AppendLine("FROM dbo.People p");
            var coid = db.CurrentSessionOrgId;
            foreach (var j in joins)
            {
                var join = mc.Joins[j].Trim();
                if (join.Contains("{orgid}"))
                {
                    isorg = true;
                }

                sb.AppendLine(join.Replace("{orgid}", coid.ToString()));
            }
            sb.AppendLine("JOIN dbo.TagPerson tp ON tp.PeopleId = p.PeopleId AND tp.Id = @qtagid");
            return sb.ToString();
        }

        public XDocument StandardColumns(bool includeRoot = true)
        {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                var w = new APIWriter(writer);
                if (includeRoot)
                {
                    w.Start("CustomReports");
                }

                w.Start("Report").Attr("name", "YourReportNameGoesHere");
                if (orgid.HasValue)
                {
                    w.Attr("showOnOrgId", orgid);
                }

                foreach (var c in mc.Columns.Values)
                {
                    w.Start("Column").Attr("name", c.Column).End();
                }

                var protectedevs = from value in Views.GetStandardExtraValues(db, "People")
                                   where value.VisibilityRoles.HasValue()
                                   select value.Name;

                var standards = (from value in Views.GetStandardExtraValues(db, "People")
                                 select value.Name).ToList();

                var extravalues = from ev in db.PeopleExtras
                                  where !protectedevs.Contains(ev.Field)
                                  where (ev.UseAllValues ?? false) == false
                                  group ev by new { ev.Field, ev.Type }
                                  into g
                                  orderby g.Key.Field
                                  select g.Key;

                foreach (var ev in extravalues)
                {
                    if (!Regex.IsMatch(ev.Type, @"Code|Date|Text|Int|Bit"))
                    {
                        continue;
                    }

                    w.Start("Column");
                    w.Attr("field", ev.Field).Attr("name", "ExtraValue" + ev.Type);
                    if (!standards.Contains(ev.Field))
                    {
                        w.Attr("disabled", "true");
                    }

                    w.End();
                }
                var statusflags = from f in db.ViewStatusFlagLists
                                  where f.RoleName == null
                                  orderby f.Name
                                  select f;
                foreach (var f in statusflags)
                {
                    w.Start("Column")
                     .Attr("description", f.Name)
                     .Attr("flag", f.Flag)
                     .Attr("name", "StatusFlag")
                     .End();
                }
                if (orgid.HasValue)
                {
                    var specialcols = from c in mc.SpecialColumns.Values
                                      where c.Context == "org"
                                      where c.Column != "SmallGroup"
                                      select c;
                    foreach (var c in specialcols)
                    {
                        w.Start("Column")
                         .Attr("name", c.Column)
                         .Attr("orgid", orgid)
                         .End();
                    }

                    var smallgroups = from sg in db.MemberTags
                                      where sg.OrgId == orgid
                                      orderby sg.Name
                                      select sg;
                    foreach (var sg in smallgroups)
                    {
                        w.Start("Column")
                         .Attr("smallgroup", sg.Name)
                         .Attr("orgid", orgid)
                         .Attr("name", "SmallGroup")
                         .End();
                    }
                }
                w.End();
                if (includeRoot)
                {
                    w.End();
                }
            }
            return doc;
        }

        public void DeleteReport(string reportName)
        {
            var xdoc = GetCustomReportXml();

            var nodeToDelete = FindReportOnDocument(xdoc, reportName);
            if (nodeToDelete != null)
            {
                nodeToDelete.Remove();
            }

            SetCustomReportsContent(xdoc.ToString());
        }

        public XElement GetReportByName()
        {
            var xdoc = GetCustomReportXml();
            return FindReportOnDocument(xdoc, Report);
        }

        public void SaveReport(string originalReportName, string newReportName, IEnumerable<CustomReportColumn> selectedColumns, bool restrictToThisOrg)
        {
            var xdoc = GetCustomReportXml();

            var newColumns = from column in selectedColumns
                             select new XElement("Column", MapCustomReportToAttributes(column));

            if (restrictToThisOrg && !orgid.HasValue)
            {
                throw new Exception("Missing OrgId");
            }

            if (originalReportName.HasValue())
            {
                var nodeToChange = FindReportOnDocument(xdoc, originalReportName);
                if (nodeToChange != null)
                {
                    nodeToChange.RemoveNodes();
                    nodeToChange.RemoveAttributes();

                    nodeToChange.Add(newColumns);
                    nodeToChange.Add(new XAttribute("name", newReportName));

                    if (restrictToThisOrg)
                    {
                        nodeToChange.Add(new XAttribute("showOnOrgId", orgid.Value));
                    }
                }
            }
            else
            {
                if (FindReportOnDocument(xdoc, newReportName) != null)
                {
                    throw new Exception("Report already exists");
                }

                var reportElement = new XElement("Report", newColumns, new XAttribute("name", newReportName));
                if (restrictToThisOrg)
                {
                    reportElement.Add(new XAttribute("showOnOrgId", orgid.Value));
                }

                xdoc.Root?.Add(reportElement);
            }
            SetCustomReportsContent(xdoc.ToString());
        }

        public string AddReport(string report, string url, string type)
        {
            if (!report.HasValue())
            {
                throw new ArgumentException("missing report name");
            }

            int typeid;
            string[] lookfor;
            switch (type)
            {
                case "PyScript":
                    typeid = ContentTypeCode.TypePythonScript;
                    lookfor = new[] { "BlueToolbarReport", "@BlueToolbarTagId" };
                    break;
                case "SqlReport":
                    typeid = ContentTypeCode.TypeSqlScript;
                    lookfor = new[] { "@qtagid", "@bluetoolbartagid" };
                    break;
                case "OrgSearchSqlReport":
                    typeid = ContentTypeCode.TypeSqlScript;
                    lookfor = new[] { "@OrgIds" };
                    break;
                case "Menu":
                    typeid = ContentTypeCode.TypeSqlScript;
                    lookfor = new[] { "menu" };
                    break;
                default:
                    throw new ArgumentException($"unknown typeid: {type}");
            }
            var content = db.Content(report, typeid);
            if (lookfor.Any(vv => content.Body.Contains(vv, ignoreCase: true)))
            {
                var xdoc = GetCustomReportXml();
                if (FindReportOnDocument(xdoc, report) != null)
                {
                    throw new ArgumentException("report already exists");
                }

                var reportElement = new XElement("Report", new XAttribute("name", report), new XAttribute("type", type));
                xdoc.Root?.Add(reportElement);
                SetCustomReportsContent(xdoc.ToString());
                return "BlueToolbar";
            }
            content = db.Content("CustomReportsMenu", "<ReportsMenu/>", ContentTypeCode.TypeText);
            var xd = XDocument.Parse(content.Body);
            var e = xd.Descendants("Report").SingleOrDefault(r => r.Attribute("link").Value == url);
            if (e != null)
            {
                throw new ArgumentException("report already exists");
            }

            e = new XElement("Report", new XAttribute("link", url), report.SpaceCamelCase());
            var col2 = xd.Descendants("Column2").SingleOrDefault();
            if (col2 == null)
            {
                col2 = new XElement("Column2");
                xd.Root?.Add(col2);
            }
            col2.Add(e);
            content.Body = xd.ToString();
            HttpRuntime.Cache.Remove(db.Host + "CustomReportsMenu");
            db.SubmitChanges();
            return "Reports Menu";
        }
        public bool Contains(string text, List<string> values)
        {
            return values.Any(text.Contains);
        }

        private XDocument GetCustomReportXml()
        {
            var body = GetCustomReportsContent();
            if (string.IsNullOrEmpty(body))
            {
                body = "<CustomReports>\n</CustomReports>";
            }

            return XDocument.Parse(body);
        }

        private static string DblQuotes(string s)
        {
            return s.Replace("'", "''");
        }

        private string GetCustomReportsContent()
        {
            return db.ContentText("CustomReports", "");
        }

        private void SetCustomReportsContent(string customReportsXml)
        {
            var content = db.Content("CustomReports");
            if (content == null)
            {
                return;
            }

            content.Body = customReportsXml;

            db.SubmitChanges();
        }

        private static IEnumerable<XAttribute> MapCustomReportToAttributes(CustomReportColumn column)
        {
            if (column.IsStatusFlag)
            {
                yield return new XAttribute("description", column.Description);
                yield return new XAttribute("flag", column.Flag);
            }

            if (column.IsExtraValue)
            {
                yield return new XAttribute("field", column.Field);
                yield return new XAttribute("disabled", column.IsDisabled);
            }

            if (column.IsSmallGroup)
            {
                yield return new XAttribute("smallgroup", column.SmallGroup);
            }

            yield return new XAttribute("name", column.Name);

            if (!string.IsNullOrEmpty(column.OrgId))
            {
                yield return new XAttribute("orgid", column.OrgId);
            }
        }

        private static XElement FindReportOnDocument(XContainer xdoc, string reportName)
        {
            return xdoc.Descendants("Report").SingleOrDefault(r => r.Attribute("name").Value == reportName);
        }

        public enum SaveReportStatus
        {
            Success,
            ReportAlreadyExists
        }

        public CustomReportViewModel EditCustomReport(CustomReportViewModel originalReportViewModel, bool? alreadySaved)
        {
            var orgName = orgid.HasValue
                ? db.Organizations.SingleOrDefault(o => o.OrganizationId == orgid.Value)?.OrganizationName
                : null;

            if (!Report.HasValue())
            {
                return new CustomReportViewModel(orgid, queryid, orgName, GetAllStandardColumns());
            }

            var vm = new CustomReportViewModel(orgid, queryid, orgName, GetAllStandardColumns(), Report);

            var reportXml = GetReportByName();
            if (reportXml == null)
            {
                throw new Exception("Report not found.");
            }

            var columns = MapXmlToCustomReportColumn(reportXml);

            var showOnOrgIdValue = reportXml.AttributeOrNull("showOnOrgId");
            int showOnOrgId;
            if (!string.IsNullOrEmpty(showOnOrgIdValue) && int.TryParse(showOnOrgIdValue, out showOnOrgId))
            {
                vm.RestrictToThisOrg = showOnOrgId == orgid;
            }

            vm.SetSelectedColumns(columns);
            vm.Columns = vm.Columns.OrderBy(cc => cc.Order).ToList();

            if (originalReportViewModel != null)
            {
                vm.ReportName = originalReportViewModel.ReportName;
            }

            vm.CustomReportSuccessfullySaved = alreadySaved.GetValueOrDefault();
            return vm;
        }

        private List<CustomReportColumn> GetAllStandardColumns()
        {
            var reportXml = StandardColumns();
            return MapXmlToCustomReportColumn(reportXml);
        }

        private static List<CustomReportColumn> MapXmlToCustomReportColumn(XContainer reportXml)
        {
            var q = from column in reportXml.Descendants("Column")
                    select new CustomReportColumn
                    {
                        Name = column.AttributeOrNull("name"),
                        Description = column.AttributeOrNull("description"),
                        Flag = column.AttributeOrNull("flag"),
                        OrgId = column.AttributeOrNull("orgid"),
                        SmallGroup = column.AttributeOrNull("smallgroup"),
                        Field = column.AttributeOrNull("field"),
                        IsDisabled = column.AttributeOrNull("disabled").ToBool(),
                    };
            return q.ToList();
        }
    }
}
