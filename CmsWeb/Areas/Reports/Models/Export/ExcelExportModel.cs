using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using CmsData;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using UtilityExtensions;
using Color = System.Drawing.Color;
using TableStyles = OfficeOpenXml.Table.TableStyles;

namespace CmsWeb.Models
{
    public static class ExcelExportModel
    {
        public static EpplusResult ToExcel(this IDataReader rd, string filename = null, bool useTable = false)
        {
            var dt = new DataTable();
            dt.Load(rd);
            return dt.ToExcel(filename, useTable);
        }

        public static EpplusResult ToExcel(this DataTable dt, string filename = "People.xlsx", bool useTable = false)
        {
            var ep = new ExcelPackage();
            ep.AddSheet(dt, filename, useTable);
            return new EpplusResult(ep, filename);
        }

        public static void AddSheet(this ExcelPackage ep, IDataReader rd, string filename = "People.xlsx", bool useTable = false)
        {
            var dt = new DataTable();
            dt.Load(rd);
            ep.AddSheet(dt, filename, useTable);
        }
        public static void AddSheet(this ExcelPackage ep, DataTable dt, string filename = "People.xlsx", bool useTable = false)
        {
            var sheetname = System.IO.Path.GetFileNameWithoutExtension(filename);
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

                if(table != null)
                    table.Columns[i].Name = name;

                var colrange = ws.Cells[1, col, count + 2, col];

                if (!name.ToLower().EndsWith("id") && type == typeof (int))
                {
                    colrange.Style.Numberformat.Format = "#,##0";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    colrange.AutoFitColumns();
                }
                else if (type == typeof (decimal))
                {
                    colrange.Style.Numberformat.Format = "#,##0";
                    colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    colrange.AutoFitColumns();
                }
                else if (type == typeof (DateTime))
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
                    colrange.AutoFitColumns();
            }
        }

        public static List<ExcelPic> List(Guid queryid)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    let spouse = Db.People.Where(pp => pp.PeopleId == p.SpouseId).Select(pp => pp.PreferredName).SingleOrDefault()
                    let familyname = p.Family.People
                                .Where(pp => pp.PeopleId == pp.Family.HeadOfHouseholdId)
                                .Select(pp => pp.LastName).SingleOrDefault()
                    orderby familyname, p.FamilyId, p.Name2
                    select new ExcelPic
                    {
                        PeopleId = p.PeopleId,
                        Title = p.TitleCode,
                        FirstName = p.PreferredName,
                        LastName = p.LastName,
                        Address = p.PrimaryAddress,
                        Address2 = p.PrimaryAddress2,
                        City = p.PrimaryCity,
                        State = p.PrimaryState,
                        Zip = p.PrimaryZip.FmtZip(),
                        Email = p.EmailAddress,
                        BirthDate = p.BirthMonth + "/" + p.BirthDay + "/" + p.BirthYear,
                        BirthDay = " " + p.BirthMonth + "/" + p.BirthDay,
                        Anniversary = " " + p.WeddingDate.Value.Month + "/" + p.WeddingDate.Value.Day,
                        JoinDate = p.JoinDate.FormatDate(),
                        JoinType = p.JoinType.Description,
                        HomePhone = p.HomePhone.FmtFone(),
                        CellPhone = p.CellPhone.FmtFone(),
                        WorkPhone = p.WorkPhone.FmtFone(),
                        MemberStatus = p.MemberStatus.Description,
                        FellowshipLeader = p.BFClass.LeaderName,
                        Spouse = spouse, 
                        Children = p.PositionInFamilyId != 10 ? "" 
                            : string.Join(", ", p.Family.People
                                .Where(cc => cc.PositionInFamilyId == 30)
                                .Where(cc => cc.Age <= 18)
                                .Select(cc => 
                                    cc.LastName == familyname 
                                        ? cc.PreferredName
                                        : cc.Name
                                    )),
                        Age = p.Age.ToString(),
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                        Married = p.MaritalStatus.Description,
                        FamilyId = p.FamilyId,
                        ImageId = p.Picture.LargeId,
                    };
            return q.Take(10000).ToList();
        }
        public static EpplusResult Result(Guid id)
        {
            var q = List(id);
            var excelpackage = new ExcelPackage();
            var ws = excelpackage.Workbook.Worksheets.Add("Sheet1");
            var r = 1;
            var c = 1;
            ws.Cells[r, c++].Value = "PeopleId";
            ws.Cells[r, c++].Value = "Title";
            ws.Cells[r, c++].Value = "FirstName";
            ws.Cells[r, c++].Value = "LastName";
            ws.Cells[r, c++].Value = "Address";
            ws.Cells[r, c++].Value = "Address2";
            ws.Cells[r, c++].Value = "City";
            ws.Cells[r, c++].Value = "State";
            ws.Cells[r, c++].Value = "Zip";
            ws.Cells[r, c++].Value = "Email";
            ws.Cells[r, c++].Value = "BirthDate";
            ws.Cells[r, c++].Value = "BirthDay";
            ws.Cells[r, c++].Value = "Anniversary";
            ws.Cells[r, c++].Value = "JoinDate";
            ws.Cells[r, c++].Value = "JoinType";
            ws.Cells[r, c++].Value = "HomePhone";
            ws.Cells[r, c++].Value = "CellPhone";
            ws.Cells[r, c++].Value = "WorkPhone";
            ws.Cells[r, c++].Value = "MemberStatus";
            ws.Cells[r, c++].Value = "FellowshipLeader";
            ws.Cells[r, c++].Value = "Spouse";
            ws.Cells[r, c++].Value = "Age";
            ws.Cells[r, c++].Value = "Grade";
            ws.Cells[r, c++].Value = "AttendPctBF";
            ws.Cells[r, c++].Value = "Married";
            ws.Cells[r, c++].Value = "FamilyId";
            ws.Cells[r, c].Value = "ImageUrl";
            r++;
            foreach (var ep in q)
            {
                c = 1;
                ws.Cells[r, c++].Value = ep.PeopleId;
                ws.Cells[r, c++].Value = ep.Title;
                ws.Cells[r, c++].Value = ep.FirstName;
                ws.Cells[r, c++].Value = ep.LastName;
                ws.Cells[r, c++].Value = ep.Address;
                ws.Cells[r, c++].Value = ep.Address2;
                ws.Cells[r, c++].Value = ep.City;
                ws.Cells[r, c++].Value = ep.State;
                ws.Cells[r, c++].Value = ep.Zip;
                ws.Cells[r, c++].Value = ep.Email;
                ws.Cells[r, c++].Value = ep.BirthDate;
                ws.Cells[r, c++].Value = ep.BirthDay;
                ws.Cells[r, c++].Value = ep.Anniversary;
                ws.Cells[r, c++].Value = ep.JoinDate;
                ws.Cells[r, c++].Value = ep.JoinType;
                ws.Cells[r, c++].Value = ep.HomePhone;
                ws.Cells[r, c++].Value = ep.CellPhone;
                ws.Cells[r, c++].Value = ep.WorkPhone;
                ws.Cells[r, c++].Value = ep.MemberStatus;
                ws.Cells[r, c++].Value = ep.FellowshipLeader;
                ws.Cells[r, c++].Value = ep.Spouse;
                ws.Cells[r, c++].Value = ep.Age;
                ws.Cells[r, c++].Value = ep.Grade;
                ws.Cells[r, c++].Value = ep.AttendPctBF;
                ws.Cells[r, c++].Value = ep.Married;
                ws.Cells[r, c++].Value = ep.FamilyId;
                ws.Cells[r, c].Value = ep.ImageUrl();

//                var i = ep.GetImage();
//                if (i != null)
//                {
//                    var xy = ImgRowHeight(ws, i);
//                    maxwid = Math.Max(xy.X, maxwid);
//                    ws.Row(r).Height = xy.Y;
//                }
                r++;
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
//            ws.Column(1).Width = maxwid;
            r = 2;
//            foreach (var ep in q)
//                AddImage(ws, r++, 1, ep.GetImage());
            return new EpplusResult(excelpackage, "people-imageurl.xlsx");
        }
    }
}