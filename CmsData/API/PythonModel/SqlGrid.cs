using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.HtmlControls;
using Dapper;
using UtilityExtensions;
using System.Web.UI;

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
        public static HtmlTable HtmlTable(IDataReader rd)
        {
            var pctnames = new List<string> {"pct", "percent"};
            var t = new HtmlTable();
            t.Attributes.Add("class", "table table-striped");
            var h = new HtmlTableRow();
            for (var i = 0; i < rd.FieldCount; i++)
            {
                var typ = rd.GetDataTypeName(i);
                var nam = rd.GetName(i).ToLower();
                string align = null;
                switch (typ.ToLower())
                {
                    case "decimal":
                        align = "right";
                        break;
                    case "int":
                        if (!nam.EndsWith("id") && !nam.EndsWith("id2"))
                            align = "right";
                        break;
                }
                h.Cells.Add(new HtmlTableCell
                {
                    InnerText = rd.GetName(i),
                    Align = align
                });
            }
            t.Rows.Add(h);
            while (rd.Read())
            {
                var r = new HtmlTableRow();
                for (var i = 0; i < rd.FieldCount; i++)
                {
                    var typ = rd.GetDataTypeName(i);
                    var nam = rd.GetName(i).ToLower();
                    string s;
                    string align = null;
                    switch (typ.ToLower())
                    {
                        case "decimal":
                            s = StartsEndsWith(pctnames, nam)
                                ? Convert.ToDecimal(rd[i]).ToString("N1") + "%"
                                : Convert.ToDecimal(rd[i]).ToString("c");
                            align = "right";
                            break;
                        case "float":
                            s = StartsEndsWith(pctnames, nam)
                                ? Convert.ToDouble(rd[i]).ToString("N1") + "%"
                                : Convert.ToDouble(rd[i]).ToString("N1");
                            align = "right";
                            break;
                        case "int":
                            var ii = rd[i].ToInt();
                            if (nam.Equal("peopleid"))
                                s = $"<a href='/Person2/{ii}' target='Person'>{ii}</a>";
                            else if (nam.EndsWith("id") || nam.EndsWith("id2"))
                                s = rd[i].ToInt().ToString();
                            else
                            {
                                s = rd[i].ToInt().ToString("N0");
                                align = "right";
                            }
                            break;
                        default:
                            s = rd[i].ToString();
                            break;
                    }
                    r.Cells.Add(new HtmlTableCell()
                    {
                        InnerHtml = s,
                        Align = align
                    });
                }
                t.Rows.Add(r);
            }
            var tc = new HtmlTableCell
            {
                ColSpan = rd.FieldCount,
                InnerText = $"Count = {t.Rows.Count - 1} rows"
            };
            var tr = new HtmlTableRow();
            tr.Cells.Add(tc);
            t.Rows.Add(tr);
            return t;
        }
        public static bool StartsEndsWith(List<string> list, string name)
        {
            return list.Any(vv => name.StartsWith(vv) || name.EndsWith(vv));
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