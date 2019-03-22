using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Org.Models;
using CmsWeb.Lifecycle;
using CmsWeb.Services.MeetingCategory;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "Meeting"), Route("{action}/{id?}")]
    public class MeetingController : CmsStaffController
    {
        private readonly IMeetingCategoryService _meetingCategoryService;

        public MeetingController(IRequestManager requestManager, IMeetingCategoryService meetingCategoryService) : base(requestManager)
        {
            _meetingCategoryService = meetingCategoryService;
        }

        [Route("~/Meeting/{id:int}")]
        public ActionResult Index(int id, bool? showall, bool? sortbyname, bool? CurrentMembers, bool? showlarge)
        {
            var m = new MeetingModel(id)
            {
                currmembers = CurrentMembers ?? false,
                showall = showall == true,
                sortbyname = sortbyname == true,
                showlarge = showlarge ?? false
            };
            if (m.meeting == null)
            {
                return RedirectShowError("no meeting");
            }

            if (Util2.OrgLeadersOnly)
            {
                var oids = CurrentDatabase.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(m.org.OrganizationId))
                {
                    return NotAllowed("You must be a leader of this organization", m.org.OrganizationName);
                }
            }
            if (m.org.LimitToRole.HasValue())
            {
                if (!User.IsInRole(m.org.LimitToRole))
                {
                    return NotAllowed("no privilege to view ", m.org.OrganizationName);
                }
            }

            DbUtil.LogActivity($"Viewing Meeting for {m.meeting.Organization.OrganizationName}");
            return View(m);
        }

        public ActionResult Names(string term)
        {
            var q = MeetingModel.Names(term, 10).ToList();
            return Json(q, JsonRequestBehavior.AllowGet);
        }

        private ActionResult NotAllowed(string error, string name)
        {
            DbUtil.LogActivity($"Trying to view Meeting for Org ({name})");
            return Content($"<h3 style='color:red'>{error}</h3>\n<a href='{"javascript: history.go(-1)"}'>{"Go Back"}</a>");
        }

        public ActionResult iPad(int? id, bool? commitsOnly)
        {
            if (!id.HasValue)
            {
                return RedirectShowError("no id");
            }

            var m = new MeetingModel(id.Value);
            m.showall = true;
            m.CommitsOnly = commitsOnly == true;
            if (m.meeting == null)
            {
                return RedirectShowError("no meeting");
            }

            if (Util2.OrgLeadersOnly
                && !CurrentDatabase.OrganizationMembers.Any(om =>
                    om.OrganizationId == m.meeting.OrganizationId
                    && om.PeopleId == Util.UserPeopleId
                    && om.MemberType.AttendanceTypeId == AttendTypeCode.Leader))
            {
                return RedirectShowError("You must be a leader of this organization to have access to this page");
            }

            DbUtil.LogActivity($"iPad Meeting for {m.meeting.OrganizationId}({m.meeting.MeetingDate:d})");
            return View(m);
        }

        [HttpPost]
        public ContentResult EditGroup(string id, string value)
        {
            var i = id.Substring(2).ToInt();
            var m = new MeetingModel(i);
            m.meeting.GroupMeetingFlag = value == "True";
            CurrentDatabase.SubmitChanges();
            if (m.meeting.GroupMeetingFlag)
            {
                return Content("Group (headcount)");
            }

            return Content("Regular");
        }

        [HttpPost]
        public ContentResult EditAttendCredit(string id, string value)
        {
            var i = id.Substring(2).ToInt();
            var m = new MeetingModel(i);
            m.meeting.AttendCreditId = value.ToInt();
            CurrentDatabase.SubmitChanges();
            return Content(m.AttendCreditType());
        }

        [HttpPost]
        public JsonResult AttendCredits()
        {
            var q = from c in CurrentDatabase.AttendCredits
                    select new
                    {
                        Code = c.Id.ToString(),
                        Value = c.Description
                    };
            return Json(q.ToDictionary(k => k.Code, v => v.Value));
        }

        [HttpPost]
        public ContentResult EditMeetingCategory(string id, string value)
        {
            var i = id.Substring(2).ToInt();
            var m = new MeetingModel(i);
            m.meeting.Description = HttpUtility.HtmlDecode(value);
            CurrentDatabase.SubmitChanges();
            return Content(HttpUtility.HtmlAttributeEncode(m.meeting.Description ?? string.Empty));
        }

        [HttpPost]
        public JsonResult MeetingCategories()
        {
            var q = _meetingCategoryService.GetMeetingCategories(false);
            var result = Json(q.ToDictionary(
                c => HttpUtility.HtmlAttributeEncode(c.Description),
                c => HttpUtility.HtmlAttributeEncode(c.Description))
            );
            return result;
        }


        [HttpPost]
        public JsonResult MeetingTypes()
        {
            var d = new Dictionary<string, string> { { "True", "Group (headcount)" }, { "False", "Regular" } };
            return Json(d);
        }

        [HttpPost]
        public ContentResult Edit(string id, string value)
        {
            try
            {
                var i = id.Substring(2).ToInt();
                var m = new MeetingModel(i);
                switch (id[0])
                {
                    case 'd':
                        m.meeting.Description = value;
                        break;
                    case 'h':
                        m.meeting.HeadCount = value.ToInt();
                        break;
                    case 't':
                        CurrentDatabase.ExecuteCommand(@"
                        update dbo.Attend set MeetingDate = {0} where MeetingId = {1};
                        update dbo.Meetings set MeetingDate = {0} where MeetingId = {1};
                        ", value.ToDate(), m.meeting.MeetingId);
                        break;
                }
                CurrentDatabase.SubmitChanges();
                return Content(value);
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.Message);
            }
        }

        [HttpPost]
        public ContentResult EditCommitment(string id, string value)
        {
            var a = id.Substring(1).Split('_').Select(vv => vv.ToInt()).ToArray();
            var c = value.ToInt2();
            if (c == 99)
            {
                c = null;
            }

            Attend.MarkRegistered(CurrentDatabase, a[1], a[0], c);
            var desc = AttendCommitmentCode.Lookup(c ?? 99);
            DbUtil.LogActivity($"EditCommitment {desc} id={id}");
            return Content(desc);
        }

        [HttpPost]
        public ContentResult JoinAllVisitors(int id)
        {
            var m = new MeetingModel(id);
            var n = 0;
            foreach (var a in m.VisitAttends())
            {
                OrganizationMember.InsertOrgMembers(CurrentDatabase,
                    m.meeting.OrganizationId, a.PeopleId, MemberTypeCode.Member,
                    DateTime.Today, null, false);
                n++;
            }
            if (n > 0)
            {
                CurrentDatabase.UpdateMainFellowship(m.meeting.OrganizationId);
                return Content($"Joined {n} visitors");
            }
            return Content("no visitors");
        }

        public ActionResult Tickets(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectShowError("no id");
            }

            var m = new MeetingModel(id.Value);
            m.showall = true;
            if (m.meeting == null)
            {
                return RedirectShowError("no meeting");
            }

            if (Util2.OrgLeadersOnly
                && !CurrentDatabase.OrganizationMembers.Any(om =>
                    om.OrganizationId == m.meeting.OrganizationId
                    && om.PeopleId == Util.UserPeopleId
                    && om.MemberType.AttendanceTypeId == AttendTypeCode.Leader))
            {
                return RedirectShowError("You must be a leader of this organization to have access to this page");
            }

            DbUtil.LogActivity($"Tickets Meeting for {m.meeting.OrganizationId}({m.meeting.MeetingDate:d})");
            return View(m);
        }

        [HttpPost]
        public ActionResult TicketMeeting(int id)
        {
            var m = new MeetingModel(id);
            return View(m);
        }

        [Authorize(Roles = "Attendance")]
        [HttpPost]
        public ActionResult ScanTicket(string wandtarget, int MeetingId, bool? requireMember, bool? requireRegistered)
        {
            var d = new ScanTicketInfo { person = new Person() };
            var pid = 0;

            if (wandtarget.StartsWith("M."))
            {
                var a = wandtarget.Split('.');
                if (a.Length != 3)
                {
                    return View(d.AddError(ScanTicketInfo.Error.noorg));
                }

                var oid = a[1].ToInt2();
                pid = a[2].ToInt();
                d.person = CurrentDatabase.LoadPersonById(pid);
                if (!oid.HasValue)
                {
                    return View(d.AddError(ScanTicketInfo.Error.noorg));
                }

                var tm = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == MeetingId);
                var mq = from m in CurrentDatabase.Meetings
                         where m.OrganizationId == a[1].ToInt()
                         where m.MeetingDate.Value.Date == tm.MeetingDate.Value.Date
                         select m;
                var mo = mq.FirstOrDefault();
                if (mo == null)
                {
                    return View(d.AddError(ScanTicketInfo.Error.nomeeting));
                }

                d.meeting = mo;
                MeetingId = mo.MeetingId;
                d.SwitchOrg = true;
                d.attended = CurrentDatabase.Attends.SingleOrDefault(aa => aa.MeetingId == MeetingId && aa.PeopleId == pid && aa.AttendanceFlag);
            }
            else
            {
                pid = wandtarget.ToInt();
                var q = from person in CurrentDatabase.People
                        where person.PeopleId == pid
                        let meeting = CurrentDatabase.Meetings.SingleOrDefault(mm => mm.MeetingId == MeetingId)
                        let attended = CurrentDatabase.Attends.SingleOrDefault(aa => aa.MeetingId == MeetingId && aa.PeopleId == pid && aa.AttendanceFlag)
                        let orgmember = CurrentDatabase.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == meeting.OrganizationId && om.PeopleId == pid)
                        select new ScanTicketInfo
                        {
                            person = person,
                            meeting = meeting,
                            attended = attended,
                            orgmember = orgmember,
                            family = from m in person.Family.People
                                     where m.PeopleId != pid
                                     let att = CurrentDatabase.Attends.SingleOrDefault(aa => aa.MeetingId == MeetingId && aa.PeopleId == m.PeopleId && aa.AttendanceFlag)
                                     let orgmem = CurrentDatabase.OrganizationMembers.SingleOrDefault(om => om.OrganizationId == meeting.OrganizationId && om.PeopleId == m.PeopleId)
                                     select new FamilyMemberInfo
                                     {
                                         PeopleId = m.PeopleId,
                                         person = m,
                                         attended = att != null,
                                         orgmember = orgmem != null
                                     }
                        };
                var d2 = q.SingleOrDefault();
                if (d2 == null)
                {
                    return View(d.AddError(ScanTicketInfo.Error.noperson));
                }

                d = d2;
            }

            d.error = ScanTicketInfo.Error.none;
            if (d.attended != null && d.attended.AttendanceFlag)
            {
                d.error = ScanTicketInfo.Error.alreadymarked;
            }
            else if (requireMember == true && d.orgmember == null)
            {
                d.error = ScanTicketInfo.Error.notmember;
            }
            else if (requireRegistered == true && (d.attended == null || d.attended.Commitment == AttendCommitmentCode.Attending))
            {
                d.error = ScanTicketInfo.Error.notregistered;
            }

            var ret = "";
            if (d.error == ScanTicketInfo.Error.none)
            {
                ret = Attend.RecordAttendance(CurrentDatabase, pid, MeetingId, true);
                if (ret.Contains("already"))
                {
                    d.error = ScanTicketInfo.Error.alreadymarkedelsewhere;
                    d.message = ret;
                }
                else
                {
                    CurrentDatabase.UpdateMeetingCounters(MeetingId);
                    CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, d.meeting);
                }
            }

            return View(d);
        }

        [Authorize(Roles = "Attendance")]
        [HttpPost]
        public ActionResult MarkAttendance(int PeopleId, int MeetingId, bool Present)
        {
            var ret = Attend.RecordAttendance(CurrentDatabase, PeopleId, MeetingId, Present);
            if (ret != "ok")
            {
                return Json(new { error = ret });
            }

            CurrentDatabase.UpdateMeetingCounters(MeetingId);
            var m = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == MeetingId);
            CurrentDatabase.Refresh(RefreshMode.OverwriteCurrentValues, m);
            var v = Json(new
            {
                m.NumPresent,
                m.NumMembers,
                m.NumVstMembers,
                m.NumRepeatVst,
                m.NumNewVisit,
                m.NumOtherAttends
            });
            return v;
        }

        [Authorize(Roles = "Attendance")]
        [HttpPost]
        public ActionResult MarkRegistered(int PeopleId, int MeetingId, int? CommitId)
        {
            try
            {
                Attend.MarkRegistered(CurrentDatabase, PeopleId, MeetingId, CommitId);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return new EmptyResult();
        }

        [Authorize(Roles = "Attendance")]
        [HttpPost]
        public ActionResult CreateMeeting(string id)
        {
            var a = id.SplitStr(".");
            var orgid = a[1].ToInt();
            var organization = CurrentDatabase.LoadOrganizationById(orgid);
            if (organization == null)
            {
                return Content($"error:Bad Orgid ({id})");
            }

            var re = new Regex(@"\A(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[01])([0-9]{2})([012][0-9])([0-5][0-9])\Z");
            if (!re.IsMatch(a[2]))
            {
                return Content($"error:Bad Date and Time ({id})");
            }

            var g = re.Match(a[2]);
            var dt = new DateTime(
                g.Groups[3].Value.ToInt() + 2000,
                g.Groups[1].Value.ToInt(),
                g.Groups[2].Value.ToInt(),
                g.Groups[4].Value.ToInt(),
                g.Groups[5].Value.ToInt(),
                0);
            var newMtg = CurrentDatabase.Meetings.FirstOrDefault(m => m.OrganizationId == orgid && m.MeetingDate == dt);
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
                DbUtil.LogActivity($"Created new meeting for {organization.OrganizationName}");
            }
            return Content($"/Meeting/{newMtg.MeetingId}?showall=true");
        }

        public ActionResult QueryAttendees(int Id)
        {
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            cc.AddNewClause(QueryType.MeetingId, CompareType.Equal, Id);
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }

        public ActionResult QueryVisitors(int Id)
        {
            var m = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == Id);
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            cc.AddNewClause(QueryType.MeetingId, CompareType.Equal, Id);
            var c = cc.AddNewClause(QueryType.AttendTypeAsOf, CompareType.OneOf, "40,VM;50,RG;60,NG");
            c.StartDate = m.MeetingDate;
            c.Program = m.Organization.Division.Program.Id.ToString();
            c.Division = (m.Organization.DivisionId ?? 0).ToString();
            c.Organization = m.OrganizationId.ToString();
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }

        public ActionResult QueryAbsents(int Id)
        {
            var m = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == Id);
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            cc.AddNewClause(QueryType.MeetingId, CompareType.NotEqual, Id);
            var c = cc.AddNewClause(QueryType.WasMemberAsOf, CompareType.Equal, "1,True");
            c.StartDate = m.MeetingDate;
            c.Program = m.Organization.Division.Program.Id.ToString();
            c.Division = (m.Organization.DivisionId ?? 0).ToString();
            c.Organization = m.OrganizationId.ToString();
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }

        public ActionResult QueryRegistered(int Id, string type)
        {
            var m = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == Id);
            var cc = CurrentDatabase.ScratchPadCondition();
            cc.Reset();
            switch (type)
            {
                case "Attending":
                    cc.AddNewClause(QueryType.RegisteredForMeetingId, CompareType.Equal, m.MeetingId);
                    break;
                case "Regrets":
                    cc.AddNewClause(QueryType.CommitmentForMeetingId, CompareType.Equal, AttendCommitmentCode.Regrets + ",RG")
                        .Quarters = m.MeetingId.ToString();
                    break;
                case "NotRegistered":
                    cc.AddNewClause(QueryType.HasCommitmentForMeetingId, CompareType.Equal, "0,False")
                        .Quarters = m.MeetingId.ToString();
                    break;
                case "UnregisteredAbsents":
                    cc.AddNewClause(QueryType.HasCommitmentForMeetingId, CompareType.Equal, "0,False")
                        .Quarters = m.MeetingId.ToString();
                    cc.AddNewClause(QueryType.MeetingId, CompareType.NotEqual, m.MeetingId);
                    break;
                case "Attends":
                    cc.AddNewClause(QueryType.RegisteredForMeetingId, CompareType.Equal, m.MeetingId);
                    cc.AddNewClause(QueryType.MeetingId, CompareType.Equal, m.MeetingId);
                    break;
                case "Absents":
                    cc.AddNewClause(QueryType.RegisteredForMeetingId, CompareType.Equal, m.MeetingId);
                    cc.AddNewClause(QueryType.MeetingId, CompareType.NotEqual, m.MeetingId);
                    break;
                case "UnregisteredAttends":
                    cc.AddNewClause(QueryType.RegisteredForMeetingId, CompareType.NotEqual, m.MeetingId);
                    cc.AddNewClause(QueryType.MeetingId, CompareType.Equal, m.MeetingId);
                    break;
            }
            cc.Save(CurrentDatabase);
            return Redirect("/Query/" + cc.Id);
        }

        public ActionResult AttendanceByGroups(int id, string prefix)
        {
            var q = from a in CurrentDatabase.Attends
                    where a.MeetingId == id
                    join om in CurrentDatabase.OrgMemMemTags
                        on new { a.OrganizationId, a.PeopleId }
                        equals new { OrganizationId = om.OrgId, om.PeopleId }
                    where prefix == null || om.MemberTag.Name.StartsWith(prefix)
                    select new { a.Person.Name, SmallGroup = om.MemberTag.Name, Attended = a.AttendanceFlag };
            var j = from i in q
                    group i by new { i.Attended, i.SmallGroup }
                    into g
                    from i in g
                    orderby i.Attended descending, i.SmallGroup, i.Name
                    select new ttt
                    {
                        Attended = i.Attended,
                        SmallGroup = i.SmallGroup,
                        Name = i.Name
                    };
            //            var list = new List<ttt>();
            //            foreach (var i in j)
            //            {
            //                list.Add(new ttt { label = "SmallGroup", name = $"{i.Key} ({i.g.Count()})"});
            //                foreach (var name in i.g)
            //                    list.Add(new ttt { label = "", name = name });
            //            }
            return View(j);
        }

        public ActionResult NewExtraValue(int id, string field, string value, bool multiline)
        {
            var m = new MeetingModel(id);
            try
            {
                var mev = new MeetingExtra { MeetingId = id, Field = field, Data = value, DataType = multiline ? "text" : null };
                CurrentDatabase.MeetingExtras.InsertOnSubmit(mev);
                CurrentDatabase.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.Message);
            }
            return View("ExtrasGrid", m.meeting);
        }

        [HttpPost]
        public ViewResult DeleteExtra(int id, string field)
        {
            var e = CurrentDatabase.MeetingExtras.Single(ee => ee.MeetingId == id && ee.Field == field);
            CurrentDatabase.MeetingExtras.DeleteOnSubmit(e);
            CurrentDatabase.SubmitChanges();
            var m = new MeetingModel(id);
            return View("ExtrasGrid", m.meeting);
        }

        [HttpPost]
        public ContentResult EditExtra(string id, string value)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var e = CurrentDatabase.MeetingExtras.Single(ee => ee.MeetingId == b[1].ToInt() && ee.Field == b[0]);
            e.Data = value;
            CurrentDatabase.SubmitChanges();
            return Content(value);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AttendCommitments()
        {
            return Json(MeetingModel.AttendCommitments());
        }

        [HttpGet]
        public ActionResult AddAbsentsToMeeting(int id)
        {
            CurrentDatabase.ExecuteCommand("dbo.AddAbsentsToMeeting {0}", id);
            return Redirect($"/Meeting/{id}");
        }

        public ActionResult Attendance(int? id, bool? currentMembers)
        {
            if (!id.HasValue)
            {
                return RedirectShowError("no id");
            }

            var m = new MeetingModel(id.Value)
            {
                currmembers = currentMembers ?? false
            };
            m.showall = true;
            if (m.meeting == null)
            {
                return RedirectShowError("no meeting");
            }

            if (Util2.OrgLeadersOnly
                && !CurrentDatabase.OrganizationMembers.Any(om =>
                    om.OrganizationId == m.meeting.OrganizationId
                    && om.PeopleId == Util.UserPeopleId
                    && om.MemberType.AttendanceTypeId == AttendTypeCode.Leader))
            {
                return RedirectShowError("You must be a leader of this organization to have access to this page");
            }

            DbUtil.LogActivity($"Attendance for Meeting for {m.meeting.OrganizationId}({m.meeting.MeetingDate:d})");
            List<Reports.Models.RollsheetModel.AttendInfo> Guests = new List<Reports.Models.RollsheetModel.AttendInfo>();
            List<Reports.Models.RollsheetModel.AttendInfo> Members = new List<Reports.Models.RollsheetModel.AttendInfo>();
            int MembersPresent = 0;
            int GuestsPresent = 0;
            var attends = m.Attends(true, "iPadAttendanceHighlight");
            foreach (var attend in attends)
            {
                if (!attend.CurrMember)
                {
                    attend.MemberType = "Guest";
                    Guests.Add(attend);
                    if (attend.Attended)
                    {
                        GuestsPresent++;
                    }
                }
                else
                {
                    if (!attend.MemberType.HasValue())
                    {
                        attend.MemberType = "Member";
                    }
                    Members.Add(attend);
                    if (attend.Attended)
                    {
                        MembersPresent++;
                    }
                }
            }
            ViewBag.Guests = Guests;
            ViewBag.Members = Members;
            ViewBag.GuestsPresent = GuestsPresent;
            ViewBag.MembersPresent = MembersPresent;
            return View(m);
        }

        public class ScanTicketInfo
        {
            public enum Error
            {
                none,
                alreadymarked,
                alreadymarkedelsewhere,
                notmember,
                notregistered,
                noperson,
                noorg,
                nomeeting
            }

            public Error error { get; set; }
            public Person person { get; set; }
            public IEnumerable<FamilyMemberInfo> family { get; set; }
            public Meeting meeting { get; set; }
            public string message { get; set; }
            public bool SwitchOrg { get; set; }

            public MeetingModel model => new MeetingModel(meeting.MeetingId);

            public Attend attended { get; set; }
            public OrganizationMember orgmember { get; set; }

            public string CssClass()
            {
                if (error == Error.none)
                {
                    return "alert alert-success";
                }

                return "alert alert-danger";
            }

            public ScanTicketInfo AddError(Error err)
            {
                error = err;
                return this;
            }
        }

        public class FamilyMemberInfo
        {
            public int PeopleId { get; set; }
            public Person person { get; set; }
            public bool attended { get; set; }
            public bool orgmember { get; set; }
        }

        public class ttt
        {
            public bool Attended { get; set; }
            public string SmallGroup { get; set; }
            public string Name { get; set; }
        }
    }
}
