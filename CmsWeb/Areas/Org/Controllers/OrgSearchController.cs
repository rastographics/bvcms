using CmsData;
using CmsData.Codes;
using CmsData.Registration;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using CmsWeb.Lifecycle;
using Dapper;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Controllers
{
    [SessionExpire]
    [RouteArea("Org", AreaPrefix = "OrgSearch"), Route("{action=index}/{id?}")]
    public class OrgSearchController : CmsStaffController
    {
        private const string STR_OrgSearch = "OrgSearch";

        public OrgSearchController(IRequestManager requestManager) : base(requestManager)
        {
        }

        [Route("~/OrgSearch/{progid:int?}/{div:int?}")]
        public ActionResult Index(int? div, int? progid, int? onlinereg, string name)
        {
            Response.NoCache();
            var m = new OrgSearchModel();
            m.StatusId = OrgStatusCode.Active;

            if (name.HasValue())
            {
                m.Name = name;
                m.StatusId = null;
            }
            if (onlinereg.HasValue)
            {
                m.OnlineReg = onlinereg;
            }

            if (div.HasValue)
            {
                m.DivisionId = div;
                if (progid.HasValue)
                {
                    m.ProgramId = progid;
                }
                else
                {
                    m.ProgramId = m.Division().ProgId;
                }

                m.TagProgramId = m.ProgramId;
                m.TagDiv = div;
            }
            else if (progid.HasValue)
            {
                m.ProgramId = progid;
                m.TagProgramId = m.ProgramId;
            }
            else if (Session[STR_OrgSearch].IsNotNull())
            {
                (Session[STR_OrgSearch] as OrgSearchInfo).CopyPropertiesTo(m);
            }

            return View(m);
        }

        [HttpPost]
        public ActionResult Results(OrgSearchModel m)
        {
            Session[STR_OrgSearch] = new OrgSearchInfo(m);
            return View(m);
        }

        [HttpPost]
        public ActionResult DivisionIds(int id)
        {
            var m = new OrgSearchModel { ProgramId = id };
            return View(m);
            //return Json(OrgSearchModel.DivisionIds(id));
        }

        [HttpPost]
        public ActionResult TagDivIds(int id)
        {
            var m = new OrgSearchModel { ProgramId = id };
            return View("DivisionIds", m);
        }

        [HttpPost]
        public ActionResult ApplyType(int id, OrgSearchModel m)
        {
            var t = (id == -1 ? (int?)null : id);
            if (t == 0)
            {
                return Content("");
            }

            var ot = CurrentDatabase.OrganizationTypes.SingleOrDefault(tt => tt.Id == id);
            if (t.HasValue || ot != null)
            {
                var q = from o in CurrentDatabase.Organizations
                        join os in m.FetchOrgs() on o.OrganizationId equals os.OrganizationId
                        select o;
                foreach (var o in q)
                {
                    o.OrganizationTypeId = t;
                }
            }
            else
            {
                return Content("error: missing type");
            }

            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }

        [HttpPost]
        public ActionResult MakeChildrenOf(int id, OrgSearchModel m)
        {
            var t = (id == -1 ? (int?)null : id);
            if (t == 0)
            {
                return Content("");
            }

            var org = CurrentDatabase.LoadOrganizationById(id);
            if (t.HasValue || org != null)
            {
                var q = from o in CurrentDatabase.Organizations
                        join os in m.FetchOrgs() on o.OrganizationId equals os.OrganizationId
                        where o.OrganizationId != id
                        select o;
                foreach (var o in q)
                {
                    o.ParentOrgId = t;
                }
            }
            else
            {
                return Content("error: missing type");
            }

            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }

        [HttpPost]
        public ActionResult RenameDiv(int id, int divid, string name)
        {
            var d = CurrentDatabase.Divisions.Single(dd => dd.Id == divid);
            d.Name = name;
            CurrentDatabase.SubmitChanges();
            var m = new OrgSearchModel { ProgramId = id };
            return View("DivisionIds", m);
        }

        [HttpPost]
        public ActionResult MakeNewDiv(int id, string name)
        {
            var d = new Division { Name = name, ProgId = id };
            d.ProgDivs.Add(new ProgDiv { ProgId = id });
            CurrentDatabase.Divisions.InsertOnSubmit(d);
            CurrentDatabase.SubmitChanges();
            var m = new OrgSearchModel { ProgramId = id, TagDiv = d.Id };
            return View("DivisionIds", m);
        }

        [HttpPost]
        public ActionResult DefaultMeetingDate(int id)
        {
            var dt = OrgSearchModel.DefaultMeetingDate(id);
            return Json(new { date = dt.Date.ToShortDateString(), time = dt.ToShortTimeString() });
        }
        [HttpPost]
        public ActionResult ExportExcel(OrgSearchModel m)
        {
            return m.OrganizationExcelList();
        }
        [HttpPost]
        public ActionResult ExportMembersExcel(OrgSearchModel m)
        {
            return m.OrgsMemberList();
        }
        [HttpPost]
        public ActionResult RegOptions(OrgSearchModel m)
        {
            return m.RegOptionsList();
        }
        [HttpPost]
        public ActionResult RegQuestionsUsage(OrgSearchModel m)
        {
            return m.RegQuestionsUsage();
        }
        [HttpPost]
        public ActionResult RegSettingUsages(OrgSearchModel m)
        {
            return m.RegSettingUsages();
        }
        [HttpPost]
        public ActionResult RegSettingsXml(OrgSearchModel m)
        {
            Response.ContentType = "text/xml";
            m.RegSettingsXml(Response.OutputStream);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult RegMessages(OrgSearchModel m, Settings.Messages messages)
        {
            Response.ContentType = "text/xml";
            m.RegMessagesXml(Response.OutputStream, messages);
            return new EmptyResult();
        }
        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            var a = id.Split('-');
            var c = new ContentResult();
            c.Content = value;
            var org = CurrentDatabase.LoadOrganizationById(a[1].ToInt());
            if (org == null)
            {
                return c;
            }

            switch (a[0])
            {
                case "bs":
                    org.BirthDayStart = value.ToDate();
                    break;
                case "be":
                    org.BirthDayEnd = value.ToDate();
                    break;
                case "rs":
                    org.RegStart = value.ToDate();
                    break;
                case "re":
                    org.RegEnd = value.ToDate();
                    break;
                case "ck":
                    org.CanSelfCheckin = value == "yes";
                    break;
                case "reg2":
                    org.UseRegisterLink2 = value == "yes";
                    c.Content = org.UseRegisterLink2 == true ? "Family" : "Individual";
                    break;
                case "so":
                    org.PublicSortOrder = value.HasValue() ? value : null;
                    break;
                case "ac":
                    if (value == "Other")
                    {
                        value = null;
                    }

                    org.AppCategory = value.HasValue() ? value : null;
                    break;
            }
            CurrentDatabase.SubmitChanges();
            return c;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SetDescription(string id, string description)
        {
            var a = id.Split('-');
            var org = CurrentDatabase.LoadOrganizationById(a[1].ToInt());
            org.Description = description;
            CurrentDatabase.SubmitChanges();
            return Content("ok");
        }

        [HttpPost]
        [Route("SqlReport/{report}")]
        public ActionResult SqlReport(OrgSearchModel m, string report, DateTime? dt1 = null, DateTime? dt2 = null)
        {
            try
            {
                var orgs = m.FetchOrgs();
                var oids = string.Join(",", orgs.Select(oo => oo.OrganizationId));
                ViewBag.ExcelUrl = $"/OrgSearch/SqlReportExcel/{report}";
                ViewBag.DisplayName = report.SpaceCamelCase();
                ViewBag.OrgIds = oids;
                ViewBag.dt1 = dt1;
                ViewBag.dt2 = dt2;
                var content = CurrentDatabase.ContentOfTypeSql(report);
                if (content.Contains("pagebreak", ignoreCase: true))
                {
                    var p = m.GetSqlParameters(oids, dt1, dt2, content);
                    ViewBag.Results = PythonModel.PageBreakTables(CurrentDatabase, content, p);
                    return View();
                }
                ViewBag.Results = m.SqlTable(report, oids, dt1, dt2);
                return View();
            }
            catch (Exception ex)
            {
                return Message(ex);
            }
        }
        [HttpPost]
        [Route("SqlReportExcel/{report}")]
        public ActionResult SqlReportExcel(string report, string orgIds, DateTime? dt1 = null, DateTime? dt2 = null)
        {
            try
            {
                var m = new OrgSearchModel();
                return m.SqlTableExcel(report, orgIds, dt1, dt2);
            }
            catch (Exception ex)
            {
                return Message(ex);
            }
        }
        [HttpPost]
        public ActionResult ToggleTag(int id, int tagdiv)
        {
            //var Db = Db;
            var organization = CurrentDatabase.LoadOrganizationById(id);
            if (tagdiv == 0)
            {
                return Json(new { error = "bad tagdiv" });
            }

            var t = organization.ToggleTag(CurrentDatabase, tagdiv);
            CurrentDatabase.SubmitChanges();
            var m = new OrgSearchModel { StatusId = 0, TagDiv = tagdiv, Name = id.ToString() };
            var o = m.OrganizationList().SingleOrDefault();
            if (o == null)
            {
                return Content("error");
            }

            return View("Row", o);
        }

        [HttpPost]
        public ActionResult MainDiv(int id, int tagdiv)
        {
            //var Db = Db;
            CurrentDatabase.SetMainDivision(id, tagdiv);
            var m = new OrgSearchModel { TagDiv = tagdiv, Name = id.ToString() };
            var o = m.OrganizationList().SingleOrDefault();
            if (o == null)
            {
                return Content("error");
            }

            return View("Row", o);
        }

        [HttpPost]
        public ActionResult Count(OrgSearchModel m)
        {
            return Content(m.FetchOrgs().Count().ToString());
        }

        [HttpPost]
        public ActionResult OrgIds(OrgSearchModel m)
        {
            var orgs = m.FetchOrgs();
            return Content(string.Join(",", orgs.Select(oo => oo.OrganizationId)));
        }
        [HttpPost]
        public ActionResult PasteSettings(OrgSearchModel m)
        {
            var frorg = (int)Session["OrgCopySettings"];
            var orgs = from os in m.FetchOrgs()
                       join o in CurrentDatabase.Organizations on os.OrganizationId equals o.OrganizationId
                       select o;
            foreach (var o in orgs)
            {
                o.CopySettings(CurrentDatabase, frorg);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult RepairTransactions(OrgSearchModel m)
        {
            foreach (var oid in m.FetchOrgs().Select(oo => oo.OrganizationId))
            {
                CurrentDatabase.PopulateComputedEnrollmentTransactions(oid);
            }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult CreateMeeting(string id)
        {
            var n = id.ToCharArray().Count(c => c == 'M');
            if (n > 1)
            {
                return RedirectShowError($"More than one barcode string found({id})");
            }

            var a = id.SplitStr(".");
            var orgid = a[1].ToInt();
            var organization = CurrentDatabase.LoadOrganizationById(orgid);
            if (organization == null)
            {
                return RedirectShowError($"Cannot interpret barcode orgid({id})");
            }

            var re = new Regex(@"\A(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])([0-9]{2})([012][0-9])([0-5][0-9])\Z");
            if (!re.IsMatch(a[2]))
            {
                return RedirectShowError($"Cannot interpret barcode datetime({id})");
            }

            var g = re.Match(a[2]);
            var dt = new DateTime(
                g.Groups[3].Value.ToInt() + 2000,
                g.Groups[1].Value.ToInt(),
                g.Groups[2].Value.ToInt(),
                g.Groups[4].Value.ToInt(),
                g.Groups[5].Value.ToInt(),
                0);
            var newMtg = CurrentDatabase.Meetings.SingleOrDefault(m => m.OrganizationId == orgid && m.MeetingDate == dt);
            if (newMtg == null)
            {
                var attsch = (from s in CurrentDatabase.OrgSchedules
                              where s.OrganizationId == organization.OrganizationId
                              where s.MeetingTime.Value.TimeOfDay == dt.TimeOfDay
                              where s.MeetingTime.Value.DayOfWeek == dt.DayOfWeek
                              select s).SingleOrDefault();
                int? attcred = null;
                if (attsch != null)
                {
                    attcred = attsch.AttendCreditId;
                }

                newMtg = new Meeting
                {
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserId1,
                    OrganizationId = orgid,
                    GroupMeetingFlag = false,
                    Location = organization.Location,
                    MeetingDate = dt,
                    AttendCreditId = attcred
                };
                CurrentDatabase.Meetings.InsertOnSubmit(newMtg);
                CurrentDatabase.SubmitChanges();
                DbUtil.LogActivity($"Creating new meeting for {dt}");
            }
            return Redirect($"/Meeting/{newMtg.MeetingId}?showall=true");
        }

        [HttpPost]
        public ActionResult CreateMeetings(DateTime dt, bool noautoabsents, OrgSearchModel model)
        {
            var orgIds = model.FetchOrgs().Select(oo => oo.OrganizationId).ToList();
            foreach (var oid in orgIds)
            {
                var db = DbUtil.Create(Util.Host);
                Meeting.FetchOrCreateMeeting(CurrentDatabase, oid, dt, noautoabsents);
                CurrentDatabase.Dispose();
            }
            DbUtil.LogActivity("Creating new meetings from OrgSearch");
            return Content("done");
        }
        [HttpPost]
        public ActionResult MovePendingToMember(OrgSearchModel m)
        {
            var orgids = string.Join(",", m.FetchOrgs().Select(mm => mm.OrganizationId));
            var i = CurrentDatabase.Connection.ExecuteScalar($@"
	UPDATE dbo.OrganizationMembers
	SET Pending = NULL
	FROM dbo.OrganizationMembers om
	JOIN dbo.SplitInts('{orgids}') i ON i.Value = om.OrganizationId
	WHERE om.Pending = 1

    SELECT @@ROWCOUNT
");
            return Content($"changed {i} people");
        }

        [HttpPost]
        [Authorize(Roles = "Attendance")]
        public ActionResult EmailAttendanceNotices(OrgSearchModel m)
        {
            m.SendNotices();
            return Content("ok");
        }

        [HttpPost]
        [Authorize(Roles = "Attendance")]
        public ActionResult DisplayAttendanceNotices(OrgSearchModel m)
        {
            var leaders = m.NoticesToSend();
            return View(leaders);
        }

        public ActionResult OrganizationStructure(bool? active, OrgSearchModel m)
        {
            var orgs = m.FetchOrgs();
            var q = from os in CurrentDatabase.ViewOrganizationStructures
                    join o in orgs on os.OrgId equals o.OrganizationId
                    select os;
            return View(q.OrderBy(oo => oo.Program).ThenBy(oo => oo.Division).ThenBy(oo => oo.Organization));
        }

        public ActionResult ConvertToSearch(OrgSearchModel m)
        {
            var s = m.ConvertToSearch();
            return s.StartsWith("Error")
                ? RedirectShowError(s)
                : Redirect(m.ConvertToSearch());
        }

        [Serializable]
        private class OrgSearchInfo
        {
            public OrgSearchInfo(OrgSearchModel m)
            {
                this.CopyPropertiesFrom(m);
            }

            public string Name { get; set; }
            public int? ProgramId { get; set; }
            public int? DivisionId { get; set; }
            public int? TypeId { get; set; }
            public int? CampusId { get; set; }
            public int? ScheduleId { get; set; }
            public int? StatusId { get; set; }
            public int? OnlineReg { get; set; }
            public bool PublicView { get; set; }
            public string ExtraValues { get; set; }
        }
    }
}
