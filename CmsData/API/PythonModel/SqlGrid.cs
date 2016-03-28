using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using Dapper;
using UtilityExtensions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CmsData
{
    public partial class PythonModel
    {
        public string SqlGrid(string sql)
        {
            var p = new DynamicParameters();

            if (sql.Contains("@BlueToolbarTagId"))
                if (dictionary.ContainsKey("BlueToolbarGuid"))
                {
                    var guid = (dictionary["BlueToolbarGuid"] as string).ToGuid();
                    if (!guid.HasValue)
                        throw new Exception("missing BlueToolbar Information");
                    var j = DbUtil.Db.PeopleQuery(guid.Value).Select(vv => vv.PeopleId).Take(1000);
                    var tag = DbUtil.Db.PopulateTemporaryTag(j);
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
                var align = HorizontalAlign.NotSet;
                switch (typ.ToLower())
                {
                    case "decimal":
                        align = HorizontalAlign.Right;
                        break;
                    case "int":
                        if(nam != "Year" && !nam.EndsWith("id") && !nam.EndsWith("id2"))
                            align = HorizontalAlign.Right;
                        break;
                }
                h.Cells.Add(new TableHeaderCell
                {
                    Text = rd.GetName(i),
                    HorizontalAlign = align
                });
            }
            t.Rows.Add(h);
            var rn = 0;
            while (rd.Read())
            {
                rn++;
                if (maxrows.HasValue && rn > maxrows)
                    continue;
                var r = new TableRow();
                for (var i = 0; i < rd.FieldCount; i++)
                {
                    var typ = rd.GetDataTypeName(i);
                    var nam = rd.GetName(i).ToLower();
                    string s;
                    var align = HorizontalAlign.NotSet;

                    switch (typ.ToLower())
                    {
                        case "decimal":
                            s = StartsEndsWith(pctnames, nam)
                                ? Convert.ToDecimal(rd[i]).ToString("N1") + "%"
                                : Convert.ToDecimal(rd[i]).ToString("c");
                            align = HorizontalAlign.Right;
                            break;
                        case "real":
                        case "float":
                            s = StartsEndsWith(pctnames, nam)
                                ? Convert.ToDouble(rd[i]).ToString("N1") + "%"
                                : Convert.ToDouble(rd[i]).ToString("N1");
                            align = HorizontalAlign.Right;
                            break;
                        case "date":
                        case "datetime":
                            var dt = rd[i].ToDate();
                            if (dt.HasValue && dt.Value.Date != dt.Value)
                                s = dt.FormatDateTime();
                            else
                                s = dt.FormatDate();
                            break;
                        case "int":
                            var ii = rd[i].ToInt();
                            if (nam.Equal("peopleid") || nam.Equal("spouseid"))
                                s = $"<a href='/Person2/{ii}' target='Person'>{ii}</a>";
                            else if (nam.Equal("organizationid"))
                                s = $"<a href='/Org/{ii}' target='Organization'>{ii}</a>";
                            else if (nam.EndsWith("id") || nam.EndsWith("id2") || nam.Equal("Year"))
                                s = rd[i].ToInt().ToString();
                            else
                            {
                                s = rd[i].ToInt().ToString("N0");
                                align = HorizontalAlign.Right;
                            }
                            break;
                        default:
                            s = rd[i].ToString();
                            if (s == "Total")
                                s = $"<strong>{s}</strong>";
                            break;
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
                name.StartsWith(vv,StringComparison.OrdinalIgnoreCase) 
                || name.EndsWith(vv,StringComparison.OrdinalIgnoreCase));
        }
        public static bool StartsEndsWith(string pattern, string name)
        {
            return name.StartsWith(pattern,StringComparison.OrdinalIgnoreCase) 
                || name.EndsWith(pattern,StringComparison.OrdinalIgnoreCase);
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