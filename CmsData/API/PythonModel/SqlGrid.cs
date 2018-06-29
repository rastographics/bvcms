using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using UtilityExtensions;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Scripting.Utils;

namespace CmsData
{
    public partial class PythonModel
    {
        public string SqlGrid(string sql)
        {
            var p = new DynamicParameters();
            int? tagid = null;

            if (sql.Contains("@BlueToolbarTagId"))
                if (dictionary.ContainsKey("BlueToolbarGuid"))
                {
                    var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
                    if (!guid.HasValue)
                        throw new Exception("missing BlueToolbar Information");
                    var j = db.PeopleQuery(guid.Value).Select(vv => vv.PeopleId).Take(1000);
                    var tag = db.PopulateTemporaryTag(j);
                    tagid = tag.Id;
                    p.Add("@BlueToolbarTagId", tag.Id);
                }
            var cs = db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                using (var rd = db.Connection.ExecuteReader(sql, p, commandTimeout: 300))
                {
                    var table = Table(rd);
                    return $@"
<!-- TagId={tagid} -->
<div class=""report box box-responsive"">
  <div class=""box-content"">
    <div class=""table-responsive"">
      {table}
      <strong>Total Count</strong> <span class=""badge""></span>
    </div>
  </div>
</div>
";
                }
            }
        }

        public static string PageBreakTables(CMSDataContext db, string sql, DynamicParameters p)
        {
            var cs = db.CurrentUser.InRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            var cn = new SqlConnection(cs);
            cn.Open();
            var sb = new StringBuilder();
            int pagebreakcol = 0;
            var pg = 1;
            while (true)
            {
                var s = sql.Replace("WHERE 1=1", $"WHERE pagebreak={pg}");
                var cmd = new SqlCommand(s, cn);
                foreach (var parm in p.ParameterNames)
                {
                    var value = p.Get<dynamic>(parm);
                    cmd.Parameters.AddWithValue(parm, value);
                }
                cmd.CommandTimeout = 1200;
                using (var rd = cmd.ExecuteReader())
                {
                    if (!rd.HasRows)
                        return sb.ToString();
                    if (pg == 1)
                    {
                        var colnames = Enumerable.Range(0, rd.FieldCount).Select(rd.GetName).ToList();
                        pagebreakcol = colnames.FindIndex(vv => vv == "pagebreak");
                    }
                    var t = HtmlTable(rd, $"pagebreak={pagebreakcol}");
                    t.RenderControl(new HtmlTextWriter(new StringWriter(sb)));
                    sb.AppendLine("<div class='page-break'></div>");
                }
                pg++;
            }
        }

        public static Table HtmlTable(IDataReader rd, string title = null, int? maxrows = null)
        {
            var pctnames = new List<string> {"pct", "percent"};
            var t = new Table();
            t.Attributes.Add("class", "table table-striped");
            if (title.HasValue())
            {
                var c = new TableHeaderCell
                {
                    ColumnSpan = rd.FieldCount,
                    Text = title,
                };
                c.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                var r = new TableHeaderRow {TableSection = TableRowSection.TableHeader};
                r.Cells.Add(c);
                t.Rows.Add(r);
            }
            var h = new TableHeaderRow {TableSection = TableRowSection.TableHeader};
            for (var i = 0; i < rd.FieldCount; i++)
            {
                var typ = rd.GetDataTypeName(i);
                var nam = rd.GetName(i).ToLower();
                if (nam == "pagebreak")
                    break;
                var align = HorizontalAlign.NotSet;
                switch (typ.ToLower())
                {
                    case "decimal":
                        align = HorizontalAlign.Right;
                        break;
                    case "int":
                        if (nam != "year" && !nam.EndsWith("id") && !nam.EndsWith("id2"))
                            align = HorizontalAlign.Right;
                        break;
                }
                if (!nam.Equal("linkfornext"))
                    h.Cells.Add(new TableHeaderCell
                    {
                        Text = rd.GetName(i),
                        HorizontalAlign = align
                    });
            }
            t.Rows.Add(h);
            int? pbcol = null;
            var rn = 0;
            string linkfornext = null;
            while (rd.Read())
            {
                if (!pbcol.HasValue && title?.StartsWith("pagebreak=") == true)
                {
                    var match = Regex.Match(title, @"pagebreak=(\d*)");
                    pbcol = match.Groups[1].Value.ToInt();
                    t.Rows[0].Cells[0].Text = rd.GetString(pbcol.Value + 1);
                }
                rn++;
                if (maxrows.HasValue && rn > maxrows)
                    break;
                var r = new TableRow();
                for (var i = 0; i < rd.FieldCount; i++)
                {
                    if (i == pbcol)
                        break;
                    var typ = rd.GetDataTypeName(i);
                    var nam = rd.GetName(i).ToLower();
                    if (nam == "linkfornext")
                    {
                        var x = rd.GetValue(i);
                        if (x != DBNull.Value)
                            linkfornext = rd.GetString(i);
                        continue;
                    }
                    string s;
                    var align = HorizontalAlign.NotSet;

                    switch (typ.ToLower())
                    {
                        case "decimal":
                            var dec = rd[i].ToNullableDecimal();
                            s = StartsEndsWith(pctnames, nam)
                                ? dec.ToString2("N1", "%")
                                : dec.ToString2("c");
                            align = HorizontalAlign.Right;
                            break;
                        case "real":
                        case "float":
                            var dbl = rd[i].ToNullableDouble();
                            s = StartsEndsWith(pctnames, nam)
                                ? dbl.ToString2("N1", "%")
                                : dbl.ToString2("N1");
                            align = HorizontalAlign.Right;
                            break;
                        case "date":
                        case "datetime":
                            var dt = rd[i].ToNullableDate();
                            if (dt.HasValue && dt.Value.Date != dt.Value)
                                s = dt.FormatDateTime();
                            else
                                s = dt.FormatDate();
                            break;
                        case "int":
                            var ii = rd[i].ToNullableInt();
                            if (nam.Equal("peopleid") || nam.Equal("spouseid"))
                                s = $"<a href='/Person2/{ii}' target='Person'>{ii}</a>";
                            else if (nam.Equal("organizationid") || nam.EndsWith("orgid"))
                                s = $"<a href='/Org/{ii}' target='Organization'>{ii}</a>";
                            else if (nam.EndsWith("id") || nam.EndsWith("id2") || nam.Equal("Year"))
                                s = ii.ToString();
                            else
                            {
                                s = ii.ToString2("N0");
                                align = HorizontalAlign.Right;
                            }
                            break;
                        default:
                            s = rd[i].ToString();
                            if (s == "Total" || s.StartsWith("Total:") || s == "Grand Total")
                                s = $"<strong>{s}</strong>";
                            if (nam.StartsWith("att") && nam.EndsWith("str"))
                            {
                                s = $"<span style='font-family: monospace'>{s}</span>";
                                align = HorizontalAlign.Right;
                            }
                            break;
                    }
                    if (linkfornext.HasValue())
                    {
                        s = $"<a href='{linkfornext}' target='link'>{s}</a>";
                        linkfornext = null;
                    }
                    r.Cells.Add(new TableCell()
                    {
                        Text = s,
                        HorizontalAlign = align,
                    });
                }
                t.Rows.Add(r);
            }
            var rowcount = $"Count = {rn} rows";
            if (maxrows.HasValue && rn > maxrows)
                rowcount = $"Displaying {maxrows} of {rn} rows";
            var tc = new TableCell()
            {
                ColumnSpan = rd.FieldCount,
                Text = rowcount,
            };
            var tr = new TableFooterRow();
            tr.Cells.Add(tc);
            t.Rows.Add(tr);
            return t;
        }

        public static bool StartsEndsWith(List<string> list, string name)
        {
            return list.Any(vv =>
                name.StartsWith(vv, StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(vv, StringComparison.OrdinalIgnoreCase));
        }

        public static bool StartsEndsWith(string pattern, string name)
        {
            return name.StartsWith(pattern, StringComparison.OrdinalIgnoreCase)
                   || name.EndsWith(pattern, StringComparison.OrdinalIgnoreCase);
        }

        public static string Table(IDataReader rd)
        {
            var t = HtmlTable(rd);
            var sb = new StringBuilder();
            t.RenderControl(new HtmlTextWriter(new StringWriter(sb)));
            return sb.ToString();
        }
    }
}
