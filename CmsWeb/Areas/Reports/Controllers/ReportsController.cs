using CmsData;
using CmsWeb.Areas.Dialog.Models;
using CmsWeb.Areas.Main.Models.Avery;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using Dapper;
using HtmlAgilityPack;
using MoreLinq;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;
using FamilyResult = CmsWeb.Areas.Reports.Models.FamilyResult;
using MeetingsModel = CmsWeb.Areas.Reports.Models.MeetingsModel;

namespace CmsWeb.Areas.Reports.Controllers
{
    [RouteArea("Reports", AreaPrefix = "Reports"), Route("{action}/{id?}")]
    public partial class ReportsController : CmsStaffController
    {
        public ReportsController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Authorize(Roles = "MembershipApp,Admin")]
        [HttpGet, Route("Application/{orgid:int}/{peopleid:int}/{content}")]
        public ActionResult Application(int orgid, int peopleid, string content)
        {
#if DEBUG2
            var c = System.IO.File.ReadAllText(Server.MapPath("/Application.html"));
            var replacements = new EmailReplacements(CurrentDatabase, c, null);
#else
            var c = CurrentDatabase.Content(content);
            if (c == null)
            {
                return Message("no content at " + content);
            }

            var replacements = new EmailReplacements(CurrentDatabase, c.Body, null);
#endif
            var p = CurrentDatabase.LoadPersonById(peopleid);
            CurrentDatabase.SetCurrentOrgId(orgid);
            ViewBag.html = replacements.DoReplacements(CurrentDatabase, p);
            return View();
        }

        [HttpGet]
        public ActionResult Attendance(int id, AttendanceModel m)
        {
            if (m.OrgId == 0)
            {
                m = new AttendanceModel() { OrgId = id };
            }

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
        public ActionResult AttendanceDetail(string dt1, string dt2, OrgSearchModel m)
        {
            var d1 = dt1.ToDate();
            if (!d1.HasValue)
            {
                d1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            }

            var d2 = dt2.ToDate();
            if (!d2.HasValue)
            {
                d2 = d1.Value.AddDays(1);
            }

            var m2 = new AttendanceDetailModel(d1.Value, d2, m);
            return View(m2);
        }

        [HttpGet]
        public ActionResult Avery(Guid? id)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            return new AveryResult { id = id.Value };
        }

        [HttpGet]
        public ActionResult Avery3(Guid? id)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            return new Avery3Result { id = id.Value };
        }

        [HttpGet]
        public ActionResult AveryAddress(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, bool? useMailFlags, int skipNum = 0)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            if (!format.HasValue())
            {
                return Content("no format");
            }

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
            {
                return Content("no query");
            }

            if (!format.HasValue())
            {
                return Content("no format");
            }

            return new DocXAveryLabels(id.Value)
            {
                Format = format,
                Titles = titles,
                Skip = skipNum,
                SortZip = sortzip,
                UseMailFlags = useMailFlags,
                UsePhone = usephone,
            };
        }

        [HttpGet]
        public ActionResult BarCodeLabels(Guid? id)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            return new BarCodeLabelsResult(id.Value);
        }

        [HttpPost]
        public ActionResult CheckinControl(CheckinControlModel m)
        {
            if (m.CheckinExport)
            {
                return m.list().ToDataTable().ToExcel("CheckinControl.xlsx");
            }

            return new CheckinControlResult { model = m };
        }

        [HttpGet, Route("ChurchAttendance/{dt?}")]
        public ActionResult ChurchAttendance(string dt)
        {
            var d = dt.ToDate();
            if (!d.HasValue)
            {
                d = ChurchAttendanceModel.MostRecentAttendedSunday();
            }

            var m = new ChurchAttendanceModel(d.Value);
            return View(m);
        }

        [HttpGet]
        public ActionResult ChurchAttendance2(string dt1, string dt2, string skipweeks)
        {
            var d1 = dt1.ToDate();
            var d2 = dt2.ToDate();
            if (!d1.HasValue)
            {
                d1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            }

            if (!d2.HasValue)
            {
                d2 = DateTime.Today;
            }

            var m = new ChurchAttendance2Model(d1, d2, skipweeks);
            return View(m);
        }

        [HttpPost]
        public ActionResult ClassList(OrgSearchModel m)
        {
            return new ClassListResult(m);
        }

        [HttpGet]
        public ActionResult Contacts(Guid? id, bool? sortAddress, string orgname)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            return new ContactsResult(id.Value, sortAddress, orgname);
        }

        [HttpGet]
        public ActionResult Decisions(int? campus, string dt1, string dt2)
        {
            if (Util2.OrgLeadersOnly)
            {
                return Redirect("/Home");
            }

            DateTime today = Util.Now.Date;
            var d1 = dt1.ToDate();
            var d2 = dt2.ToDate();
            if (!d1.HasValue)
            {
                d1 = new DateTime(today.Year, 1, 1);
            }

            if (!d2.HasValue)
            {
                d2 = today;
            }

            var m = new DecisionSummaryModel(d1, d2) { Campus = campus };
            return View(m);
        }

        [HttpGet, Route("DecisionsToQuery/{command}/{key}")]
        public ActionResult DecisionsToQuery(string command, string key, int? campus, string dt1, string dt2)
        {
            if (Util2.OrgLeadersOnly)
            {
                return Redirect("/Home");
            }

            var d1 = dt1.ToDate();
            var d2 = dt2.ToDate();
            var r = new DecisionSummaryModel(d1, d2) { Campus = campus }.ConvertToSearch(command, key);
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
            {
                return new EnrollmentControlResult { OrgSearch = m, UseCurrentTag = usecurrenttag ?? false };
            }

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
            {
                return RedirectShowError("must start with orgsearch");
            }

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
        public ActionResult Meetings(string dt1, string dt2, int? programid, int? divisionid)
        {
            var d1 = dt1.ToDate();
            var d2 = dt2.ToDate();
            var m = new MeetingsModel { Dt1 = d1, Dt2 = d2, ProgramId = programid, DivisionId = divisionid };
            return View(m);
        }
        [HttpPost]
        public ActionResult Meetings(MeetingsModel m)
        {
            if (!m.Dt1.HasValue)
            {
                m.Dt1 = ChurchAttendanceModel.MostRecentAttendedSunday();
            }

            if (!m.Dt2.HasValue)
            {
                m.Dt2 = m.Dt1.Value.AddDays(1);
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult MeetingsForMonth(string dt1, OrgSearchModel m)
        {
            var orgs = string.Join(",", m.FetchOrgs().Select(oo => oo.OrganizationId));
            var d1 = dt1.ToDate();
            if (!d1.HasValue)
            {
                throw new ArgumentException($"invalid date: {dt1}", nameof(dt1));
            }

            ViewBag.Month = d1.Value.ToString("MMMM yyyy");
            d1 = new DateTime(d1.Value.Year, d1.Value.Month, 1);
            var dt2 = d1.Value.AddMonths(1).AddDays(-1);
            var hasmeetings = CurrentDatabase.MeetingsDataForDateRange(orgs, d1, dt2).AsEnumerable().Any();
            if (!hasmeetings)
            {
                return RedirectShowError("No meetings to show");
            }

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
            {
                return Content("no query");
            }

            return new AveryResult { namesonly = true, id = id.Value };
        }

        [HttpPost]
        public ActionResult OrgLeaders(OrgSearchModel m)
        {
            return new OrgLeadersResult(m);
        }

        [HttpGet]
        public ActionResult PastAttendee(int? id)
        {
            if (!id.HasValue)
            {
                return Content("no orgid");
            }

            return new PastAttendeeResult(id);
        }

        [HttpGet]
        public ActionResult Prospect(Guid? id, bool? Form, bool? Alpha)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            return new ProspectResult(id.Value, Form ?? false, Alpha ?? false);
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
            var q = cn.Query("RecentAbsentsSP2", new { orgs }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            return View(q);
        }

        [HttpGet, Route("RecentAbsents1/{oid}/{qid}/{otherorgidfilter?}")]
        public ActionResult RecentAbsents1(int oid, Guid qid, int? otherorgidfilter)
        {
            var filter = CurrentDatabase.OrgFilters.SingleOrDefault(vv => vv.QueryId == qid);
            if (filter == null)
            {
                return Message("Expired OrgFilter");
            }

            var m = new RecentAbsentsViewModel(oid, qid, otherorgidfilter);
            return View(m);
        }
        [HttpGet, Route("RecentAbsentsSg/{oid}/{otherorgidfilter?}/{smallgroup?}")]
        public ActionResult RecentAbsentsSg(int oid, int? otherorgidfilter, string smallgroup)
        {
            var filter = CurrentDatabase.NewOrgFilter(oid);
            filter.GroupSelect = Util.PickFirst(smallgroup, "NONE");
            ViewBag.SmallGroup = filter.GroupSelect;
            var m = new RecentAbsentsViewModel(oid, filter.QueryId, otherorgidfilter);
            return View("RecentAbsents1", m);
        }

        [HttpGet]
        public ActionResult Registration(Guid? id, int? oid)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            return new RegistrationResult(id, oid);
        }

        [HttpGet]
        public ActionResult RegistrationExcel(Guid? id, int? oid)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

            var table = RegistrationResult.ExcelData(id, oid);
            if (table == null)
            {
                return Content("no results");
            }

            return table.ToExcel("Registrations.xlsx");
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public ActionResult RegistrationSummary(int? days, string sort)
        {
            var q = CurrentDatabase.RecentRegistrations(days ?? 90);
            q = sort == "Organization"
                ? q.OrderBy(rr => rr.OrganizationName).ThenByDescending(rr => rr.Completed)
                : q.OrderByDescending(rr => rr.Dt2);
            return View(q);
        }

        [HttpGet]
        public ActionResult RollLabels(Guid? id, string format, bool? titles, bool? usephone, bool? sortzip, bool? useMailFlags)
        {
            if (!id.HasValue)
            {
                return Content("no query");
            }

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
            bool? altnames, string highlight, int? useword, OrgSearchModel m)
        {
            DateTime? dt2 = dt.ToDate();
            if (!dt2.HasValue)
            {
                return Message("no date");
            }

            var mi = new NewMeetingInfo
            {
                ByGroup = bygroup > 0,
                GroupFilterPrefix = sgprefix,
                UseAltNames = altnames == true,
                HighlightGroup = highlight,
                MeetingDate = dt2.Value
            };
            if (useword == 1)
            {
                return new DocXRollsheetResult { OrgSearchModel = m, NewMeetingInfo = mi };
            }

            return new RollsheetResult { OrgSearchModel = m, NewMeetingInfo = mi };
        }

        [HttpPost, Route("RollsheetForOrg/{queryid}")]
        public ActionResult RollsheetForOrg(Guid queryid, NewMeetingInfo mi)
        {
            if (mi.UseWord == true)
            {
                return new DocXRollsheetResult { QueryId = queryid, NewMeetingInfo = mi };
            }

            return new RollsheetResult { QueryId = queryid, NewMeetingInfo = mi };
        }

        [HttpGet, Route("RollsheetForMeeting/{meetingid:int}")]
        public ActionResult RollsheetForMeeting(int meetingid)
        {
            return new RollsheetResult() { MeetingId = meetingid };
        }

        [HttpPost]
        public ActionResult Rollsheets(NewMeetingInfo mi, OrgSearchModel m)
        {
            if (mi.UseWord == true)
            {
                return new DocXRollsheetResult { OrgSearchModel = m, NewMeetingInfo = mi };
            }

            return new RollsheetResult { OrgSearchModel = m, NewMeetingInfo = mi };
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

        [HttpPost]
        public ActionResult SgMap(OrgSearchModel m)
        {
            return Redirect("/Sgmap/Index/" + m.DivisionId);
        }

        [HttpPost]
        public ActionResult ShirtSizes(OrgSearchModel m)
        {
            var orgs = m.FetchOrgs();
            var q = from om in CurrentDatabase.OrganizationMembers
                    join o in orgs on om.OrganizationId equals o.OrganizationId
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
            {
                return Content("no meetingid");
            }

            return new VisitsAbsentsResult(id);
        }

        [HttpGet]
        public ActionResult VisitsAbsents2(int? id, string prefix = "")
        {
            //This is basically a Contact Report version of the Visits Absents
            if (!id.HasValue)
            {
                return Content("no meetingid");
            }

            return new VisitsAbsentsResult2(id.Value, prefix);
        }

        [HttpGet]
        public ActionResult VitalStats()
        {
            if (Util2.OrgLeadersOnly)
            {
                return Redirect("/Home");
            }

            var script = CurrentDatabase.ContentOfTypePythonScript("VitalStats");
            if (!script.HasValue())
            {
                script = System.IO.File.ReadAllText(Server.MapPath("/Content/VitalStats.py"));
            }

            ViewBag.table = script.Contains("class VitalStats")
                ? QueryFunctions.OldVitalStats(CurrentDatabase, script)
                : PythonModel.RunScript(CurrentDatabase.Host, script);

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
        public ActionResult WeeklyDecisions(int? campus, string sunday)
        {
            if (Util2.OrgLeadersOnly)
            {
                return Redirect("/Home");
            }

            var sun = sunday.ToDate();
            var m = new WeeklyDecisionsModel(sun) { Campus = campus };
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
            var q = from e in CurrentDatabase.EmailQueues
                    where e.Body.Contains("ssl.cf2.rackcdn.com")
                    select e;
            var images = new List<string>();
            foreach (var e in q)
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(e.Body);
                var nodes = doc.DocumentNode.SelectNodes("//img[@src]");
                if (nodes == null)
                {
                    continue;
                }

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
    }
}
