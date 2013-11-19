using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using CmsData.Codes;

namespace CmsWeb.Areas.Dialog.Controllers
{
    public class SearchAddController : CmsStaffController
    {
        public ActionResult Index(int? id, string type, string from)
        {
            var m = new SearchModel { typeid = id, type = type, from = from };
            Organization org = null;
            m.CampusId = null;
            switch (m.type)
            {
                case "addpeople":
                    m.EntryPointId = 0;
                    break;
                case "addtotag":
                    m.EntryPointId = null;
                    break;
                case "family":
                case "relatedfamily":
                    m.EntryPointId = 0;
                    break;
                case "org":
                case "pending":
                case "prospect":
                    org = DbUtil.Db.LoadOrganizationById(id);
                    m.CampusId = org.CampusId;
                    m.EntryPointId = org.EntryPointId ?? 0;
                    break;
                case "visitor":
                case "registered":
                    org = (from meeting in DbUtil.Db.Meetings
                           where meeting.MeetingId == id
                           select meeting.Organization).Single();
                    m.EntryPointId = org.EntryPointId ?? 0;
                    m.CampusId = org.CampusId;
                    break;
                case "contactee":
                    m.EntryPointId = 0;
                    break;
                case "contactor":
                    m.EntryPointId = 0;
                    break;
                case "contributor":
                    m.EntryPointId = 0;
                    break;
            }
            return View(m);
        }
        [HttpPost]
        public ActionResult Results(SearchModel m)
        {
            DbUtil.Db.SetNoLock();
            return View(m);
        }
        [HttpPost]
        public ActionResult ResultsFamily(SearchModel m)
        {
            DbUtil.Db.SetNoLock();
            return View(m);
        }
        [HttpPost]
        public ActionResult SearchPerson(SearchModel m)
        {
            return View(m);
        }
        [HttpPost]
        public ActionResult SearchFamily(SearchModel m)
        {
            m.dob = null;
            var a = m.name.SplitStr(" ");
            m.name = a[a.Length - 1];
            return View(m);
        }
        [HttpPost]
        public ActionResult SearchCancel(SearchModel m)
        {
            if (m.List.Count > 0)
                return View("List", m);
            return Complete("0", m);
        }
        [HttpPost]
        public ActionResult SearchFamilyCancel(SearchModel m)
        {
            return View("SearchPerson", m);
        }
        [HttpPost]
        public ActionResult PersonCancel(int id, SearchModel m)
        {
            m.List.RemoveAt(id);
            if (m.List.Count > 0)
                return View("List", m);
            return View("SearchPerson", m);
        }
        [HttpPost]
        public ActionResult Select(int id, SearchModel m)
        {
            if (m.List.AsEnumerable().Any(li => li.PeopleId == id))
                return View("List", m);

            var p = DbUtil.Db.LoadPersonById(id);
            var s = new SearchPersonModel
            {
                PeopleId = id,
                FamilyId = m.type == "family" ? m.typeid.Value : p.FamilyId,
                first = p.FirstName,
                goesby = p.NickName,
                last = p.LastName,
                marital = p.MaritalStatusId,
                email = p.EmailAddress,
                suffix = p.SuffixCode,
                title = p.TitleCode,
                dob = p.DOB,
                gender = p.GenderId,
                phone = p.CellPhone,
                context = m.type,
            };
            s.LoadFamily();
            m.List.Add(s);
			if (m.OnlyOne)
				return Complete(m.typeid.ToString(), m);
            return View("List", m);
        }

        [HttpPost]
        public ActionResult AddNewFamily(SearchModel m)
        {
            var p = m.List[m.List.Count - 1];
            p.ValidateModelForNew(ModelState, true);
            if (!ModelState.IsValid)
                return View("FormFull", m);
            return View("List", m);
        }
        [HttpPost]
        public ActionResult AddToFamily(SearchModel m)
        {
            var p = m.List[m.List.Count - 1];
            p.ValidateModelForNew(ModelState, false);
            if (!ModelState.IsValid)
                return View("FormAbbreviated", m);
            return View("List", m);
        }
        private SearchPersonModel NewPerson(int FamilyId, SearchModel m)
        {
            var p = new SearchPersonModel
            {
                FamilyId = FamilyId,
                index = m.List.Count,
                gender = 99,
                marital = 99,
                campus = m.CampusId,
                entrypoint = m.EntryPointId,
                context = m.type,
            };
#if DEBUG
            p.first = "David";
            p.last = "Carr." + DateTime.Now.Millisecond;
            p.gender = 0;
            p.marital = 0;
            p.dob = "na";
            p.email = "na";
            p.phone = "na";
            p.address = "na";
            p.zip = "na";
            p.homephone = "na";
#endif
            m.List.Add(p);
            return p;
        }
        [HttpPost]
        public ActionResult FormAbbreviated(int id, SearchModel m)
        {
            var org = DbUtil.Db.LoadOrganizationById(id);
            var p = NewPerson(id, m);
            if (id < 0)
            {
                var f = m.List.FirstOrDefault(fm => fm.FamilyId == id);
                p.address = f.address;
                p.city = f.city;
                p.state = f.state;
                p.zip = f.zip;
                p.homephone = f.homephone;
            }
            else
                p.LoadFamily();
            return View(m);
        }
        [HttpPost]
        public ActionResult FormFull(SearchModel m)
        {
            int id = 0;
            if (m.List.Count > 0)
                id = m.List.Min(i => i.FamilyId) - 1;
            if (id >= 0)
                id = -1;
            var p = NewPerson(id, m);
#if DEBUG
            //p.address = "235 Riveredge Cv";
            //p.city = "Cordova";
            //p.state = "TN";
            //p.zip = "38018";
            //p.homephone = "9017581862";
#endif
            return View(m);
        }
        [HttpPost]
        public ActionResult Complete(string id, SearchModel m)
        {
			var iid = id.ToInt();
            switch (m.type)
            {
                case "addpeople":
                    return AddPeople(m, OriginCode.MainMenu);
                case "addtotag":
                    return AddPeopleToTag(id, m, 0);
                case "family":
                    return AddFamilyMembers(iid, m, OriginCode.NewFamilyMember);
                case "relatedfamily":
                    return AddRelatedFamilys(iid, m, OriginCode.NewFamilyMember);
                case "org":
                    return AddOrgMembers(iid, m, OriginCode.Enrollment);
                case "pending":
                    return AddOrgMembers(iid, m, OriginCode.Enrollment, pending: true);
                case "prospect":
                    return AddProspects(iid, m, OriginCode.Enrollment);
                case "visitor":
                    return AddVisitors(iid, m, OriginCode.Visit);
                case "registered":
                    return AddRegistered(iid, m, OriginCode.Visit);
                case "contactee":
                    return AddContactees(iid, m, OriginCode.Visit);
                case "contactor":
                    return AddContactors(iid, m, 0);
                case "contributor":
                    return AddContributor(iid, m, OriginCode.Contribution);
                case "taskdelegate":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected", url="/Task/Delegate/", pid = m.List[0].PeopleId });
                    break;
                case "taskdelegate2":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected2", url = "/Task/Action/", pid = m.List[0].PeopleId });
                    break;
                case "taskabout":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected", url = "/Task/ChangeAbout/", pid = m.List[0].PeopleId });
                    break;
                case "mergeto":
                    if (m.List.Count > 0)
						return Json(new { close = true, how = "addselected", pid = m.List[0].PeopleId });
                    break;
                case "taskowner":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected", url = "/Task/ChangeOwner/", pid = m.List[0].PeopleId });
                    break;
            }
            return Json(new { close = true });
        }

        private JsonResult AddContactees(int id, SearchModel m, int origin)
        {
            if (id > 0)
            {
                var c = DbUtil.Db.Contacts.Single(ct => ct.ContactId == id);
                foreach (var p in m.List)
                {
                    AddPerson(p, m.List, OriginCode.Visit, m.EntryPointId);
                    var ctee = c.contactees.SingleOrDefault(ct =>
                        ct.ContactId == id && ct.PeopleId == p.person.PeopleId);
                    if (ctee == null)
                    {
                        ctee = new Contactee
                        {
                            ContactId = id,
                            PeopleId = p.person.PeopleId,
                        };
                        c.contactees.Add(ctee);
                    }
                }
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { close = true, how = "addselected" });
        }
        private JsonResult AddContactors(int id, SearchModel m, int origin)
        {
            if (id > 0)
            {
                var c = DbUtil.Db.Contacts.SingleOrDefault(ct => ct.ContactId == id);
                if (c == null)
                    return Json(new { close = true, how = "CloseAddDialog" });
                foreach (var p in m.List)
                {
                    AddPerson(p, m.List, origin, m.EntryPointId);
                    var ctor = c.contactsMakers.SingleOrDefault(ct =>
                        ct.ContactId == id && ct.PeopleId == p.person.PeopleId);
                    if (ctor == null)
                    {
                        ctor = new Contactor
                        {
                            ContactId = id,
                            PeopleId = p.person.PeopleId,
                        };
                        c.contactsMakers.Add(ctor);
                    }
                }
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { close = true, how = "addselected" });
        }
        private JsonResult AddFamilyMembers(int id, SearchModel m, int origin)
        {
            if (id > 0)
            {
                var f = DbUtil.Db.Families.Single(fa => fa.FamilyId == id);

                foreach (var i in m.List)
                {
                    var isnew = i.IsNew;
                    AddPerson(i, m.List, origin, m.EntryPointId);
                    if (!isnew)
                    {
                        var fm = f.People.SingleOrDefault(fa => fa.PeopleId == i.person.PeopleId);
                        if (fm != null)
                            continue; // already a member of this family

                        if (i.person.Age < 18)
                            i.person.PositionInFamilyId = PositionInFamily.Child;
                        else if (i.family.People.Count(per =>
                                    per.PositionInFamilyId == PositionInFamily.PrimaryAdult)
                                    < 2)
                            i.person.PositionInFamilyId = PositionInFamily.PrimaryAdult;
                        else
                            i.person.PositionInFamilyId = PositionInFamily.SecondaryAdult;
                        i.family.People.Add(i.person); // add selected person to target family
                    }
                }
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { close = true, how = "addselected" });
        }
        private JsonResult AddRelatedFamilys(int id, SearchModel m, int origin)
        {
            if (id > 0)
            {
                foreach (var p in m.List)
                {
                    AddPerson(p, m.List, origin, m.EntryPointId);
                    SearchModel.AddRelatedFamily(id, p.PeopleId.Value);
                }
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { close = true, how = "addselected" });
        }
        private JsonResult AddPeople(SearchModel m, int origin)
        {
            foreach (var p in m.List)
                AddPerson(p, m.List, origin, m.EntryPointId);
            DbUtil.Db.SubmitChanges();
            return Json(new { close = true, how = "CloseAddDialog" });
        }
        private JsonResult AddOrgMembers(int id, SearchModel m, int origin, bool pending = false)
        {
            string message = null;
            if (id > 0)
            {
                var org = DbUtil.Db.LoadOrganizationById(id);
                if (pending == false && m.List.Count == 1 && org.AllowAttendOverlap != true)
                {
                    var om = DbUtil.Db.OrganizationMembers.FirstOrDefault(mm => 
                        mm.OrganizationId != id
                        && mm.MemberTypeId != 230 // inactive
                        && mm.MemberTypeId != 500 // inservice
                        && mm.Organization.AllowAttendOverlap != true
                        && mm.PeopleId == m.List[0].PeopleId
                        && mm.Organization.OrgSchedules.Any(ss => 
                            DbUtil.Db.OrgSchedules.Any(os => 
                                os.OrganizationId == id 
                                && os.ScheduleId == ss.ScheduleId)));
                    if (om != null)
                    {
                        message = "Already a member of {0} (orgid) with same schedule".Fmt(om.OrganizationId);
                        return Json(new { close = true, how = "CloseAddDialog", message = message });
                    }
                }
                foreach (var p in m.List)
                {
                    AddPerson(p, m.List, origin, m.EntryPointId);
                    OrganizationMember.InsertOrgMembers(DbUtil.Db,
                        id, p.PeopleId.Value, 220, Util.Now, null, pending);
                }
                DbUtil.Db.SubmitChanges();
				DbUtil.Db.UpdateMainFellowship(id);
            }
            return Json(new { close = true, how = "rebindgrids", message = message });
        }
        private JsonResult AddProspects(int id, SearchModel m, int origin)
        {
            string message = null;
            if (id > 0)
            {
                foreach (var p in m.List)
                {
                    AddPerson(p, m.List, origin, m.EntryPointId);
                    OrganizationMember.InsertOrgMembers(DbUtil.Db,
                        id, p.PeopleId ?? 0, MemberTypeCode.Prospect, Util.Now, null, pending: false);
                }
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { close = true, how = "rebindgrids", message = message });
        }
        private JsonResult AddContributor(int id, SearchModel m, int origin)
        {
            if (id > 0)
            {
                var p = m.List[0];
                var c = DbUtil.Db.Contributions.Single(cc => cc.ContributionId == id);
                AddPerson(p, m.List, origin, m.EntryPointId);
                c.PeopleId = p.PeopleId;

                if (c.BankAccount.HasValue())
                {
                    var ci = DbUtil.Db.CardIdentifiers.SingleOrDefault(k => k.Id == c.BankAccount);
                    if (ci == null)
                    {
                        ci = new CardIdentifier
                        {
                            Id = c.BankAccount,
                            CreatedOn = Util.Now,
                        };
                        DbUtil.Db.CardIdentifiers.InsertOnSubmit(ci);
                    }
                    ci.PeopleId = p.PeopleId;
                }
                DbUtil.Db.SubmitChanges();
                return Json(new { close = true, how = "addselected", cid = id, pid = p.PeopleId, name = p.person.Name2 });
            }
            return Json(new { close = true, how = "addselected" });
        }
        private JsonResult AddPeopleToTag(string id, SearchModel m, int origin)
        {
            if (id.HasValue())
            {
                foreach (var p in m.List)
                {
                    AddPerson(p, m.List, origin, m.EntryPointId);
					Person.Tag(DbUtil.Db, p.person.PeopleId, id, Util2.CurrentTagOwnerId, DbUtil.TagTypeId_Personal);
                }
                DbUtil.Db.SubmitChanges();
            }
			return Json(new { close = true, how = "addselected" });
        }

        private JsonResult AddVisitors(int id, SearchModel m, int origin)
        {
            var sb = new StringBuilder();
            if (id > 0)
            {
                var meeting = DbUtil.Db.Meetings.SingleOrDefault(me => me.MeetingId == id);
                foreach (var p in m.List)
                {
                    var isnew = p.IsNew;
                    AddPerson(p, m.List, origin, m.EntryPointId);
					if (isnew)
						p.person.UpdateValue("CampusId", meeting.Organization.CampusId);
                    var err = Attend.RecordAttendance(p.PeopleId.Value, id, true);
                    if (err != "ok")
                        sb.AppendLine(err);
                }
                DbUtil.Db.SubmitChanges();
                DbUtil.Db.UpdateMeetingCounters(meeting.MeetingId);
            }
            return Json(new { close = true, how = "addselected", error = sb.ToString() });
        }
        private JsonResult AddRegistered(int id, SearchModel m, int origin)
        {
            if (id > 0)
            {
                var meeting = DbUtil.Db.Meetings.SingleOrDefault(me => me.MeetingId == id);
                foreach (var p in m.List)
                {
                    var isnew = p.IsNew;
                    AddPerson(p, m.List, origin, m.EntryPointId);
                    if (isnew)
                        p.person.CampusId = meeting.Organization.CampusId;
                    Attend.MarkRegistered(DbUtil.Db, p.PeopleId.Value, id, 1);
                }
                DbUtil.Db.SubmitChanges();
                DbUtil.Db.UpdateMeetingCounters(meeting.MeetingId);
            }
            return Json(new { close = true, how = "addselected" });
        }
        private void AddPerson(SearchPersonModel p, IList<SearchPersonModel> list, int originid, int? entrypointid)
        {
            if (p.IsNew)
                p.AddPerson(originid, p.entrypoint, p.campus);
            else 
            {
                if (entrypointid != 0 && 
                        (!p.person.EntryPointId.HasValue || p.person.EntryPointId == 0))
                    p.person.EntryPointId = entrypointid;
                if (originid != 0 &&
                        (!p.person.OriginId.HasValue || p.person.OriginId == 0))
                    p.person.OriginId = originid;
                DbUtil.Db.SubmitChanges();
            }
            if (p.FamilyId < 0) // fix up new family pointers
            {
                var q = from m in list
                        where m.FamilyId == p.FamilyId
                        select m;
                var list2 = q.ToList();
                foreach (var m in list2)
                    m.FamilyId = p.person.FamilyId;
            }
            Util2.CurrentPeopleId = p.person.PeopleId;
            Session["ActivePerson"] = p.person.Name;
        }
    }
}
