using CmsData;
using CmsData.Codes;
using CmsData.View;
using CmsWeb.CheckInAPI;
using CmsWeb.Lifecycle;
using CmsWeb.Models;
using ImageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Public.Controllers
{
    public class CheckInAPIController : CMSBaseController
    {
        public CheckInAPIController(IRequestManager requestManager) : base(requestManager)
        {
        }

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
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.data = JsonConvert.SerializeObject(new CheckInInformation(getSettings(), getCampuses(), getLabelFormats()));

            return br;
        }

        private List<CheckInSettingsEntry> getSettings()
        {
            return (from s in CurrentDatabase.CheckInSettings
                    select new CheckInSettingsEntry
                    {
                        name = s.Name,
                        settings = s.Settings
                    }).ToList();
        }

        private List<CheckInCampus> getCampuses()
        {
            return (from c in CurrentDatabase.Campus
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
            List<CheckInLabelFormat> labels = (from e in CurrentDatabase.LabelFormats
                                               select new CheckInLabelFormat
                                               {
                                                   name = e.Name,
                                                   size = e.Size,
                                                   format = e.Format,
                                               }).ToList();

            foreach (CheckInLabelFormat label in labels)
            {
                label.parse();
            }

            return labels;
        }

        [HttpPost]
        public ActionResult NumberSearch(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInNumberSearch cns = JsonConvert.DeserializeObject<CheckInNumberSearch>(dataIn.data);

            DbUtil.LogActivity("Check-In Number Search: " + cns.search);

            List<CheckinMatch> matches = CurrentDatabase.CheckinMatch(cns.search).ToList();

            CheckInMessage br = new CheckInMessage();
            br.setNoError();

            int tzOffset = CurrentDatabase.Setting("TZOffset", "0").ToInt();

            List<CheckInFamily> families = new List<CheckInFamily>();

            if (matches.Count > 0)
            {
                foreach (CheckinMatch match in matches)
                {
                    if (match.Familyid != null)
                    {
                        CheckInFamily family = new CheckInFamily(match.Familyid.Value, match.Name, match.Locked ?? false);

                        List<CheckinFamilyMember> members = (from a in CurrentDatabase.CheckinFamilyMembers(match.Familyid, cns.campus, cns.day).ToList()
                                                             orderby a.Position, a.Position == 10 ? a.Genderid : 10, a.Age descending, a.Hour
                                                             select a).ToList();

                        foreach (CheckinFamilyMember member in members)
                        {
                            family.addMember(member, cns.day, tzOffset);
                        }

                        families.Add(family);

                        br.count++;
                    }
                }

                br.data = SerializeJSON(families, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult Family(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInFamilySearch cfs = JsonConvert.DeserializeObject<CheckInFamilySearch>(dataIn.data);

            DbUtil.LogActivity("Check-In Family: " + cfs.familyID);

            CheckInMessage br = new CheckInMessage();
            br.setNoError();

            int tzOffset = CurrentDatabase.Setting("TZOffset", "0").ToInt();

            List<CheckInFamily> families = new List<CheckInFamily>();

            FamilyCheckinLock familyLock = CurrentDatabase.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == dataIn.argInt);

            CheckInFamily family = new CheckInFamily(cfs.familyID, "", familyLock?.Locked ?? false);

            List<CheckinFamilyMember> members = (from a in CurrentDatabase.CheckinFamilyMembers(cfs.familyID, cfs.campus, cfs.day).ToList()
                                                 orderby a.Position, a.Position == 10 ? a.Genderid : 10, a.Age descending, a.Hour
                                                 select a).ToList();

            foreach (CheckinFamilyMember member in members)
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
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);

            Family family = CurrentDatabase.Families.First(fam => fam.FamilyId == dataIn.argInt);

            CheckInMessage br = new CheckInMessage();
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
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);

            FamilyCheckinLock lockf = CurrentDatabase.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == dataIn.argInt);

            if (lockf == null)
            {
                lockf = new FamilyCheckinLock { FamilyId = dataIn.argInt, Created = DateTime.Now };
                CurrentDatabase.FamilyCheckinLocks.InsertOnSubmit(lockf);
            }

            lockf.Locked = true;
            lockf.Created = DateTime.Now;

            CurrentDatabase.SubmitChanges();

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.id = dataIn.argInt;
            return br;
        }

        [HttpPost]
        public ActionResult UnLockFamily(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);

            FamilyCheckinLock lockf = CurrentDatabase.FamilyCheckinLocks.SingleOrDefault(f => f.FamilyId == dataIn.argInt);

            if (lockf != null)
            {
                lockf.Locked = false;
                CurrentDatabase.SubmitChanges();
            }

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.id = dataIn.argInt;
            return br;
        }

        [HttpPost]
        public ActionResult FetchPerson(string data)
        {
            // Authenticate first
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);

            CheckInMessage br = new CheckInMessage();

            Person person = CurrentDatabase.People.SingleOrDefault(p => p.PeopleId == dataIn.argInt);

            if (person == null)
            {
                br.setError(CheckInMessage.API_ERROR_PERSON_NOT_FOUND);
                br.data = "Person not found.";
                return br;
            }

            br.setNoError();
            br.count = 1;

            if (dataIn.device == CheckInMessage.API_DEVICE_ANDROID)
            {
                br.data = SerializeJSON(new CheckInPerson().populate(person), dataIn.version);
            }
            else
            {
                List<CheckInPerson> mp = new List<CheckInPerson> { new CheckInPerson().populate(person) };
                br.data = SerializeJSON(mp, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchImage(string data)
        {
            // Authenticate first
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInFetchImage cifi = JsonConvert.DeserializeObject<CheckInFetchImage>(dataIn.data);

            CheckInMessage br = new CheckInMessage();

            if (cifi.id == 0)
            {
                return br.setData("The ID for the person cannot be set to zero");
            }

            br.data = "The picture was not found.";

            Person person = CurrentDatabase.People.SingleOrDefault(pp => pp.PeopleId == cifi.id);

            if (person == null || person.PictureId == null)
            {
                return br;
            }

            Image image = null;

            switch (cifi.size)
            {
                case 0: // 50 x 50
                    image = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId);
                    break;

                case 1: // 120 x 120
                    image = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);
                    break;

                case 2: // 320 x 400
                    image = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId);
                    break;

                case 3: // 570 x 800
                    image = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId);
                    break;
            }

            if (image == null)
            {
                return br;
            }

            br.data = Convert.ToBase64String(image.Bits);
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult SaveImage(string data)
        {
            // Authenticate first
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInSaveImage cisi = JsonConvert.DeserializeObject<CheckInSaveImage>(dataIn.data);

            CheckInMessage br = new CheckInMessage();

            byte[] imageBytes = Convert.FromBase64String(cisi.image);

            Person person = CurrentDatabase.People.SingleOrDefault(pp => pp.PeopleId == cisi.id);

            if (person != null && person.Picture != null)
            {
                // Thumb image
                Image imageDataThumb = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId);

                if (imageDataThumb != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataThumb);
                }

                // Small image
                Image imageDataSmall = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId);

                if (imageDataSmall != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataSmall);
                }

                // Medium image
                Image imageDataMedium = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId);

                if (imageDataMedium != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataMedium);
                }

                // Large image
                Image imageDataLarge = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId);

                if (imageDataLarge != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataLarge);
                }

                person.Picture.ThumbId = Image.NewImageFromBits(imageBytes, 50, 50).Id;
                person.Picture.SmallId = Image.NewImageFromBits(imageBytes, 120, 120).Id;
                person.Picture.MediumId = Image.NewImageFromBits(imageBytes, 320, 400).Id;
                person.Picture.LargeId = Image.NewImageFromBits(imageBytes).Id;
            }
            else
            {
                Picture newPicture = new Picture
                {
                    ThumbId = Image.NewImageFromBits(imageBytes, 50, 50).Id,
                    SmallId = Image.NewImageFromBits(imageBytes, 120, 120).Id,
                    MediumId = Image.NewImageFromBits(imageBytes, 320, 400).Id,
                    LargeId = Image.NewImageFromBits(imageBytes).Id
                };

                if (person != null)
                {
                    person.Picture = newPicture;
                }
            }

            person?.LogPictureUpload(CurrentDatabase, Util.UserPeopleId ?? 1);

            CurrentDatabase.SubmitChanges();

            br.setNoError();
            br.data = "Image updated";
            br.id = cisi.id;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult SaveFamilyImage(string data)
        {
            // Authenticate first
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInSaveImage cisi = JsonConvert.DeserializeObject<CheckInSaveImage>(dataIn.data);

            CheckInMessage br = new CheckInMessage();

            byte[] imageBytes = Convert.FromBase64String(cisi.image);

            Family family = CurrentDatabase.Families.SingleOrDefault(pp => pp.FamilyId == cisi.id);

            if (family != null && family.Picture != null)
            {
                // Thumb image
                Image imageDataThumb = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == family.Picture.ThumbId);

                if (imageDataThumb != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataThumb);
                }

                // Small image
                Image imageDataSmall = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == family.Picture.SmallId);

                if (imageDataSmall != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataSmall);
                }

                // Medium image
                Image imageDataMedium = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == family.Picture.MediumId);

                if (imageDataMedium != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataMedium);
                }

                // Large image
                Image imageDataLarge = CurrentImageDatabase.Images.SingleOrDefault(i => i.Id == family.Picture.LargeId);

                if (imageDataLarge != null)
                {
                    CurrentImageDatabase.Images.DeleteOnSubmit(imageDataLarge);
                }

                family.Picture.ThumbId = Image.NewImageFromBits(imageBytes, 50, 50).Id;
                family.Picture.SmallId = Image.NewImageFromBits(imageBytes, 120, 120).Id;
                family.Picture.MediumId = Image.NewImageFromBits(imageBytes, 320, 400).Id;
                family.Picture.LargeId = Image.NewImageFromBits(imageBytes).Id;
            }
            else
            {
                Picture newPicture = new Picture
                {
                    ThumbId = Image.NewImageFromBits(imageBytes, 50, 50).Id,
                    SmallId = Image.NewImageFromBits(imageBytes, 120, 120).Id,
                    MediumId = Image.NewImageFromBits(imageBytes, 320, 400).Id,
                    LargeId = Image.NewImageFromBits(imageBytes).Id
                };

                if (family != null)
                {
                    family.Picture = newPicture;
                }
            }

            CurrentDatabase.SubmitChanges();

            br.setNoError();
            br.data = "Image updated.";
            br.id = cisi.id;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult AddEditPerson(string data)
        {
            // Authenticate first
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInAddEditPerson aep = JsonConvert.DeserializeObject<CheckInAddEditPerson>(dataIn.data);
            aep.clean();

            CheckInAddEditPersonResults results = new CheckInAddEditPersonResults();

            Family f;
            Person p;

            if (aep.edit)
            {
                p = CurrentDatabase.LoadPersonById(aep.id);

                f = CurrentDatabase.Families.First(fam => fam.FamilyId == p.FamilyId);

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

                p = new Person
                {
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserId,
                    MemberStatusId = MemberStatusCode.JustAdded,
                    AddressTypeId = 10,
                    OriginId = OriginCode.Visit,
                    EntryPoint = getCheckInEntryPointID()
                };

                if (aep.campus > 0)
                {
                    p.CampusId = aep.campus;
                }

                p.Name = "";

                if (aep.familyID > 0)
                {
                    f = CurrentDatabase.Families.First(fam => fam.FamilyId == aep.familyID);
                }
                else
                {
                    results.newFamily = true;

                    f = new Family();
                    CurrentDatabase.Families.InsertOnSubmit(f);
                }

                f.HomePhone = aep.homePhone;
                f.AddressLineOne = aep.address;
                f.AddressLineTwo = aep.address2;
                f.CityName = aep.city;
                f.StateCode = aep.state;
                f.ZipCode = aep.zipcode;
                f.CountryName = aep.country;

                f.People.Add(p);

                p.PositionInFamilyId = CurrentDatabase.ComputePositionInFamily(aep.getAge(), aep.maritalStatusID == MaritalStatusCode.Married, f.FamilyId) ?? PositionInFamily.PrimaryAdult;
            }

            p.FirstName = aep.firstName;
            p.LastName = aep.lastName;
            p.NickName = aep.goesBy;
            p.AltName = aep.altName;

            if (dataIn.version >= CheckInMessage.API_V3)
            {
                p.SetRecReg().Fname = aep.father;
                p.SetRecReg().Mname = aep.mother;
            }

            // Check-In API Version 2 or greater adds the ability to clear the birthday
            if (dataIn.version >= CheckInMessage.API_V2)
            {
                if (aep.birthdaySet && aep.birthday != null)
                {
                    p.BirthDay = aep.birthday.Value.Day;
                    p.BirthMonth = aep.birthday.Value.Month;
                    p.BirthYear = aep.birthday.Value.Year;
                }
                else
                {
                    if (aep.birthdayClear)
                    {
                        p.BirthDay = null;
                        p.BirthMonth = null;
                        p.BirthYear = null;
                    }
                }
            }
            else
            {
                if (aep.birthday != null)
                {
                    p.BirthDay = aep.birthday.Value.Day;
                    p.BirthMonth = aep.birthday.Value.Month;
                    p.BirthYear = aep.birthday.Value.Year;
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

            p.SetRecReg().ActiveInAnotherChurch = !string.IsNullOrEmpty(aep.church);
            p.OtherPreviousChurch = aep.church;

            CurrentDatabase.SubmitChanges();

            results.familyID = f.FamilyId;
            results.peopleID = p.PeopleId;
            results.position = p.PositionInFamilyId;

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.count = 1;
            br.data = SerializeJSON(results, dataIn.version);

            return br;
        }

        private EntryPoint getCheckInEntryPointID()
        {
            EntryPoint checkInEntryPoint = (from e in CurrentDatabase.EntryPoints
                                            where e.Code == "CHECKIN"
                                            select e).FirstOrDefault();

            if (checkInEntryPoint != null)
            {
                return checkInEntryPoint;
            }
            else
            {
                int maxEntryPointID = CurrentDatabase.EntryPoints.Max(e => e.Id);

                EntryPoint entry = new EntryPoint
                {
                    Id = maxEntryPointID + 1,
                    Code = "CHECKIN",
                    Description = "Check-In",
                    Hardwired = true
                };

                CurrentDatabase.EntryPoints.InsertOnSubmit(entry);
                CurrentDatabase.SubmitChanges();

                return entry;
            }
        }

        [HttpPost]
        public ActionResult RecordAttend(string data)
        {
            // Authenticate first
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInAttend cia = JsonConvert.DeserializeObject<CheckInAttend>(dataIn.data);

            Meeting meeting = CurrentDatabase.Meetings.SingleOrDefault(m => m.OrganizationId == cia.orgID && m.MeetingDate == cia.datetime);

            if (meeting == null)
            {
                int meetingID = CurrentDatabase.CreateMeeting(cia.orgID, cia.datetime);

                meeting = CurrentDatabase.Meetings.SingleOrDefault(m => m.MeetingId == meetingID);
            }

            Attend.RecordAttend(CurrentDatabase, cia.peopleID, cia.orgID, cia.present, cia.datetime);

            CurrentDatabase.UpdateMeetingCounters(cia.orgID);
            DbUtil.LogActivity($"Check-In Record Attend Org ID:{cia.orgID} People ID:{cia.peopleID} User ID:{Util.UserPeopleId} Attended:{cia.present}");

            // Check Entry Point and replace if Check-In
            Person person = CurrentDatabase.People.FirstOrDefault(p => p.PeopleId == cia.peopleID);

            if (person != null && person.EntryPoint != null && person.EntryPoint.Code != null && person.EntryPoint.Code == "CHECKIN" && meeting != null)
            {
                person.EntryPoint = meeting.Organization.EntryPoint;
                CurrentDatabase.SubmitChanges();
            }

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult ClassSearch(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInClassSearch ccs = JsonConvert.DeserializeObject<CheckInClassSearch>(dataIn.data);

            DbUtil.LogActivity("Check-In Class Search: " + ccs.peopleID);

            var person = (from p in CurrentDatabase.People
                          where p.PeopleId == ccs.peopleID
                          select new { p.FamilyId, p.BirthDate, p.Grade }).SingleOrDefault();

            if (person == null)
            {
                return CheckInMessage.createErrorReturn("Person not found", CheckInMessage.API_ERROR_PERSON_NOT_FOUND);
            }

            CheckInMessage br = new CheckInMessage();
            br.setNoError();

            List<CheckInOrganization> orgs = (from o in CurrentDatabase.Organizations
                                              let sc = o.OrgSchedules.FirstOrDefault()
                                              let meetingHours = CurrentDatabase.GetTodaysMeetingHours(o.OrganizationId, ccs.day)
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
                                                  location = o.Location,
                                                  allowOverlap = o.AllowAttendOverlap
                                              }).ToList();

            // Add lead time adjustment for different timezones here
            int tzOffset = CurrentDatabase.Setting("TZOffset", "0").ToInt();

            foreach (CheckInOrganization org in orgs)
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
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInNameSearch cns = JsonConvert.DeserializeObject<CheckInNameSearch>(dataIn.data);
            cns.splitName();

            DbUtil.LogActivity("Check-In Name Search: " + cns.name);

            IQueryable<Person> q = CurrentDatabase.People.Select(p => p);

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

            List<CheckInPerson> q2 = (from p in q
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
                                          altName = p.AltName,
                                          cell = p.CellPhone,
                                          home = p.HomePhone,
                                          address = p.Family.AddressLineOne,
                                          age = p.Age ?? 0
                                      }).Take(200).ToList();

            foreach (CheckInPerson person in q2)
            {
                person.loadImage();
            }

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.count = q2.Count();
            br.data = SerializeJSON(q2, dataIn.version);

            return br;
        }

        [HttpPost]
        public ActionResult JoinOrg(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInJoinOrg cjo = JsonConvert.DeserializeObject<CheckInJoinOrg>(dataIn.data);

            OrganizationMember om = CurrentDatabase.OrganizationMembers.SingleOrDefault(m => m.PeopleId == cjo.peopleID && m.OrganizationId == cjo.orgID);

            if (om == null && cjo.join)
            {
                om = OrganizationMember.InsertOrgMembers(CurrentDatabase, cjo.orgID, cjo.peopleID, MemberTypeCode.Member, DateTime.Today);
            }

            if (om != null && !cjo.join)
            {
                om.Drop(CurrentDatabase, DateTime.Now);

                DbUtil.LogActivity($"Dropped {om.PeopleId} for {om.Organization.OrganizationId} via {dataIn.getSourceOS()} app", peopleid: om.PeopleId, orgid: om.OrganizationId);
            }

            CurrentDatabase.SubmitChanges();

            // Check Entry Point and replace if Check-In
            Person person = CurrentDatabase.People.FirstOrDefault(p => p.PeopleId == cjo.peopleID);

            if (person?.EntryPoint != null && person.EntryPoint.Code == "CHECKIN" && om != null)
            {
                person.EntryPoint = om.Organization.EntryPoint;
                CurrentDatabase.SubmitChanges();
            }

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult PrintLabels(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            List<CheckInPrintLabel> labels = JsonConvert.DeserializeObject<List<CheckInPrintLabel>>(dataIn.data);

            string securityCode = CurrentDatabase.NextSecurityCode().Select(c => c.Code).Single();

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

            CurrentDatabase.PrintJobs.InsertOnSubmit(job);
            CurrentDatabase.SubmitChanges();

            CheckInMessage br = new CheckInMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult SaveSettings(string data)
        {
            if (!Auth())
            {
                return CheckInMessage.createErrorReturn("Authentication failed, please try again", CheckInMessage.API_ERROR_INVALID_CREDENTIALS);
            }

            CheckInMessage dataIn = CheckInMessage.createFromString(data);
            CheckInSettingsEntry entry = JsonConvert.DeserializeObject<CheckInSettingsEntry>(dataIn.data);

            CheckInSetting setting = (from e in CurrentDatabase.CheckInSettings
                                      where e.Name == entry.name
                                      select e).SingleOrDefault();

            CheckInMessage br = new CheckInMessage();

            if (setting == null)
            {
                setting = new CheckInSetting
                {
                    Name = entry.name,
                    Settings = entry.settings
                };

                CurrentDatabase.CheckInSettings.InsertOnSubmit(setting);

                br.data = "Settings saved";
            }
            else
            {
                setting.Settings = entry.settings;

                br.data = "Settings updated";
            }

            CurrentDatabase.SubmitChanges();

            br.setNoError();
            br.id = setting.Id;
            br.count = 1;

            return br;
        }

        // ReSharper disable once UnusedParameter.Local
        private static string SerializeJSON(object item, int version)
        {
            return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss" });
        }
    }
}
