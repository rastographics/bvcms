using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Xml.Linq;
using CmsWeb.Areas.Dialog.Models;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Main.Models.Avery;
using CmsWeb.Areas.Main.Models.Directories;
using CmsWeb.Areas.Manage.Controllers;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Areas.Reports.ViewModels;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Models;
using Dapper;
using HtmlAgilityPack;
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
        [Authorize(Roles = "MembershipApp,Admin")]
        [HttpGet, Route("Application/{orgid:int}/{peopleid:int}/{content}")]
        public ActionResult Application(int orgid, int peopleid, string content)
        {
#if DEBUG2
            var c = System.IO.File.ReadAllText(Server.MapPath("/Application.html"));
            var replacements = new EmailReplacements(DbUtil.Db, c, null);
#else
            var c = DbUtil.Db.Content(content);
            if (c == null)
                return Message("no content at " + content);
            var replacements = new EmailReplacements(DbUtil.Db, c.Body, null);
#endif
            var p = DbUtil.Db.LoadPersonById(peopleid);
            DbUtil.Db.SetCurrentOrgId(orgid);
            ViewBag.html = replacements.DoReplacements(DbUtil.Db, p);
            return View();
        }

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
            var dt1 = Dt1.ToDate();
            if (!dt1.HasValue)
                dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            var dt2 = Dt2.ToDate();
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
            if (m.CheckinExport)
                return m.list().ToDataTable().ToExcel("CheckinControl.xlsx");
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
            return new ClassListResult(m) { orgid = org == "curr" ? DbUtil.Db.CurrentOrg.Id : null };
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
            var r = new DecisionSummaryModel(dt1, dt2) { Campus = campus }.ConvertToSearch(command, key);
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
            var json = m.EncodedJson();
            var mm = OrgSearchModel.DecodedJson(json);
            return RedirectToAction("EnrollmentControl2a", new { json });
        }

        [HttpGet, ValidateInput(false)]
        public ActionResult EnrollmentControl2a(string json)
        {
            var m = OrgSearchModel.DecodedJson(json);
            ViewBag.json = json;
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
            var m = new MeetingsModel { Dt1 = dt1, Dt2 = dt2, ProgramId = programid, DivisionId = divisionid };
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
            var r = m.ConvertToSearch(type);
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
            return new OrgLeadersResult(m) { orgid = org == "curr" ? DbUtil.Db.CurrentOrg.Id : null };
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

        [HttpPost, Route("RallyRollsheetForOrg/{orgid:int}")]
        public ActionResult RallyRollsheetForOrg(int orgid, NewMeetingInfo mi)
        {
            return new RallyRollsheetResult { orgid = orgid, NewMeetingInfo = mi };
        }

        [HttpGet, Route("RallyRollsheetForMeeting/{meetingid:int}")]
        public ActionResult RallyRollsheetForMeeting(int meetingid)
        {
            return new RallyRollsheetResult { meetingid = meetingid };
        }

        [HttpPost]
        public ActionResult RallyRollsheets(NewMeetingInfo mi, OrgSearchModel m)
        {
            return new RallyRollsheetResult
            {
                OrgSearchModel = m,
                NewMeetingInfo = mi
            };
        }

        [HttpPost]
        public ActionResult RecentAbsents(OrgSearchModel m)
        {
            var orgs = string.Join(",", m.FetchOrgs().Select(oo => oo.OrganizationId));
            var cn = new SqlConnection(Util.ConnectionString);
            cn.Open();
            var q = cn.Query("RecentAbsentsSP3", new { orgs = orgs }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
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

        [HttpGet]
        public ActionResult RegistrationExcel(Guid? id, int? oid)
        {
            if (!id.HasValue)
                return Content("no query");

            var peopleQuery = DbUtil.Db.PeopleQuery(id.Value);
            if (!oid.HasValue)
                oid = DbUtil.Db.CurrentOrgId;
            var results = (from p in peopleQuery
                           let rr = p.RecRegs.SingleOrDefault() ?? new RecReg()
                           let headOfHousehold = p.Family.HeadOfHousehold
                           let headOfHouseholdSpouse = p.Family.HeadOfHouseholdSpouse
                           orderby p.Name2
                           select new
                           {
                               Person = p,
                               RecReg = rr,
                               HeadOfHousehold = headOfHousehold,
                               HeadOfHouseholdSpouse = headOfHouseholdSpouse,
                               OrgMembers = p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == oid),
                               p.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == oid).Organization,
                           }).ToList();

            if (!results.Any())
                return Content("no results");

            var table = new DataTable();

            foreach (var x in results)
            {
                Settings setting = null;
                if (x.Organization != null)
                    setting = DbUtil.Db.CreateRegistrationSettings(x.Organization.OrganizationId);

                var row = table.NewRow();

                AddValue(table, row, "Name", x.Person.Name);
                AddValue(table, row, "PrimaryAddress", x.Person.PrimaryAddress);
                AddValue(table, row, "PrimaryAddress2", x.Person.PrimaryAddress2);
                AddValue(table, row, "CityStateZip", x.Person.CityStateZip);
                AddValue(table, row, "EmailAddress", x.Person.EmailAddress);

                if (x.Person.HomePhone.HasValue())
                    AddValue(table, row, "HomePhone", x.Person.HomePhone.FmtFone("H"));
                if (x.Person.CellPhone.HasValue())
                    AddValue(table, row, "CellPhone", x.Person.CellPhone.FmtFone("C"));

                AddValue(table, row, "DOB", x.Person.DOB);

                AddValue(table, row, "HeadOfHouseholdName", x.HeadOfHousehold?.Name);
                if (!string.IsNullOrEmpty(x.HeadOfHousehold?.CellPhone))
                    AddValue(table, row, "HeadOfHouseholdCellPhone", x.HeadOfHousehold?.CellPhone.FmtFone("C"));
                if (!string.IsNullOrEmpty(x.HeadOfHousehold?.HomePhone))
                    AddValue(table, row, "HeadOfHouseholdHomePhone", x.HeadOfHousehold?.HomePhone.FmtFone("H"));

                AddValue(table, row, "HeadOfHouseholdSpouseName", x.HeadOfHouseholdSpouse?.Name);
                if (!string.IsNullOrEmpty(x.HeadOfHouseholdSpouse?.CellPhone))
                    AddValue(table, row, "HeadOfHouseholdSpouseCellPhone", x.HeadOfHouseholdSpouse?.CellPhone.FmtFone("C"));
                if (!string.IsNullOrEmpty(x.HeadOfHouseholdSpouse?.HomePhone))
                    AddValue(table, row, "HeadOfHouseholdSpouseHomePhone", x.HeadOfHouseholdSpouse?.HomePhone.FmtFone("H"));

                if (x.Organization == null || SettingVisible(setting, "AskSize"))
                    AddValue(table, row, "ShirtSize", x.RecReg.ShirtSize);

                if (x.Organization == null || SettingVisible(setting, "AskRequest"))
                    AddValue(table, row, ((AskRequest)setting.AskItem("AskRequest")).Label, x.OrgMembers?.Request);


                AddValue(table, row, "MedicalDescription", x.RecReg.MedicalDescription);

                if (x.Organization == null || SettingVisible(setting, "AskTylenolEtc"))
                {
                    AddValue(table, row, "Tylenol", x.RecReg.Tylenol);
                    AddValue(table, row, "Advil", x.RecReg.Advil);
                    AddValue(table, row, "Robitussin", x.RecReg.Robitussin);
                    AddValue(table, row, "Maalox", x.RecReg.Maalox);
                }

                if (x.Organization == null || SettingVisible(setting, "AskEmContact"))
                {
                    AddValue(table, row, "Emcontact", x.RecReg.Emcontact);
                    AddValue(table, row, "Emphone", x.RecReg.Emphone.FmtFone());
                }

                if (x.Organization == null || SettingVisible(setting, "AskInsurance"))
                {
                    AddValue(table, row, "Insurance", x.RecReg.Insurance);
                    AddValue(table, row, "Policy", x.RecReg.Policy);
                }

                if (x.Organization == null || SettingVisible(setting, "AskDoctor"))
                {
                    AddValue(table, row, "Doctor", x.RecReg.Doctor);
                    AddValue(table, row, "Docphone", x.RecReg.Docphone.FmtFone());
                }

                if (x.Organization == null || SettingVisible(setting, "AskParents"))
                {
                    AddValue(table, row, "Mname", x.RecReg.Mname);
                    AddValue(table, row, "Fname", x.RecReg.Fname);
                }

                if (x.OrgMembers?.OnlineRegData != null)
                {
                    var qlist = from qu in DbUtil.Db.ViewOnlineRegQAs
                                where qu.OrganizationId == x.OrgMembers.OrganizationId
                                where qu.Type == "question" || qu.Type == "text"
                                where qu.PeopleId == x.OrgMembers.PeopleId
                                select qu;
                    var counter = 0;
                    foreach (var qu in qlist)
                    {
                        AddValue(table, row, $"Question{counter}", qu.Question);
                        AddValue(table, row, $"Answer{counter}", qu.Answer);
                        counter++;
                    }

                    if (x.OrgMembers?.UserData != null)
                    {
                        var a = Regex.Split(x.OrgMembers.UserData, @"\s*--Add comments above this line--\s*", RegexOptions.Multiline);
                        if (a.Length > 0)
                        {
                            AddValue(table, row, "Comments", a[0]);
                        }
                    }

                    if (x.OrgMembers != null)
                    {
                        var groups = string.Join(", ", x.OrgMembers.OrgMemMemTags.Select(om => om.MemberTag.Name).ToArray());
                        AddValue(table, row, "Groups", groups);
                    }
                }

                table.Rows.Add(row);
            }

            return table.ToExcel(filename: "Registrations.xlsx");
        }

        private static void AddValue(DataTable table, DataRow row, string columnName, object value)
        {
            if (!table.Columns.Contains(columnName))
                table.Columns.Add(columnName);

            row[columnName] = value;
        }

        private static bool SettingVisible(Settings setting, string name)
        {
            if (setting != null)
                return setting.AskVisible(name);
            return false;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult RegistrationSummary(int? days, string sort)
        {
            var q = DbUtil.Db.RecentRegistrations(days ?? 90);
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

        [HttpPost]
        public ActionResult Rollsheet(string dt, int? bygroup, string sgprefix,
            bool? altnames, string highlight, OrgSearchModel m)
        {
            DateTime? dt2 = dt.ToDate();
            if (!dt2.HasValue)
                return Message("no date");
            var mi = new NewMeetingInfo()
            {
                ByGroup = bygroup > 0,
                GroupFilterPrefix = sgprefix,
                UseAltNames = altnames == true,
                HighlightGroup = highlight,
                MeetingDate = dt2.Value
            };
            return new RollsheetResult
            {
                OrgSearchModel = m,
                NewMeetingInfo = mi
            };
        }

        [HttpPost, Route("RollsheetForOrg/{orgid:int?}")]
        public ActionResult RollsheetForOrg(int? orgid, NewMeetingInfo mi)
        {
            return new RollsheetResult
            {
                orgid = orgid,
                NewMeetingInfo = mi,
            };
        }

        [HttpGet, Route("RollsheetForMeeting/{meetingid:int}")]
        public ActionResult RollsheetForMeeting(int meetingid)
        {
            return new RollsheetResult { meetingid = meetingid };
        }

        [HttpPost]
        public ActionResult Rollsheets(NewMeetingInfo mi, OrgSearchModel m)
        {
            return new RollsheetResult
            {
                OrgSearchModel = m,
                NewMeetingInfo = mi
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
                return Content($"<pre style='font-family:monospace'>{m.Sql(id, report)}\n</pre>");
            }
            catch (Exception ex)
            {
                return Message(ex.Message);
            }
        }

        [HttpGet]
        [Route("SqlReport/{report}/{id?}")]
        public ActionResult SqlReport(Guid id, string report)
        {
            var content = DbUtil.Db.ContentOfTypeSql(report);
            if (content == null)
                return Message("no content");
            if (!CanRunScript(content.Body))
                return Message("Not Authorized to run this script");
            if (!content.Body.Contains("@qtagid"))
                return Message("missing @qtagid");

            var tag = DbUtil.Db.PopulateSpecialTag(id, DbUtil.TagTypeId_Query);

            var cs = User.IsInRole("Finance")
                ? Util.ConnectionStringReadOnlyFinance
                : Util.ConnectionStringReadOnly;
            using (var cn = new SqlConnection(cs))
            {
                cn.Open();
                var p = new DynamicParameters();
                p.Add("@qtagid", tag.Id);
                ViewBag.name = report;
                using (var rd = cn.ExecuteReader(content.Body, p))
                    ViewBag.report = GridResult.Table(rd);
                return View();
            }
        }



        /// <summary>
        /// PyScript ActionResult to handle a Python Custom Script being called as a Custom Report 
        /// from the Blue Toolbar.  
        /// The Function also verifies that the Python script contains the Query Function call to 
        /// "BlueToolbarReport" which in turn returns the first 1000 people records in the BlueToolbar context. 
        /// The Python script is then rendered and the output is sent to the View PyScript.cshtml
        /// </summary>
        [HttpGet]
        [Route("PyScript/{report}/{id?}")]
        public ActionResult PyScript(Guid id, string report)
        {
            var content = DbUtil.Db.ContentOfTypePythonScript(report);
            if (content == null)
                return Content("no script named " + report);
            if (!CanRunScript(content))
                return Message("Not Authorized to run this script");
            if (!content.Contains("BlueToolbarReport") && !content.Contains("@BlueToolbarTagId"))
                return Content("Missing Call to Query Function 'BlueToolbarReport'");
            if (id == Guid.Empty)
                return Content("Must be run from the BlueToolbar");

            var pe = new PythonModel(Util.Host);

            pe.DictionaryAdd("BlueToolbarGuid", id.ToCode());
            foreach (var key in Request.QueryString.AllKeys)
                pe.DictionaryAdd(key, Request.QueryString[key]);

            pe.RunScript(content);

            return View(pe);

        }
        private bool CanRunScript(string script)
        {
            if (!script.StartsWith("#Roles=") && !script.StartsWith("--Roles"))
                return true;
            var re = new Regex("(--|#)Roles=(?<roles>.*)", RegexOptions.IgnoreCase);
            var roles = re.Match(script).Groups["roles"].Value.Split(',').Select(aa => aa.Trim());
            if (!roles.Any(rr => User.IsInRole(rr)))
                return false;
            return true;
        }

        [HttpPost]
        public ActionResult SGMap(OrgSearchModel m)
        {
            return Redirect("/Sgmap/Index/" + m.DivisionId);
        }

        [HttpPost]
        public ActionResult ShirtSizes(string org, OrgSearchModel m)
        {
            var orgid = org == "curr" ? DbUtil.Db.CurrentOrg.Id : null;
            var orgs = orgid.HasValue
                ? OrgSearchModel.FetchOrgs(orgid.Value)
                : m.FetchOrgs();
            var q = from om in DbUtil.Db.OrganizationMembers
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
        public ActionResult VisitsAbsents2(int? id, string prefix = "")
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
                return Content("no meetingid");
            return new VisitsAbsentsResult2(id.Value, prefix);
        }

        [HttpGet]
        public ActionResult VitalStats()
        {
            var script = DbUtil.Db.ContentOfTypePythonScript("VitalStats");
            if (!script.HasValue())
                script = System.IO.File.ReadAllText(Server.MapPath("/Content/VitalStats.py"));

            ViewBag.table = script.Contains("class VitalStats")
                ? QueryFunctions.OldVitalStats(DbUtil.Db, script)
                : PythonModel.RunScript(DbUtil.Db.Host, script);

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

        public class ImageInfo
        {
            public string Src { get; set; }
            public string File { get; set; }
            public DateTime Dt { get; set; }
            public ImageInfo(string src, string file, DateTime dt)
            {
                Src = src;
                File = file;
                Dt = dt;
            }
        }
        [HttpGet]
        public ActionResult EmailImages()
        {
            var q = from e in DbUtil.Db.EmailQueues
                    where e.Body.Contains("ssl.cf2.rackcdn.com")
                    select e;
            var images = new List<string>();
            foreach (var e in q)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(e.Body);
                var nodes = doc.DocumentNode.SelectNodes("//img[@src]");
                if (nodes == null)
                    continue;
                var snodes = nodes.Select(node => node.Attributes["src"].Value);
                images.AddRange(snodes.Where(img => img.Contains("ssl.cf2.rackcdn.com")));
            }
            var uniqImages = images.Distinct();
            var re = new Regex(@"rackcdn.com/.*?\.(\d+)\.(.*)");
            var dateTimeFormatInfo = new DateTimeFormatInfo();
            var list = new List<ImageInfo>();
            foreach (var img in uniqImages)
            {
                if (img.Contains("ssl.cf2.rackcdn.com"))
                {
                    var dts = re.Match(img).Groups[1].Value;
                    var dt = DateTime.ParseExact(dts, "yyMMddmmss", dateTimeFormatInfo);
                    var file = re.Match(img).Groups[2].Value;
                    list.Add(new ImageInfo(img, file, dt));
                }
            }
            return View(list.OrderByDescending(vv => vv.Dt));
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
        [Authorize(Roles = "Admin, Design")]
        public ActionResult EditCustomReport(int? orgId, string reportName, Guid queryId)
        {
            CustomReportViewModel originalReportViewModel = null;
            if (TempData[TempDataModelStateKey] != null)
            {
                ModelState.Merge((ModelStateDictionary)TempData[TempDataModelStateKey]);
                originalReportViewModel = TempData[TempDataCustomReportKey] as CustomReportViewModel;
            }

            var m = new CustomReportsModel(DbUtil.Db, orgId);

            var orgName = (orgId.HasValue)
                ? DbUtil.Db.Organizations.SingleOrDefault(o => o.OrganizationId == orgId.Value).OrganizationName
                : null;

            CustomReportViewModel model;
            if (string.IsNullOrEmpty(reportName))
            {
                model = new CustomReportViewModel(orgId, queryId, orgName, GetAllStandardColumns(m));
                return View(model);
            }

            model = new CustomReportViewModel(orgId, queryId, orgName, GetAllStandardColumns(m), reportName);

            var reportXml = m.GetReportByName(reportName);
            if (reportXml == null)
                throw new Exception("Report not found.");

            var columns = MapXmlToCustomReportColumn(reportXml);

            var showOnOrgIdValue = reportXml.AttributeOrNull("showOnOrgId");
            int showOnOrgId;
            if (!string.IsNullOrEmpty(showOnOrgIdValue) && int.TryParse(showOnOrgIdValue, out showOnOrgId))
                model.RestrictToThisOrg = showOnOrgId == orgId;

            model.SetSelectedColumns(columns);

            if (originalReportViewModel != null)
                model.ReportName = originalReportViewModel.ReportName;

            var alreadySaved = TempData[TempDataSuccessfulSaved] as bool?;
            model.CustomReportSuccessfullySaved = alreadySaved.GetValueOrDefault();

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Design")]
        public ActionResult EditCustomReport(CustomReportViewModel viewModel)
        {
            if (!viewModel.Columns.Any(c => c.IsSelected))
                ModelState.AddModelError("Columns", "At least one column must be selected.");

            if (ModelState.IsValid)
            {
                viewModel.ReportName = SecurityElement.Escape(viewModel.ReportName.Trim());

                var m = new CustomReportsModel(DbUtil.Db, viewModel.OrgId);
                var result = m.SaveReport(viewModel.OriginalReportName, viewModel.ReportName,
                    viewModel.Columns.Where(c => c.IsSelected), viewModel.RestrictToThisOrg);

                switch (result)
                {
                    case CustomReportsModel.SaveReportStatus.ReportAlreadyExists:
                        ModelState.AddModelError("ReportName", "A report by this name already exists.");
                        break;
                    default:
                        TempData[TempDataSuccessfulSaved] = true;
                        break;
                }
            }

            if (!ModelState.IsValid)
            {
                TempData[TempDataModelStateKey] = ModelState;
                TempData[TempDataCustomReportKey] = viewModel;
                return RedirectToAction("EditCustomReport", new { reportName = viewModel.OriginalReportName, orgId = viewModel.OrgId, queryId = viewModel.QueryId });
            }

            return RedirectToAction("EditCustomReport", new { reportName = viewModel.ReportName, orgId = viewModel.OrgId, queryId = viewModel.QueryId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Design")]
        public JsonResult DeleteCustomReport(int? orgId, string reportName)
        {
            if (string.IsNullOrEmpty(reportName))
                return new JsonResult { Data = "Report name is required." };

            var m = new CustomReportsModel(DbUtil.Db, orgId);
            m.DeleteReport(reportName);

            return new JsonResult { Data = "success" };
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
    }
}
