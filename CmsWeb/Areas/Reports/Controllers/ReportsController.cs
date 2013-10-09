using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsWeb.Areas.Main.Models.Avery;
using CmsWeb.Areas.Main.Models.Directories;
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

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaUrl = "Reports2")]
    public class ReportsController : CmsStaffController
    {
        [GET("Reports2/Attendance/{id}")]
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
        [GET("Reports2/WeeklyAttendance/{id}")]
        public ActionResult WeeklyAttendance(Guid id)
        {
            return new WeeklyAttendanceResult(id);
        }
        [GET("Reports2/Family/{id}")]
        public ActionResult Family(Guid id)
        {
            return new Main.Models.Report.FamilyResult(id);
        }
        [GET("Reports2/BarCodeLabels/{id}")]
        public ActionResult BarCodeLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new BarCodeLabelsResult(id.Value);
        }
        [GET("Reports2/Contacts/{id:guid}")]
        public ActionResult Contacts(Guid? id, bool? sortAddress, string orgname)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ContactsResult(id.Value, sortAddress, orgname);
        }

        [GET("Reports2/Rollsheet/{id}")]
        public ActionResult Rollsheet(Guid id, string org, string dt, int? meetingid, int? bygroup, string sgprefix, bool? altnames, string highlight, OrgSearchModel m)
        {
            var dt2 = dt.ToDate();
            return new RollsheetResult
            {
                qid = id,
                orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new int[] { 0 },
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                highlightsg = highlight,
                Model = m
            };
        }

        [GET("Reports2/RallyRollsheet/{id}")]
        public ActionResult RallyRollsheet(Guid id, string org, string dt, int? meetingid, int? bygroup, string sgprefix, bool? altnames, string highlight, OrgSearchModel m)
        {
            var dt2 = dt.ToDate();

            return new RallyRollsheetResult
            {
                qid = id,
                orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new int[] { 0 },
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                Model = m
            };
        }
        [POST("Reports2/OrgLeaders/{org}")]
        public ActionResult OrgLeaders(string org, OrgSearchModel m)
        {
            return new OrgLeadersResult(m) { orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null };
        }
        [POST("Reports2/ClassList/{org}")]
        public ActionResult ClassList(string org, OrgSearchModel m)
        {
            return new ClassListResult(m) { orgid = org == "curr" ? (int?)Util2.CurrentOrgId : null };
        }
        public class ShirtSizeInfo
        {
            public string Size { get; set; }
            public int Count { get; set; }
        }
        [POST("Reports2/ShirtSizes/{org}")]
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
        [GET("Reports2/Roster1/{id}")]
        public ActionResult Roster1(Guid id, int? oid)
        {
            return new RosterResult
            {
                qid = id,
                org = oid,
            };
        }
        [POST("Reports2/Roster")]
        public ActionResult Roster(OrgSearchModel m)
        {
            return new RosterListResult(m);
        }
        [GET("Reports2/Avery/{id}")]
        public ActionResult Avery(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult { id = id };
        }
        [GET("Reports2/NameLabels/{id}")]
        public ActionResult NameLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult { namesonly = true, id = id };
        }
        [GET("Reports2/Avery3/{id}")]
        public ActionResult Avery3(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new Avery3Result { id = id };
        }
        //public ActionResult Coupons()
        //{
        //    return new CouponsResult(null, null);
        //}
        [GET("Reports2/AveryAddress/{id}")]
        public ActionResult AveryAddress(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, int skipNum = 0)
        {
            if (!id.HasValue)
                return Content("no query");
            if (!format.HasValue())
                return Content("no format");
            return new AveryAddressResult
            {
                id = id,
                format = format,
                titles = titles,
                usephone = usephone ?? false,
                skip = skipNum,
                sortzip = sortzip
            };
        }
        [GET("Reports2/RollLabels/{id}")]
        public ActionResult RollLabels(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RollLabelsResult
            {
                qid = id,
                format = format,
                titles = titles ?? false,
                usephone = usephone ?? false,
                sortzip = sortzip
            };
        }
        [GET("Reports2/Prospect/{id}")]
        public ActionResult Prospect(Guid? id, bool? Form, bool? Alpha)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ProspectResult(id, Form ?? false, Alpha ?? false);
        }
        [GET("Reports2/Attendee/{id}")]
        public ActionResult Attendee(int? id)
        {
            if (!id.HasValue)
                return Content("no meetingid");
            return new AttendeeResult(meetingid: id);
        }
        [GET("Reports2/VisitsAbsents/{id}")]
        public ActionResult VisitsAbsents(int? id)
        {
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult(meetingid: id);
        }
        [GET("Reports2/VisitsAbsents2/{id}")]
        public ActionResult VisitsAbsents2(int? id)
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult2(meetingid: id);
        }
        [GET("Reports2/PastAttendee/{id}")]
        public ActionResult PastAttendee(int? id)
        {
            if (!id.HasValue)
                return Content("no orgid");
            return new PastAttendeeResult(orgid: id);
        }
        [GET("Reports2/Registration/{id}")]
        public ActionResult Registration(Guid? id, int? oid)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RegistrationResult(id, oid);
        }
        [GET("Reports2/ChurchAttendance/{id}")]
        public ActionResult ChurchAttendance(string id)
        {
            var dt2 = id.ToDate();
            if (!dt2.HasValue)
                dt2 = ChurchAttendanceModel.MostRecentAttendedSunday();
            var m = new ChurchAttendanceModel(dt2.Value);
            return View(m);
        }
        [GET("Reports2/WeeklyDecisions/{id}")]
        public ActionResult WeeklyDecisions(string id)
        {
            var dt = id.ToDate();
            var m = new WeeklyDecisionsModel(dt);
            return View(m);
        }
        [POST("Reports2/Decisions")]
        public ActionResult Decisions()
        {
            DbUtil.LogActivity("Viewing Decision Summary Rpt");
            var today = Util.Now.Date;
            var dt1 = new DateTime(today.Year, 1, 1);
            var dt2 = today;
            var m = new DecisionSummaryModel(dt1, dt2);
            return View(m);
        }
        [POST("Reports2/Decisions/{dt1}/{dt2}")]
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

        [POST("Reports2/DecisionsToQuery/{command}/{key}")]
        public ActionResult DecisionsToQuery(string command, string key, DateTime? dt1, DateTime? dt2)
        {
            var r = new DecisionSummaryModel(dt1, dt2).QueryBuider(command, key);
            return Redirect(r);
        }

        [GET("Reports2/ChurchAttendance2/{dt1:datetime?}/{dt2:datetime?}")]
        public ActionResult ChurchAttendance2(DateTime? Dt1, DateTime? Dt2, string skipweeks)
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
        [GET("Reports2/AttendanceDetail/{dt1}/{dt2}")]
        public ActionResult AttendanceDetail(DateTime? Dt1, DateTime? Dt2, OrgSearchModel m)
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
        [GET("Reports2/RecentAbsents/{id}")]
        public ActionResult RecentAbsents1(Guid id)
        {
            int? divid = null;
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP", new { orgid = id, divid = divid, days = 36 },
                            commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View("RecentAbsents", q);
        }

        [POST("Reports2/RecentAbsents/{id}")]
        public ActionResult RecentAbsents(Guid id, OrgSearchModel m)
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

        [POST("Reports2/Meetings")]
        public ActionResult Meetings(MeetingsModel m)
        {
            if (!m.FromWeekAtAGlance)
                m.Dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!m.Dt2.HasValue)
                m.Dt2 = m.Dt1.Value.AddDays(1);
            return View(m);
        }
        [GET("Reports2/QueryStats")]
        public ActionResult QueryStats()
        {
            return new QueryStatsResult();
        }
        [GET("Reports2/VitalStats")]
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
        [GET("Reports2/ExtraValues")]
        public ActionResult ExtraValues()
        {
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
        [GET("Reports2/ExtraValueData")]
        public ActionResult ExtraValueData()
        {
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
        [GET("Reports2/ExtraValuesGrid/{id}")]
        [GET("Reports2/ExtraValuesGrid/{id}/{sort}")]
        public ActionResult ExtraValuesGrid(Guid id, string sort)
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
            return View(rdr);
        }
        [GET("Reports2/ExtraValuesGrid2/{id}")]
        [GET("Reports2/ExtraValuesGrid2/{id}/{sort}")]
        public ActionResult ExtraValuesGrid2(Guid id, string sort)
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
            return View(rdr);
        }
        [GET("Reports2/FamilyDirectory/{id}")]
        public ActionResult FamilyDirectory(Guid id)
        {
            return new FamilyDir(id);
        }
        [GET("Reports2/FamilyDirectoryCompact/{id}")]
        public ActionResult FamilyDirectoryCompact(Guid id)
        {
            return new CompactDir(id);
        }
        [GET("Reports2/PictureDirectory/{id}")]
        public ActionResult PictureDirectory(Guid id)
        {
            return new PictureDir(id);
        }
        [GET("Reports2/EmployerAddress/{id}")]
        public ActionResult EmployerAddress(Guid id)
        {
            return new EmployerAddress(id, true);
        }

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
        [POST("Reports2/EnrollmentControl")]
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
        [POST("Reports2/CheckinControl")]
        public ActionResult CheckinControl(CheckinControlModel m)
        {
            return new CheckinControlResult { model = m };
        }
        [POST("Reports2/EnrollmentControl2")]
        public ActionResult EnrollmentControl2(EnrollmentControlModel m)
        {
            return View(m);
        }
    }
}
