using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CmsData;
using CmsData.API;
using CmsData.ExtraValue;
using CmsData.View;
using CmsWeb.Areas.Reports.ViewModels;
using CmsWeb.Models;
using Dapper;
using UtilityExtensions;

namespace CmsWeb.Areas.Reports.Models
{
    public class CustomReportsModel
    {
        private readonly CMSDataContext _db;
        private readonly CustomColumnsModel mc;
        private readonly int? orgid;

        public CustomReportsModel(CMSDataContext db)
        {
            _db = db;
            mc = new CustomColumnsModel();
        }

        public CustomReportsModel(CMSDataContext db, int? orgid)
            : this(db)
        {
            this.orgid = orgid;
        }

        public class ReportItem
        {
            public ReportItem(string report, string type = "Custom")
            {
                this.report = report;
                this.type = type;
            }
            public string report { get; set; }
            public string type { get; set; }
            public override string ToString()
            {
                return report;
            }
        }
        public IEnumerable<ReportItem> ReportList()
        {
            var list = new List<ReportItem>();
            var body = GetCustomReportsContent();
            if (body.HasValue())
            {
                var xdoc = XDocument.Parse(body);
                if (xdoc.Root == null)
                    return list;
                var roles = DbUtil.Db.CurrentUser.Roles;
                var q = from e in xdoc.Root.Elements("Report")
                        let r = (string)e.Attribute("name")
                        let role = (string)e.Attribute("role")
                        let oid = ((string)e.Attribute("showOnOrgId")).ToInt()
                        let typ = ((string)e.Attribute("type") ?? "Custom")
                        where oid == 0 || oid == orgid
                        where role == null || roles.Contains(role)
                        where r != null
                        where r != "AllColumns"
                        select new ReportItem(r, typ);

                foreach (var r in q.Where(r => !list.Contains(r)))
                    list.Add(r);
            }
            list.Add(new ReportItem("AllColumns"));
            return list;
        }

        public EpplusResult Result(Guid id, string report)
        {
            var cs = _db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            var sql = Sql(id, report);
            return cn.ExecuteReader(sql).ToExcel(report + ".xlsx");
        }
        public EpplusResult Result(string SavedQuery, string report)
        {
            var cs = _db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            var q = _db.PeopleQuery2(SavedQuery);
            var sql = Sql(q, report);
            return cn.ExecuteReader(sql).ToExcel(report + ".xlsx");
        }

        public string Sql(Guid id, string report)
        {
            var q = _db.PeopleQuery(id);
            return Sql(q, report);
        }
        public string Sql(IQueryable<Person> q, string report)
        {
            XDocument xdoc;
            if (report == "AllColumns")
            {
                xdoc = StandardColumns(includeRoot: true);
            }
            else
            {
                var body = GetCustomReportsContent();
                if (string.IsNullOrEmpty(body))
                    throw new Exception("missing CustomReports");
                xdoc = XDocument.Parse(body);
            }

            if (xdoc.Root == null)
                throw new Exception("missing xml root");

            var r = (from e in xdoc.Root.Elements("Report")
                     where (string)e.Attribute("name") == report || report == "AllColumns"
                     select e).SingleOrDefault();
            if (r == null)
                throw new Exception("no report");

            var tag = _db.PopulateSpecialTag(q, DbUtil.TagTypeId_Query);
            var sb = new StringBuilder($"DECLARE @tagId INT = {tag.Id}\nSELECT\n");

            Dictionary<string, StatusFlagList> flags = null;
            var comma = "";
            var joins = new List<string>();
            foreach (var e in r.Elements("Column"))
            {
                if ((string)e.Attribute("disabled") == "true")
                    continue;
                var name = e.Attribute("name").Value;
                if (name == "StatusFlag")
                {
                    var cc = mc.SpecialColumns[name];
                    if (flags == null)
                        flags = _db.ViewStatusFlagLists.Where(ff => ff.RoleName == null).ToDictionary(ff => ff.Flag, ff => ff);
                    var flag = (string)e.Attribute("flag");
                    if (!flag.HasValue())
                        throw new Exception("missing flag on column " + cc.Column);
                    if (!flags.ContainsKey(flag))
                        throw new Exception($"missing flag '{flag}' on column {cc.Column}");
                    var sel = cc.Select.Replace("{flag}", flag);
                    var desc = (string)e.Attribute("description");
                    if (!desc.HasValue())
                        desc = flags[flag].Name;
                    sel = sel.Replace("{desc}", desc);
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, sel, DblQuotes(desc));
                }
                else if (name == "SmallGroup")
                {
                    var cc = mc.SpecialColumns[name];
                    var oid = (string)e.Attribute("orgid");
                    if (!oid.HasValue())
                        throw new Exception("missing orgid on column " + cc.Column);
                    var sel = cc.Select.Replace("{orgid}", oid);
                    var smallgroup = (string)e.Attribute("smallgroup");
                    if (!smallgroup.HasValue())
                        continue;
                    sel = sel.Replace("{smallgroup}", DblQuotes(smallgroup));
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, sel, DblQuotes(smallgroup));
                }
                else if (name.StartsWith("OrgMember"))
                {
                    var cc = mc.SpecialColumns[name];
                    var oid = (string)e.Attribute("orgid");
                    if (!oid.HasValue())
                        throw new Exception("missing orgid on column " + cc.Column);

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
                        throw new Exception("missing orgid on column " + cc.Column);

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
                        throw new Exception("missing field on column " + cc.Column);
                    var sel = cc.Select.Replace("{field}", DblQuotes(field));
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, sel, DblQuotes(field));
                }
                else
                {
                    if (!mc.Columns.ContainsKey(name))
                        throw new Exception($"missing column named '{name}'");
                    var cc = mc.Columns[name];
                    sb.AppendFormat("\t{0}{1} AS [{2}]\n", comma, cc.Select, DblQuotes(cc.Column));
                    if (cc.Join.HasValue())
                        if (!joins.Contains(cc.Join))
                            joins.Add(cc.Join);
                }
                comma = ",";
            }
            sb.AppendLine("FROM dbo.People p");
            var coid = DbUtil.Db.CurrentOrgId0;
            foreach (var j in joins)
            {
                sb.AppendLine(mc.Joins[j].Replace("{orgid}", coid.ToString()));
            }
            sb.AppendLine("JOIN dbo.TagPerson tp ON tp.PeopleId = p.PeopleId");
            sb.AppendLine("WHERE tp.Id = @tagId\n");
            return sb.ToString();
        }

        public XDocument StandardColumns(bool includeRoot = true)
        {
            var doc = new XDocument();
            using (var writer = doc.CreateWriter())
            {
                var w = new APIWriter(writer);
                if (includeRoot)
                    w.Start("CustomReports");
                w.Start("Report").Attr("name", "YourReportNameGoesHere");
                if (orgid.HasValue)
                    w.Attr("showOnOrgId", orgid);
                foreach (var c in mc.Columns.Values)
                    w.Start("Column").Attr("name", c.Column).End();

                var protectedevs = from value in Views.GetStandardExtraValues(_db, "People")
                                   where value.VisibilityRoles.HasValue()
                                   select value.Name;

                var standards = (from value in Views.GetStandardExtraValues(_db, "People")
                                 select value.Name).ToList();

                var extravalues = from ev in _db.PeopleExtras
                                  where !protectedevs.Contains(ev.Field)
                                  where ev.UseAllValues != true
                                  group ev by new { ev.Field, ev.Type } into g
                                  orderby g.Key.Field
                                  select g.Key;

                foreach (var ev in extravalues)
                {
                    if (!Regex.IsMatch(ev.Type, @"Code|Date|Text|Int|Bit"))
                        continue;
                    w.Start("Column");
                    w.Attr("field", ev.Field).Attr("name", "ExtraValue" + ev.Type);
                    if (!standards.Contains(ev.Field))
                        w.Attr("disabled", "true");
                    w.End();
                }
                var statusflags = from f in _db.ViewStatusFlagLists
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
                        w.Start("Column")
                            .Attr("name", c.Column)
                            .Attr("orgid", orgid)
                            .End();

                    var smallgroups = from sg in _db.MemberTags
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
                    w.End();
            }
            return doc;
        }

        public void DeleteReport(string reportName)
        {
            var xdoc = GetCustomReportXml();

            var nodeToDelete = FindReportOnDocument(xdoc, reportName);
            if (nodeToDelete != null)
                nodeToDelete.Remove();

            SetCustomReportsContent(xdoc.ToString());
        }

        public XElement GetReportByName(string reportName)
        {
            var xdoc = GetCustomReportXml();
            return FindReportOnDocument(xdoc, reportName);
        }

        public SaveReportStatus SaveReport(string originalReportName, string newReportName, IEnumerable<CustomReportColumn> selectedColumns, bool restrictToThisOrg)
        {
            var xdoc = GetCustomReportXml();

            var newColumns = from column in selectedColumns
                             select new XElement("Column", MapCustomReportToAttributes(column));

            if (!string.IsNullOrEmpty(originalReportName))
            {
                var nodeToChange = FindReportOnDocument(xdoc, originalReportName);
                if (nodeToChange != null)
                {
                    nodeToChange.RemoveNodes();
                    nodeToChange.RemoveAttributes();

                    nodeToChange.Add(newColumns);
                    nodeToChange.Add(new XAttribute("name", newReportName));

                    if (restrictToThisOrg)
                        nodeToChange.Add(new XAttribute("showOnOrgId", orgid.Value));
                }
            }
            else
            {
                if (FindReportOnDocument(xdoc, newReportName) != null)
                    return SaveReportStatus.ReportAlreadyExists;

                var reportElement = new XElement("Report", newColumns, new XAttribute("name", newReportName));
                if (restrictToThisOrg)
                    reportElement.Add(new XAttribute("showOnOrgId", orgid.Value));
                xdoc.Root.Add(reportElement);
            }

            SetCustomReportsContent(xdoc.ToString());

            return SaveReportStatus.Success;
        }

        private XDocument GetCustomReportXml()
        {
            var body = GetCustomReportsContent();
            if (string.IsNullOrEmpty(body))
                body = "<CustomReports>\n</CustomReports>";

            return XDocument.Parse(body);
        }

        private static string DblQuotes(string s)
        {
            return s.Replace("'", "''");
        }

        private string GetCustomReportsContent()
        {
            return _db.ContentText("CustomReports", "");
        }

        private void SetCustomReportsContent(string customReportsXml)
        {
            var content = _db.Content("CustomReports");
            if (content == null)
                return;

            content.Body = customReportsXml;

            _db.SubmitChanges();
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
                yield return new XAttribute("orgid", column.OrgId);
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
    }
}
