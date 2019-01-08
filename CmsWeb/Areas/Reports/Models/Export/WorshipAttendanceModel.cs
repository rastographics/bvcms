using CmsData;
using Dapper;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class WorshipAttendanceModel
    {
        public WorshipAttendanceModel() { }
        
        public class WorshipAttendInfo
        {
            public int PeopleId { get; set; }
            public string Name { get; set; }
            public int? Age { get; set; }
            public double? Wor04 { get; set; }
            public double? MF04 { get; set; }
            public double? Wor12 { get; set; }
            public double? MF12 { get; set; }
            public double? Wor26 { get; set; }
            public double? MF26 { get; set; }
            public double? Wor52 { get; set; }
            public double? MF52 { get; set; }
            public string WorshipAttStr { get; set; }
        }

        public static EpplusResult Attendance(Guid queryid)
        {
            var t = DbUtil.Db.PopulateSpecialTag(queryid, DbUtil.TagTypeId_Query);
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query<WorshipAttendInfo>("WorshipAttendance", new { tagid = t.Id }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            var cols = typeof(WorshipAttendInfo).GetProperties();
            var count = q.Count();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A2"].LoadFromCollection(q);
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Attends");
            table.TableStyle = TableStyles.Light9;
            table.ShowFilter = false;
            for (var i = 0; i < cols.Length; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];
                if (name.EndsWith("04") || name.EndsWith("12") || name.EndsWith("26") || name.EndsWith("52"))
                {
                    colrange.Style.Numberformat.Format = "0.0";
                }

                if (name.StartsWith("Worship"))
                {
                    var cr = ws.Cells[2, col, count + 2, col];
                    cr.Style.Font.SetFromFont(new Font("Courier New", 10f));
                }
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "WorshipAttendance.xlsx");
        }
    }
}
