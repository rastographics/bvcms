using System;
using System.Linq;
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
            var meeting = Meeting.FetchOrCreateMeeting(DbUtil.Db, id, datetime);
            return new RollListResult(meeting);
        }
        [HttpPost]
        public ActionResult RecordAttend( int id, int PeopleId, bool Present )
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
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

            CmsData.Family f;
            if (m.addtofamilyid > 0)
                f = DbUtil.Db.Families.First(fam => fam.People.Any(pp => pp.PeopleId == m.addtofamilyid));
            else
                f = new CmsData.Family();

            if (m.goesby == "(Null)")
                m.goesby = null;
            var position = PositionInFamily.Child;
            if (m.dob.Age0() >= 18)
                if (f.People.Count(per =>
                     per.PositionInFamilyId == PositionInFamily.PrimaryAdult)
                     < 2)
                    position = PositionInFamily.PrimaryAdult;
                else
                    position = PositionInFamily.SecondaryAdult;

            var p = Person.Add(f, position,
                null, Trim(m.first), Trim(m.goesby), Trim(m.last), m.dob, false, m.gender,
                    OriginCode.Visit, null);

            var z = DbUtil.Db.ZipCodes.SingleOrDefault(zc => zc.Zip == m.zip.Zip5());
            if (!m.home.HasValue() && m.cell.HasValue())
                m.home = m.cell;

			if (m.addtofamilyid == 0)
			{
				p.Family.HomePhone = m.home.GetDigits();
				p.Family.AddressLineOne = m.addr;
				p.Family.CityName = z != null ? z.City : null;
				p.Family.StateCode = z != null ? z.State : null;
				p.Family.ZipCode = m.zip;
			}
        	p.EmailAddress = Trim(m.email);
            if (m.cell.HasValue())
                p.CellPhone = m.cell.GetDigits();
            p.MaritalStatusId = m.marital;
            p.GenderId = m.gender;
            DbUtil.Db.SubmitChanges(); 
            DbUtil.LogActivity("iPhone AddPerson {0}".Fmt(p.PeopleId));
            var meeting = DbUtil.Db.Meetings.Single(mm => mm.MeetingId == id);
            Attend.RecordAttendance(p.PeopleId, id, true);
            DbUtil.Db.UpdateMeetingCounters(id);
            return new RollListResult(meeting, p.PeopleId);
        }
        [HttpPost]
        public ActionResult JoinUnJoinOrg(int PeopleId, int OrgId, bool Member)
        {
			if (!AccountModel.AuthenticateMobile())
                return Content("not authorized");
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == PeopleId && m.OrganizationId == OrgId);
            if (om == null && Member)
                om = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                    OrgId, PeopleId, MemberTypeCode.Member, DateTime.Now, null, false);
            else if (om != null && !Member)
                om.Drop(DbUtil.Db, addToHistory: true);
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
