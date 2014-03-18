using System;
using System.Collections.Generic;
using System.Linq;
using CmsData;
using OfficeOpenXml;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public static class ExcelExportModel
    {
        public static List<ExcelPic> List(Guid queryid)
        {
            var Db = DbUtil.Db;
            var query = Db.PeopleQuery(queryid);
            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    let spouse = Db.People.Where(pp => pp.PeopleId == p.SpouseId).Select(pp => pp.PreferredName).SingleOrDefault()
                    orderby p.Name2
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
                            : string.Join(", ", p.Family.People.Where(cc => cc.PositionInFamilyId == 30).Select(cc => cc.PreferredName)),
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
            var maxwid = 0.0;
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
//        private static void AddImage(ExcelWorksheet ws, int r, int c, ImageData.Image i)
//        {
//            if (i == null)
//                return;
//            var img = i.GetBitmap(160, 200);
//            var pic = ws.Drawings.AddPicture("img_" + i.Id, img);
//            pic.From.Column = c - 1;
//            pic.From.Row = r - 1;
//            pic.From.ColumnOff = ExcelHelper.Pixel2MTU(1);
//            pic.From.RowOff = ExcelHelper.Pixel2MTU(1);
//            pic.SetSize(img.Width, img.Height);
//        }
//
//        class XY
//        {
//            public double X { get; set; }
//            public double Y { get; set; }
//        }
//        private static XY ImgRowHeight(ExcelWorksheet ws, ImageData.Image i)
//        {
//            if (i == null)
//                return null;
//            var img = i.GetBitmap(160, 200);
//            var xy = new XY
//            {
//                Y = ExcelHelper.Pixel2RowHeight(img.Height + 2),
//                X = ExcelHelper.Pixel2ColumnWidth(ws, img.Width)
//            };
//            return xy;
//        }
    }
}