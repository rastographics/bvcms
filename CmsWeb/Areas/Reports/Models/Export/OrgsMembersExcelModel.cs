using System.Reflection;
using CmsData.View;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using System.Linq;
using CmsData;
using OfficeOpenXml.Style;
using TableStyles = OfficeOpenXml.Table.TableStyles;

namespace CmsWeb.Models
{
    public class OrgsMembersExcelModel
    {
        public static EpplusResult Export()
        {
            var co = DbUtil.Db.CurrentOrg;
            var filter = DbUtil.Db.OrgPeople(co.Id, co.First(), co.Last(), co.SgFilter, co.FilterIndividuals,
                co.FilterTag).Select(pp => pp.PeopleId).ToList();
            var list = DbUtil.Db.CurrOrgMembers2(co.Id.ToString(), string.Join(",", filter));

            var count = list.Count();
            if(count == 0)
                return new EpplusResult("EmptyResult.xlsx");

            var cols = typeof(CurrOrgMember).GetProperties();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            ws.Cells["A2"].LoadFromCollection(list);
            return FormatResult(ws, count, cols, ep);
        }

        private static EpplusResult FormatResult(ExcelWorksheet ws, int count, PropertyInfo[] cols, ExcelPackage ep)
        {
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Members");
            table.ShowFilter = false;
            table.TableStyle = TableStyles.Light9;
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