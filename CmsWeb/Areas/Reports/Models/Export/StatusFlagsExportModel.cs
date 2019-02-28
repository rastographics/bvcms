using CmsData;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class StatusFlagsExportModel
    {
        public StatusFlagsExportModel() { }
        public static EpplusResult StatusFlagsList(Guid qid, string flags)
        {
            var collist = from ss in DbUtil.Db.ViewStatusFlagNamesRoles.ToList()
                          where ss.Role == null || HttpContextFactory.Current.User.IsInRole(ss.Role)
                          select ss;

            string cols = null;

            if (flags.HasValue())
            {
                cols = string.Join(",\n", from f in flags.Split(',')
                                          join c in collist on f equals c.Flag
                                          select $"\tss.{c.Flag} as [{c.Flag}_{c.Name}]");
            }
            else
            {
                cols = string.Join(",\n", from c in collist
                                          where c.Role == null || HttpContextFactory.Current.User.IsInRole(c.Role)
                                          select $"\tss.{c.Flag} as [{c.Name}]");
            }

            var tag = DbUtil.Db.PopulateSpecialTag(qid, DbUtil.TagTypeId_Query);
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var noBirthYearRole = HttpContextFactory.Current.User.IsInRole(DbUtil.Db.Setting("NoBirthYearRole", ""));
            var cmd = new SqlCommand($@"
SELECT
    md.PeopleId,
    md.[First],
    md.[Last],
    {(noBirthYearRole ? "" : "md.Age,")}
    md.Marital,
    md.Decision,
    md.DecisionDt,
    md.JoinDt,
    md.Baptism,
    {cols}
FROM StatusFlagColumns ss
JOIN MemberData md ON md.PeopleId = ss.PeopleId
JOIN dbo.TagPerson tp ON tp.PeopleId = md.PeopleId
WHERE tp.Id = @p1", cn);
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            var rd = cmd.ExecuteReader();


            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");

            var dt = new DataTable();
            dt.Load(rd);
            ws.Cells["A1"].LoadFromDataTable(dt, true);
            var count = dt.Rows.Count;
            var range = ws.Cells[1, 1, count + 1, dt.Columns.Count];
            var table = ws.Tables.Add(range, "Members");
            table.TableStyle = TableStyles.Light9;
            table.ShowFilter = false;
            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var col = i + 1;
                var name = dt.Columns[i].ColumnName;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];

                if (name.Contains("Date") || name.EndsWith("Dt"))
                {
                    colrange.Style.Numberformat.Format = "mm-dd-yy";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Column(col).Width = 12;
                }
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "StatusFlags.xlsx");
        }
    }
}
