using CmsData;
using CmsData.Codes;
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
                    CheckInFamily family = new CheckInFamily(match.Familyid.Value, match.Name, match.Locked ?? false);

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

            FamilyCheckinLock familyLock = DbUtil.Db.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == dataIn.argInt);

            CheckInFamily family = new CheckInFamily(cfs.familyID, "", familyLock == null ? false : familyLock.Locked);

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
        public ActionResult FamilyInfo(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            Family family = DbUtil.Db.Families.First(fam => fam.FamilyId == dataIn.argInt);

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;
            br.id = family.FamilyId;
            br.data = SerializeJSON(new CheckInFamilyInfo(family), dataIn.version);
            return br;
        }

        [HttpPost]
        public ActionResult LockFamily(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);


            var lockf = DbUtil.Db.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == dataIn.argInt);

            if (lockf == null)
            {
                lockf = new FamilyCheckinLock { FamilyId = dataIn.argInt, Created = DateTime.Now };
                DbUtil.Db.FamilyCheckinLocks.InsertOnSubmit(lockf);
            }

            lockf.Locked = true;
            lockf.Created = DateTime.Now;

            DbUtil.Db.SubmitChanges();

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.id = dataIn.argInt;
            return br;
        }

        [HttpPost]
        public ActionResult UnLockFamily(string data)
        {
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var lockf = DbUtil.Db.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == dataIn.argInt);

            if (lockf != null)
            {
                lockf.Locked = false;
                DbUtil.Db.SubmitChanges();
            }

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.id = dataIn.argInt;
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
        public ActionResult FetchImage(string data)
        {
            // Authenticate first
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInFetchImage cifi = JsonConvert.DeserializeObject<CheckInFetchImage>(dataIn.data);

            BaseMessage br = new BaseMessage();
            if (cifi.id == 0) return br.setData("The ID for the person cannot be set to zero");

            br.data = "The picture was not found.";

            var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == cifi.id);

            if (person.PictureId != null)
            {
                ImageData.Image image = null;

                switch (cifi.size)
                {
                    case 0: // 50 x 50
                    image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId);
                    break;

                    case 1: // 120 x 120
                    image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);
                    break;

                    case 2: // 320 x 400
                    image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId);
                    break;

                    case 3: // 570 x 800
                    image = ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId);
                    break;

                }

                if (image != null)
                {
                    br.data = Convert.ToBase64String(image.Bits);
                    br.count = 1;
                    br.setNoError();
                }
            }

            return br;
        }

        [HttpPost]
        public ActionResult SaveImage(string data)
        {
            // Authenticate first
            if (!Auth())
                return BaseMessage.createErrorReturn("Authentication failed, please try again", BaseMessage.API_ERROR_INVALID_CREDENTIALS);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            CheckInSaveImage cisi = JsonConvert.DeserializeObject<CheckInSaveImage>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var imageBytes = Convert.FromBase64String(cisi.image);

            var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == cisi.id);

            if (person.Picture != null)
            {
                if (ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId) != null)
                    ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId));

                if (ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId) != null)
                    ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId));

                if (ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId) != null)
                    ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId));

                if (ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId) != null)
                    ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId));

                person.Picture.ThumbId = ImageData.Image.NewImageFromBits(imageBytes, 50, 50).Id;
                person.Picture.SmallId = ImageData.Image.NewImageFromBits(imageBytes, 120, 120).Id;
                person.Picture.MediumId = ImageData.Image.NewImageFromBits(imageBytes, 320, 400).Id;
                person.Picture.LargeId = ImageData.Image.NewImageFromBits(imageBytes).Id;
            }
            else
            {
                var newPicture = new Picture();

                newPicture.ThumbId = ImageData.Image.NewImageFromBits(imageBytes, 50, 50).Id;
                newPicture.SmallId = ImageData.Image.NewImageFromBits(imageBytes, 120, 120).Id;
                newPicture.MediumId = ImageData.Image.NewImageFromBits(imageBytes, 320, 400).Id;
                newPicture.LargeId = ImageData.Image.NewImageFromBits(imageBytes).Id;

                person.Picture = newPicture;
            }

            DbUtil.Db.SubmitChanges();

            br.setNoError();
            br.data = "Image updated";
            br.id = cisi.id;
            br.count = 1;

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

            CheckInAddEditPersonResults results = new CheckInAddEditPersonResults();

            Family f;
            Person p;

            if (aep.edit)
            {
                p = DbUtil.Db.LoadPersonById(aep.id);

                f = DbUtil.Db.Families.First(fam => fam.FamilyId == p.FamilyId);

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
                results.newPerson = true;

                p = new Person();

                p.CreatedDate = Util.Now;
                p.CreatedBy = Util.UserId;

                p.MemberStatusId = MemberStatusCode.JustAdded;
                p.AddressTypeId = 10;

                p.OriginId = OriginCode.Visit;

                p.Name = "";

                if (aep.familyID > 0)
                {
                    f = DbUtil.Db.Families.First(fam => fam.FamilyId == aep.familyID);
                }
                else
                {
                    results.newFamily = true;

                    f = new Family();
                    DbUtil.Db.Families.InsertOnSubmit(f);
                }

                f.HomePhone = aep.homePhone;
                f.AddressLineOne = aep.address;
                f.AddressLineTwo = aep.address2;
                f.CityName = aep.city;
                f.StateCode = aep.state;
                f.ZipCode = aep.zipcode;
                f.CountryName = aep.country;

                f.People.Add(p);

            }

            p.FirstName = aep.firstName;
            p.LastName = aep.lastName;
            p.NickName = aep.goesBy;

            if (aep.birthday != null)
            {
                p.BirthDay = aep.birthday.Value.Day;
                p.BirthMonth = aep.birthday.Value.Month;
                p.BirthYear = aep.birthday.Value.Year;

                p.PositionInFamilyId = PositionInFamily.Child;

                if (p.GetAge() >= 18)
                {
                    if (f.People.Count(per => per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2)
                        p.PositionInFamilyId = PositionInFamily.PrimaryAdult;
                    else
                        p.PositionInFamilyId = PositionInFamily.SecondaryAdult;
                }
            }

            p.GenderId = aep.genderID;
            p.MaritalStatusId = aep.maritalStatusID;

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

            results.familyID = f.FamilyId;
            results.peopleID = p.PeopleId;
            results.position = p.PositionInFamilyId;

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;
            br.data = SerializeJSON(results, dataIn.version);

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
                      }).Take(200).ToList();

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
                om.Drop(DbUtil.Db, DateTime.Now);

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