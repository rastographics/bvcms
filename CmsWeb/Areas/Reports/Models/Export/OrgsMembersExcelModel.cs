using System.Reflection;
using CmsData.View;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using System.Linq;
using OfficeOpenXml.Style;

namespace CmsWeb.Models
{
    public class OrgsMembersExcelModel
    {
        public static EpplusResult Export(OrgSearchModel m)
        {
            var q = m.OrgsMemberList();

            var cols = typeof(CurrOrgMember).GetProperties();
            var count = q.Count();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A2"].LoadFromCollection(q);
            return FormatResult(ws, count, cols, ep);
        }
        public static EpplusResult Export(int id)
        {
            var q = ExportInvolvements.OrgMemberList(id);

            var cols = typeof(CurrOrgMember).GetProperties();
            var count = q.Count();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A2"].LoadFromCollection(q);
            return FormatResult(ws, count, cols, ep);
        }

        private static EpplusResult FormatResult(ExcelWorksheet ws, int count, PropertyInfo[] cols, ExcelPackage ep)
        {
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Members");
            table.ShowFilter = false;
            int userdatacol = 1;
            int groupcol = 1;
            for (var i = 0; i < cols.Length; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];

                if (name.Contains("Date"))
                {
                    colrange.Style.Numberformat.Format = "mm-dd-yy";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    ws.Column(col).Width = 12;
                }
                if (name == "UserData")
                {
                    colrange.Style.WrapText = true;
                    userdatacol = col;
                }
                if (name == "Groups")
                {
                    colrange.Style.WrapText = true;
                    groupcol = col;
                }
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            ws.Column(userdatacol).Width = 40.0;
            ws.Column(groupcol).Width = 60.0;
            return new EpplusResult(ep, "OrgMember.xlsx");
        }
    }
}