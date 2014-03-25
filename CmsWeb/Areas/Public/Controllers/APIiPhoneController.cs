using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;
using CmsWeb.Models.iPhone;
using CmsData.Codes;
using SearchModel = CmsWeb.Models.iPhone.SearchModel;

namespace CmsWeb.Areas.Public.Controllers
{
    [ValidateInput(false)]
    public class APIiPhoneController : CmsController
    {
        public ActionResult FetchImage(int id)
        {
            if (!AccountModel.AuthenticateMobile("Access"))
                return Content("not authorized");
			Response.NoCache();
            var person = DbUtil.Db.People.Single(pp => pp.PeopleId == id);
            if (person.PictureId != null)
                return new CmsWeb.Models.ImageResult(person.Picture.MediumId ?? 0);
            return new CmsWeb.Models.ImageResult(0);
        }
        public ActionResult Search(string name, string comm, string addr)
        {
			if (!AccountModel.AuthenticateMobile(checkorgmembersonly: true))
                return Content("not authorized");
			Response.NoCache();

            var m = new SearchModel(name, comm, addr);
            return new SearchResult0(m.PeopleList(), m.Count);
        }
        public ActionResult SearchResults(string name, string comm, string addr)
        {
            if (!AccountModel.AuthenticateMobile(checkorgmembersonly: true))
                return Content("not authorized");
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Access"))
                return Content("not authorized");
            Response.NoCache();

            DbUtil.LogActivity("iphone search '{0}'".Fmt(name));
            var m = new SearchModel(name, comm, addr);
            return new SearchResult(m.PeopleList(), m.Count);
        }
        public ActionResult DetailResults(int id)
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
			Response.NoCache();
            DbUtil.LogActivity("iphone view ({0})".Fmt(id));
            return new DetailResult(id);
        }
        public ActionResult Organizations()
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
			Response.NoCache();
			if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return new OrgResult(null);
            return new OrgResult(Util.UserPeopleId);
        }
        [HttpPost]
        public ActionResult RollList( int id, DateTime datetime )
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
			var u = DbUtil.Db.Users.Single(uu => uu.Username == AccountModel.UserName2);
            DbUtil.LogActivity("iphone RollList {0} {1:g}".Fmt(id, datetime));
            var meeting = Meeting.FetchOrCreateMeeting(DbUtil.Db, id, datetime);
            return new RollListResult(meeting);
        }
        [HttpPost]
        public ActionResult RecordAttend( int id, int PeopleId, bool Present )
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
            DbUtil.LogActivity("iphone attend(org:{0} person:{1} {2})".Fmt(id, PeopleId, Present));
            Attend.RecordAttendance(PeopleId, id, Present);
            DbUtil.Db.UpdateMeetingCounters(id);
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult RecordVisit( int id, int PeopleId )
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
            Attend.RecordAttendance(PeopleId, id, true);
            DbUtil.Db.UpdateMeetingCounters(id);
            var meeting = DbUtil.Db.Meetings.Single(mm => mm.MeetingId == id);
            return new RollListResult(meeting);
        }
        public class PersonInfo
        {
            public int addtofamilyid { get; set; }
            public string addr { get; set; }
            public string zip { get; set; }
            public string first { get; set; }
            public string last { get; set; }
            public string goesby { get; set; }
            public string dob { get; set; }
            public string email { get; set; }
            public string cell { get; set; }
            public string home { get; set; }
            public int marital { get; set; }
            public int gender { get; set; }
        }
        [HttpPost]
        public ActionResult AddPerson(int id, PersonInfo m)
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");

            var f = m.addtofamilyid > 0 
                ? DbUtil.Db.Families.First(fam => fam.People.Any(pp => pp.PeopleId == m.addtofamilyid)) 
                : new CmsData.Family();

            if (m.goesby == "(Null)")
                m.goesby = null;
            var position = PositionInFamily.Child;
            if (m.dob.Age0() >= 18)
                position = f.People.Count(per => per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2 
                    ? PositionInFamily.PrimaryAdult 
                    : PositionInFamily.SecondaryAdult;

            var p = Person.Add(f, position,
                null, Trim(m.first), Trim(m.goesby), Trim(m.last), m.dob, false, m.gender,
                    OriginCode.Visit, null);

            DbUtil.LogActivity("iPhone AddPerson {0}".Fmt(p.PeopleId));
            UpdatePerson(p, m);
            var meeting = DbUtil.Db.Meetings.Single(mm => mm.MeetingId == id);
            Attend.RecordAttendance(p.PeopleId, id, true);
            DbUtil.Db.UpdateMeetingCounters(id);
            return new RollListResult(meeting, p.PeopleId);
        }
        private void UpdatePerson(Person p, PersonInfo m)
        {
            var psb = new List<ChangeDetail>();
            var fsb = new List<ChangeDetail>();
            var z = DbUtil.Db.ZipCodes.SingleOrDefault(zc => zc.Zip == m.zip.Zip5());
            if (!m.home.HasValue() && m.cell.HasValue())
                m.home = m.cell;

            var keys = Request.Form.AllKeys.ToList();

            if (keys.Contains("home"))
                p.Family.UpdateValue(fsb, "HomePhone", m.home.GetDigits());
            if (keys.Contains("addr"))
                p.Family.UpdateValue(fsb, "AddressLineOne", m.addr);
            if (keys.Contains("zip"))
            {
                p.Family.UpdateValue(fsb, "CityName", z != null ? z.City : null);
                p.Family.UpdateValue(fsb, "StateCode", z != null ? z.State : null);
                p.Family.UpdateValue(fsb, "ZipCode", m.zip);
                var rc = DbUtil.Db.FindResCode(m.zip, null);
                p.Family.UpdateValue(fsb, "ResCodeId", rc.ToString());
            }
            if (keys.Contains("goesby"))
                p.UpdateValue(psb, "NickName", Trim(m.goesby));
            if (keys.Contains("first"))
                p.UpdateValue(psb, "FirstName", Trim(m.first));
            if (keys.Contains("last"))
                p.UpdateValue(psb, "LastName", Trim(m.last));
            if (keys.Contains("dob"))
            {
                DateTime dt;
                DateTime.TryParse(m.dob, out dt);
                if (p.BirthDate != dt)
                    p.UpdateValue(psb, "DOB", m.dob);
            }
            if (keys.Contains("email"))
                p.UpdateValue(psb, "EmailAddress", Trim(m.email));
            if (keys.Contains("cell"))
                p.UpdateValue(psb, "CellPhone", m.cell.GetDigits());
            if (keys.Contains("marital"))
                p.UpdateValue(psb, "MaritalStatusId", m.marital);
            if (keys.Contains("gender"))
                p.UpdateValue(psb, "GenderId", m.gender);

            p.LogChanges(DbUtil.Db, psb);
            p.Family.LogChanges(DbUtil.Db, fsb, p.PeopleId, Util.UserPeopleId ?? 0);
            DbUtil.Db.SubmitChanges();
            if (!DbUtil.Db.Setting("NotifyCheckinChanges", "true").ToBool() || (psb.Count <= 0 && fsb.Count <= 0))
                return;
            var sb = new StringBuilder();
            foreach (var c in psb)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
            foreach (var c in fsb)
                sb.AppendFormat("<tr><td>{0}</td><td>{1}</td><td>{2}</td></tr>\n", c.Field, c.Before, c.After);
            DbUtil.Db.EmailRedacted(p.FromEmail, DbUtil.Db.GetNewPeopleManagers(),
                "Basic Person Info Changed during checkin on " + Util.Host,
                "{0} changed the following information for {1} ({2}):<br />\n<table>{3}</table>"
                    .Fmt(Util.UserName, p.PreferredName, p.LastName, sb.ToString()));
        }
        [HttpPost]
        public ActionResult JoinUnJoinOrg(int PeopleId, int OrgId, bool Member)
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == PeopleId && m.OrganizationId == OrgId);
            if (om == null && Member)
            {
                om = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                    OrgId, PeopleId, MemberTypeCode.Member, DateTime.Now, null, false);
                DbUtil.LogActivity("iphone join(org:{0} person:{1})".Fmt(OrgId, PeopleId));
            }
            else if (om != null && !Member)
            {
                om.Drop(DbUtil.Db, addToHistory: true);
                DbUtil.LogActivity("iphone drop(org:{0} person:{1})".Fmt(OrgId, PeopleId));
            }
            DbUtil.Db.SubmitChanges();
            return Content("OK");
        }
        private static string Trim(string s)
        {
        	return s.HasValue() ? s.Trim() : s;
        }

		[HttpPost]
        public ActionResult RollList2(int id, DateTime datetime)
            // id = OrganizationId
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
            return new RollListResult(id, datetime);
        }
        [HttpPost]
        public ActionResult RecordAttend2(int id, DateTime datetime, int PeopleId, bool Present)
            // id = OrganizationId
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
			var u = DbUtil.Db.Users.Single(uu => uu.Username == AccountModel.UserName2);
            RecordAttend2Extracted(id, PeopleId, Present, datetime, u);
            return new EmptyResult();
        }
        [HttpPost]
        public ActionResult RecordVisit2(int id, DateTime datetime, int PeopleId)
            // id = OrganizationId
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
			var u = DbUtil.Db.Users.Single(uu => uu.Username == AccountModel.UserName2);

            RecordAttend2Extracted(id, PeopleId, true, datetime, u);
            return new RollListResult(id, datetime);
        }
        private static void RecordAttend2Extracted(int id, int PeopleId, bool Present, DateTime dt, User u)
        {
            var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.OrganizationId == id && m.MeetingDate == dt);
            if (meeting == null)
            {
                meeting = new CmsData.Meeting
                {
                    OrganizationId = id,
                    MeetingDate = dt,
                    CreatedDate = Util.Now,
                    CreatedBy = u.UserId,
                    GroupMeetingFlag = false,
                };
                DbUtil.Db.Meetings.InsertOnSubmit(meeting);
                DbUtil.Db.SubmitChanges();
                var acr = (from s in DbUtil.Db.OrgSchedules
                               where s.OrganizationId == id
                               where s.SchedTime.Value.TimeOfDay == dt.TimeOfDay
                               where s.SchedDay == (int)dt.DayOfWeek
                               select s.AttendCreditId).SingleOrDefault();
				meeting.AttendCreditId = acr;
            }
            Attend.RecordAttendance(PeopleId, meeting.MeetingId, Present);
            DbUtil.Db.UpdateMeetingCounters(id);
            DbUtil.LogActivity("Mobile RecAtt o:{0} p:{1} u:{2} a:{3}".Fmt(meeting.OrganizationId, PeopleId, Util.UserPeopleId, Present));
//            var n = DbUtil.Db.Attends.Count(a => a.MeetingId == meeting.MeetingId && a.AttendanceFlag == true);
//            if (n == 0)
//            {
//                DbUtil.Db.Meetings.DeleteOnSubmit(meeting);
//                DbUtil.Db.SubmitChanges();
//            }
        }
    }
}
