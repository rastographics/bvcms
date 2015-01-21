using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsData;
using CmsData.View;
using CmsWeb.Areas.Main.Models.Avery;
using CmsWeb.Areas.Main.Models.Directories;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Areas.Reports.ViewModels;
using CmsWeb.Models;
using Dapper;
using MoreLinq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using UtilityExtensions;
using UtilityExtensions.Extensions;
using FamilyResult = CmsWeb.Areas.Reports.Models.FamilyResult;
using MeetingsModel = CmsWeb.Areas.Reports.Models.MeetingsModel;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaPrefix = "Reports"), Route("{action}/{id?}")]
    public class ReportsController : CmsStaffController
    {
        [HttpGet]
        public ActionResult Attendance(int id, AttendanceModel m)
        {
            if (m.OrgId == 0)
                m = new AttendanceModel() { OrgId = id };
            return View(m);
        }
        [HttpPost]
        public ActionResult Attendance(AttendanceModel m)
        {
            return View(m);
        }
        [HttpGet]
        public ActionResult Attendee(int id)
        {
            return new AttendeeResult(id);
        }

        [HttpPost]
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

        [HttpGet]
        public ActionResult Avery(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult { id = id.Value };
        }

        [HttpGet]
        public ActionResult Avery3(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new Avery3Result { id = id.Value };
        }

        [HttpGet]
        public ActionResult AveryAddress(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, bool? useMailFlags, int skipNum = 0)
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
                sortzip = sortzip,
                useMailFlags = useMailFlags,
            };
        }
        [HttpGet]
        public ActionResult AveryAddressWord(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, bool? useMailFlags, int skipNum = 0)
        {
            if (!id.HasValue)
                return Content("no query");
            if (!format.HasValue())
                return Content("no format");
            return new AveryAddressWordResult
            {
                id = id.Value,
                format = format,
                titles = titles,
                usephone = usephone ?? false,
                skip = skipNum,
                sortzip = sortzip,
                useMailFlags = useMailFlags,
            };
        }

        [HttpGet]
        public ActionResult BarCodeLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new BarCodeLabelsResult(id.Value);
        }

        [HttpPost]
        public ActionResult CheckinControl(CheckinControlModel m)
        {
            return new CheckinControlResult { model = m };
        }

        [HttpGet, Route("ChurchAttendance/{dt:datetime?}")]
        public ActionResult ChurchAttendance(DateTime? dt)
        {
            if (!dt.HasValue)
                dt = ChurchAttendanceModel.MostRecentAttendedSunday();
            var m = new ChurchAttendanceModel(dt.Value);
            return View(m);
        }

        [HttpGet]
        public ActionResult ChurchAttendance2(DateTime? dt1, DateTime? dt2, string skipweeks)
        {
            if (!dt1.HasValue)
                dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!dt2.HasValue)
                dt2 = DateTime.Today;
            var m = new ChurchAttendance2Model(dt1, dt2, skipweeks);
            return View(m);
        }

        [HttpPost]
        public ActionResult ClassList(string org, OrgSearchModel m)
        {
            return new ClassListResult(m) { orgid = org == "curr" ? Util2.CurrentOrgId : null };
        }

        [HttpGet]
        public ActionResult CompactPictureDirectory(Guid id)
        {
            var s = DbUtil.Db.ContentText("CompactDirectoryParameters", Resource1.CompactDirectoryParameters);
            return new CompactPictureDir(id, s);
        }

        [HttpGet]
        public ActionResult CompactPictureDirectory2(Guid id)
        {
            var s = DbUtil.Db.ContentText("CompactDirectoryParameters2", Resource1.CompactDirectoryParameters2);
            return new CompactPictureDir(id, s);
        }

        [HttpGet]
        public ActionResult FamilyPictureDirectory(Guid id)
        {
            var s = DbUtil.Db.ContentText("CompactDirectoryParameters2", Resource1.CompactDirectoryParameters2);
            return new FamilyPictureDir(id);
        }

        [HttpGet]
        public ActionResult Contacts(Guid? id, bool? sortAddress, string orgname)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ContactsResult(id.Value, sortAddress, orgname);
        }

        [HttpGet]
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

        [HttpGet, Route("DecisionsToQuery/{command}/{key}")]
        public ActionResult DecisionsToQuery(string command, string key, int? campus, DateTime? dt1, DateTime? dt2)
        {
            string r = new DecisionSummaryModel(dt1, dt2) { Campus = campus }.ConvertToSearch(command, key);
            return Redirect(r);
        }

        [HttpGet]
        public ActionResult EmployerAddress(Guid id)
        {
            return new EmployerAddress(id, true);
        }

        [HttpPost]
        public ActionResult EnrollmentControl(bool? excel, bool? usecurrenttag, OrgSearchModel m)
        {
            if (excel != true)
                return new EnrollmentControlResult { OrgSearch = m, UseCurrentTag = usecurrenttag ?? false };

            var d = (from p in EnrollmentControlModel.List(m, usecurrenttag: usecurrenttag ?? false)
                     orderby p.Name
                     select p).ToDataTable();
            return d.ToExcel("EnrollmentControl.xlsx");
        }

        [HttpPost]
        public ActionResult EnrollmentControl2(OrgSearchModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult EnrollmentControl2a(OrgSearchModel m)
        {
            return View(m);
        }
        [HttpGet, ValidateInput(false)]
        public ActionResult EnrollmentControl2a(string json)
        {
            var m = OrgSearchModel.DecodedJson(json);
            if (m == null)
                return RedirectShowError("must start with orgsearch");
            return View(m);
        }
        [HttpGet, Route("EnrollmentControl2b/{na}"), ValidateInput(false)]
        public ActionResult EnrollmentControl2b(string na, string j)
        {
            var m = OrgSearchModel.DecodedJson(j);
            return View(EnrollmentControlModel.List(m, na));
        }

        [HttpGet]
        public ActionResult ExtraValueData()
        {
            return Redirect("/ExtraValue/Summary");
        }

        [HttpGet]
        public ActionResult ExtraValues()
        {
            return Redirect("/ExtraValue/Summary");
        }

        [HttpGet]
        public ActionResult ExtraValuesGrid2(Guid id, string sort, bool alternate = false)
        {
            return RunExtraValuesGrid(id, sort, alternate);
        }

        [HttpGet]
        public ActionResult Family(Guid id)
        {
            return new FamilyResult(id);
        }

        [HttpGet]
        public ActionResult FamilyDirectory(Guid id)
        {
            return new FamilyDir(id);
        }

        [HttpGet]
        public ActionResult FamilyDirectoryCompact(Guid id)
        {
            return new CompactDir(id);
        }

        [HttpGet]
        public ActionResult Meetings(DateTime dt1, DateTime dt2, int? programid, int? divisionid)
        {
            MeetingsModel m = new MeetingsModel() {Dt1 = dt1, Dt2 = dt2, ProgramId = programid, DivisionId = divisionid}; 
            return View(m);
        }
        [HttpPost]
        public ActionResult Meetings(MeetingsModel m)
        {
            if (!m.Dt1.HasValue)
                m.Dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            if (!m.Dt2.HasValue)
                m.Dt2 = m.Dt1.Value.AddDays(1);
            return View(m);
        }

        [HttpPost]
        public ActionResult MeetingsForMonth(DateTime dt1, OrgSearchModel m)
        {
            var orgs = string.Join(",", m.FetchOrgs().Select(oo => oo.OrganizationId));
            ViewBag.Month = dt1.ToString("MMMM yyyy");
            dt1 = new DateTime(dt1.Year, dt1.Month, 1);
            var dt2 = dt1.AddMonths(1).AddDays(-1);
            var hasmeetings = DbUtil.Db.MeetingsDataForDateRange(orgs, dt1, dt2).AsEnumerable().Any();
            if (!hasmeetings)
                return RedirectShowError("No meetings to show");

            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("MeetingsForDateRange", new
            {
                orgs,
                startdate = dt1,
                enddate = dt2,
            }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View(q);
        }
        [HttpPost, Route("MeetingsToQuery/{type}")]
        public ActionResult MeetingsToQuery(string type, MeetingsModel m)
        {
            string r = m.ConvertToSearch(type);
            TempData["autorun"] = true;
            return Redirect(r);
        }
        [HttpGet, Route("MissionTripFunding/{orgid:int}")]
        public ActionResult MissionTripFunding(int orgid)
        {
            return View(MissionTripFundingModel.List(orgid));
        }
        [HttpPost]
        public ActionResult MissionTripFunding(OrgSearchModel m)
        {
            return View(MissionTripFundingModel.List(m));
        }
        [HttpGet, Route("MissionTripSenders/{orgid:int}")]
        public ActionResult MissionTripSenders(int orgid)
        {
            return MissionTripSendersModel.List(orgid);
        }

        [HttpPost]
        public ActionResult MissionTripSenders(OrgSearchModel m)
        {
            return MissionTripSendersModel.List(m);
        }

        [HttpGet]
        public ActionResult NameLabels(Guid? id)
        {
            if (!id.HasValue)
                return Content("no query");
            return new AveryResult { namesonly = true, id = id.Value };
        }

        [HttpPost]
        public ActionResult OrgLeaders(string org, OrgSearchModel m)
        {
            return new OrgLeadersResult(m) { orgid = org == "curr" ? Util2.CurrentOrgId : null };
        }

        [HttpGet]
        public ActionResult PastAttendee(int? id)
        {
            if (!id.HasValue)
                return Content("no orgid");
            return new PastAttendeeResult(id);
        }

        [HttpGet]
        public ActionResult PictureDirectory(Guid id)
        {
            return new PictureDir(id);
        }

        [HttpGet]
        public ActionResult Prospect(Guid? id, bool? Form, bool? Alpha)
        {
            if (!id.HasValue)
                return Content("no query");
            return new ProspectResult(id.Value, Form ?? false, Alpha ?? false);
        }

        [HttpGet]
        public ActionResult QueryStats()
        {
            return new QueryStatsResult();
        }

        public ActionResult RallyRollsheet(string org, string dt, int? meetingid, int? bygroup, string sgprefix,
            bool? altnames, string highlight, OrgSearchModel m)
        {
            DateTime? dt2 = dt.ToDate();

            return new RallyRollsheetResult
            {
                orgid = org == "curr" ? Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new[] { 0 },
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                Model = m
            };
        }

        [HttpPost]
        public ActionResult RecentAbsents(OrgSearchModel m)
        {
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP2", new
            {
                name = m.Name,
                prog = m.ProgramId,
                div = m.DivisionId,
                campus = m.CampusId,
                sched = m.ScheduleId,
                status = m.StatusId,
                onlinereg = m.OnlineReg,
                orgtypeid = m.TypeId
            }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View(q);
        }

        [HttpGet, Route("RecentAbsents1/{id}/{idfilter?}")]
        public ActionResult RecentAbsents1(int id, int? idfilter)
        {
            var m = new RecentAbsentsViewModel(id, idfilter);
            return View(m);
        }

        [HttpGet]
        public ActionResult Registration(Guid? id, int? oid)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RegistrationResult(id, oid);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult RegistrationSummary(int? days, string sort)
        {
            IQueryable<RecentRegistration> q = DbUtil.Db.RecentRegistrations(days ?? 90);
            q = sort == "Organization"
                ? q.OrderBy(rr => rr.OrganizationName).ThenByDescending(rr => rr.Completed)
                : q.OrderByDescending(rr => rr.Dt2);
            return View(q);
        }

        [HttpGet]
        public ActionResult RollLabels(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, bool? useMailFlags)
        {
            if (!id.HasValue)
                return Content("no query");
            return new RollLabelsResult
            {
                qid = id.Value,
                format = format,
                titles = titles ?? false,
                usephone = usephone ?? false,
                sortzip = sortzip,
                useMailFlags = useMailFlags ?? false
            };
        }

        [HttpGet]
        [Route("Rollsheet")]
        [Route("Rollsheet/{meetingid:int}")]
        public ActionResult Rollsheet(int? meetingid, string org, string dt, int? bygroup, string sgprefix,
            bool? altnames, string highlight)
        {
            DateTime? dt2 = dt.ToDate();
            return new RollsheetResult
            {
                orgid = org == "curr" ? Util2.CurrentOrgId : null,
                groups = org == "curr" ? Util2.CurrentGroups : new[] { 0 },
                meetingid = meetingid,
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                highlightsg = highlight,
            };
        }
        [HttpPost]
        public ActionResult Rollsheet(string dt, int? bygroup, string sgprefix,
            bool? altnames, string highlight, OrgSearchModel m)
        {
            DateTime? dt2 = dt.ToDate();
            return new RollsheetResult
            {
                groups = new[] { 0 },
                bygroup = bygroup.HasValue,
                sgprefix = sgprefix,
                dt = dt2,
                altnames = altnames,
                highlightsg = highlight,
                Model = m
            };
        }

        [HttpGet]
        public ActionResult Roster(Guid id, int? oid)
        {
            return new RosterListResult
            {
                qid = id,
                orgid = oid,
            };
        }

        [HttpPost]
        public ActionResult Roster(OrgSearchModel m)
        {
            return new RosterListResult(m);
        }

        [HttpGet]
        public ActionResult Roster1(Guid id, int? oid)
        {
            return new RosterResult
            {
                qid = id,
                org = oid,
            };
        }

        [HttpPost]
        public ActionResult Roster1(OrgSearchModel m)
        {
            return new RosterResult(m);
        }

        private ActionResult RunExtraValuesGrid(Guid id, string sort, bool alternate)
        {
            var roles = CMSRoleProvider.provider.GetRolesForUser(Util.UserName);
            var xml = XDocument.Parse(DbUtil.Db.Content("StandardExtraValues2", "<Fields/>"));
            var fields = (from ff in xml.Root.Descendants("Value")
                          let vroles = ff.Attribute("VisibilityRoles")
                          where vroles != null && (vroles.Value.Split(',').All(rr => !roles.Contains(rr)))
                          select ff.Attribute("Name").Value);
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
            if (alternate)
                return View("ExtraValuesGrid2", rdr);
            return View("ExtraValuesGrid", rdr);
        }

        [HttpGet]
        [Route("Custom/{report}/{id?}")]
        public ActionResult CustomReport(Guid id, string report)
        {
            var m = new CustomReportsModel(DbUtil.Db);
            return m.Result(id, report);
        }

        [HttpGet]
        [Route("CustomSql/{report}/{id?}")]
        public ActionResult CustomReportSql(Guid id, string report)
        {
            try
            {
                var m = new CustomReportsModel(DbUtil.Db);
                return Content("<pre style='font-family:monospace'>{0}\n</pre>".Fmt(
                    m.Sql(id, report)));
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
        }

        [HttpGet]
        [Route("CustomColumns/{orgid?}")]
        public ActionResult CustomColumns(int? orgid)
        {
            try
            {
                Response.ContentType = "text/plain";
                var m = new CustomReportsModel(DbUtil.Db, orgid);
                var s = m.StandardColumns(includeRoot: false).ToString();
                foreach (var line in s.SplitLines())
                    Response.Write("  {0}\n".Fmt(line));
                Response.Write("\n");
                return new EmptyResult();
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult SGMap(OrgSearchModel m)
        {
            return Redirect("/Sgmap/Index/" + m.DivisionId);
        }

        [HttpPost]
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

        [HttpGet]
        public ActionResult VisitsAbsents(int? id)
        {
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult(id);
        }

        [HttpGet]
        public ActionResult VisitsAbsents2(int? id)
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult2(id);
        }

        [HttpGet]
        public ActionResult VitalStats()
        {
            ViewData["table"] = QueryFunctions.VitalStats(DbUtil.Db);
            return View();
        }

        [HttpPost]
        public ActionResult WeeklyAttendance(WeeklyAttendanceModel m)
        {
            var q = m.Attendances();
            var cols = typeof(WeeklyAttendanceModel.AttendInfo).GetProperties();
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

        [HttpGet]
        public ActionResult WeeklyAttendance(Guid id)
        {
            return new WeeklyAttendanceResult(id);
        }

        [HttpGet]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCustomReport(int? orgId, string reportName)
        {
            CustomReportViewModel originalReportViewModel = null;
            if (TempData[TempDataModelStateKey] != null)
            {
                ModelState.Merge((ModelStateDictionary) TempData[TempDataModelStateKey]);
                originalReportViewModel = TempData[TempDataCustomReportKey] as CustomReportViewModel;
            }

            var m = new CustomReportsModel(DbUtil.Db, orgId);

            if (string.IsNullOrEmpty(reportName))
                return View(new CustomReportViewModel(orgId, GetAllStandardColumns(m)));

            var reportXml = m.GetReportByName(reportName);

            if (reportXml == null)
                throw new Exception("Report not found.");

            var columns = MapXmlToCustomReportColumn(reportXml);

            var model = new CustomReportViewModel(orgId, GetAllStandardColumns(m), reportName);
            model.QueryId = DbUtil.Db.QueryInCurrentOrg().QueryId;

            var showOnOrgIdValue = reportXml.AttributeOrNull("showOnOrgId");
            int showOnOrgId;
            if (!string.IsNullOrEmpty(showOnOrgIdValue) && int.TryParse(showOnOrgIdValue, out showOnOrgId))
                model.RestrictToThisOrg = showOnOrgId == orgId;

            model.SetSelectedColumns(columns);

            if (originalReportViewModel != null)
                model.ReportName = originalReportViewModel.ReportName;

            var alreadySaved = TempData[TempDataSuccessfulSaved] as bool?;
            model.CustomReportSuccessfullySaved = alreadySaved.GetValueOrDefault();

            var runReport = TempData[TempDataRunReportAfterSaving] as bool?;
            model.RunReportOnLoad = runReport.GetValueOrDefault();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult EditCustomReport(CustomReportViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.ReportName))
                ModelState.AddModelError("ReportName", "The report name is required.");

            if (!ModelState.IsValid)
            {
                TempData[TempDataModelStateKey] = ModelState;
                TempData[TempDataCustomReportKey] = viewModel;
                return RedirectToAction("EditCustomReport", new { reportName = viewModel.OriginalReportName, orgId = viewModel.OrgId });
            }

            var m = new CustomReportsModel(DbUtil.Db, viewModel.OrgId);
            m.SaveReport(viewModel.OriginalReportName, viewModel.ReportName, viewModel.Columns.Where(c => c.IsSelected), viewModel.RestrictToThisOrg);

            TempData[TempDataSuccessfulSaved] = true;

            if (!string.IsNullOrEmpty(Request.Form["run-report"]))
                TempData[TempDataRunReportAfterSaving] = true;

            return RedirectToAction("EditCustomReport", new { reportName = viewModel.ReportName, orgId = viewModel.OrgId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult DeleteCustomReport(int? orgId, string reportName)
        {
            if (string.IsNullOrEmpty(reportName))
                return new JsonResult {Data = "Report name is required."};

            var m = new CustomReportsModel(DbUtil.Db, orgId);
            m.DeleteReport(reportName);

            return new JsonResult {Data = "success"};
        }

        private static IEnumerable<CustomReportColumn> GetAllStandardColumns(CustomReportsModel model)
        {
            var reportXml = model.StandardColumns();
            return MapXmlToCustomReportColumn(reportXml);
        }

        private static IEnumerable<CustomReportColumn> MapXmlToCustomReportColumn(XContainer reportXml)
        {
            return from column in reportXml.Descendants("Column")
                   select new CustomReportColumn
                   {
                       Name = column.AttributeOrNull("name"),
                       Description = column.AttributeOrNull("description"),
                       Flag = column.AttributeOrNull("flag"),
                       OrgId = column.AttributeOrNull("orgid"),
                       SmallGroup = column.AttributeOrNull("smallgroup"),
                       Field = column.AttributeOrNull("field"),
                       IsDisabled = column.AttributeOrNull("disabled").ToBool(),
                   };
        }

        private const string TempDataModelStateKey = "ModelState";
        private const string TempDataCustomReportKey = "InvalidCustomReportViewModel";
        private const string TempDataSuccessfulSaved = "CustomReportSuccessfullySaved";
        private const string TempDataRunReportAfterSaving = "TempDataRunReportAfterSaving";
    }
}