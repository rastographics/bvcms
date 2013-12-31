using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Main.Models.Report;
using CmsData;
using System.IO;
using CmsWeb.Code;
using CmsWeb.Models;
using Dapper;
using NPOI.HSSF.UserModel;
using UtilityExtensions;
using System.Text;
using System.Data.SqlClient;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaUrl = "Reports")]
    public class ReportsController : CmsStaffController
    {
        public ActionResult Attendance(int id)
        {

            try
            {
                var m = new AttendanceModel(id);
                UpdateModel(m);
                return View(m);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
//        public ActionResult WeeklyAttendance(Guid id)
//        {
//            return new WeeklyAttendanceResult(id);
//        }
//        public ActionResult Family(int? id)
//        {
//            return new CmsWeb.Areas.Main.Models.Report.FamilyResult(id);
//        }
//        public ActionResult BarCodeLabels(int? id)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new BarCodeLabelsResult(id.Value);
//        }
//        public ActionResult Contacts(int? id, bool? sortAddress, string orgname)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new ContactsResult(id.Value, sortAddress, orgname);
//        }
//
//        public ActionResult Rollsheet(int? id, string org, string dt, int? meetingid, int? bygroup, string sgprefix, bool? altnames, string highlight, OrgSearchModel m)
//        {
//            var dt2 = dt.ToDate();
//            return new RollsheetResult
//            {
//                qid = id,
//                orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null,
//                groups = org == "curr" ? Util2.CurrentGroups : new int[] { 0 },
//                meetingid = meetingid,
//                bygroup = bygroup.HasValue,
//                sgprefix = sgprefix,
//                dt = dt2,
//                altnames = altnames,
//                highlightsg = highlight,
//                Model = m
//            };
//        }

//        public ActionResult RallyRollsheet(int? id, string org, string dt, int? meetingid, int? bygroup, string sgprefix, bool? altnames, string highlight, OrgSearchModel m)
//        {
//            var dt2 = dt.ToDate();
//
//            return new RallyRollsheetResult
//            {
//                qid = id,
//                orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null,
//                groups = org == "curr" ? Util2.CurrentGroups : new int[] { 0 },
//                meetingid = meetingid,
//                bygroup = bygroup.HasValue,
//                sgprefix = sgprefix,
//                dt = dt2,
//                altnames = altnames,
//                Model = m
//            };
//        }
        public ActionResult OrgLeaders(string org, OrgSearchModel m)
        {
            return new OrgLeadersResult(m) { orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null };
        }
        public ActionResult SGMap(OrgSearchModel m)
        {
            return Redirect("/Sgmap/Index/" + m.DivisionId);
        }
        public ActionResult ClassList(string org, OrgSearchModel m)
        {
            return new ClassListResult(m) { orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null };
        }
        public class ShirtSizeInfo
        {
            public string Size { get; set; }
            public int Count { get; set; }
        }
        public ActionResult ShirtSizes(string org, OrgSearchModel m)
        {
            var orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null;
            var orgs = m.FetchOrgs();
            var q = from om in DbUtil.Db.OrganizationMembers
                    join o in orgs on om.OrganizationId equals o.OrganizationId
                    where o.OrganizationId == orgid || (orgid ?? 0) == 0
                    group 1 by om.ShirtSize into g
                    select new ShirtSizeInfo
                    {
                        Size = g.Key,
                        Count = g.Count(),
                    };
            return View(q);
        }
//        public ActionResult Roster1(int? id, int? oid, OrgSearchModel m)
//        {
//            return new RosterResult(m)
//            {
//                qid = id,
//                org = oid,
//            };
//        }
//        public ActionResult Roster(int? id, int? oid, OrgSearchModel m)
//        {
//            return new RosterListResult(m)
//                {
//                    qid = id,
//                    orgid = oid,
//                };
//        }
//        public ActionResult Avery(int? id)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new AveryResult { id = id };
//        }
//        public ActionResult NameLabels(int? id)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new AveryResult { namesonly = true, id = id };
//        }
//        public ActionResult Avery3(int? id)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new Avery3Result { id = id };
//        }
        //public ActionResult Coupons()
        //{
        //    return new CouponsResult(null, null);
        //}
//        public ActionResult AveryAddress(int? id, string format, bool? titles, bool? usephone, bool? sortzip, int skipNum = 0)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            if (!format.HasValue())
//                return Content("no format");
//            return new AveryAddressResult
//            {
//                id = id,
//                format = format,
//                titles = titles,
//                usephone = usephone ?? false,
//                skip = skipNum,
//                sortzip = sortzip
//            };
//        }
//        public ActionResult RollLabels(int? id, string format, bool? titles, bool? usephone, bool? sortzip)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new RollLabelsResult
//            {
//                qid = id,
//                format = format,
//                titles = titles ?? false,
//                usephone = usephone ?? false,
//                sortzip = sortzip
//            };
//        }
//        public ActionResult Prospect(int? id, bool? Form, bool? Alpha)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new ProspectResult(id, Form ?? false, Alpha ?? false);
//        }
//        public ActionResult Attendee(int? id)
//        {
//            if (!id.HasValue)
//                return Content("no meetingid");
//            return new AttendeeResult(id);
//        }
        public ActionResult VisitsAbsents(int? id)
        {
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult(id);
        }
        public ActionResult VisitsAbsents2(int? id)
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult2(id);
        }
        public ActionResult PastAttendee(int? id)
        {
            if (!id.HasValue)
                return Content("no orgid");
            return new PastAttendeeResult(id);
        }
//        public ActionResult Registration(int? id, int? oid)
//        {
//            if (!id.HasValue)
//                return Content("no query");
//            return new RegistrationResult(id, oid);
//        }
        public ActionResult ChurchAttendance(string id)
        {
            var dt2 = id.ToDate();
            if (!dt2.HasValue)
                dt2 = ChurchAttendanceModel.MostRecentAttendedSunday();
            var m = new ChurchAttendanceModel(dt2.Value);
            return View(m);
        }
        public ActionResult WeeklyDecisions(string id)
        {
            var dt = id.ToDate();
            var m = new WeeklyDecisionsModel(dt);
            return View(m);
        }
        [GET("Reports/Decisions")]
        public ActionResult Decisions()
        {
            DbUtil.LogActivity("Viewing Decision Summary Rpt");
            var today = Util.Now.Date;
            var dt1 = new DateTime(today.Year, 1, 1);
            var dt2 = today;
            var m = new DecisionSummaryModel(dt1, dt2);
            return View(m);
        }
        [POST("Reports/Decisions")]
        public ActionResult Decisions(DateTime? dt1, DateTime? dt2)
        {
            var today = Util.Now.Date;
            if (!dt1.HasValue)
                dt1 = new DateTime(today.Year, 1, 1);
            if (!dt2.HasValue)
                dt2 = today;
            var m = new DecisionSummaryModel(dt1, dt2);
            return View(m);
        }

        [POST("Reports/DecisionsToQuery/{command}/{key}")]
        public ActionResult DecisionsToQuery(string command, string key, DateTime? dt1, DateTime? dt2)
        {
            var r = new DecisionSummaryModel(dt1, dt2).ConvertToSearch(command, key);
            return Redirect(r);
        }

        public ActionResult ChurchAttendance2(string Dt1, string Dt2, string skipweeks)
        {
            var dt1 = Dt1.ToDate();
            var dt2 = Dt2.ToDate();
            if (!dt1.HasValue)
                dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!dt2.HasValue)
                dt2 = DateTime.Today;
            var m = new ChurchAttendance2Model(dt1, dt2, skipweeks);
            return View(m);
        }
        public ActionResult AttendanceDetail(string Dt1, string Dt2, OrgSearchModel m)
        {
            var dt1 = Dt1.ToDate();
            if (!dt1.HasValue)
                dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            var dt2 = Dt2.ToDate();
            if (!dt2.HasValue)
                dt2 = dt1.Value.AddDays(1);
            var m2 = new AttendanceDetailModel(dt1.Value, dt2, m);
            return View(m2);
        }
        public ActionResult RecentAbsents1(int? id)
        {
            int? divid = null;
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP", new { orgid = id, divid = divid, days = 36 },
                            commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View("RecentAbsents", q);
        }

        [HttpPost]
        public ActionResult RecentAbsents(int? id, OrgSearchModel m)
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

        public ActionResult Meetings(MeetingsModel m)
        {
            if (!m.FromWeekAtAGlance && !m.Dt1.HasValue)
                m.Dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!m.Dt2.HasValue && m.Dt1.HasValue)
                m.Dt2 = m.Dt1.Value.AddDays(1);
            return View(m);
        }
        [POST("Reports/MeetingsToQuery/{type}")]
        public ActionResult MeetingsToQuery(string type, MeetingsModel m)
        {
            var r = m.ConvertToSearch(type);
            TempData["autorun"] = true;
            return Redirect(r);
        }
        public ActionResult Test()
        {
            return new TestResult();
        }
        public ActionResult QueryStats()
        {
            return new QueryStatsResult();
        }
        public ActionResult VitalStats()
        {
            ViewData["table"] = CmsData.QueryFunctions.VitalStats(DbUtil.Db);
            return View();
        }
        public class ExtraInfo
        {
            public string Field { get; set; }
            public string Value { get; set; }
            public string type { get; set; }
            public int Count { get; set; }
        }
        public ActionResult ExtraValues()
        {
            if (ViewExtensions2.UseNewLook())
                return Redirect("/ExtraValue/Summary");
            var ev = StandardExtraValues.GetExtraValues();
            var q = from e in DbUtil.Db.PeopleExtras
                    where e.StrValue != null || e.BitValue != null
                    let TypeValue = e.StrValue != null ? "Code" : "Bit"
                    group e by new { e.Field, val = e.StrValue ?? (e.BitValue == true ? "1" : "0"), TypeValue } into g
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
        public ActionResult ExtraValueData()
        {
            if (ViewExtensions2.UseNewLook())
                return Redirect("/ExtraValue/Summary");
            var ev = StandardExtraValues.GetExtraValues();
            var q = from e in DbUtil.Db.PeopleExtras
                    where e.StrValue == null && e.BitValue == null
                    let TypeValue = e.DateValue != null ? "Date" : e.Data != null ? "Text" : e.IntValue != null ? "Int" : "?"
                    group e by new { e.Field, TypeValue } into g
                    select new ExtraInfo
                    {
                        Field = g.Key.Field,
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
//        public ActionResult ExtraValuesGrid(int id, string sort, bool alternate = false)
//        {
//            return RunExtraValuesGrid(id, sort, alternate);
//        }
        public ActionResult ExtraValuesGrid2(Guid id, string sort, bool alternate = false)
        {
            return RunExtraValuesGrid(id, sort, alternate);
        }
        private ActionResult RunExtraValuesGrid(Guid id, string sort, bool alternate)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var xml = XDocument.Parse(DbUtil.Db.Content("StandardExtraValues.xml", "<Fields/>"));
            var fields = (from ff in xml.Root.Elements("Field")
                          let vroles = ff.Attribute("VisibilityRoles")
                          where vroles != null && (vroles.Value.Split(',').All(rr => !roles.Contains(rr)))
                          select ff.Attribute("name").Value);
            var nodisplaycols = string.Join("|", fields);

            var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_ExtraValues);
            var cmd = new SqlCommand("dbo.ExtraValues @p1, @p2, @p3");
            cmd.Parameters.AddWithValue("@p1", tag.Id);
            cmd.Parameters.AddWithValue("@p2", sort ?? "");
            cmd.Parameters.AddWithValue("@p3", nodisplaycols);
            cmd.Connection = new SqlConnection(Util.ConnectionString);
            cmd.Connection.Open();
            var rdr = cmd.ExecuteReader();
            ViewBag.queryid = id;
            if(alternate)
                return View("ExtraValuesGrid2", rdr);
            return View("ExtraValuesGrid", rdr);
        }
//        public ActionResult FamilyDirectory(int id)
//        {
//            return new FamilyDir(id);
//        }
//        public ActionResult FamilyDirectoryCompact(int id)
//        {
//            return new CompactDir(id);
//        }
//        public ActionResult PictureDirectory(int id)
//        {
//            return new PictureDir(id);
//        }
//        public ActionResult EmployerAddress(int id)
//        {
//            return new EmployerAddress(id, true);
//        }

        public class QueryStatsResult : ActionResult
        {
            StringBuilder sb = new StringBuilder();
            public override void ExecuteResult(ControllerContext context)
            {
                var dt = DateTime.Parse("1/1/1900");
                var firstrunid = DateTime.Now.Date.Subtract(dt).Days - 200;
                var q = from s in DbUtil.Db.QueryStats
                        where s.RunId > firstrunid
                        group s by s.RunId into g
                        orderby g.Key descending
                        select new
                        {
                            g.Key,
                            list = from s in g.OrderBy(ss => ss.StatId)
                                   select new { Count = s.Count as int?, s.StatId }
                        };
                var rows = q.Count();

                var d = new List<Dictionary<string, string>>();

                var q3 = from s in DbUtil.Db.QueryStats
                         where s.RunId > firstrunid
                         group s by s.StatId into g
                         orderby g.Key
                         select new { g.Key, g.OrderByDescending(ss => ss.RunId).First().Description };

                var head = q3.ToDictionary(ss => ss.Key, ss => ss.Description);

                var Response = context.HttpContext.Response;
                foreach (var r in q)
                {
                    var row = new Dictionary<string, string>();
                    row["S00"] = dt.AddDays(r.Key).ToString("d");
                    foreach (var s in r.list)
                        row[s.StatId] = s.Count.ToString2("N0");
                    d.Add(row);
                }
                Response.Write("<table cellpadding=4>\n<tr><td>Date</td>");
                foreach (var c in head)
                    Response.Write("<td align='right'>{0}</td>".Fmt(c.Value));
                Response.Write("</tr>\n");
                foreach (var r in d)
                {
                    Response.Write("<tr><td>{0}</td>".Fmt(r["S00"]));
                    foreach (var c in head)
                    {
                        if (r.ContainsKey(c.Key))
                            Response.Write("<td align='right'>{0}</td>".Fmt(r[c.Key]));
                        else
                            Response.Write("<td></td>");
                    }
                    Response.Write("</tr>\n");
                }
                Response.Write("</table>");
            }
        }
        public ActionResult CheckinControl(CheckinControlModel m)
        {
            return new CheckinControlResult { model = m };
        }
        public ActionResult EnrollmentControl(bool? excel, EnrollmentControlModel m)
        {
            if (excel != true)
                return new EnrollmentControlResult { model = m };

            var d = from p in m.list()
                    orderby p.Name
                    select p;
            var workbook = new HSSFWorkbook(); // todo: Convert all Excel exports to this approach
            var sheet = workbook.CreateSheet("EnrollmentControl");
            var rowIndex = 0;
            var row = sheet.CreateRow(rowIndex);
            row.CreateCell(0).SetCellValue("PeopleId");
            row.CreateCell(1).SetCellValue("Name");
            row.CreateCell(2).SetCellValue("Organization");
            row.CreateCell(3).SetCellValue("Location");
            row.CreateCell(4).SetCellValue("MemberType");
            rowIndex++;
            sheet.DisplayRowColHeadings = true;

            foreach (var i in d)
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
        public ActionResult EnrollmentControl2(EnrollmentControlModel m)
        {
            return View(m);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult RecentRegistrations(int? days, int? orgid, string sort)
        {
            var q = from r in DbUtil.Db.Registrations(days ?? 90)
                where (orgid ?? 0) == 0 || r.OrganizationId == orgid
                select r;
            if (!User.IsInRole("Finance"))
                q = q.Where(rr => !rr.OrganizationName.Contains("Giving"));
            q = sort == "Organization" 
                ? q.OrderBy(rr => rr.OrganizationName).ThenByDescending(rr => rr.Completed) 
                : q.OrderByDescending(rr => rr.Stamp);
            return View(q);
        }
        [Authorize(Roles = "Admin")]
        public ActionResult RegistrationSummary(int? days, string sort)
        {
            var q = DbUtil.Db.RecentRegistrations(days ?? 90);
            q = sort == "Organization" 
                ? q.OrderBy(rr => rr.OrganizationName).ThenByDescending(rr => rr.Completed) 
                : q.OrderByDescending(rr => rr.Dt2);
            return View(q);
        }
    }
}
