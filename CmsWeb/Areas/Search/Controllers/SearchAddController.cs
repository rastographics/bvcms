using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AttributeRouting;
using AttributeRouting.Web.Mvc;
using CmsData;
using CmsWeb.Areas.People.Models.Person;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Code;
using UtilityExtensions;
using System.Text.RegularExpressions;
using System.Data.Linq;
using CmsData.Codes;

namespace CmsWeb.Areas.Search.Controllers
{
    [RouteArea("Search", AreaUrl = "SearchAdd2")]
    public class SearchAddController : CmsStaffController
    {
        [POST("SearchAdd2/Dialog/{type}/{typeid?}")]
        public ActionResult Dialog(string type, string typeid)
        {
            var m = new SearchAddModel { typeid = typeid, type = type };
            Organization org = null;
            m.CampusId = null;
            switch (m.type.ToLower())
            {
                case "addpeople":
                case "menu":
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
                    org = DbUtil.Db.LoadOrganizationById(typeid.ToInt());
                    m.CampusId = org.CampusId;
                    m.EntryPointId = org.EntryPointId ?? 0;
                    break;
                case "visitor":
                case "registered":
                    org = (from meeting in DbUtil.Db.Meetings
                           where meeting.MeetingId == typeid.ToInt()
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
            return View("SearchPerson", m);
        }

        [POST("SearchAdd2/Results/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult Results(int? page, int? size, string sort, string dir, SearchAddModel m)
        {
            DbUtil.Db.SetNoLock();            
            m.Pager.Set("/SearchAdd2/Results", page ?? 1, size ?? 15, "na", "na");
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/ResultsFamily/{page?}/{size?}/{sort?}/{dir?}")]
        public ActionResult ResultsFamily(int? page, int? size, string sort, string dir, SearchAddModel m)
        {
            DbUtil.Db.SetNoLock();
            m.Pager.Set("/SearchAdd2/ResultsFamily", page ?? 1, size ?? 15, "na", "na");
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/SearchPerson")]
        public ActionResult SearchPerson(SearchAddModel m)
        {
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/SearchFamily")]
        public ActionResult SearchFamily(SearchAddModel m)
        {
            string first, last;
            Util.NameSplit(m.Name, out first, out last);
            m.Name = last;
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/CancelPerson/{id}")]
        public ActionResult CancelPerson(int id, SearchAddModel m)
        {
            m.List.RemoveAt(id);
            ModelState.Clear();
            if (m.List.Count > 0)
                return View("List", m);
            return View("SearchPerson", m);
        }

        [POST("SearchAdd2/Select/{id}")]
        public ActionResult Select(int id, SearchAddModel m)
        {
            if (m.List.AsEnumerable().Any(li => li.PeopleId == id))
                return View("List", m);

            var p = DbUtil.Db.LoadPersonById(id);
            var s = new SearchPersonModel
            {
                PeopleId = id,
                FamilyId = m.type == "family" ? m.typeid.ToInt() : p.FamilyId,
                First = p.FirstName,
                GoesBy = p.NickName,
                Last = p.LastName,
                MaritalStatus = new CodeInfo(p.MaritalStatusId, "MaritalStatus"),
                Email = p.EmailAddress,
                Suffix = p.SuffixCode,
                Title = new CodeInfo(p.TitleCode, "Title"),
                dob = p.DOB,
                Gender = new CodeInfo(p.GenderId, "Gender"),
                Phone = p.CellPhone,
                context = m.type,
                EntryPoint = new CodeInfo(p.EntryPointId, "EntryPoint"),
                Campus = new CodeInfo(p.CampusId, "Campus"),
                BeenValidated = true
            };
            s.LoadAddress();
            m.List.Add(s);
			if (m.OnlyOne)
				return CommitAdd(m);
            ModelState.Clear();
            return View("List", m);
        }

        [POST("SearchAdd2/NewPerson/{familyid}")]
        public ActionResult NewPerson(int familyid, SearchAddModel m)
        {
            m.NewPerson(familyid);
            ModelState.Clear();
            return View(m);
        }

        [POST("SearchAdd2/AddNewPerson/{noCheckDuplicate?}")]
        public ActionResult AddNewPerson(SearchAddModel m, string noCheckDuplicate)
        {
            var p = m.List[m.List.Count - 1];
            if(!noCheckDuplicate.HasValue())
                p.CheckDuplicate();
            if (!ModelState.IsValid || p.PotentialDuplicate.HasValue())
                return View("NewPerson", m);
            p.LoadAddress();
            p.BeenValidated = true;
            if(p.isNewFamily)
                return View("NewAddress", m);
            return View("List", m);
        }

        [POST("SearchAdd2/AddNewAddress/{NoCheck?}")]
        public ActionResult AddNewAddress(SearchAddModel m, string noCheck)
        {
            var p = m.List[m.List.Count - 1];
            if(noCheck.HasValue() == false)
                p.AddressInfo.ValidateAddress(ModelState);
            if (!ModelState.IsValid)
                return View("NewAddress", m);
            if (p.AddressInfo.Error.HasValue())
            {
                ModelState.Clear();
                return View("NewAddress", m);
            }
            return View("List", m);
        }

        [POST("SearchAdd2/CommitAdd")]
        public ActionResult CommitAdd(SearchAddModel m)
        {
            var id = m.typeid;
			var iid = m.typeid.ToInt();
            switch (m.type.ToLower())
            {
                case "menu":
                case "addpeople":
                    return AddPeople(m, OriginCode.MainMenu);
                case "addtotag":
                    return AddPeopleToTag(id, m, 0);
                case "family":
                    return AddFamilyMembers(iid, m, OriginCode.NewFamilyMember);
                case "relatedfamily":
                    return AddRelatedFamilys(iid, m, OriginCode.NewFamilyMember);
                case "org":
                    return AddOrgMembers(iid, m, false, OriginCode.Enrollment);
                case "pending":
                    return AddOrgMembers(iid, m, true, OriginCode.Enrollment);
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
                        return Json(new { close = true, how = "addselected", url = "/Task/Delegate/", pid = m.List[0].PeopleId, from = m.type });
                    break;
                case "taskdelegate2":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected2", url = "/Task/Action/", pid = m.List[0].PeopleId, from = m.type });
                    break;
                case "taskabout":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected", url = "/Task/ChangeAbout/", pid = m.List[0].PeopleId, from = m.type });
                    break;
                case "mergeto":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected", pid = m.List[0].PeopleId, from = m.type });
                    break;
                case "taskowner":
                    if (m.List.Count > 0)
                        return Json(new { close = true, how = "addselected", url = "/Task/ChangeOwner/", pid = m.List[0].PeopleId, from = m.type });
                    break;
            }
            return Json(new { close = true, from = m.type });
        }

        private JsonResult AddContactees(int id, SearchAddModel m, int origin)
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
            return Json(new { close = true, how = "addselected", from = m.type });
        }
        private JsonResult AddContactors(int id, SearchAddModel m, int origin)
        {
            if (id > 0)
            {
                var c = DbUtil.Db.Contacts.SingleOrDefault(ct => ct.ContactId == id);
                if (c == null)
                    return Json(new { close = true, how = "CloseAddDialog", from = m.type });
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
            return Json(new { close = true, how = "addselected", from = m.type });
        }
        private JsonResult AddFamilyMembers(int id, SearchAddModel m, int origin)
        {
            if (id > 0)
            {
                var p = DbUtil.Db.LoadPersonById(id);

                foreach (var i in m.List)
                {
                    var isnew = i.IsNew;
                    AddPerson(i, m.List, origin, m.EntryPointId);
                    if (!isnew)
                    {
                        var fm = p.Family.People.SingleOrDefault(fa => fa.PeopleId == i.person.PeopleId);
                        if (fm != null)
                            continue; // already a member of this family

                        if (i.person.Age < 18)
                            i.person.PositionInFamilyId = PositionInFamily.Child;
                        else if (p.Family.People.Count(per =>
                                    per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2)
                            i.person.PositionInFamilyId = PositionInFamily.PrimaryAdult;
                        else
                            i.person.PositionInFamilyId = PositionInFamily.SecondaryAdult;
                        p.Family.People.Add(i.person);
                    }
                }
                DbUtil.Db.SubmitChanges();
            }
            return Json(new { pid = id, from = m.type });
        }
        private JsonResult AddRelatedFamilys(int id, SearchAddModel m, int origin)
        {
            var p = m.List[0];
            AddPerson(p, m.List, origin, m.EntryPointId);
            var key = SearchAddModel.AddRelatedFamily(id, p.PeopleId.Value);
            try
            {
                DbUtil.Db.SubmitChanges();
            }
            catch (Exception)
            {
                throw;
            }
            return Json(new { from = m.type, pid = id, key });
        }
        private JsonResult AddPeople(SearchAddModel m, int origin)
        {
            foreach (var p in m.List)
                AddPerson(p, m.List, origin, m.EntryPointId);
            DbUtil.Db.SubmitChanges();
            return Json(new { pid = m.List[0].PeopleId, from = m.type });
        }
        private JsonResult AddOrgMembers(int id, SearchAddModel m, bool pending, int origin)
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
                        return Json(new { close = true, how = "CloseAddDialog", message = message, from = m.type });
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
            return Json(new { close = true, how = "rebindgrids", message = message, from = m.type });
        }
        private JsonResult AddContributor(int id, SearchAddModel m, int origin)
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
                return Json(new { close = true, how = "addselected", cid = id, pid = p.PeopleId, name = p.person.Name2, from = m.type });
            }
            return Json(new { close = true, how = "addselected", from = m.type });
        }
        private JsonResult AddPeopleToTag(string id, SearchAddModel m, int origin)
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
            return Json(new { close = true, how = "addselected", from = m.type });
        }
        private JsonResult AddVisitors(int id, SearchAddModel m, int origin)
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
            return Json(new { close = true, how = "addselected", error = sb.ToString(), from = m.type });
        }
        private JsonResult AddRegistered(int id, SearchAddModel m, int origin)
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
            return Json(new { close = true, how = "addselected", from = m.type });
        }
        private void AddPerson(SearchPersonModel p, IList<SearchPersonModel> list, int originid, int? entrypointid)
        {
            if (p.IsNew)
                p.AddPerson(originid, p.EntryPoint.Value.ToInt(), p.Campus.Value.ToInt());
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
