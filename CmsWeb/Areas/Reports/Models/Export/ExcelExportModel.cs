using CmsData;
using CmsData.API;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using UtilityExtensions;
using DirectoryInfo = CmsWeb.Areas.Reports.Models.DirectoryInfo;

namespace CmsWeb.Models
{
    public static class ExcelExportModel
    {
        public static void SaveAs(this DataTable dt, string filename = "People.xlsx", bool useTable = false)
        {
            var ep = new ExcelPackage();
            ep.AddSheet(dt, filename, useTable);
            ep.SaveAs(new FileInfo(filename));
        }
        public static EpplusResult ToExcel(this DataTable dt, string filename = "People.xlsx", bool useTable = false)
        {
            var ep = new ExcelPackage();
            ep.AddSheet(dt, filename, useTable);
            return new EpplusResult(ep, filename);
        }
        public static EpplusResult ToExcel(this IDataReader rd, string filename = null, bool useTable = false, bool fromSql = false)
        {
            DataTable dt;
            if (fromSql)
            {
                dt = rd.DataReaderToTable();
            }
            else
            {
                dt = new DataTable();
                dt.Load(rd);
            }
            return dt.ToExcel(filename, useTable);
        }

        public static DataTable ToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in props)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (var item in items)
            {
                var values = new object[props.Length];
                for (var i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static IEnumerable<dynamic> AsEnumerable(this ExcelPackage package)
        {
            var ws = package.Workbook.Worksheets.First();
            return ws.AsEnumerable();
        }
        public static IEnumerable<dynamic> AsEnumerable(this ExcelWorksheet ws)
        {
            var header = ws.GetHeaderColumns();
            var r = 2;
            while (r <= ws.Dimension.End.Row)
            {
                var dict = new Dictionary<string, object>();
                foreach (var kv in header)
                {
                    dict[kv.Key] = ws.Cells[r, kv.Value].Value;
                }

                var record = new DynamicData(dict);
                r++;
                yield return record;
            }
        }
        public static Dictionary<string, int> GetHeaderColumns(this ExcelWorksheet sheet)
        {
            var names = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var n = 0;
            foreach (var c in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
            {
                n++;
                if (c.Text.HasValue())
                {
                    names.Add(c.Text, n);
                }
            }
            return names;
        }

        public static List<ExcelPic> List(Guid queryid)
        {
            //var Db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);
            var q = from p in query
                    let om = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == p.BibleFellowshipClassId)
                    let spouse = DbUtil.Db.People.Where(pp => pp.PeopleId == p.SpouseId).Select(pp => pp.PreferredName).SingleOrDefault()
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
                        BMon = p.BirthMonth,
                        BYear = p.BirthYear,
                        BDay = p.BirthDay,
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
                        Age = Person.AgeDisplay(p.Age, p.PeopleId).ToString(),
                        School = p.SchoolOther,
                        Grade = p.Grade.ToString(),
                        AttendPctBF = (om == null ? 0 : om.AttendPct == null ? 0 : om.AttendPct.Value),
                        Married = p.MaritalStatus.Description,
                        FamilyId = p.FamilyId,
                        ImageId = p.Picture.LargeId,
                    };
            return q.Take(10000).ToList();
        }
        public static List<DirectoryInfo> DirectoryList(Guid queryid)
        {
            //var db = Db;
            var query = DbUtil.Db.PeopleQuery(queryid);
            var q = from p in query
                    let spouse = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == p.SpouseId)
                    let familytitle =
                        p.Family.FamilyExtras.Where(mm => mm.Field == "FamilyName" && mm.Data != null)
                            .Select(mm => mm.Data)
                            .SingleOrDefault()
                    let familyname = familytitle ?? p.Family.People
                        .Where(pp => pp.PeopleId == pp.Family.HeadOfHouseholdId)
                        .Select(pp => pp.LastName).SingleOrDefault()
                    let couplename = DbUtil.Db.FamilyExtras.SingleOrDefault(ff => ff.FamilyId == p.FamilyId && ff.Field == "CoupleName").Data
                    orderby familyname, p.FamilyId, p.Name2
                    select new DirectoryInfo()
                    {
                        Person = p,
                        SpouseFirst = spouse.PreferredName,
                        SpouseLast = spouse.LastName,
                        SpouseEmail = spouse.EmailAddress,
                        SpouseCellPhone = spouse.CellPhone,
                        SpouseDoNotPublishPhone = spouse.DoNotPublishPhones,
                        Children = p.PositionInFamilyId != 10 ? ""
                            : string.Join(", ", (from cc in p.Family.People
                                                 where cc.PositionInFamilyId == 30
                                                 //where cc.Age <= 18
                                                 select cc.LastName == familyname
                                                         ? cc.PreferredName
                                                         : cc.Name).ToList()),
                        FamilyTitle = familytitle ?? ("The " + p.Name + " Family"),
                        ImageId = p.Picture.LargeId,
                        FamImageId = p.Family.Picture.LargeId,
                        CoupleName = couplename,
                        ChildrenAges = p.PositionInFamilyId != 10
                            ? ""
                            : string.Join("~", p.Family.People
                                .Where(cc => cc.PositionInFamilyId > 10)
                                .Select(cc =>
                                    $"{(cc.LastName == familyname ? cc.PreferredName : cc.Name)}|{Person.AgeDisplay(cc.Age, cc.PeopleId)}|{cc.BirthMonth}|{cc.BirthDay}"
                                )),
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
                ws.Cells[r, c++].Value = Person.AgeDisplay(ep.Age, ep.PeopleId);
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
