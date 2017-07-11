using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using UtilityExtensions;

namespace CmsData
{
    public static class CreateExcel
    {
        public static byte[] ToExcelBytes(this IDataReader rd, string filename = null, bool useTable = false)
        {
            var dt = new DataTable();
            dt.Load(rd);
            var ep = new ExcelPackage();
            ep.AddSheet(dt, filename, useTable);
            return ep.GetAsByteArray();
        }

        public static DataTable DataReaderToTable(this IDataReader rd)
        {
            var schema = rd.GetSchemaTable();
            if (schema == null)
                throw new Exception("null schema in DataReaderToTable");
            var dt = new DataTable();
            var cols = new List<DataColumn>();
            foreach (DataRow row in schema.Rows)
            {
                var name = row["ColumnName"].ToString();
                if (name.Equal("linkfornext"))
                    continue;
                var col = new DataColumn(name, (Type) row["DataType"]);
                cols.Add(col);
                dt.Columns.Add(col);
            }
            while (rd.Read())
            {
                var row = dt.NewRow();
                foreach (var col in cols)
                    row[col] = rd[col.ColumnName];
                dt.Rows.Add(row);
            }
            return dt;
        }

        public static void AddSheet(this ExcelPackage ep, IDataReader rd, string filename, bool useTable = false)
        {
            var dt = new DataTable();
            dt.Load(rd);
            ep.AddSheet(dt, filename, useTable);
        }

        public static ExcelWorksheet AddSheet(this ExcelPackage ep, DataTable dt, string filename, bool useTable = false)
        {
            var sheetname = Path.GetFileNameWithoutExtension(filename);
            var ws = ep.Workbook.Worksheets.Add(sheetname);
            ws.Cells["A1"].LoadFromDataTable(dt, true);
            var count = dt.Rows.Count;
            using (var header = ws.Cells[1, 1, 1, dt.Columns.Count])
            {
                header.Style.Font.Bold = true;
                header.Style.Fill.PatternType = ExcelFillStyle.Solid;
                header.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(91, 154, 212));
                header.Style.Font.Color.SetColor(Color.White);
            }

            ExcelTable table = null;
            if (useTable)
            {
                var range = ws.Cells[1, 1, count + 1, dt.Columns.Count];
                table = ws.Tables.Add(range, sheetname);
                table.TableStyle = TableStyles.Light9;
                table.ShowFilter = false;
            }

            for (var i = 0; i < dt.Columns.Count; i++)
            {
                var col = i + 1;
                var name = dt.Columns[i].ColumnName;
                var type = dt.Columns[i].DataType;

                if (table != null)
                    table.Columns[i].Name = name;

                var colrange = ws.Cells[1, col, count + 2, col];

                if (name.Contains("Info") || name.Contains("Classes") || name == "Questions" || name == "Address")
                {
                    colrange.Style.WrapText = true;
                    ws.Column(col).Width = 40.0;
                }
                else if (!name.ToLower().EndsWith("id") && type == typeof(int))
                {
                    colrange.Style.Numberformat.Format = "#,##0";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    colrange.AutoFitColumns();
                }
                else if (type == typeof(decimal))
                {
                    colrange.Style.Numberformat.Format = "#,##0.00";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    colrange.AutoFitColumns();
                }
                else if ((type == typeof(float) || type == typeof(double)) && PythonModel.StartsEndsWith("Pct", name))
                {
                    colrange.Style.Numberformat.Format = "#####0.0";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    colrange.AutoFitColumns();
                }
                else if (type == typeof(DateTime))
                {
                    if (name.EndsWith("Time"))
                    {
                        colrange.Style.Numberformat.Format = "m/d/yy h:mm AM/PM";
                        ws.Column(col).Width = 16;
                    }
                    else
                    {
                        colrange.Style.Numberformat.Format = "m/d/yy";
                        ws.Column(col).Width = 12;
                    }
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                }
                else
                    try
                    {
                        colrange.AutoFitColumns();
                    }
                    catch (Exception)
                    {
                        ws.Column(col).Width = 50;
                    }
            }
            return ws;
        }
    }
}