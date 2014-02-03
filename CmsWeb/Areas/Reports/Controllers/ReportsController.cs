using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsData.View;
using CmsWeb.Areas.Main.Models.Avery;
using CmsWeb.Areas.Main.Models.Directories;
using CmsWeb.Areas.Main.Models.Report;
using CmsWeb.Code;
using CmsWeb.Models;
using Dapper;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using UtilityExtensions;
using FamilyResult = CmsWeb.Areas.Main.Models.Report.FamilyResult;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaUrl = "Reports")]
    public class ReportsController : CmsStaffController
    {
        [GET("Reports/Attendance/{id}")]
        public ActionResult Attendance(int id, AttendanceModel m)
        {
            if(m.OrgId == 0)
                m = new AttendanceModel() { OrgId = id };
            return View(m);
        }
        [POST("Reports/Attendance")]
        public ActionResult Attendance(AttendanceModel m)
        {
            return View(m);
        }
        [GET("Reports/Attendee/{id}")]
        public ActionResult Attendee(int id)
        {
            return new AttendeeResult(id);
        }

        [POST("Reports/AttendanceDetail")]
        public ActionResult AttendanceDetail(string Dt1, string Dt2, OrgSearchModel m)
        {
            DateTime? dt1 = Dt1.ToDate();
            if (!dt1.HasValue)
                dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            DateTime? dt2 = Dt2.ToDate();
            if (!dt2.HasValue)
                dt2 = dt1.Value.AddDays(1);
            var m2 = new AttendanceDetailModel(dt1.Value, dt2, m);
            return View(m2);
        }

        [GET("Reports/Avery/{id}")]
        public ActionResult Avery(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult {id = id.Value};
        }

        [GET("Reports/Avery3/{id}")]
        public ActionResult Avery3(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new Avery3Result {id = id.Value};
        }

        [GET("Reports/AveryAddress/{id}")]
        public ActionResult AveryAddress(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, int skipNum = 0)
        {
            if (!id.HasValue)
                return Content("no query");
            if (!format.HasValue())
                return Content("no format");
            return new AveryAddressResult
            {
                id = id.Value,
                format = format,
                titles = titles,
                usephone = usephone ?? false,
                skip = skipNum,
                sortzip = sortzip
            };
        }

        [GET("Reports/BarCodeLabels/{id}")]
        public ActionResult BarCodeLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new BarCodeLabelsResult(id.Value);
        }

        [POST("Reports/CheckinControl")]
        public ActionResult CheckinControl(CheckinControlModel m)
        {
            return new CheckinControlResult {model = m};
        }

        [GET("Reports/ChurchAttendance/{dt:datetime?}")]
        public ActionResult ChurchAttendance(DateTime? dt)
        {
            if (!dt.HasValue)
                dt = ChurchAttendanceModel.MostRecentAttendedSunday();
            var m = new ChurchAttendanceModel(dt.Value);
            return View(m);
        }

        [GET("Reports/ChurchAttendance2")]
        public ActionResult ChurchAttendance2(DateTime? dt1, DateTime? dt2, string skipweeks)
        {
            if (!dt1.HasValue)
                dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!dt2.HasValue)
                dt2 = DateTime.Today;
            var m = new ChurchAttendance2Model(dt1, dt2, skipweeks);
            return View(m);
        }

        [POST("Reports/ClassList")]
        public ActionResult ClassList(string org, OrgSearchModel m)
        {
            return new ClassListResult(m) {orgid = org == "curr" ? Util2.CurrentOrgId : null};
        }

        [GET("Reports/Contacts/{id:guid}")]
        public ActionResult Contacts(Guid? id, bool? sortAddress, string orgname)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ContactsResult(id.Value, sortAddress, orgname);
        }

        [GET("Reports/Decisions")]
        public ActionResult Decisions(int? campus, DateTime? dt1, DateTime? dt2)
        {
            DateTime today = Util.Now.Date;
            if (!dt1.HasValue)
                dt1 = new DateTime(today.Year, 1, 1);
            if (!dt2.HasValue)
                dt2 = today;
            var m = new DecisionSummaryModel(dt1, dt2) { Campus = campus };
            return View(m);
        }

        [GET("Reports/DecisionsToQuery/{command}/{key}")]
        public ActionResult DecisionsToQuery(string command, string key, int? campus, DateTime? dt1, DateTime? dt2)
        {
            string r = new DecisionSummaryModel(dt1, dt2) { Campus = campus }.ConvertToSearch(command, key);
            return Redirect(r);
        }

        [GET("Reports/EmployerAddress/{id}")]
        public ActionResult EmployerAddress(Guid id)
        {
            return new EmployerAddress(id, true);
        }

        [POST("Reports/EnrollmentControl")]
        public ActionResult EnrollmentControl(bool? excel, EnrollmentControlModel m)
        {
            if (excel != true)
                return new EnrollmentControlResult {model = m};

            IOrderedEnumerable<EnrollmentControlModel.MemberInfo> d = from p in m.list()
                orderby p.Name
                select p;
            var workbook = new HSSFWorkbook(); // todo: Convert all Excel exports to this approach
            ISheet sheet = workbook.CreateSheet("EnrollmentControl");
            int rowIndex = 0;
            IRow row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("PeopleId");
            row.CreateCell(1).SetCellValue("Name");
            row.CreateCell(2).SetCellValue("Organization");
            row.CreateCell(3).SetCellValue("Location");
            row.CreateCell(4).SetCellValue("MemberType");
            rowIndex++;
            sheet.DisplayRowColHeadings = true;

            foreach (EnrollmentControlModel.MemberInfo i in d)
            {
                row = sheet.CreateRow(rowIndex);
                row.CreateCell(0).SetCellValue(i.Id);
                row.CreateCell(1).SetCellValue(i.Name);
                row.CreateCell(2).SetCellValue(i.Organization);
                row.CreateCell(3).SetCellValue(i.Location);
                row.CreateCell(4).SetCellValue(i.MemberType);
                rowIndex++;
            }
            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            sheet.AutoSizeColumn(2);
            sheet.AutoSizeColumn(3);
            sheet.AutoSizeColumn(4);
            string saveAsFileName = string.Format("EnrollmentControl-{0:d}.xls", DateTime.Now);
            var ms = new MemoryStream();
            workbook.Write(ms);
            return File(ms.ToArray(), "application/vnd.ms-excel", "attachment;filename=" + saveAsFileName);
        }

        [POST("Reports/EnrollmentControl2")]
        public ActionResult EnrollmentControl2(EnrollmentControlModel m)
        {
            return View(m);
        }

        [GET("Reports/ExtraValueData")]
        public ActionResult ExtraValueData()
        {
            if (ViewExtensions2.UseNewLook())
                return Redirect("/ExtraValue/Summary");
            var q = from e in DbUtil.Db.PeopleExtras
                where e.StrValue == null && e.BitValue == null
                let TypeValue = e.DateValue != null 
                    ? "Date" : e.Data != null 
                    ? "Text" : e.IntValue != null 
                    ? "Int" : "?"
                group e by new {e.Field, TypeValue}
                into g
                select new ExtraInfo
                {
                    Field = g.Key.Field,
                    type = g.Key.TypeValue,
                    Count = g.Count(),
                };

            var ev = StandardExtraValues.GetExtraValues();
            var list = from e in q.ToList()
                let f = ev.SingleOrDefault(ff => ff.name == e.Field)
                where f == null || f.UserCanView()
                orderby e.Field
                select e;
            return View(list);
        }

        [GET("Reports/ExtraValues")]
        public ActionResult ExtraValues()
        {
            if (ViewExtensions2.UseNewLook())
                return Redirect("/ExtraValue/Summary");
            var ev = StandardExtraValues.GetExtraValues();
            var q = from e in DbUtil.Db.PeopleExtras
                where e.StrValue != null || e.BitValue != null
                let TypeValue = e.StrValue != null ? "Code" : "Bit"
                group e by new {e.Field, val = e.StrValue ?? (e.BitValue == true ? "1" : "0"), TypeValue}
                into g
                select new ExtraInfo
                {
                    Field = g.Key.Field,
                    Value = g.Key.val,
                    type = g.Key.TypeValue,
                    Count = g.Count(),
                };

            var list = from e in q.ToList()
                let f = ev.SingleOrDefault(ff => ff.name == e.Field)
                where f == null || f.UserCanView()
                orderby e.Field
                select e;
            return View(list);
        }

        [GET("Reports/ExtraValuesGrid2/{id}")]
        public ActionResult ExtraValuesGrid2(Guid id, string sort, bool alternate = false)
        {
            return RunExtraValuesGrid(id, sort, alternate);
        }

        [GET("Reports/Family/{id}")]
        public ActionResult Family(Guid id)
        {
            return new FamilyResult(id);
        }

        [GET("Reports/FamilyDirectory/{id}")]
        public ActionResult FamilyDirectory(Guid id)
        {
            return new FamilyDir(id);
        }

        [GET("Reports/FamilyDirectoryCompact/{id}")]
        public ActionResult FamilyDirectoryCompact(Guid id)
        {
            return new CompactDir(id);
        }

        [GET("Reports/Meetings")]
        public ActionResult Meetings(DateTime dt1, DateTime dt2, int? programid, int? divisionid)
        {
            var m = new MeetingsModel() {Dt1 = dt1, Dt2 = dt2, ProgramId = programid, DivisionId = divisionid};
            return View(m);
        }
        [POST("Reports/Meetings")]
        public ActionResult Meetings(MeetingsModel m)
        {
            if (!m.Dt1.HasValue)
                m.Dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!m.Dt2.HasValue)
                m.Dt2 = m.Dt1.Value.AddDays(1);
            return View(m);
        }

        [POST("Reports/MeetingsToQuery/{type}")]
        public ActionResult MeetingsToQuery(string type, MeetingsModel m)
        {
            string r = m.ConvertToSearch(type);
            TempData["autorun"] = true;
            return Redirect(r);
        }

        [GET("Reports/NameLabels/{id}")]
        public ActionResult NameLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult {namesonly = true, id = id.Value};
        }

        [POST("Reports/OrgLeaders")]
        public ActionResult OrgLeaders(string org, OrgSearchModel m)
        {
            return new OrgLeadersResult(m) {orgid = org == "curr" ? Util2.CurrentOrgId : null};
        }

        [GET("Reports/PastAttendee/{id}")]
        public ActionResult PastAttendee(int? id)
        {
            if (!id.HasValue)
                return Content("no orgid");
            return new PastAttendeeResult(id);
        }

        [GET("Reports/PictureDirectory/{id}")]
        public ActionResult PictureDirectory(Guid id)
        {
            return new PictureDir(id);
        }

        [GET("Reports/Prospect/{id}")]
        public ActionResult Prospect(Guid? id, bool? Form, bool? Alpha)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ProspectResult(id.Value, Form ?? false, Alpha ?? false);
        }

        [GET("Reports/QueryStats")]
        public ActionResult QueryStats()
        {
            return new QueryStatsResult();
        }

        [POST("Reports/RallyRollsheet/{id}")]
        public ActionResult RallyRollsheet(Guid id, string org, string dt, int? meetingid, int? bygroup, string sgprefix,
            bool? altnames, string highlight, OrgSearchModel m)
        {
            DateTime? dt2 = dt.ToDate();

            return new RallyRollsheetResult
            {
                qid = id,
                orgid = org == "curr" ? Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new[] {0},
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                Model = m
            };
        }

        [POST("Reports/RecentAbsents")]
        public ActionResult RecentAbsents(OrgSearchModel m)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP2", new
            {
                name = m.Name,
                prog = m.ProgramId,
                div = m.DivisionId,
                type = m.TypeId,
                campus = m.CampusId,
                sched = m.ScheduleId,
                status = m.StatusId,
                onlinereg = m.OnlineReg,
                mainfellowship = m.MainFellowship,
                parentorg = m.ParentOrg
            }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View(q);
        }

        [GET("Reports/RecentAbsents/{id}")]
        public ActionResult RecentAbsents1(int? id)
        {
            int? divid = null;
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP", new {orgid = id, divid, days = 36},
                commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View("RecentAbsents", q);
        }

        [Authorize(Roles = "Admin")]
        [GET("Reports/RecentRegistrations")]
        public ActionResult RecentRegistrations(int? days, int? orgid, string sort)
        {
            IQueryable<Registration> q = from r in DbUtil.Db.Registrations(days ?? 90)
                where (orgid ?? 0) == 0 || r.OrganizationId == orgid
                select r;
            if (!User.IsInRole("Finance"))
                q = q.Where(rr => !rr.OrganizationName.Contains("Giving"));
            q = sort == "Organization"
                ? q.OrderBy(rr => rr.OrganizationName).ThenByDescending(rr => rr.Completed)
                : q.OrderByDescending(rr => rr.Stamp);
            return View(q);
        }

        [GET("Reports/Registration/{id}")]
        public ActionResult Registration(Guid? id, int? oid)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RegistrationResult(id, oid);
        }

        [Authorize(Roles = "Admin")]
        [GET("Reports/RegistrationSummary")]
        public ActionResult RegistrationSummary(int? days, string sort)
        {
            IQueryable<RecentRegistration> q = DbUtil.Db.RecentRegistrations(days ?? 90);
            q = sort == "Organization"
                ? q.OrderBy(rr => rr.OrganizationName).ThenByDescending(rr => rr.Completed)
                : q.OrderByDescending(rr => rr.Dt2);
            return View(q);
        }

        [GET("Reports/RollLabels/{id}")]
        public ActionResult RollLabels(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RollLabelsResult
            {
                qid = id.Value,
                format = format,
                titles = titles ?? false,
                usephone = usephone ?? false,
                sortzip = sortzip
            };
        }

        [GET("Reports/Rollsheet")]
        public ActionResult Rollsheet(string org, string dt, int? meetingid, int? bygroup, string sgprefix,
            bool? altnames, string highlight)
        {
            DateTime? dt2 = dt.ToDate();
            return new RollsheetResult
            {
                orgid = org == "curr" ? Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new[] {0},
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                highlightsg = highlight,
            };
        }
        [POST("Reports/Rollsheet")]
        public ActionResult Rollsheet(string dt, int? bygroup, string sgprefix,
            bool? altnames, string highlight, OrgSearchModel m)
        {
            DateTime? dt2 = dt.ToDate();
            return new RollsheetResult
            {
                groups = new[] {0},
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                highlightsg = highlight,
                Model = m
            };
        }

        [GET("Reports/Roster/{id:guid}")]
        public ActionResult Roster(Guid id, int? oid)
        {
            return new RosterListResult
            {
                qid = id,
                orgid = oid,
            };
        }

        [POST("Reports/Roster")]
        public ActionResult Roster(OrgSearchModel m)
        {
            return new RosterListResult(m);
        }

        [GET("Reports/Roster1/{id:guid}")]
        public ActionResult Roster1(Guid id, int? oid)
        {
            return new RosterResult
            {
                qid = id,
                org = oid,
            };
        }

        [POST("Reports/Roster1")]
        public ActionResult Roster1(OrgSearchModel m)
        {
            return new RosterResult(m);
        }

        private ActionResult RunExtraValuesGrid(Guid id, string sort, bool alternate)
        {
            string[] roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            XDocument xml = XDocument.Parse(DbUtil.Db.Content("StandardExtraValues.xml", "<Fields/>"));
            IEnumerable<string> fields = (from ff in xml.Root.Elements("Field")
                let vroles = ff.Attribute("VisibilityRoles")
                where vroles != null && (vroles.Value.Split(',').All(rr => !roles.Contains(rr)))
                select ff.Attribute("name").Value);
            string nodisplaycols = string.Join("|", fields);

            Tag tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_ExtraValues);
            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", sort ?? "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            SqlDataReader rdr = cmd.ExecuteReader();
            ViewBag.queryid = id;
            if (alternate)
                return View("ExtraValuesGrid2", rdr);
            return View("ExtraValuesGrid", rdr);
        }

        [POST("Reports/SGMap")]
        public ActionResult SGMap(OrgSearchModel m)
        {
            return Redirect("/Sgmap/Index/" + m.DivisionId);
        }

        [POST("Reports/ShirtSizes")]
        public ActionResult ShirtSizes(string org, OrgSearchModel m)
        {
            int? orgid = org == "curr" ? Util2.CurrentOrgId : null;
            IQueryable<Organization> orgs = m.FetchOrgs();
            IQueryable<ShirtSizeInfo> q = from om in DbUtil.Db.OrganizationMembers
                join o in orgs on om.OrganizationId equals o.OrganizationId
                where o.OrganizationId == orgid || (orgid ?? 0) == 0
                group 1 by om.ShirtSize
                into g
                select new ShirtSizeInfo
                {
                    Size = g.Key,
                    Count = g.Count(),
                };
            return View(q);
        }

        [GET("Reports/VisitsAbsents/{id}")]
        public ActionResult VisitsAbsents(int? id)
        {
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult(id);
        }

        [GET("Reports/VisitsAbsents2/{id}")]
        public ActionResult VisitsAbsents2(int? id)
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult2(id);
        }

        [GET("Reports/VitalStats")]
        public ActionResult VitalStats()
        {
            ViewData["table"] = QueryFunctions.VitalStats(DbUtil.Db);
            return View();
        }

        [POST("Reports/WeeklyAttendance")]
        public ActionResult WeeklyAttendance(WeeklyAttendanceModel m)
        {
            var q = m.Attendances();
            var cols = typeof (WeeklyAttendanceModel.AttendInfo).GetProperties();
            var count = q.Count();
            var ep = new ExcelPackage();
            var ws = ep.Workbook.Worksheets.Add("Sheet1");
            //ws.SetExcelHeader("PeopleId", "Name", "Age", "MainFellowship", "OrgId", "Teacher", "1+ Attendance Per Week Across All Groups", "AttendPct", "Count", "FirstDate", "LastDate");
            ws.Cells["A2"].LoadFromCollection(q);
            var range = ws.Cells[1, 1, count + 1, cols.Length];
            var table = ws.Tables.Add(range, "Attends");
            for (var i = 0; i < cols.Length; i++)
            {
                var col = i + 1;
                var name = cols[i].Name;
                table.Columns[i].Name = name;
                var colrange = ws.Cells[1, col, count + 2, col];
                switch (name)
                {
                    case "AttendStr":
                        table.Columns[i].Name = "1+ Attendance Per Week Across All Groups";
                        break;
                    case "AttendPct":
                        colrange.Style.Numberformat.Format = "0.0";
                        colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Column(col).Width = 8;
                        break;
                    case "FirstDate":
                    case "LastDate":
                        colrange.Style.Numberformat.Format = "mm-dd-yy";
                        colrange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        ws.Column(col).Width = 12;
                        break;
                }
            }
            ws.Cells[ws.Dimension.Address].AutoFitColumns();
            return new EpplusResult(ep, "WeeklyAttendance.xlsx");
        }

        [GET("Reports/WeeklyAttendance/{id}")]
        public ActionResult WeeklyAttendance(Guid id)
        {
            return new WeeklyAttendanceResult(id);
        }

        [GET("Reports/WeeklyDecisions")]
        public ActionResult WeeklyDecisions(int? campus, DateTime? sunday)
        {
            var m = new WeeklyDecisionsModel(sunday) { Campus = campus };
            return View(m);
        }

        public class ExtraInfo
        {
            public string Field { get; set; }
            public string Value { get; set; }
            public string type { get; set; }
            public int Count { get; set; }
        }

        public class ShirtSizeInfo
        {
            public string Size { get; set; }
            public int Count { get; set; }
        }
    }
}