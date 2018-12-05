using CmsData;
using CmsData.Codes;
using CmsData.View;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Xml.Linq;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class CheckInModel
    {
        public CheckInModel() { }
        public string GetNextPrintJobs(string kiosks)
        {
            var a = (kiosks ?? "unknown").Replace(" ", "").Split(',');
            var q = from d in DbUtil.Db.PrintJobs
                    where a.Contains(d.Id)
                    orderby d.Stamp
                    select d;
            var x = XDocument.Parse("<PrintJobs><jobs /></PrintJobs>");
            foreach (var j in q)
            {
                var d = XDocument.Parse(j.Data);
                var jobs = (XElement)x.Root.FirstNode;
                jobs.Add(d.Root);
            }
            DbUtil.Db.PrintJobs.DeleteAllOnSubmit(q);
            DbUtil.Db.SubmitChanges();
            return x.ToString();
        }

        public void SavePrintJob(string kiosk, string xml)
        {
            if (!kiosk.HasValue())
            {
                return;
            }

            var d = new PrintJob { Id = kiosk.Replace(" ", ""), Data = xml, Stamp = DateTime.Now };
            DbUtil.Db.PrintJobs.InsertOnSubmit(d);
            DbUtil.Db.SubmitChanges();
        }

        public List<CheckinMatch> MatchOld(string id)
        {
            id = id.GetDigits();
            var ph = id.PadLeft(10, '0');
            var p7 = ph.Substring(3);
            var ac = ph.Substring(0, 3);
            var q1 = from f in DbUtil.Db.Families
                     where f.HeadOfHousehold.DeceasedDate == null
                     where f.HomePhoneLU.StartsWith(p7)
                           || f.People.Any(p => p.CellPhoneLU.StartsWith(p7)
                                                || p.PeopleExtras.Any(ee => ee.Data == id && ee.Field == "PIN"))
                     let flock = f.FamilyCheckinLocks
                         .FirstOrDefault(l => SqlMethods.DateDiffSecond(l.Created, DateTime.Now) < 60)
                     orderby f.FamilyId
                     select new CheckinMatch
                     {
                         Familyid = f.FamilyId,
                         Areacode = f.HomePhoneAC,
                         Name = f.HeadOfHousehold.Name,
                         Phone = id,
                         Locked = flock == null ? false : flock.Locked
                     };
            var matches = q1.ToList();
            if (matches.Count > 1)
            {
                matches = matches.Where(m => m.Areacode == ac || ac == "000").ToList();
            }

            return matches;
        }

        public List<CheckinMatch> Find(string id)
        {
            var q2 = from kc in DbUtil.Db.CardIdentifiers
                     where kc.Id == id
                     select new CheckinMatch
                     {
                         Familyid = kc.Person.FamilyId,
                         Areacode = kc.Person.Family.HomePhoneAC,
                         Name = kc.Person.Family.HeadOfHousehold.Name,
                         Phone = id
                     };
            var matches = q2.ToList();
            if (matches.Count > 0)
            {
                return matches;
            }

            if (DbUtil.Db.Setting("UseOldCheckinMatch", "false").ToBool())
            {
                return MatchOld(id);
            }

            return DbUtil.Db.CheckinMatch(id).ToList();
        }

        public List<CheckinFamilyMember> FamilyMembers(int id, int campus, int thisday)
        {
            DbUtil.LogActivity($"CheckinFamily({id},{campus},{thisday})");
            var list = (from a in DbUtil.Db.CheckinFamilyMembers(id, campus, thisday).ToList()
                        orderby a.Position, a.Position == 10 ? a.Gender : "U", a.Age
                        select a).ToList();
            return list;
        }

        public List<CheckinFamilyMember> FamilyMembersOld(int id, int campus, int thisday)
        {
            var normalLabelsMemTypes = new[]
            {
                MemberTypeCode.Member,
                MemberTypeCode.InActive
            };
            var now = Util.Now;
            // get org members first
            var members =
                from om in DbUtil.Db.OrganizationMembers
                let meetingHours = DbUtil.Db.GetTodaysMeetingHours(om.OrganizationId, thisday)
                let recreg = om.Person.RecRegs.FirstOrDefault()
                where om.Organization.CanSelfCheckin.Value
                where (om.Pending ?? false) == false
                where om.Organization.CampusId == campus || om.Organization.CampusId == null
                where om.Organization.OrganizationStatusId == OrgStatusCode.Active
                where om.Person.FamilyId == id
                where om.Person.DeceasedDate == null
                from meeting in meetingHours
                let CheckedIn = DbUtil.Db.Attends.FirstOrDefault(aa =>
                    aa.OrganizationId == om.OrganizationId
                    && aa.PeopleId == om.PeopleId
                    && aa.MeetingDate == meeting.Hour
                    && aa.AttendanceFlag) != null
                select new CheckinFamilyMember
                {
                    Id = om.PeopleId,
                    Position = om.Person.PositionInFamilyId,
                    MemberVisitor = "M",
                    Name = om.Person.Name,
                    First = om.Person.FirstName,
                    PreferredName = om.Person.PreferredName,
                    Last = om.Person.LastName,
                    BYear = om.Person.BirthYear,
                    BMon = om.Person.BirthMonth,
                    BDay = om.Person.BirthDay,
                    ClassX = om.Organization.OrganizationName,
                    Leader = om.Organization.LeaderName,
                    OrgId = om.OrganizationId,
                    Location = om.Organization.Location,
                    Age = om.Person.Age ?? 0,
                    Gender = om.Person.Gender.Code,
                    NumLabels = normalLabelsMemTypes.Contains(om.MemberTypeId)
                        ? (om.Organization.NumCheckInLabels ?? 1)
                        : (om.Organization.NumWorkerCheckInLabels ?? 0),
                    Hour = meeting.Hour,
                    CheckedIn = CheckedIn,
                    Goesby = om.Person.NickName,
                    Email = om.Person.EmailAddress,
                    Addr = om.Person.Family.AddressLineOne,
                    Zip = om.Person.Family.ZipCode,
                    Home = om.Person.Family.HomePhone,
                    Cell = om.Person.CellPhone,
                    Marital = om.Person.MaritalStatusId,
                    Genderid = om.Person.GenderId,
                    CampusId = om.Person.CampusId,
                    Allergies = recreg.MedicalDescription,
                    Emfriend = recreg.Emcontact,
                    Emphone = recreg.Emphone,
                    Activeother = recreg.ActiveInAnotherChurch ?? false,
                    Parent = recreg.Mname ?? recreg.Fname,
                    Grade = om.Person.Grade,
                    HasPicture = om.Person.PictureId != null,
                    Custody = (om.Person.CustodyIssue ?? false),
                    Transport = (om.Person.OkTransport ?? false),
                    RequiresSecurityLabel =
                        normalLabelsMemTypes.Contains(om.MemberTypeId) // regular member
                        && (om.Person.Age ?? 0) < 18 // less than 18
                        && (om.Organization.NoSecurityLabel ?? false) == false, // org uses security

                    Church = om.Person.OtherNewChurch
                };

            // now get recent visitors
            var today = DateTime.Today;

            var visitors =
                from a in DbUtil.Db.Attends
                where a.Person.FamilyId == id
                where a.Person.DeceasedDate == null
                where a.Meeting.Organization.CanSelfCheckin.Value
                where a.Meeting.Organization.AllowNonCampusCheckIn == true
                      || a.Meeting.Organization.CampusId == campus || campus == 0
                where a.Meeting.Organization.OrganizationStatusId == OrgStatusCode.Active
                where a.AttendanceFlag &&
                      (a.MeetingDate >= a.Meeting.Organization.FirstMeetingDate.Value.Date || a.Meeting.Organization.FirstMeetingDate == null)
                where a.AttendanceFlag && (a.MeetingDate >= a.Meeting.Organization.VisitorDate.Value)
                where Attend.VisitAttendTypes.Contains(a.AttendanceTypeId.Value)
                where !a.Meeting.Organization.OrganizationMembers.Any(om => om.PeopleId == a.PeopleId)
                group a by new { a.PeopleId, a.Meeting.OrganizationId }
                into g
                let a = g.OrderByDescending(att => att.MeetingDate).First()
                let meetingHours = DbUtil.Db.GetTodaysMeetingHours(a.Meeting.OrganizationId, thisday)
                let recreg = a.Person.RecRegs.FirstOrDefault()
                from meeting in meetingHours
                let CheckedIn = DbUtil.Db.Attends.FirstOrDefault(aa =>
                    aa.Meeting.OrganizationId == a.Meeting.OrganizationId
                    && aa.PeopleId == a.PeopleId
                    && aa.MeetingDate == meeting.Hour
                    && aa.AttendanceFlag) != null
                select new CheckinFamilyMember
                {
                    Id = a.PeopleId,
                    Position = a.Person.PositionInFamilyId,
                    MemberVisitor = "V",
                    Name = a.Person.Name,
                    First = a.Person.FirstName,
                    PreferredName = a.Person.PreferredName,
                    Last = a.Person.LastName,
                    BYear = a.Person.BirthYear,
                    BMon = a.Person.BirthMonth,
                    BDay = a.Person.BirthDay,
                    ClassX = a.Meeting.Organization.OrganizationName,
                    Leader = a.Meeting.Organization.LeaderName,
                    OrgId = a.OrganizationId,
                    //OrgId = (a.Organization.CanSelfCheckin ?? false) ? a.OrganizationId : 0,
                    Location = a.Meeting.Organization.Location,
                    Age = a.Person.Age ?? 0,
                    Gender = a.Person.Gender.Code,
                    NumLabels = a.Meeting.Organization.NumCheckInLabels ?? 1,
                    Hour = meeting.Hour,
                    CheckedIn = CheckedIn,
                    Goesby = a.Person.NickName,
                    Email = a.Person.EmailAddress,
                    Addr = a.Person.Family.AddressLineOne,
                    Zip = a.Person.Family.ZipCode,
                    Home = a.Person.Family.HomePhone,
                    Cell = a.Person.CellPhone,
                    Marital = a.Person.MaritalStatusId,
                    Genderid = a.Person.GenderId,
                    CampusId = a.Person.CampusId,
                    Allergies = recreg.MedicalDescription,
                    Emfriend = recreg.Emcontact,
                    Emphone = recreg.Emphone,
                    Activeother = recreg.ActiveInAnotherChurch ?? false,
                    Parent = recreg.Mname ?? recreg.Fname,
                    Grade = a.Person.Grade,
                    HasPicture = a.Person.PictureId != null,
                    Custody = (a.Person.CustodyIssue ?? false),
                    Transport = (a.Person.OkTransport ?? false),
                    RequiresSecurityLabel = ((a.Person.Age ?? 0) < 18) && (a.Meeting.Organization.NoSecurityLabel ?? false) == false,
                    Church = a.Person.OtherNewChurch
                };

            var list = members.ToList();
            list.AddRange(visitors.ToList());

            // now get rest of family
            const string PleaseVisit = "No self check-in meetings available";
            // find a org on campus that allows an older, new visitor to check in to
            var qv = from o in DbUtil.Db.Organizations
                     let meetingHours = DbUtil.Db.GetTodaysMeetingHours(o.OrganizationId, thisday)
                     where o.OrganizationStatusId == OrgStatusCode.Active
                     where o.CampusId == campus || o.CampusId == null
                     where o.CanSelfCheckin == true
                     where o.AllowNonCampusCheckIn == true
                     from meeting in meetingHours
                     select new
                     {
                         VisitorOrgName = o.OrganizationName,
                         VisitorOrgId = o.OrganizationId,
                         VisitorOrgHour = meeting.Hour
                     };
            var vo = qv.FirstOrDefault() ?? new
            {
                VisitorOrgName = PleaseVisit,
                VisitorOrgId = 0,
                VisitorOrgHour = (DateTime?)null
            };
            var otherfamily =
                from p in DbUtil.Db.People
                where p.FamilyId == id
                where p.DeceasedDate == null
                where !list.Select(a => a.Id).Contains(p.PeopleId)
                let oldervisitor = (p.CampusId != campus || p.CampusId == null) && p.Age > 12
                let recreg = p.RecRegs.FirstOrDefault()
                select new CheckinFamilyMember
                {
                    Id = p.PeopleId,
                    Position = p.PositionInFamilyId,
                    Name = p.Name,
                    First = p.FirstName,
                    PreferredName = p.PreferredName,
                    Last = p.LastName,
                    BYear = p.BirthYear,
                    BMon = p.BirthMonth,
                    BDay = p.BirthDay,
                    ClassX = oldervisitor ? vo.VisitorOrgName : PleaseVisit,
                    OrgId = oldervisitor ? vo.VisitorOrgId : 0,
                    Leader = "",
                    Age = p.Age ?? 0,
                    Gender = p.Gender.Code,
                    NumLabels = 1,
                    Hour = vo.VisitorOrgHour,
                    Goesby = p.NickName,
                    Email = p.EmailAddress,
                    Addr = p.Family.AddressLineOne,
                    Zip = p.Family.ZipCode,
                    Home = p.Family.HomePhone,
                    Cell = p.CellPhone,
                    Marital = p.MaritalStatusId,
                    Genderid = p.GenderId,
                    CampusId = p.CampusId,
                    Allergies = recreg.MedicalDescription,
                    Emfriend = recreg.Emcontact,
                    Emphone = recreg.Emphone,
                    Activeother = recreg.ActiveInAnotherChurch ?? false,
                    Parent = recreg.Mname ?? recreg.Fname,
                    Grade = p.Grade,
                    HasPicture = p.PictureId != null,
                    Custody = p.CustodyIssue ?? false,
                    Transport = p.OkTransport ?? false,
                    RequiresSecurityLabel = false,
                    Church = p.OtherNewChurch
                };
            list.AddRange(otherfamily.ToList());
            var list2 = list.OrderBy(a => a.Position)
                .ThenByDescending(a => a.Position == 10 ? a.Gender : "U")
                .ThenBy(a => a.Age).ToList();
            return list2;
        }

        private bool HasImage(int? imageid)
        {
            var q = from i in ImageData.DbUtil.Db.Images
                    where i.Id == imageid
                    select i.Length;
            var len = q.SingleOrDefault();
            return len > 0;
        }

        public void RecordAttend(int PeopleId, int OrgId, bool Present, int thisday)
        {
            var q = from o in DbUtil.Db.Organizations
                    where o.OrganizationId == OrgId
                    let p = DbUtil.Db.People.Single(pp => pp.PeopleId == PeopleId)
                    select new
                    {
                        MeetingId = DbUtil.Db.GetTodaysMeetingId(OrgId, thisday),
                        MeetingTime = DbUtil.Db.GetTodaysMeetingHours(OrgId, thisday).First().Hour,
                        o.Location,
                        OrgEntryPoint = o.EntryPointId,
                        p.EntryPointId
                    };
            var info = q.Single();
            var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingId == info.MeetingId);
            if (info.EntryPointId == null)
            {
                var p = DbUtil.Db.LoadPersonById(PeopleId);
                if (info.OrgEntryPoint > 0)
                {
                    p.EntryPointId = info.OrgEntryPoint;
                }
            }
            if (meeting == null)
            {
                var acr = (from s in DbUtil.Db.OrgSchedules
                           where s.OrganizationId == OrgId
                           where s.SchedTime.Value.TimeOfDay == info.MeetingTime.Value.TimeOfDay
                           where s.SchedDay == thisday
                           select s.AttendCreditId).SingleOrDefault();
                meeting = new Meeting
                {
                    OrganizationId = OrgId,
                    MeetingDate = info.MeetingTime,
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserId1,
                    GroupMeetingFlag = false,
                    Location = info.Location,
                    AttendCreditId = acr
                };
                DbUtil.Db.Meetings.InsertOnSubmit(meeting);
                DbUtil.Db.SubmitChanges();
            }
            Attend.RecordAttendance(PeopleId, meeting.MeetingId, Present);
            DbUtil.Db.UpdateMeetingCounters(meeting.MeetingId);
        }

        public void JoinUnJoinOrg(int PeopleId, int OrgId, bool Member)
        {
            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == PeopleId && m.OrganizationId == OrgId);
            if (om == null && Member)
            {
                om = OrganizationMember.InsertOrgMembers(DbUtil.Db,
                    OrgId, PeopleId, MemberTypeCode.Member, DateTime.Now, null, false);
            }
            else if (om != null && !Member)
            {
                om.Drop(DbUtil.Db);
            }

            DbUtil.Db.SubmitChanges();

            var org = DbUtil.Db.LoadOrganizationById(OrgId);
            if (org != null && org.NotifyIds.HasValue())
            {
                var p = DbUtil.Db.LoadPersonById(PeopleId);
                var what = Member ? "joined" : "dropped";
                //                DbUtil.Db.Email(DbUtil.Db.Util.AdminMail,
                //                    DbUtil.Db.PeopleFromPidString(org.NotifyIds),
                //                    $"cms check-in, {what} class on " + DbUtil.Db.CmsHost,
                //                    $"<a href='{Util.ServerLink("/Person2/" + PeopleId)}/Person2/{PeopleId}'>{p.Name}</a> {what} {org.OrganizationName}");
                DbUtil.LogActivity($"cms check-in, {what} class ({p.PeopleId})");
            }
        }

        internal static bool UseOldCheckin()
        {
            return DbUtil.Db.Setting("UseOldCheckin", "false").ToBool();
        }
    }
}
