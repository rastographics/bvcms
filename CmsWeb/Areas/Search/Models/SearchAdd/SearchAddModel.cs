using CmsData;
using CmsData.Classes.RoleChecker;
using CmsData.Codes;
using CmsWeb.Code;
using CmsWeb.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Areas.Search.Models
{
    public class SearchAddModel : SearchResultsModel
    {
        private readonly string[] noaddtypes = { "relatedfamily", "mergeto", "contactor", "taskdelegate", "taskowner", "taskdelegate2", "addtoemail" };
        private readonly string[] onlyonetypes = { "taskdelegate", "taskowner", "taskdelegate2", "mergeto", "relatedfamily" };
        private Organization org;

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public SearchAddModel()
        {
            Init();
        }

        public SearchAddModel(CMSDataContext db) : base(db)
        {
            Init();
        }

        override protected void Init()
        {
            base.Init();
            PendingList = new List<PendingPersonModel>();
            DisplaySkipSearch = true;
            Fuzzy = true;
        }

        public SearchAddModel(CMSDataContext db, string context, string contextid, bool displaySkipSearch = true, bool isbusiness = false) : this(db)
        {
            DisplaySkipSearch = displaySkipSearch;
            AddContext = context;
            PrimaryKeyForContextType = contextid;
            CampusId = null;
            IsBusiness = isbusiness;
            switch (AddContext.ToLower())
            {
                case "addpeople":
                case "menu":
                    EntryPointId = 0;
                    break;
                case "addtotag":
                    EntryPointId = null;
                    break;
                case "mergeto":
                    EntryPointId = null;
                    break;
                case "family":
                case "relatedfamily":
                    EntryPointId = 0;
                    break;
                case "org":
                case "pending":
                case "prospect":
                case "inactive":
                    org = db.LoadOrganizationById(contextid.ToInt());
                    CampusId = org.CampusId;
                    EntryPointId = org.EntryPointId ?? 0;
                    break;
                case "visitor":
                case "registered":
                    org = (from meeting in db.Meetings
                           where meeting.MeetingId == contextid.ToInt()
                           select meeting.Organization).Single();
                    EntryPointId = org.EntryPointId ?? 0;
                    CampusId = org.CampusId;
                    break;
                case "contactee":
                    EntryPointId = 0;
                    break;
                case "contactor":
                    EntryPointId = 0;
                    break;
                case "contributor":
                    EntryPointId = 0;
                    break;
                case "addtoemail":
                    EntryPointId = 0;
                    break;
            }
        }

        public List<PendingPersonModel> PendingList
        {
            get;
            set;
        }

        public string DialogTitle
        {
            get
            {
                switch (AddContext.ToLower())
                {
                    case "addpeople":
                    case "menu":
                        return "Add to Database";
                    case "mergeto":
                        return "Merge Duplicate Into";
                    case "addtotag":
                        return "Add to Tag";
                    case "family":
                        return "Add to Family";
                    case "relatedfamily":
                        return "Add as Related Family";
                    case "org":
                        return "Add as Member of Organization";
                    case "prospect":
                        return "Add as Prospect of Organization";
                    case "pending":
                        return "Add as Pending Member of Organization";
                    case "inactive":
                        if (org == null)
                        {
                            org = CurrentDatabase.LoadOrganizationById(PrimaryKeyForContextType.ToInt());
                        }

                        return $"Add as {(org.IsMissionTrip == true ? "Sender of Mission Trip" : "Inactive Member of Organization")}";
                    case "visitor":
                        return "Add as Visitor to Meeting";
                    case "registered":
                        return "Add as Registered for Future Meeting";
                    case "contactee":
                        return "Add as Contactee for Ministry Contact";
                    case "contactor":
                        return "Add as Contactor for Ministry Contact";
                    case "contributor":
                        return "Add for a Gift";
                    case "taskdelegate":
                    case "taskdelegate2":
                        return "Delegate Task";
                    case "taskowner":
                        return "Transfer Task Ownership";
                    case "taskabout":
                        return "Change who Task is Regarding";
                    case "addtoemail":
                        return "Add To Email";
                }
                return "";
            }
        }

        public string PrimaryKeyForContextType { get; set; }

        public bool CanAdd => !noaddtypes.Contains(AddContext.ToLower());

        public bool OnlyOne => onlyonetypes.Contains(AddContext.ToLower());

        public new bool ShowLimitedSearch => RoleChecker.HasSetting(SettingName.LimitedSearchPerson, false);
        public bool Fuzzy { get; set; }
        public bool IsBusiness { get; set; }

        public int NewFamilyId { get; set; }
        public int? EntryPointId { get; set; }
        public int? CampusId { get; set; }
        public int Index { get; set; }
        public bool DisplaySkipSearch { get; set; }

        public int NextNewFamilyId()
        {
            NewFamilyId--;
            return NewFamilyId;
        }

        public PendingPersonModel NewPerson(int familyid, bool isbusiness = false)
        {
            var campuslist = CurrentDatabase.Setting("CampusRequired") ? "CampusNoNoCampus" : "Campus";
            var p = new PendingPersonModel(CurrentDatabase);
            if (isbusiness)
            {
                p.FamilyId = familyid;
                p.index = PendingList.Count;
                p.Campus = new CodeInfo(CampusId, campuslist);
                p.EntryPoint = new CodeInfo(EntryPointId, "EntryPoint");
                p.context = AddContext;
                p.IsBusiness = isbusiness;
                p.Gender = new CodeInfo(0, "Gender");
                p.MaritalStatus = new CodeInfo(0, "MaritalStatus");
                p.FirstName = "na";                
            }
            else
            {
                p.FamilyId = familyid;
                p.index = PendingList.Count;
                p.Gender = new CodeInfo(99, "Gender");
                p.MaritalStatus = new CodeInfo(99, "MaritalStatus");
                p.Campus = new CodeInfo(CampusId, campuslist);
                p.EntryPoint = new CodeInfo(EntryPointId, "EntryPoint");
                p.context = AddContext;
                p.IsBusiness = isbusiness;
            }

            if (familyid == 0)
            {
                p.FamilyId = NextNewFamilyId();
                p.IsNewFamily = true;
            }
            PendingList.Add(p);
            return p;
        }

        public static string AddRelatedFamily(CMSDataContext db, int peopleid, int relatedPersonId)
        {
            var p = db.LoadPersonById(peopleid);
            var rf = p.AddRelated(db, relatedPersonId);
            return $"#rf-{rf.FamilyId}-{rf.RelatedFamilyId}";
        }

        internal void AddExisting(int id)
        {
            var p = CurrentDatabase.LoadPersonById(id);
            var pp = new PendingPersonModel(CurrentDatabase);
            pp.CopyPropertiesFrom(p);
            pp.LoadAddress();
            PendingList.Add(pp);
        }

        internal dynamic CommitAdd()
        {
            var id = PrimaryKeyForContextType;
            var iid = PrimaryKeyForContextType.ToInt();
            try
            {
                switch (AddContext.ToLower())
                {
                    case "menu":
                    case "addpeople":
                        return AddPeople(OriginCode.MainMenu);
                    case "addtotag":
                        return AddPeopleToTag(id, 0);
                    case "family":
                        return AddFamilyMembers(iid, OriginCode.NewFamilyMember);
                    case "relatedfamily":
                        return AddRelatedFamilys(iid, OriginCode.NewFamilyMember);
                    case "org":
                        return AddOrgMembers(iid, OriginCode.Enrollment);
                    case "pending":
                        return AddOrgMembers(iid, OriginCode.Enrollment, pending: true);
                    case "inactive":
                        return AddOrgMembers(iid, OriginCode.Enrollment, MemberTypeCode.InActive);
                    case "prospect":
                    case "prospects":
                        return AddOrgMembers(iid, OriginCode.Enrollment, MemberTypeCode.Prospect);
                    case "visitor":
                        return AddVisitors(iid, OriginCode.Visit);
                    case "registered":
                        return AddRegistered(iid, OriginCode.Visit);
                    case "contactee":
                        return AddContactees(iid, OriginCode.Visit);
                    case "contactor":
                        return AddContactors(iid, 0);
                    case "contributor":
                        return AddContributor(iid, OriginCode.Contribution);
                    case "taskdelegate":
                        if (PendingList.Count > 0)
                        {
                            return new ReturnResult { close = true, how = "addselected", url = "/Task/Delegate/", pid = PendingList[0].PeopleId, from = AddContext };
                        }

                        break;
                    case "taskdelegate2":
                        if (PendingList.Count > 0)
                        {
                            return new ReturnResult { close = true, how = "addselected2", url = "/Task/Action/1", pid = PendingList[0].PeopleId, from = AddContext };
                        }

                        break;
                    case "taskabout":
                        if (PendingList.Count > 0)
                        {
                            return new ReturnResult { close = true, how = "addselected", url = "/Task/ChangeAbout/", pid = PendingList[0].PeopleId, from = AddContext };
                        }

                        break;
                    case "taskowner":
                        if (PendingList.Count > 0)
                        {
                            return new ReturnResult { close = true, how = "addselected", url = "/Task/ChangeOwner/", pid = PendingList[0].PeopleId, from = AddContext };
                        }

                        break;
                    case "mergeto":
                        if (PendingList.Count > 0)
                        {
                            return new ReturnResult { close = true, how = "addselected", pid = PrimaryKeyForContextType.ToInt(), pid2 = PendingList[0].PeopleId, from = AddContext };
                        }

                        break;
                    case "addtoemail":
                        if (PendingList.Count > 0)
                        {
                            var people = new List<ReturnResult>();
                            foreach (var p in PendingList)
                            {
                                string email = null;
                                if (!string.IsNullOrEmpty(p.Person.EmailAddress) && (p.Person.SendEmailAddress1 ?? true))
                                {
                                    email = p.Person.EmailAddress;
                                }

                                if (email == null && !string.IsNullOrEmpty(p.Person.EmailAddress2) && (p.Person.SendEmailAddress2 ?? false))
                                {
                                    email = p.Person.EmailAddress2;
                                }

                                people.Add(new ReturnResult { close = true, how = "addselected", pid = p.PeopleId, from = AddContext, name = p.Person.Name, email = email });
                            }
                            return people;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                return new ReturnResult { close = true, how = "addselected", error = ex.Message, from = AddContext };
            }
            return new ReturnResult { close = true, from = AddContext };
        }

        private ReturnResult AddContactees(int id, int origin)
        {
            var c = CurrentDatabase.Contacts.SingleOrDefault(ct => ct.ContactId == id);
            if (c != null)
            {
                foreach (var p in PendingList)
                {
                    AddPerson(p, PendingList, OriginCode.Visit, EntryPointId);
                    var ctee = c.contactees.SingleOrDefault(ct =>
                        ct.ContactId == id && ct.PeopleId == p.Person.PeopleId);
                    if (ctee == null)
                    {
                        ctee = new Contactee
                        {
                            ContactId = id,
                            PeopleId = p.Person.PeopleId
                        };
                        c.contactees.Add(ctee);
                        DbUtil.LogActivity("AddContactee " + id, peopleid: p.PeopleId);
                    }
                }
                CurrentDatabase.SubmitChanges();
            }
            return new ReturnResult { close = true, from = AddContext, cid = id };
        }

        private ReturnResult AddContactors(int id, int origin)
        {
            var c = CurrentDatabase.Contacts.SingleOrDefault(ct => ct.ContactId == id);
            if (c == null)
            {
                return new ReturnResult { close = true, how = "CloseAddDialog", from = AddContext };
            }

            foreach (var p in PendingList)
            {
                AddPerson(p, PendingList, origin, EntryPointId);
                var ctor = c.contactsMakers.SingleOrDefault(ct =>
                    ct.ContactId == id && ct.PeopleId == p.Person.PeopleId);
                if (ctor == null)
                {
                    ctor = new Contactor
                    {
                        ContactId = id,
                        PeopleId = p.Person.PeopleId
                    };
                    c.contactsMakers.Add(ctor);
                    DbUtil.LogActivity("AddContactor " + id, peopleid: p.PeopleId);
                }
            }
            CurrentDatabase.SubmitChanges();
            return new ReturnResult { close = true, cid = id, from = AddContext };
        }

        private ReturnResult AddFamilyMembers(int id, int origin)
        {
            if (id > 0)
            {
                var p = CurrentDatabase.LoadPersonById(id);

                foreach (var i in PendingList)
                {
                    var isnew = i.IsNew;
                    AddPerson(i, PendingList, origin, EntryPointId);
                    if (!isnew)
                    {
                        var fm = p.Family.People.SingleOrDefault(fa => fa.PeopleId == i.Person.PeopleId);
                        if (fm != null)
                        {
                            continue; // already a member of this family
                        }

                        if (i.Person.Age < 18)
                        {
                            i.Person.PositionInFamilyId = PositionInFamily.Child;
                        }
                        else if (p.Family.People.Count(per =>
                            per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2)
                        {
                            i.Person.PositionInFamilyId = PositionInFamily.PrimaryAdult;
                        }
                        else
                        {
                            i.Person.PositionInFamilyId = PositionInFamily.SecondaryAdult;
                        }

                        p.Family.People.Add(i.Person);
                    }
                    DbUtil.LogActivity("AddFamilyMembers " + p.FamilyId, i.PeopleId);
                }
                CurrentDatabase.SubmitChanges();
            }
            return new ReturnResult { pid = id, from = AddContext };
        }

        private ReturnResult AddRelatedFamilys(int id, int origin)
        {
            var p = PendingList[0];
            AddPerson(p, PendingList, origin, EntryPointId);
            var key = AddRelatedFamily(CurrentDatabase, id, p.PeopleId.Value);
            try
            {
                CurrentDatabase.SubmitChanges();
                DbUtil.LogActivity("AddRelatedFamily " + p.PeopleId, peopleid: p.PeopleId);
            }
            catch (Exception)
            {
                throw;
            }
            return new ReturnResult { from = AddContext, pid = id, key = key };
        }

        private ReturnResult AddPeople(int origin)
        {
            foreach (var p in PendingList)
            {
                AddPerson(p, PendingList, origin, EntryPointId);
            }

            CurrentDatabase.SubmitChanges();
            return new ReturnResult { pid = PendingList[0].PeopleId, from = AddContext };
        }

        private ReturnResult AddOrgMembers(int id, int origin, int membertypeid = MemberTypeCode.Member, bool pending = false)
        {
            string message = null;
            if (id > 0)
            {
                var org = CurrentDatabase.LoadOrganizationById(id);
                if (pending == false && PendingList.Count == 1 && org.AllowAttendOverlap != true)
                {
                    var om = CurrentDatabase.OrganizationMembers.FirstOrDefault(mm =>
                        mm.OrganizationId != id
                        && mm.MemberTypeId != 230 // inactive
                        && mm.MemberTypeId != 500 // inservice
                        && mm.Organization.AllowAttendOverlap != true
                        && mm.PeopleId == PendingList[0].PeopleId
                        && mm.Organization.OrgSchedules.Any(ss =>
                            CurrentDatabase.OrgSchedules.Any(os =>
                                os.OrganizationId == id
                                && os.ScheduleId == ss.ScheduleId)));
                    if (om != null)
                    {
                        message = ViewExtensions2.Markdown($@"
**Already a member of {om.OrganizationId} (orgid) with same schedule**

You can do one of these things:

* Drop the person from the other org first
* Use the 'move' feature to transfer them to the new org
* Use Allow Attendance Overlap, if appropriate
* See <a href=""https://docs.touchpointsoftware.com/Organizations/AlreadyAMember.html"" 
  title=""Already a Member"" target=""_blank"">this help article</a>
").ToString();
                        message = $@"<div style=""text-align: left"">{message}</div>";
                        return new ReturnResult { close = true, how = "CloseAddDialog", error = message, from = AddContext };
                    }
                }
                foreach (var p in PendingList)
                {
                    AddPerson(p, PendingList, origin, EntryPointId);
                    var om = OrganizationMember.InsertOrgMembers(CurrentDatabase,
                        id, p.PeopleId.Value, membertypeid, Util.Now, null, pending);
                    if (membertypeid == MemberTypeCode.InActive && org.IsMissionTrip == true)
                    {
                        om.AddToGroup(CurrentDatabase, "Sender");
                    }

                    if (om.CreatedDate.HasValue)
                    {
                        if ((DateTime.Now - om.CreatedDate.Value).TotalSeconds < 5)
                        {
                            var type = pending
                                ? "Pending"
                                : membertypeid == MemberTypeCode.InActive
                                    ? "Inactive"
                                    : membertypeid == MemberTypeCode.Prospect
                                        ? "Prospect"
                                        : "Member";
                            DbUtil.LogActivity($"Org{type} Add", om.OrganizationId, om.PeopleId);
                        }
                    }
                }
                CurrentDatabase.SubmitChanges();
                CurrentDatabase.UpdateMainFellowship(id);
            }
            return new ReturnResult { close = true, how = "rebindgrids", error = message, from = AddContext };
        }

        public ReturnResult AddContributor(int id, int origin)
        {
            var c = CurrentDatabase.Contributions.SingleOrDefault(cc => cc.ContributionId == id);
            if (c != null)
            {
                var p = PendingList[0];
                AddPerson(p, PendingList, origin, EntryPointId);

                var pushpayev = CurrentDatabase.PeopleExtras.SingleOrDefault(ev => ev.PeopleId == c.PeopleId && ev.Field == "PushPayKey");
                if (pushpayev != null && (c.PeopleId != p.PeopleId))
                {
                    // move the pushpay key with the contribution
                    CurrentDatabase.AddExtraValueDataIfNotExist(p.PeopleId, "PushPayKey", null, null, pushpayev.Data, null, null);
                    CurrentDatabase.PeopleExtras.DeleteOnSubmit(pushpayev);
                }
                c.PeopleId = p.PeopleId;

                if (c.BankAccount.HasValue())
                {
                    var ci = CurrentDatabase.CardIdentifiers.SingleOrDefault(k => k.Id == c.BankAccount);
                    if (ci == null)
                    {
                        ci = new CardIdentifier
                        {
                            Id = c.BankAccount,
                            CreatedOn = Util.Now
                        };
                        CurrentDatabase.CardIdentifiers.InsertOnSubmit(ci);
                    }
                    ci.PeopleId = p.PeopleId;
                }
                CurrentDatabase.SubmitChanges();
                DbUtil.LogActivity("AddContributor " + c.ContributionId, peopleid: p.PeopleId);
                return new ReturnResult { close = true, how = "addselected", cid = id, pid = p.PeopleId, name = p.Person.Name2, from = AddContext };
            }
            return new ReturnResult { close = true, how = "addselected", from = AddContext };
        }

        private ReturnResult AddPeopleToTag(string id, int origin)
        {
            if (id.HasValue())
            {
                foreach (var p in PendingList)
                {
                    AddPerson(p, PendingList, origin, EntryPointId);
                    Person.Tag(CurrentDatabase, p.Person.PeopleId, id, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                }
                CurrentDatabase.SubmitChanges();
                DbUtil.LogActivity("AddPeopleToTag");
            }
            return new ReturnResult { close = true, how = "addselected", from = AddContext };
        }

        private ReturnResult AddVisitors(int id, int origin)
        {
            var sb = new StringBuilder();
            var meeting = CurrentDatabase.Meetings.SingleOrDefault(me => me.MeetingId == id);
            if (meeting != null)
            {
                foreach (var p in PendingList)
                {
                    var isnew = p.IsNew;
                    AddPerson(p, PendingList, origin, EntryPointId);
                    if (isnew)
                    {
                        p.Person.UpdateValue("CampusId", meeting.Organization.CampusId);
                    }

                    if (!p.PeopleId.HasValue)
                    {
                        continue;
                    }

                    var err = Attend.RecordAttendance(p.PeopleId.Value, id, true);
                    DbUtil.LogActivity("AddVisitor", meeting.OrganizationId, p.PeopleId);
                    if (err != "ok")
                    {
                        sb.AppendLine(err);
                    }
                }
                CurrentDatabase.SubmitChanges();
                CurrentDatabase.UpdateMeetingCounters(meeting.MeetingId);
            }
            return new ReturnResult { close = true, how = "addselected", error = sb.ToString(), from = AddContext };
        }

        private ReturnResult AddRegistered(int id, int origin)
        {
            var meeting = CurrentDatabase.Meetings.SingleOrDefault(me => me.MeetingId == id);
            if (meeting != null)
            {
                foreach (var p in PendingList)
                {
                    var isnew = p.IsNew;
                    AddPerson(p, PendingList, origin, EntryPointId);
                    if (isnew)
                    {
                        p.Person.CampusId = meeting.Organization.CampusId;
                    }

                    if (!p.PeopleId.HasValue)
                    {
                        continue;
                    }

                    Attend.MarkRegistered(CurrentDatabase, p.PeopleId.Value, id, 1);
                    DbUtil.LogActivity("AddRegistered " + meeting.MeetingDate.FormatDateTm(), meeting.OrganizationId, p.PeopleId);
                }
                CurrentDatabase.SubmitChanges();
                CurrentDatabase.UpdateMeetingCounters(meeting.MeetingId);
            }
            return new ReturnResult { close = true, how = "addselected", from = AddContext };
        }

        private void AddPerson(PendingPersonModel p, List<PendingPersonModel> list, int originid, int? entrypointid)
        {
            if (p.IsNew)
            {
                p.AddPerson(originid, p.EntryPoint.Value.ToInt(), p.Campus.Value.ToInt(), p.IsBusiness);
            }
            else
            {
                if (entrypointid != 0 &&
                    (!p.Person.EntryPointId.HasValue || p.Person.EntryPointId == 0))
                {
                    p.Person.EntryPointId = entrypointid;
                }

                if (originid != 0 &&
                    (!p.Person.OriginId.HasValue || p.Person.OriginId == 0))
                {
                    p.Person.OriginId = originid;
                }

                CurrentDatabase.SubmitChanges();
            }
            if (p.FamilyId < 0) // fix up new family pointers
            {
                var q = from m in list
                        where m.FamilyId == p.FamilyId
                        select m;
                var list2 = q.ToList();
                foreach (var m in list2)
                {
                    m.FamilyId = p.Person.FamilyId;
                }
            }
            Util2.CurrentPeopleId = p.Person.PeopleId;
            Util.ActivePerson = p.Person.Name;
        }

        public class ReturnResult
        {
            public bool close { get; set; }
            public string how { get; set; }
            public string url { get; set; }
            public int? pid { get; set; }
            public int? pid2 { get; set; }
            public string from { get; set; }
            public string name { get; set; }
            public int? cid { get; set; }
            public string message { get; set; }
            public string error { get; set; }
            public string key { get; set; }
            public string email { get; set; }
        }
    }
}
