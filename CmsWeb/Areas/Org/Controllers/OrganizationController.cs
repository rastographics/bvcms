using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using CmsWeb.Areas.Org.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Org.Controllers
{
    [RouteArea("Org", AreaPrefix = "Organization"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [SessionExpire]
    public class OrganizationController : CmsStaffController
    {
        const string needNotify = "WARNING: please add the notify persons on messages tab.";

        [HttpGet, Route("~/Organization/{id:int}")]
        public ActionResult Index(int id)
        {
            var db = DbUtil.Db;
            db.CurrentOrg = new CurrentOrg(id);

            var m = new OrganizationModel(id);

            if (m.org == null)
                return Content("organization not found");

            if (Util2.OrgMembersOnly)
            {
                if (m.org.SecurityTypeId == 3)
                    return NotAllowed("You do not have access to this page", m.org.OrganizationName);
                else if (m.org.OrganizationMembers.All(om => om.PeopleId != Util.UserPeopleId))
                    return NotAllowed("You must be a member of this organization", m.org.OrganizationName);
            }
            else if (Util2.OrgLeadersOnly)
            {
                var oids = DbUtil.Db.GetLeaderOrgIds(Util.UserPeopleId);
                if (!oids.Contains(m.org.OrganizationId))
                    return NotAllowed("You must be a leader of this organization", m.org.OrganizationName);
                var sgleader = DbUtil.Db.IsSmallGroupLeaderOnly(id, Util.UserPeopleId);
                if (sgleader.HasValue)
                {
                    db.CurrentOrg.Groups = new []{ sgleader.Value };
                    db.CurrentOrg.GroupsMode = 0;
                }
            }
            if (m.org.LimitToRole.HasValue())
                if (!User.IsInRole(m.org.LimitToRole))
                    return NotAllowed("no privilege to view ", m.org.OrganizationName);

            DbUtil.LogActivity("Viewing Organization ({0})".Fmt(m.org.OrganizationName), m.org.OrganizationName, orgid: id);

            ViewBag.OrganizationContext = true;
            ViewBag.orgname = m.org.FullName;
            ViewBag.model = m;
            ViewBag.selectmode = 0;
            InitExportToolbar(id);
            Session["ActiveOrganization"] = m.org.OrganizationName;
            return View(m);
        }
        private ActionResult NotAllowed(string error, string name)
        {
            DbUtil.LogActivity("Trying to view Organization ({0})".Fmt(name));
            return Content("<h3 style='color:red'>{0}</h3>\n<a href='{1}'>{2}</a>"
                                    .Fmt(error, "javascript: history.go(-1)", "Go Back"));
        }

        [Authorize(Roles = "Delete")]
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            if (org == null)
                return Content("error, bad orgid");
            if (id == 1)
                return Content("Cannot delete first org");
            if (!org.PurgeOrg(DbUtil.Db))
                return Content("error, not deleted");
            var currorg = Util2.CurrentOrganization;
            currorg.Id = 0;
            currorg.Groups = null;
            currorg.GroupsPrefix = null;
            currorg.GroupsMode = 0;
            DbUtil.LogActivity("Delete Org {0}".Fmt(Session["ActiveOrganization"]));
            Session.Remove("ActiveOrganization");
            return Content("ok");
        }

        [HttpPost]
        public ActionResult NewMeeting(string d, string t, int AttendCredit, bool group)
        {
            var organization = DbUtil.Db.LoadOrganizationById(Util2.CurrentOrganization.Id);
            if (organization == null)
                return Content("error: no org");
            DateTime dt;
            if (!DateTime.TryParse(d + " " + t, out dt))
                return Content("error: bad date");
            var mt = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingDate == dt
                    && m.OrganizationId == organization.OrganizationId);

            if (mt != null)
                return Content("/Meeting/" + mt.MeetingId);

            mt = new CmsData.Meeting
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId1,
                OrganizationId = organization.OrganizationId,
                GroupMeetingFlag = group,
                Location = organization.Location,
                MeetingDate = dt,
                AttendCreditId = AttendCredit
            };
            DbUtil.Db.Meetings.InsertOnSubmit(mt);
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Creating new meeting for {0}".Fmt(organization.OrganizationName));
            return Content("/Meeting/" + mt.MeetingId);
        }
        private void InitExportToolbar(int oid)
        {
            ViewBag.oid = oid;
            var qid = DbUtil.Db.QueryInCurrentOrg().QueryId;
            ViewBag.queryid = qid;
            ViewBag.currentQid = qid;
            ViewBag.previousQid = DbUtil.Db.QueryPreviousCurrentOrg().QueryId;
            ViewBag.visitedQid = DbUtil.Db.QueryVisitedCurrentOrg().QueryId;
            ViewBag.pendingQid = DbUtil.Db.QueryPendingCurrentOrg().QueryId;
            ViewBag.inactiveQid = DbUtil.Db.QueryInactiveCurrentOrg().QueryId;
            ViewBag.prospectsQid = DbUtil.Db.QueryProspectCurrentOrg().QueryId;
            ViewBag.TagAction = "/Organization/TagAll/" + qid;
            ViewBag.UnTagAction = "/Organization/UnTagAll/" + qid;
            ViewBag.AddContact = "/Organization/AddContact/" + qid;
            ViewBag.AddTasks = "/Organization/AddTasks/" + qid;
            ViewBag.OrganizationContext = true;
            if (!DbUtil.Db.Organizations.Any(oo => oo.ParentOrgId == oid)) 
                return;
            ViewBag.ParentOrgContext = true;
            ViewBag.leadersqid = DbUtil.Db.QueryLeadersUnderCurrentOrg().QueryId;
            ViewBag.membersqid = DbUtil.Db.QueryMembersUnderCurrentOrg().QueryId;
        }

        [HttpPost]
        public ActionResult CurrMemberGrid(CurrentOrg currorg)
        {
            DbUtil.Db.CurrentOrg = currorg;
            var m = new MemberModel(GroupSelectCode.Member);
            UpdateModel(m.Pager);
            ViewBag.OrgMemberContext = true;
            ViewBag.orgname = Session["ActiveOrganization"] + " - Members";
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult PendingMemberGrid(CurrentOrg currorg)
        {
            DbUtil.Db.CurrentOrg = currorg;
            var m = new MemberModel(GroupSelectCode.Pending);
            UpdateModel(m.Pager);
            ViewBag.orgname = Session["ActiveOrganization"] + " - Pending Members";
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult InactiveMemberGrid(CurrentOrg currorg)
        {
            DbUtil.Db.CurrentOrg = currorg;
            var m = new MemberModel(GroupSelectCode.Inactive);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Inactive for {0}".Fmt(Session["ActiveOrganization"]));
            ViewBag.orgname = Session["ActiveOrganization"] + " - Inactive Members";
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult ProspectGrid(CurrentOrg currorg)
        {
            DbUtil.Db.CurrentOrg = currorg;
            var m = new MemberModel(GroupSelectCode.Prospect);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Prospects for {0}".Fmt(Session["ActiveOrganization"]));
            ViewBag.orgname = Session["ActiveOrganization"] + " - Prospects";
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult PrevMemberGrid(CurrentOrg currorg)
        {
            DbUtil.Db.CurrentOrg = currorg;
            var m = new PrevMemberModel();
            UpdateModel(m.Pager);
            ViewBag.orgname = Session["ActiveOrganization"] + " - Previous Members";
            DbUtil.LogActivity("Viewing Prev Members for {0}".Fmt(Session["ActiveOrganization"]));
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult VisitorGrid(int id, CurrentOrg currorg)
        {
            var m = new VisitorModel(id, currorg);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Visitors for {0}".Fmt(Session["ActiveOrganization"]));
            ViewBag.orgname = Session["ActiveOrganization"] + " - Guests";
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult MeetingGrid(int id, bool future)
        {
            var m = new MeetingsModel(id, future);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Meetings for {0}".Fmt(Session["ActiveOrganization"]));
            return PartialView(m);
        }

        [HttpPost]
        public ActionResult SettingsOrg(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult SettingsOrgEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult SettingsOrgUpdate(int id)
        {
            //DbUtil.Db.CurrentOrg.CheckId(id);
            var m = new OrganizationModel(id);
            UpdateModel(m);
            if (!m.org.LimitToRole.HasValue())
                m.org.LimitToRole = null;
            DbUtil.LogActivity("Update SettingsOrg {0}".Fmt(m.org.OrganizationName));
            if (ModelState.IsValid)
            {
                m.UpdateSchedules();
                DbUtil.Db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, m.org.OrgSchedules);
                return View("SettingsOrg", m);
            }
            return PartialView("SettingsOrgEdit", m);
        }

        [HttpPost]
        public ActionResult SettingsMeetings(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult SettingsMeetingsEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult SettingsMeetingsUpdate(int id)
        {
            var m = new OrganizationModel(id);
            m.schedules.Clear();

            UpdateModel(m);
            m.UpdateSchedules();
            DbUtil.Db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, m.org.OrgSchedules);
            DbUtil.LogActivity("Update SettingsMeetings {0}".Fmt(m.org.OrganizationName));
            return PartialView("SettingsMeetings", m);
        }

        [HttpPost]
        public ActionResult NewSchedule()
        {
            var s = new ScheduleInfo(
                new OrgSchedule
                {
                    SchedDay = 0,
                    SchedTime = DateTime.Parse("8:00 AM"),
                    AttendCreditId = 1
                });
            return PartialView("EditorTemplates/ScheduleInfo", s);
        }

        [HttpPost]
        public ActionResult OrgInfo(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult OrgInfoEdit(int id)
        {
            var m = new OrganizationModel(id);
            return PartialView(m);
        }
        [HttpPost]
        public ActionResult OrgInfoUpdate(int id)
        {
            var m = new OrganizationModel(id);
            UpdateModel(m);
            if (m.org.CampusId == 0)
                m.org.CampusId = null;
            if (m.org.OrganizationTypeId == 0)
                m.org.OrganizationTypeId = null;
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("Update OrgInfo {0}".Fmt(m.org.OrganizationName));
            return PartialView("OrgInfo", m);
        }

        private static Settings GetRegSettings(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var m = new Settings(org.RegSetting, DbUtil.Db, id);
            return m;
        }
        [HttpPost]
        public ActionResult OnlineRegAdmin(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegAdminEdit(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        public ActionResult OnlineRegAdminUpdate(int id)
        {
            var m = GetRegSettings(id);
            m.AgeGroups.Clear();
            DbUtil.LogActivity("Update OnlineRegAdmin {0}".Fmt(m.org.OrganizationName));
            try
            {
                UpdateModel(m);
                if (m.org.OrgPickList.HasValue() && m.org.RegistrationTypeId == RegistrationTypeCode.JoinOrganization)
                    m.org.OrgPickList = null;

                var os = new Settings(m.ToString(), DbUtil.Db, id);
                m.org.RegSetting = os.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("OnlineRegAdmin", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("OnlineRegAdminEdit", m);
            }
        }

        [HttpPost]
        public ActionResult OnlineRegQuestions(int id)
        {
            return View(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegQuestionsEdit(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        public ActionResult OnlineRegQuestionsUpdate(int id)
        {
            var m = GetRegSettings(id);
            DbUtil.LogActivity("Update OnlineRegQuestions {0}".Fmt(m.org.OrganizationName));
            m.AskItems.Clear();
            m.TimeSlots.list.Clear();
            try
            {
                if (!TryUpdateModel(m))
                {
                    var q = from e in ModelState.Values
                            where e.Errors.Count > 0
                            select e.Errors[0].ErrorMessage;
                    throw new Exception(q.First());
                }
                string s = m.ToString();
                m = new Settings(s, DbUtil.Db, id);
                m.org.RegSetting = m.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("OnlineRegQuestions", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return Content("error:" + ex.Message);
                //return View("OnlineRegQuestionsEdit", m);
            }
        }

        [HttpPost]
        public ActionResult OnlineRegFees(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegFeesEdit(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        public ActionResult OnlineRegFeesUpdate(int id)
        {
            var m = GetRegSettings(id);
            m.OrgFees.list.Clear();
            try
            {
                DbUtil.LogActivity("Update OnlineRegFees {0}".Fmt(m.org.OrganizationName));
                UpdateModel(m);
                var os = new Settings(m.ToString(), DbUtil.Db, id);
                m.org.RegSetting = os.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("OnlineRegFees", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("OnlineRegFeesEdit", m);
            }
        }

        [HttpPost]
        public ActionResult OnlineRegMessages(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegMessagesEdit(int id)
        {
            return PartialView(GetRegSettings(id));
        }
        [HttpPost]
        public ActionResult OnlineRegMessagesUpdate(int id)
        {
            var m = GetRegSettings(id);
            DbUtil.LogActivity("Update OnlineRegMessages {0}".Fmt(m.org.OrganizationName));
            //m.VoteTags.Clear();
            try
            {
                UpdateModel(m);
                var os = new Settings(m.ToString(), DbUtil.Db, id);
                m.org.RegSetting = os.ToString();
                DbUtil.Db.SubmitChanges();
                if (!m.org.NotifyIds.HasValue())
                    ModelState.AddModelError("Form", needNotify);
                return PartialView("OnlineRegMessages", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return PartialView("OnlineRegMessagesEdit", m);
            }
        }

        [HttpPost]
        public ActionResult NewMenuItem(string id)
        {
            return PartialView("EditorTemplates/MenuItem", new AskMenu.MenuItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewDropdownItem(string id)
        {
            return PartialView("EditorTemplates/DropdownItem", new AskDropdown.DropdownItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewCheckbox(string id)
        {
            return PartialView("EditorTemplates/CheckboxItem", new AskCheckboxes.CheckboxItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewGradeOption(string id)
        {
            return PartialView("EditorTemplates/GradeOption", new AskGradeOptions.GradeOption { Name = id });
        }
        [HttpPost]
        public ActionResult NewYesNoQuestion(string id)
        {
            return PartialView("EditorTemplates/YesNoQuestion", new AskYesNoQuestions.YesNoQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewSize(string id)
        {
            return PartialView("EditorTemplates/Size", new AskSize.Size { Name = id });
        }
        [HttpPost]
        public ActionResult NewExtraQuestion(string id)
        {
            return PartialView("EditorTemplates/ExtraQuestion", new AskExtraQuestions.ExtraQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewText(string id)
        {
            return PartialView("EditorTemplates/Text", new AskExtraQuestions.ExtraQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewOrgFee(string id)
        {
            return PartialView("EditorTemplates/OrgFee", new OrgFees.OrgFee { Name = id });
        }
        [HttpPost]
        public ActionResult NewAgeGroup()
        {
            return PartialView("EditorTemplates/AgeGroup", new Settings.AgeGroup());
        }
        [HttpPost]
        public ActionResult NewTimeSlot(string id)
        {
            return PartialView("EditorTemplates/TimeSlot", new TimeSlots.TimeSlot { Name = id });
        }

        [HttpPost]
        public ActionResult NewAsk(string id, string type)
        {
            var template = "EditorTemplates/" + type;
            switch (type)
            {
                case "AnswersNotRequired":
                case "AskSMS":
                case "AskEmContact":
                case "AskInsurance":
                case "AskDoctor":
                case "AskAllergies":
                case "AskTylenolEtc":
                case "AskParents":
                case "AskCoaching":
                case "AskChurch":
                    return PartialView(template, new Ask(type) { Name = id });
                case "AskCheckboxes":
                    return PartialView(template, new AskCheckboxes() { Name = id });
                case "AskDropdown":
                    return PartialView(template, new AskDropdown() { Name = id });
                case "AskMenu":
                    return PartialView(template, new AskMenu() { Name = id });
                case "AskSuggestedFee":
                    return PartialView(template, new AskSuggestedFee() { Name = id });
                case "AskSize":
                    return PartialView(template, new AskSize() { Name = id });
                case "AskRequest":
                    return PartialView(template, new AskRequest() { Name = id });
                case "AskHeader":
                    return PartialView(template, new AskHeader() { Name = id });
                case "AskInstruction":
                    return PartialView(template, new AskInstruction() { Name = id });
                case "AskTickets":
                    return PartialView(template, new AskTickets() { Name = id });
                case "AskYesNoQuestions":
                    return PartialView(template, new AskYesNoQuestions() { Name = id });
                case "AskExtraQuestions":
                    return PartialView(template, new AskExtraQuestions() { Name = id });
                case "AskText":
                    return PartialView(template, new AskText() { Name = id });
                case "AskGradeOptions":
                    return PartialView(template, new AskGradeOptions() { Name = id });
            }
            return Content("unexpected type " + type);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult SmallGroups()
        //{
        //    var m = new OrganizationModel(Util2.CurrentOrgId, Util2.CurrentGroups, Util2.CurrentGroupsMode);
        //    return View(m);
        //}
        [Authorize(Roles = "Edit")]
        public ActionResult CopySettings()
        {
            if (Util.SessionTimedOut() || DbUtil.Db.CurrentOrg.Id == 0)
                return Redirect("/");
            Session["OrgCopySettings"] = DbUtil.Db.CurrentOrg.Id;
            return Redirect("/OrgSearch/");
        }
        [HttpPost, Route("Join/{oid:int}/{pid:int}")]
        public ActionResult Join(int oid, int pid)
        {
            var org = DbUtil.Db.LoadOrganizationById(oid);
            if (org.AllowAttendOverlap != true)
            {
                var om = DbUtil.Db.OrganizationMembers.FirstOrDefault(mm =>
                    mm.OrganizationId != oid
                    && mm.MemberTypeId != 230 // inactive
                    && mm.MemberTypeId != 500 // inservice
                    && mm.Organization.AllowAttendOverlap != true
                    && mm.PeopleId == pid
                    && mm.Organization.OrgSchedules.Any(ss =>
                        DbUtil.Db.OrgSchedules.Any(os =>
                            os.OrganizationId == oid
                            && os.ScheduleId == ss.ScheduleId)));
                if (om != null)
                {
                    DbUtil.LogActivity("Same Hour Joining Org {0}({1})".Fmt(org.OrganizationName, pid));
                    return Content("Already a member of {0} at this hour".Fmt(om.OrganizationId));
                }
            }
            OrganizationMember.InsertOrgMembers(DbUtil.Db,
                oid, pid, MemberTypeCode.Member,
                DateTime.Now, null, false);
            DbUtil.Db.UpdateMainFellowship(oid);
            DbUtil.LogActivity("Joining Org {0}({1})".Fmt(org.OrganizationName, pid));
            return Content("ok");
        }
        [HttpPost, Route("AddProspect/{oid:int}/{pid:int}")]
        public ActionResult AddProspect(int oid, int pid)
        {
            var org = DbUtil.Db.LoadOrganizationById(oid);
            OrganizationMember.InsertOrgMembers(DbUtil.Db,
                oid, pid, MemberTypeCode.Prospect,
                DateTime.Now, null, false);
            DbUtil.LogActivity("Adding Prospect {0}({1})".Fmt(org.OrganizationName, pid));
            return Content("ok");
        }
        [HttpPost, Route("ShowVisitor/{oid:int}/{pid:int}/{ticks:long}/{show}")]
        public ActionResult ShowVisitor(int oid, int pid, long ticks, string show)
        {
			var dt = new DateTime(ticks); // ticks here is meeting time
            var attend = DbUtil.Db.Attends.SingleOrDefault(aa => aa.PeopleId == pid && aa.OrganizationId == oid && aa.MeetingDate == dt);
            if(attend == null)
                return Content("attendance not found");
            attend.NoShow = show.Equal("hide");
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("ShowVisitor {0},{1},{2},{3}".Fmt(oid, pid, attend.AttendId, show));
            return Content("ok");
        }
        [HttpPost, Route("ShowProspect/{oid:int}/{pid:int}/{show}")]
        public ActionResult ShowProspect(int oid, int pid, string show)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(aa => aa.OrganizationId == oid && aa.PeopleId == pid);
            if(om == null)
                return Content("member not found");
            om.Hidden = show.Equal("hide");
            DbUtil.Db.SubmitChanges();
            DbUtil.LogActivity("ShowProspect {0},{1},{2}".Fmt(oid, pid, show));
            return Content("ok");
        }

        [HttpPost]
        public ActionResult ToggleTag(int id)
        {
            var t = Person.ToggleTag(id, Util2.CurrentTagName, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
            DbUtil.Db.SubmitChanges();
            return Content(t ? "Remove" : "Add");
        }
        [HttpPost]
        public ContentResult TagAll(Guid id, string tagname, bool? cleartagfirst)
        {
            if (!tagname.HasValue())
                return Content("no tag name");
            DbUtil.Db.SetNoLock();
            var q = DbUtil.Db.PeopleQuery(id);
            if (Util2.CurrentTagName == tagname && !(cleartagfirst ?? false))
            {
                DbUtil.Db.TagAll(q);
                return Content("Remove");
            }
            var tag = DbUtil.Db.FetchOrCreateTag(tagname, Util.UserPeopleId, DbUtil.TagTypeId_Personal);
            if (cleartagfirst ?? false)
                DbUtil.Db.ClearTag(tag);
            DbUtil.Db.TagAll(q, tag);
            Util2.CurrentTag = tagname;
            DbUtil.Db.TagCurrent();
            return Content("Manage");
        }
        [HttpPost]
        public ContentResult UnTagAll(Guid id)
        {
            DbUtil.Db.SetNoLock();
            var q = DbUtil.Db.PeopleQuery(id);
            DbUtil.Db.UnTagAll(q);
            return Content("Add");
        }
        [HttpPost]
        public ActionResult AddContact(Guid id)
        {
            var cid = CmsData.Contact.AddContact(id);
            return Content("/Contact2/" + cid);
        }
        [HttpPost]
        public ActionResult AddTasks(Guid id)
        {
            var c = new ContentResult();
            c.Content = Task.AddTasks(DbUtil.Db, id).ToString();
            return c;
        }
        public ActionResult NotifyIds(int id, string field)
        {
            if (Util.SessionTimedOut())
                return Content("<script type='text/javascript'>window.onload = function() { parent.location = '/'; }</script>");
            Response.NoCache();
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            DbUtil.Db.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            DbUtil.Db.CurrentOrg.Id = id;
            DbUtil.Db.SubmitChanges();
            var o = DbUtil.Db.LoadOrganizationById(id);
            string notifyids = null;
            switch (field)
            {
                case "notifyids":
                    notifyids = o.NotifyIds;
                    break;
                case "giftnotifyids":
                    notifyids = o.GiftNotifyIds;
                    break;
            }
            var q = DbUtil.Db.PeopleFromPidString(notifyids).Select(p => p.PeopleId);
            foreach (var pid in q)
                t.PersonTags.Add(new TagPerson { PeopleId = pid });
            DbUtil.Db.SubmitChanges();
            return Redirect("/SearchUsers?ordered=true&topid=" + q.FirstOrDefault());
        }
        public ActionResult OrgPickList(int id)
        {
            if (Util.SessionTimedOut())
                return Content("<script type='text/javascript'>window.onload = function() { parent.location = '/'; }</script>");
            Response.NoCache();
            DbUtil.Db.CurrentOrg.Id = id;
            var o = DbUtil.Db.LoadOrganizationById(id);
            Session["orgPickList"] = (o.OrgPickList ?? "").Split(',').Select(oo => oo.ToInt()).ToList();
            return Redirect("/SearchOrgs/" + id);
        }
        [HttpPost]
        public ActionResult UpdateNotifyIds(int id, int topid, string field)
        {
            var t = DbUtil.Db.FetchOrCreateTag(Util.SessionId, Util.UserPeopleId, DbUtil.TagTypeId_AddSelected);
            var selected_pids = (from p in t.People(DbUtil.Db)
                                 orderby p.PeopleId == topid ? "0" : "1"
                                 select p.PeopleId).ToArray();
            var o = DbUtil.Db.LoadOrganizationById(id);
            var notifyids = string.Join(",", selected_pids);
            switch (field)
            {
                case "notifyids":
                    o.NotifyIds = notifyids;
                    break;
                case "giftnotifyids":
                    o.GiftNotifyIds = notifyids;
                    break;
            }
            DbUtil.Db.TagPeople.DeleteAllOnSubmit(t.PersonTags);
            DbUtil.Db.Tags.DeleteOnSubmit(t);
            DbUtil.Db.SubmitChanges();
            ViewBag.OrgId = id;
            ViewBag.field = field;
            var view = ViewExtensions2.RenderPartialViewToString2(this, "NotifyList2", notifyids);
            return Content(view);
            //return View("NotifyList2", notifyids);
        }
        [HttpPost]
        public ActionResult UpdateOrgIds(int id, string list)
        {
            var o = DbUtil.Db.LoadOrganizationById(id);
            DbUtil.Db.CurrentOrg.Id = id;
            var m = new Settings(o.RegSetting, DbUtil.Db, id);
            m.org = o;
            o.OrgPickList = list;
            DbUtil.Db.SubmitChanges();
            return PartialView("OrgPickList2", m);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult NewExtraValue(int id, string field, string value, bool multiline)
        {
            var m = new OrganizationModel(id);
            try
            {
                m.org.AddEditExtra(DbUtil.Db, field, value, multiline);
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception ex)
            {
                return Content("error: " + ex.Message);
            }
            return PartialView("ExtrasGrid", m.org);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult DeleteExtra(int id, string field)
        {
            var e = DbUtil.Db.OrganizationExtras.Single(ee => ee.OrganizationId == id && ee.Field == field);
            DbUtil.Db.OrganizationExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            var m = new OrganizationModel(id);
            return PartialView("ExtrasGrid", m.org);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ContentResult EditExtra(string id, string value)
        {
            var a = id.SplitStr("-", 2);
            var b = a[1].SplitStr(".", 2);
            var e = DbUtil.Db.OrganizationExtras.Single(ee => ee.OrganizationId == b[1].ToInt() && ee.Field == b[0]);
            e.Data = value;
            DbUtil.Db.SubmitChanges();
            return Content(value);
        }
        [HttpPost]
        public ActionResult Reminders(int id, bool? emailall)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var m = new CmsData.API.APIOrganization(DbUtil.Db);
            try
            {
                if (org.RegistrationTypeId == RegistrationTypeCode.ChooseVolunteerTimes)
                    m.SendVolunteerReminders(id, emailall ?? false);
                else
                    m.SendEventReminders(id);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
            return Content("ok");
        }
        public ActionResult DialogAdd(int id, string type)
        {
            ViewBag.OrgID = id;
            return View("DialogAdd" + type);
        }
        public ActionResult AddMESEvent(int id, string mesID)
        {
            OrganizationExtra ev;
            string[] mesEvent = mesID.Split('|');
            string name = "ministrEspace:" + mesEvent[0];
            ev = (from e in DbUtil.Db.OrganizationExtras
                  where e.Field == name
                  select e).SingleOrDefault();
            if (ev == null)
            {
                ev = new OrganizationExtra();
                ev.OrganizationId = id;
                ev.Field = name;
                ev.Data = mesEvent[1];
                DbUtil.Db.OrganizationExtras.InsertOnSubmit(ev);
            }
            else
            {
                ev.Data = mesEvent[1];
            }
            DbUtil.Db.SubmitChanges();
            return RedirectToAction("Index", "Organization", new { id = id });
        }

        [Route("GotoMeetingForDate/{oid:int}/{ticks:long}")]
        public ActionResult GotoMeetingForDate(int oid, long ticks)
        {
			var dt = new DateTime(ticks); // ticks here is meeting time
            var q = from m in DbUtil.Db.Meetings
                where m.OrganizationId == oid
                where m.MeetingDate == dt
                select m;
            var meeting = q.FirstOrDefault();
            if (meeting != null)
                return Redirect("/Meeting/" + meeting.MeetingId);
            return Message("no meeting at " + dt.FormatDateTm());
        }

        public ActionResult ReGenPaylinks(int id)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var q = from om in org.OrganizationMembers
                    select om;

            foreach (var om in q)
            {
                if (!om.TranId.HasValue) continue;
                var estr = HttpUtility.UrlEncode(Util.Encrypt(om.TranId.ToString()));
                var link = Util.ResolveServerUrl("/OnlineReg/PayAmtDue?q=" + estr);
                om.PayLink = link;
            }
            DbUtil.Db.SubmitChanges();
            return View(org);
        }
    }
}