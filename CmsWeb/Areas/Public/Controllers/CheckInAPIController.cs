using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Public.Models.CheckInAPI;
using CmsWeb.CheckInAPI;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Controllers
{
    public class CheckInAPIController : Controller
    {
        public ActionResult Exists()
        {
            return Content("1");
        }

        private static bool Auth()
        {
            return AccountModel.AuthenticateMobile("Checkin").IsValid;
        }

        public ActionResult Authenticate(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            var br = new BaseMessage();
            br.error = 0;
            br.data = JsonConvert.SerializeObject(new CheckInInformation(getSettings(), getCampuses(), getLabelFormats()));
            return br;
        }

        private List<CheckInSettingsEntry> getSettings()
        {
            return (from s in DbUtil.Db.CheckInSettings
                    select new CheckInSettingsEntry
                    {
                        name = s.Name,
                        settings = s.Settings
                    }).ToList();
        }

        private List<CheckInCampus> getCampuses()
        {
            return (from c in DbUtil.Db.Campus
                    where c.Organizations.Any(o => o.CanSelfCheckin == true)
                    orderby c.Id
                    select new CheckInCampus
                    {
                        id = c.Id,
                        name = c.Description
                    }).ToList();
        }

        private List<CheckInLabelFormat> getLabelFormats()
        {
            var labels = (from e in DbUtil.Db.LabelFormats
                          select new CheckInLabelFormat
                          {
                              name = e.Name,
                              size = e.Size,
                              format = e.Format,
                          }).ToList();

            foreach (var label in labels)
            {
                label.parse();
            }

            return labels;
        }

        [HttpPost]
        public ActionResult NumberSearch(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInNumberSearch cns = JsonConvert.DeserializeObject<CheckInNumberSearch>(dataIn.data);

            DbUtil.LogActivity("Check-In Number Search: " + cns.search);

            var matches = DbUtil.Db.CheckinMatch(cns.search).ToList();

            BaseMessage br = new BaseMessage();
            br.setNoError();

            int tzOffset = DbUtil.Db.Setting("TZOffset", "0").ToInt();

            List<CheckInFamily> families = new List<CheckInFamily>();

            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    if (match.Locked.HasValue && match.Locked.Value) continue;

                    CheckInFamily family = new CheckInFamily(match.Familyid.Value, match.Name);

                    var members = (from a in DbUtil.Db.CheckinFamilyMembers(match.Familyid, cns.campus, cns.day).ToList()
                                   orderby a.Position, a.Position == 10 ? a.Genderid : 10, a.Age descending, a.Hour
                                   select a).ToList();

                    foreach (var member in members)
                    {
                        family.addMember(member, cns.day, tzOffset);
                    }

                    families.Add(family);
                    br.count++;
                }

                br.data = SerializeJSON(families, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult Family(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInFamilySearch cfs = JsonConvert.DeserializeObject<CheckInFamilySearch>(dataIn.data);

            DbUtil.LogActivity("Check-In Family: " + cfs.familyID);

            BaseMessage br = new BaseMessage();
            br.setNoError();

            int tzOffset = DbUtil.Db.Setting("TZOffset", "0").ToInt();

            List<CheckInFamily> families = new List<CheckInFamily>();

            CheckInFamily family = new CheckInFamily(cfs.familyID, "");

            var members = (from a in DbUtil.Db.CheckinFamilyMembers(cfs.familyID, cfs.campus, cfs.day).ToList()
                           orderby a.Position, a.Position == 10 ? a.Genderid : 10, a.Age descending, a.Hour
                           select a).ToList();

            foreach (var member in members)
            {
                family.addMember(member, cfs.day, tzOffset);
            }

            families.Add(family);
            br.count = 1;

            br.data = SerializeJSON(families, dataIn.version);
            return br;
        }

        [HttpPost]
        public ActionResult FetchPerson(string data)
        {
            // Authenticate first
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            BaseMessage br = new BaseMessage();

            var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == dataIn.argInt);

            if (person == null)
            {
                br.setError(BaseMessage.API_ERROR_PERSON_NOT_FOUND);
                br.data = "Person not found.";
                return br;
            }

            br.setNoError();
            br.count = 1;

            if (dataIn.device == BaseMessage.API_DEVICE_ANDROID)
            {
                br.data = SerializeJSON(new CheckInPerson().populate(person), dataIn.version);
            }
            else
            {
                List<CheckInPerson> mp = new List<CheckInPerson>();
                mp.Add(new CheckInPerson().populate(person));
                br.data = SerializeJSON(mp, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult AddEditPerson(string data)
        {
            // Authenticate first
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInAddEditPerson aep = JsonConvert.DeserializeObject<CheckInAddEditPerson>(dataIn.data);
            aep.clean();

            Person p;

            if (aep.edit)
            {
                p = DbUtil.Db.LoadPersonById(aep.id);
            }
            else
            {
                p = new Person();

                p.CreatedDate = Util.Now;
                p.CreatedBy = Util.UserId;

                p.MemberStatusId = MemberStatusCode.JustAdded;
                p.AddressTypeId = 10;

                p.OriginId = OriginCode.Visit;

                p.Name = "";
            }

            p.FirstName = aep.firstName;
            p.LastName = aep.lastName;
            p.NickName = aep.goesBy;

            if (aep.birthday != null)
            {
                p.BirthDay = aep.birthday.Value.Day;
                p.BirthMonth = aep.birthday.Value.Month;
                p.BirthYear = aep.birthday.Value.Year;
            }

            p.GenderId = aep.genderID;
            p.MaritalStatusId = aep.maritalStatusID;

            Family f;

            if (aep.familyID > 0)
            {
                f = DbUtil.Db.Families.First(fam => fam.FamilyId == aep.familyID);

                if (aep.edit)
                {
                    f.HomePhone = aep.homePhone;
                    f.AddressLineOne = aep.address;
                    f.AddressLineTwo = aep.address2;
                    f.CityName = aep.city;
                    f.StateCode = aep.state;
                    f.ZipCode = aep.zipcode;
                    f.CountryName = aep.country;
                }
                else
                {
                    if (aep.homePhone.Length > 0)
                        f.HomePhone = aep.homePhone;

                    if (aep.address.Length > 0)
                        f.AddressLineOne = aep.address;

                    if (aep.address2.Length > 0)
                        f.AddressLineTwo = aep.address2;

                    if (aep.city.Length > 0)
                        f.CityName = aep.city;

                    if (aep.state.Length > 0)
                        f.StateCode = aep.state;

                    if (aep.zipcode.Length > 0)
                        f.ZipCode = aep.zipcode;

                    if (aep.country.Length > 0)
                        f.CountryName = aep.country;
                }
            }
            else
            {
                f = new Family();

                f.HomePhone = aep.homePhone;
                f.AddressLineOne = aep.address;
                f.AddressLineTwo = aep.address2;
                f.CityName = aep.city;
                f.StateCode = aep.state;
                f.ZipCode = aep.zipcode;
                f.CountryName = aep.country;

                DbUtil.Db.Families.InsertOnSubmit(f);
            }

            if (!aep.edit)
            {
                f.People.Add(p);

                p.PositionInFamilyId = PositionInFamily.Child;

                if (aep.birthday != null && p.GetAge() >= 18)
                {
                    if (f.People.Count(per => per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2)
                        p.PositionInFamilyId = PositionInFamily.PrimaryAdult;
                    else
                        p.PositionInFamilyId = PositionInFamily.SecondaryAdult;
                }
            }

            p.FixTitle();

            p.EmailAddress = aep.eMail;
            p.CellPhone = aep.cellPhone;
            p.HomePhone = aep.homePhone;

            p.SetRecReg().MedicalDescription = aep.allergies;

            p.SetRecReg().Emcontact = aep.emergencyName;
            p.SetRecReg().Emphone = aep.emergencyPhone.Truncate(50);

            p.SetRecReg().ActiveInAnotherChurch = aep.church != null && aep.church.Length > 0;
            p.OtherPreviousChurch = aep.church;

            DbUtil.Db.SubmitChanges();

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.id = p.PeopleId;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult RecordAttend(string data)
        {
            // Authenticate first
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInAttend cia = JsonConvert.DeserializeObject<CheckInAttend>(dataIn.data);

            var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.OrganizationId == cia.orgID && m.MeetingDate == cia.datetime);

            if (meeting == null)
            {
                meeting = new Meeting
                {
                    OrganizationId = cia.orgID,
                    MeetingDate = cia.datetime,
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserPeopleId ?? 0,
                    GroupMeetingFlag = false,
                };

                DbUtil.Db.Meetings.InsertOnSubmit(meeting);
                DbUtil.Db.SubmitChanges();

                var acr = (from s in DbUtil.Db.OrgSchedules
                           where s.OrganizationId == cia.orgID
                           where s.SchedTime.Value.TimeOfDay == cia.datetime.TimeOfDay
                           where s.SchedDay == (int)cia.datetime.DayOfWeek
                           select s.AttendCreditId).SingleOrDefault();

                meeting.AttendCreditId = acr;
            }

            Attend.RecordAttendance(cia.peopleID, meeting.MeetingId, cia.present);

            DbUtil.Db.UpdateMeetingCounters(cia.orgID);
            DbUtil.LogActivity($"Check-In Record Attend Org ID:{meeting.OrganizationId} People ID:{cia.peopleID} User ID:{Util.UserPeopleId} Attended:{cia.present}");

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult ClassSearch(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInClassSearch ccs = JsonConvert.DeserializeObject<CheckInClassSearch>(dataIn.data);

            DbUtil.LogActivity("Check-In Class Search: " + ccs.peopleID);

            var person = (from p in DbUtil.Db.People
                          where p.PeopleId == ccs.peopleID
                          select new { p.FamilyId, p.BirthDate, p.Grade }).SingleOrDefault();

            if (person == null)
                return BaseMessage.createErrorReturn("Person not found", BaseMessage.API_ERROR_PERSON_NOT_FOUND);

            BaseMessage br = new BaseMessage();
            br.setNoError();

            var orgs = (from o in DbUtil.Db.Organizations
                        let sc = o.OrgSchedules.FirstOrDefault()
                        let meetingHours = DbUtil.Db.GetTodaysMeetingHours(o.OrganizationId, ccs.day)
                        let bdaystart = o.BirthDayStart ?? DateTime.MaxValue
                        where (o.SuspendCheckin ?? false) == false || ccs.noAgeCheck
                        where person.BirthDate == null || person.BirthDate <= o.BirthDayEnd || o.BirthDayEnd == null || ccs.noAgeCheck
                        where person.BirthDate == null || person.BirthDate >= o.BirthDayStart || o.BirthDayStart == null || ccs.noAgeCheck
                        where o.CanSelfCheckin == true
                        where (o.ClassFilled ?? false) == false
                        where (o.CampusId == null && o.AllowNonCampusCheckIn == true) || o.CampusId == ccs.campus || ccs.campus == 0
                        where o.OrganizationStatusId == OrgStatusCode.Active
                        orderby sc.SchedTime.Value.TimeOfDay, bdaystart, o.OrganizationName
                        from meeting in meetingHours
                        select new CheckInOrganization()
                        {
                            id = o.OrganizationId,
                            leader = o.LeaderName,
                            name = o.OrganizationName,
                            hour = meeting.Hour.Value,
                            birthdayStart = o.BirthDayStart,
                            birthdayEnd = o.BirthDayEnd,
                            location = o.Location
                        }).ToList();

            // Add lead time adjustment for different timezones here
            int tzOffset = DbUtil.Db.Setting("TZOffset", "0").ToInt();

            foreach (var org in orgs)
            {
                org.adjustLeadTime(ccs.day, tzOffset);
            }

            br.data = SerializeJSON(orgs, dataIn.version);

            return br;
        }

        [HttpPost]
        public ActionResult NameSearch(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInNameSearch cns = JsonConvert.DeserializeObject<CheckInNameSearch>(dataIn.data);
            cns.splitName();

            DbUtil.LogActivity("Check-In Name Search: " + cns.name);

            var q = DbUtil.Db.People.Select(p => p);

            if (cns.first.HasValue())
            {
                q = from p in q
                    where (p.LastName.StartsWith(cns.last) || p.MaidenName.StartsWith(cns.last))
                        && (p.FirstName.StartsWith(cns.first) || p.NickName.StartsWith(cns.first) || p.MiddleName.StartsWith(cns.first))
                    select p;
            }
            else
            {
                q = from p in q
                    where p.LastName.StartsWith(cns.last) || p.FirstName.StartsWith(cns.last) || p.NickName.StartsWith(cns.last) || p.MiddleName.StartsWith(cns.last)
                    select p;
            }

            var q2 = (from p in q
                      let recreg = p.RecRegs.FirstOrDefault()
                      orderby p.Name2, p.PeopleId
                      where p.DeceasedDate == null
                      select new CheckInPerson
                      {
                          id = p.PeopleId,
                          familyID = p.FamilyId,
                          first = p.PreferredName,
                          last = p.LastName,
                          goesby = p.NickName,
                          cell = p.CellPhone,
                          home = p.HomePhone,
                          address = p.Family.AddressLineOne,
                          age = p.Age ?? 0
                      }).ToList();

            foreach (var person in q2)
            {
                person.loadImage();
            }

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = q2.Count();
            br.data = SerializeJSON(q2, dataIn.version);

            return br;
        }

        [HttpPost]
        public ActionResult JoinOrg(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInJoinOrg cjo = JsonConvert.DeserializeObject<CheckInJoinOrg>(dataIn.data);

            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == cjo.peopleID && m.OrganizationId == cjo.orgID);

            if (om == null && cjo.join)
                om = OrganizationMember.InsertOrgMembers(DbUtil.Db, cjo.orgID, cjo.peopleID, MemberTypeCode.Member, DateTime.Now, null, false);

            if (om != null && !cjo.join)
            {
                om.Drop(DbUtil.Db, DateTime.Today);

                DbUtil.LogActivity($"Dropped {om.PeopleId} for {om.Organization.OrganizationId} via {dataIn.getSourceOS()} app", peopleid: om.PeopleId, orgid: om.OrganizationId);
            }

            DbUtil.Db.SubmitChanges();

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult PrintLabels(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            List<CheckInPrintLabel> labels = JsonConvert.DeserializeObject<List<CheckInPrintLabel>>(dataIn.data);

            string securityCode = DbUtil.Db.NextSecurityCode(DateTime.Today).Select(c => c.Code).Single();

            StringBuilder builder = new StringBuilder();

            XmlWriter writer = XmlWriter.Create(builder);
            writer.WriteStartDocument();
            writer.WriteStartElement("PrintJob");

            writer.WriteElementString("securitycode", securityCode);

            writer.WriteStartElement("list");

            foreach (CheckInPrintLabel label in labels)
            {
                label.writeToXML(writer, securityCode);
            }

            // list
            writer.WriteEndElement();
            // PrintJob
            writer.WriteEndElement();
            writer.Close();

            PrintJob job = new PrintJob { Id = dataIn.argString, Data = builder.ToString(), Stamp = DateTime.Now };

            DbUtil.Db.PrintJobs.InsertOnSubmit(job);
            DbUtil.Db.SubmitChanges();

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult SaveSettings(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInSettingsEntry entry = JsonConvert.DeserializeObject<CheckInSettingsEntry>(dataIn.data);

            var setting = (from e in DbUtil.Db.CheckInSettings
                           where e.Name == entry.name
                           select e).SingleOrDefault();

            BaseMessage br = new BaseMessage();

            if (setting == null)
            {
                setting = new CheckInSetting();
                setting.Name = entry.name;
                setting.Settings = entry.settings;

                DbUtil.Db.CheckInSettings.InsertOnSubmit(setting);

                br.data = "Settings saved";
            }
            else
            {
                setting.Settings = entry.settings;

                br.data = "Settings updated";
            }

            DbUtil.Db.SubmitChanges();

            br.setNoError();
            br.id = setting.Id;
            br.count = 1;

            return br;
        }

        private static string SerializeJSON(Object item, int version)
        {
            return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss" });
        }
    }
}