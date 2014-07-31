using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Registration;
using UtilityExtensions;
using CmsWeb.Models.OrganizationPage;
using CmsData.Codes;

namespace CmsWeb.Areas.Main.Controllers
{
    [RouteArea("Main", AreaPrefix = "Organization"), Route("{action}/{id?}")]
    [ValidateInput(false)]
    [SessionExpire]
    public class OrganizationController : CmsStaffController
    {
        const string needNotify = "WARNING: please add the notify persons on messages tab.";

        [Route("~/Organization/{id:int}")]
        public ActionResult Index(int id)
        {
            if (Util2.CurrentOrgId != id)
            {
                Util2.CurrentGroups = null;
                Util2.CurrentGroupsPrefix = null;
                Util2.CurrentGroupsMode = 0;
            }

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
            }
            if (m.org.LimitToRole.HasValue())
                if (!User.IsInRole(m.org.LimitToRole))
                    return NotAllowed("no privilege to view ", m.org.OrganizationName);

            DbUtil.LogActivity("Viewing Organization ({0})".Fmt(m.org.OrganizationName), m.org.OrganizationName, orgid: id);

            Util2.CurrentOrgId = m.org.OrganizationId;
            ViewBag.OrganizationContext = true;
            ViewBag.orgname = m.org.FullName;
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

        [Authorize(Roles = "Admin")]
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
            Util2.CurrentOrgId = 0;
            Util2.CurrentGroups = null;
            Util2.CurrentGroupsPrefix = null;
            Util2.CurrentGroupsMode = 0;
            DbUtil.LogActivity("Delete Org {0}".Fmt(Session["ActiveOrganization"]));
            Session.Remove("ActiveOrganization");
            return Content("ok");
        }

        [HttpPost]
        public ActionResult NewMeeting(string d, string t, int AttendCredit, bool group)
        {
            var organization = DbUtil.Db.LoadOrganizationById(Util2.CurrentOrgId);
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
        public ActionResult CurrMemberGrid(int id, int[] smallgrouplist, int? selectmode, string namefilter, string sgprefix)
        {
            ViewBag.OrgMemberContext = true;
            Util2.CurrentGroups = smallgrouplist;
            Util2.CurrentGroupsPrefix = sgprefix;
            Util2.CurrentGroupsMode = selectmode ?? 0;
            ViewBag.orgname = Session["ActiveOrganization"] + " - Members";
            var m = new MemberModel(id, MemberModel.GroupSelect.Active, namefilter, sgprefix);
            UpdateModel(m.Pager);
            return View(m);
        }
        [HttpPost]
        public ActionResult PrevMemberGrid(int id, string namefilter, bool? ShowProspects)
        {
            var m = new PrevMemberModel(id, namefilter) { ShowProspects = ShowProspects ?? false };
            UpdateModel(m.Pager);
            ViewBag.orgname = Session["ActiveOrganization"] + " - Previous Members";
            DbUtil.LogActivity("Viewing Prev Members for {0}".Fmt(Session["ActiveOrganization"]));
            return View(m);
        }
        [HttpPost]
        public ActionResult VisitorGrid(int id, string namefilter)
        {
            var m = new VisitorModel(id, namefilter);
            ViewBag.orgname = Session["ActiveOrganization"] + " - Guests";
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Visitors for {0}".Fmt(Session["ActiveOrganization"]));
            return View("VisitorGrid", m);
        }
        [HttpPost]
        public ActionResult PendingMemberGrid(int id, string namefilter)
        {
            ViewBag.orgname = Session["ActiveOrganization"] + " - Pending Members";
            var m = new MemberModel(id, MemberModel.GroupSelect.Pending, namefilter);
            UpdateModel(m.Pager);
            return View(m);
        }
        [HttpPost]
        public ActionResult InactiveMemberGrid(int id, string namefilter)
        {
            ViewBag.orgname = Session["ActiveOrganization"] + " - Inactive Members";
            var m = new MemberModel(id, MemberModel.GroupSelect.Inactive, namefilter);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Inactive for {0}".Fmt(Session["ActiveOrganization"]));
            return View(m);
        }
        [HttpPost]
        public ActionResult ProspectGrid(int id, string namefilter)
        {
            ViewBag.orgname = Session["ActiveOrganization"] + " - Prospects";
            var m = new MemberModel(id, MemberModel.GroupSelect.Prospect, namefilter);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Prospects for {0}".Fmt(Session["ActiveOrganization"]));
            return View(m);
        }
        [HttpPost]
        public ActionResult MeetingGrid(int id, bool future)
        {
            var m = new MeetingModel(id, future);
            UpdateModel(m.Pager);
            DbUtil.LogActivity("Viewing Meetings for {0}".Fmt(Session["ActiveOrganization"]));
            return View(m);
        }

        [HttpPost]
        public ActionResult SettingsOrg(int id)
        {
            var m = new OrganizationModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult SettingsOrgEdit(int id)
        {
            var m = new OrganizationModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult SettingsOrgUpdate(int id)
        {
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
            return View("SettingsOrgEdit", m);
        }

        [HttpPost]
        public ActionResult SettingsMeetings(int id)
        {
            var m = new OrganizationModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult SettingsMeetingsEdit(int id)
        {
            var m = new OrganizationModel(id);
            return View(m);
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
            return View("SettingsMeetings", m);
            //return View("SettingsMeetingsEdit", m);
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
            return View("EditorTemplates/ScheduleInfo", s);
        }

        [HttpPost]
        public ActionResult OrgInfo(int id)
        {
            var m = new OrganizationModel(id);
            return View(m);
        }
        [HttpPost]
        public ActionResult OrgInfoEdit(int id)
        {
            var m = new OrganizationModel(id);
            return View(m);
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
            return View("OrgInfo", m);
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
            return View(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegAdminEdit(int id)
        {
            return View(GetRegSettings(id));
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
                return View("OnlineRegAdmin", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return View("OnlineRegAdminEdit", m);
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
            return View(GetRegSettings(id));
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
                return View("OnlineRegQuestions", m);
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
            return View(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegFeesEdit(int id)
        {
            return View(GetRegSettings(id));
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
                return View("OnlineRegFees", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return View("OnlineRegFeesEdit", m);
            }
        }

        [HttpPost]
        public ActionResult OnlineRegMessages(int id)
        {
            return View(GetRegSettings(id));
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ActionResult OnlineRegMessagesEdit(int id)
        {
            return View(GetRegSettings(id));
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
                return View("OnlineRegMessages", m);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Form", ex.Message);
                return View("OnlineRegMessagesEdit", m);
            }
        }

        [HttpPost]
        public ActionResult NewMenuItem(string id)
        {
            return View("EditorTemplates/MenuItem", new AskMenu.MenuItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewDropdownItem(string id)
        {
            return View("EditorTemplates/DropdownItem", new AskDropdown.DropdownItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewCheckbox(string id)
        {
            return View("EditorTemplates/CheckboxItem", new AskCheckboxes.CheckboxItem { Name = id });
        }
        [HttpPost]
        public ActionResult NewGradeOption(string id)
        {
            return View("EditorTemplates/GradeOption", new AskGradeOptions.GradeOption { Name = id });
        }
        [HttpPost]
        public ActionResult NewYesNoQuestion(string id)
        {
            return View("EditorTemplates/YesNoQuestion", new AskYesNoQuestions.YesNoQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewSize(string id)
        {
            return View("EditorTemplates/Size", new AskSize.Size { Name = id });
        }
        [HttpPost]
        public ActionResult NewExtraQuestion(string id)
        {
            return View("EditorTemplates/ExtraQuestion", new AskExtraQuestions.ExtraQuestion { Name = id });
        }
        [HttpPost]
        public ActionResult NewOrgFee(string id)
        {
            return View("EditorTemplates/OrgFee", new OrgFees.OrgFee { Name = id });
        }
        [HttpPost]
        public ActionResult NewAgeGroup()
        {
            return View("EditorTemplates/AgeGroup", new Settings.AgeGroup());
        }
        [HttpPost]
        public ActionResult NewTimeSlot(string id)
        {
            return View("EditorTemplates/TimeSlot", new TimeSlots.TimeSlot { Name = id });
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
                    return View(template, new Ask(type) { Name = id });
                case "AskCheckboxes":
                    return View(template, new AskCheckboxes() { Name = id });
                case "AskDropdown":
                    return View(template, new AskDropdown() { Name = id });
                case "AskMenu":
                    return View(template, new AskMenu() { Name = id });
                case "AskSuggestedFee":
                    return View(template, new AskSuggestedFee() { Name = id });
                case "AskSize":
                    return View(template, new AskSize() { Name = id });
                case "AskRequest":
                    return View(template, new AskRequest() { Name = id });
                case "AskHeader":
                    return View(template, new AskHeader() { Name = id });
                case "AskInstruction":
                    return View(template, new AskInstruction() { Name = id });
                case "AskTickets":
                    return View(template, new AskTickets() { Name = id });
                case "AskYesNoQuestions":
                    return View(template, new AskYesNoQuestions() { Name = id });
                case "AskExtraQuestions":
                    return View(template, new AskExtraQuestions() { Name = id });
                case "AskGradeOptions":
                    return View(template, new AskGradeOptions() { Name = id });
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
            if (Util.SessionTimedOut() || Util2.CurrentOrgId == 0)
                return Redirect("/");
            Session["OrgCopySettings"] = Util2.CurrentOrgId;
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
            Util2.CurrentOrgId = id;
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
            Util2.CurrentOrgId = id;
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
            var o = DbUtil.Db.LoadOrganizationById(Util2.CurrentOrgId);
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
            Util2.CurrentOrgId = id;
            var m = new Settings(o.RegSetting, DbUtil.Db, id);
            m.org = o;
            o.OrgPickList = list;
            DbUtil.Db.SubmitChanges();
            return View("OrgPickList2", m);
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
            return View("ExtrasGrid", m.org);
        }
        [HttpPost]
        [Authorize(Roles = "Edit")]
        public ViewResult DeleteExtra(int id, string field)
        {
            var e = DbUtil.Db.OrganizationExtras.Single(ee => ee.OrganizationId == id && ee.Field == field);
            DbUtil.Db.OrganizationExtras.DeleteOnSubmit(e);
            DbUtil.Db.SubmitChanges();
            var m = new OrganizationModel(id);
            return View("ExtrasGrid", m.org);
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