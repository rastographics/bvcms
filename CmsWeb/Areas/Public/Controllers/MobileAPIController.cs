using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using CmsWeb.Models.iPhone;
using ImageData;
using Newtonsoft.Json;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Public.Controllers
{
    public class MobileAPIController : Controller
    {
        private UserValidationResult AuthenticateUser()
        {
            var hasInvalidAuthHeaders = (string.IsNullOrEmpty(Request.Headers["Authorization"]) &&
                // the below checks are only necessary for the old iOS application
                                         (string.IsNullOrEmpty(Request.Headers["username"]) ||
                                          string.IsNullOrEmpty(Request.Headers["password"])));

            var hasInvalidSessionTokenHeader = string.IsNullOrEmpty(Request.Headers["SessionToken"]);

            if (hasInvalidAuthHeaders && hasInvalidSessionTokenHeader)
                throw new HttpException(400, "Either Authorization headers or a SessionToken header are required.");

            return AccountModel.AuthenticateMobile();
        }

        [HttpPost]
        public ActionResult Authenticate()
        {
            var result = AuthenticateUser();

            if (!result.IsValid)
                return BaseMessage.createErrorReturn("You are not authorized!", MapStatusToError(result.Status));

            var br = new BaseMessage();
            br.error = 0;
            br.data = result.User.ApiSessions.Single().SessionToken.ToString();
            return br;
        }

        [HttpPost]
        public ActionResult CheckSessionToken(string data)
        {
            var result = AuthenticateUser();

            var br = new BaseMessage();
            if (!result.IsValid)
                return BaseMessage.createErrorReturn("You are not authorized!", MapStatusToError(result.Status));

            // TODO: optionally validate the API type call with different timeouts (i.e. giving might only be 2 minutes versus 30 minutes for everything else)
            //var dataIn = BaseMessage.createFromString(data);

            br.error = 0;

            return br;
        }

        [HttpPost]
        public ActionResult ResetSessionToken()
        {
            var sessionToken = Request.Headers["SessionToken"];
            if (string.IsNullOrEmpty(sessionToken))
                throw new HttpException(400, "The SessionToken header is required.");

            var pinHeader = Request.Headers["PIN"];
            var authorizationHeader = Request.Headers["Authorization"];

            // if the user is resetting a session without their PIN, then credentials will be required
            if (string.IsNullOrEmpty(pinHeader))
            {
                // clear out the session token temporarily to ensure that authentication happens solely by credentials
                Request.Headers.Remove("SessionToken");

                if (string.IsNullOrEmpty(authorizationHeader))
                    throw new HttpException(400, "Either the Authorization or PIN header is required because the session is getting reset.");
            }

            var result = AccountModel.ResetSessionExpiration(sessionToken);
            var br = new BaseMessage();

            if (!result.IsValid)
                return BaseMessage.createErrorReturn("You are not authorized!", MapStatusToError(result.Status));

            br.error = 0;
            return br;
        }

        [HttpPost]
        public ActionResult CheckLogin(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_LOGIN)
                return BaseMessage.createTypeErrorReturn();

            var givingOrg = (from e in DbUtil.Db.Organizations
                             where e.RegistrationTypeId == 8
                             select e).FirstOrDefault();

            var roles = from r in DbUtil.Db.UserRoles
                        where r.UserId == Util.UserId
                        orderby r.Role.RoleName
                        select new MobileRole
                        {
                            id = r.RoleId,
                            name = r.Role.RoleName
                        };

            var ms = new MobileSettings();

            ms.peopleID = Util.UserPeopleId ?? 0;
            ms.userID = Util.UserId;

            ms.givingEnabled = DbUtil.Db.Setting("EnableMobileGiving", "true") == "true" ? 1 : 0;
            ms.givingAllowCC = DbUtil.Db.Setting("NoCreditCardGiving", "false") == "false" ? 1 : 0;
            ms.givingOrgID = givingOrg != null ? givingOrg.OrganizationId : 0;
            ms.roles = roles.ToList();

            // Everything is in order, start the return
            var br = new BaseMessage();
            br.error = 0;
            br.id = Util.UserPeopleId ?? 0;
            br.data = JsonConvert.SerializeObject(ms);

            return br;
        }

        [HttpPost]
        public ActionResult OneTimeGivingLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_GIVING_ONE_TIME_LINK_GIVING)
                return BaseMessage.createTypeErrorReturn();

            var givingOrgId = DbUtil.Db.Organizations
                .Where(o => o.RegistrationTypeId == RegistrationTypeCode.OnlineGiving)
                .Select(x => x.OrganizationId).FirstOrDefault();

            var ot = GetOneTimeLink(givingOrgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity("APIPerson GetOneTimeRegisterLink {0}, {1}".Fmt(OrgId, PeopleId));

            var br = new BaseMessage();
            br.data = Util.CmsHost2 + "OnlineReg/RegisterLink/" + ot.Id.ToCode();
            br.error = 0;
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeManagedGivingLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_GIVING_ONE_TIME_LINK_MANAGED_GIVING)
                return BaseMessage.createTypeErrorReturn();

            var managedGivingOrgId = DbUtil.Db.Organizations
                .Where(o => o.RegistrationTypeId == RegistrationTypeCode.ManageGiving)
                .Select(x => x.OrganizationId).FirstOrDefault();

            var ot = GetOneTimeLink(managedGivingOrgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity("APIPerson GetOneTimeRegisterLink {0}, {1}".Fmt(OrgId, PeopleId));

            var br = new BaseMessage();
            br.data = Util.CmsHost2 + "OnlineReg/RegisterLink/" + ot.Id.ToCode();
            br.error = 0;
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeRegisterLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_REGISTRATION_ONE_TIME_LINK)
                return BaseMessage.createTypeErrorReturn();

            var orgId = dataIn.data.ToInt();

            var ot = GetOneTimeLink(orgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity("APIPerson GetOneTimeRegisterLink {0}, {1}".Fmt(OrgId, PeopleId));

            var br = new BaseMessage();
            br.data = Util.CmsHost2 + "OnlineReg/RegisterLink/" + ot.Id.ToCode();
            br.error = 0;
            return br;
        }
        [HttpPost]
        public ActionResult OneTimeRegisterLink2(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_REGISTRATION_ONE_TIME_LINK)
                return BaseMessage.createTypeErrorReturn();

            var orgId = dataIn.data.ToInt();

            var ot = GetOneTimeLink(orgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity("APIPerson GetOneTimeRegisterLink2 {0}, {1}".Fmt(OrgId, PeopleId));

            var br = new BaseMessage();
            br.data = Util.CmsHost2 + "OnlineReg/RegisterLink2/" + ot.Id.ToCode();
            br.error = 0;
            return br;
        }

        [HttpPost]
        public ActionResult FetchPeople(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_PEOPLE_SEARCH)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mps = JsonConvert.DeserializeObject<MobilePostSearch>(dataIn.data);

            var br = new BaseMessage();

            var m = new SearchModel(mps.name, mps.comm, mps.addr);

            br.error = 0;
            br.type = BaseMessage.API_TYPE_PEOPLE_SEARCH;
            br.count = m.Count;

            switch (dataIn.device)
            {
                case BaseMessage.API_DEVICE_ANDROID:
                    {
                        var mpl = new Dictionary<int, MobilePerson>();

                        MobilePerson mp;

                        foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(20))
                        {
                            mp = new MobilePerson().populate(item);
                            mpl.Add(mp.id, mp);
                        }

                        br.data = JsonConvert.SerializeObject(mpl);
                        break;
                    }

                case BaseMessage.API_DEVICE_IOS:
                    {
                        var mp = new List<MobilePerson>();

                        foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(20))
                        {
                            mp.Add(new MobilePerson().populate(item));
                        }

                        br.data = JsonConvert.SerializeObject(mp);
                        break;
                    }
            }
            return br;
        }
        private List<MobileRegistrationCategory> GetRegistrations()
        {
            var categories = new List<MobileRegistrationCategory>();
            var q = from o in DbUtil.Db.ViewAppRegistrations.ToList()
                    select new MobileRegistration
                    {
                        OrgId = o.OrganizationId, 
                        Name = o.Title ?? o.OrganizationName,
                        UseRegisterLink2 = o.UseRegisterLink2 ?? false, 
                        Description = o.Description,
                        PublicSortOrder = o.PublicSortOrder
                    };
            foreach (var line in DbUtil.Db.Content("AppRegistrations", "\tRegistrations").SplitLines())
            {
                var a = line.Split('\t');
                var prefix = a[0].TrimEnd(':');
                var title = a[1];
                var r = (from mm in q
                         where mm.Category == prefix
                         orderby mm.PublicSortOrder
                         select mm).ToList();
                if(r.Any())
                    categories.Add(new MobileRegistrationCategory()
                    {
                        Title = title, 
                        Registrations = r
                    });
            }
            return categories;
        }

        public ActionResult Registrations()
        {
            return Content(JsonConvert.SerializeObject(GetRegistrations(), Formatting.Indented), "text/plain");
        }
        public ActionResult FetchRegistrations(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_REGISTRATIONS)
                return BaseMessage.createTypeErrorReturn();

            var br = new BaseMessage();

            br.error = 0;
            br.type = BaseMessage.API_TYPE_REGISTRATIONS;
            //br.count = m.Count;

            br.data = JsonConvert.SerializeObject(GetRegistrations());
            return br;
        }

        [HttpPost]
        public ActionResult FetchPerson(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_PERSON_REFRESH)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mpfs = JsonConvert.DeserializeObject<MobilePostFetchPerson>(dataIn.data);

            var br = new BaseMessage();

            var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == mpfs.id);

            if (person == null)
            {
                br.error = 1;
                br.data = "Person not found.";
                return br;
            }

            br.error = 0;
            br.type = BaseMessage.API_TYPE_PERSON_REFRESH;
            br.count = 1;

            if (dataIn.device == BaseMessage.API_DEVICE_ANDROID)
            {
                br.data = JsonConvert.SerializeObject(new MobilePerson().populate(person));
            }
            else
            {
                var mp = new List<MobilePerson>();
                mp.Add(new MobilePerson().populate(person));
                br.data = JsonConvert.SerializeObject(mp);
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchImage(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_PERSON_IMAGE_REFRESH)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mpfi = JsonConvert.DeserializeObject<MobilePostFetchImage>(dataIn.data);

            var br = new BaseMessage();
            if (mpfi.id == 0) return br.setData("The ID for the person cannot be set to zero");

            br.data = "The picture was not found.";
            br.type = BaseMessage.API_TYPE_PERSON_IMAGE_REFRESH;

            var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == mpfi.id);

            if (person.PictureId != null)
            {
                Image image = null;

                switch (mpfi.size)
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
                    br.error = 0;
                }
            }

            return br;
        }

        [HttpPost]
        public ActionResult SaveImage(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_PERSON_IMAGE_ADD)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>(dataIn.data);

            var br = new BaseMessage();

            var imageBytes = Convert.FromBase64String(mpsi.image);

            var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == mpsi.id);

            if (person.Picture != null)
            {
                ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.ThumbId));
                ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.SmallId));
                ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.MediumId));
                ImageData.DbUtil.Db.Images.DeleteOnSubmit(ImageData.DbUtil.Db.Images.SingleOrDefault(i => i.Id == person.Picture.LargeId));

                person.Picture.ThumbId = Image.NewImageFromBits(imageBytes, 50, 50).Id;
                person.Picture.SmallId = Image.NewImageFromBits(imageBytes, 120, 120).Id;
                person.Picture.MediumId = Image.NewImageFromBits(imageBytes, 320, 400).Id;
                person.Picture.LargeId = Image.NewImageFromBits(imageBytes).Id;
            }
            else
            {
                var newPicture = new Picture();

                newPicture.ThumbId = Image.NewImageFromBits(imageBytes, 50, 50).Id;
                newPicture.SmallId = Image.NewImageFromBits(imageBytes, 120, 120).Id;
                newPicture.MediumId = Image.NewImageFromBits(imageBytes, 320, 400).Id;
                newPicture.LargeId = Image.NewImageFromBits(imageBytes).Id;

                person.Picture = newPicture;
            }

            DbUtil.Db.SubmitChanges();

            br.error = 0;
            br.data = "Image updated.";
            br.id = mpsi.id;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult FetchOrgs(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_ORG_REFRESH)
                return BaseMessage.createTypeErrorReturn();

            var pid = Util.UserPeopleId;
            var oids = DbUtil.Db.GetLeaderOrgIds(pid);
            var dt = DateTime.Parse("8:00 AM");

            var roles = DbUtil.Db.CurrentRoles();

            IQueryable<Organization> q = null;

            if (Util2.OrgLeadersOnly)
            {
                q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where oids.Contains(o.OrganizationId)
                    where o.SecurityTypeId != 3
                    select o;
            }
            else
            {
                q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where (o.OrganizationMembers.Any(om => om.PeopleId == pid // either a leader, who is not pending / inactive
                                  && (om.Pending ?? false) == false
                                  && (om.MemberTypeId != MemberTypeCode.InActive)
                                  && (om.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                            )
                         || oids.Contains(o.OrganizationId))
                    // or a leader of a parent org
                    where o.SecurityTypeId != 3
                    select o;
            }

            var orgs = from o in q
                       let sc = o.OrgSchedules.FirstOrDefault()
                       // SCHED
                       select new OrganizationInfo
                       {
                           id = o.OrganizationId,
                           name = o.OrganizationName,
                           time = sc.SchedTime ?? dt,
                           day = sc.SchedDay ?? 0
                       };

            var br = new BaseMessage();
            var mo = new List<MobileOrganization>();

            br.error = 0;
            br.type = BaseMessage.API_TYPE_ORG_REFRESH;
            br.count = orgs.Count();

            foreach (var item in orgs)
            {
                mo.Add(new MobileOrganization().populate(item));
            }

            br.data = JsonConvert.SerializeObject(mo);
            return br;
        }

        [HttpPost]
        public ActionResult FetchOrgRollList(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_ORG_ROLL_REFRESH)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mprl = JsonConvert.DeserializeObject<MobilePostRollList>(dataIn.data);

            var meetingId = DbUtil.Db.CreateMeeting(mprl.id, mprl.datetime);
            var people = RollsheetModel.RollList(meetingId, mprl.id, mprl.datetime);

            var br = new BaseMessage();
            var ma = new List<MobileAttendee>();

            br.id = meetingId;
            br.error = 0;
            br.type = BaseMessage.API_TYPE_ORG_ROLL_REFRESH;
            br.count = people.Count();

            foreach (var person in people)
            {
                ma.Add(new MobileAttendee().populate(person));
            }

            br.data = JsonConvert.SerializeObject(ma);
            return br;
        }

        [HttpPost]
        public ActionResult RecordAttend(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_ORG_RECORD_ATTEND)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mpa = JsonConvert.DeserializeObject<MobilePostAttend>(dataIn.data);

            var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.OrganizationId == mpa.orgID && m.MeetingDate == mpa.datetime);

            if (meeting == null)
            {
                meeting = new Meeting
                {
                    OrganizationId = mpa.orgID,
                    MeetingDate = mpa.datetime,
                    CreatedDate = Util.Now,
                    CreatedBy = Util.UserPeopleId ?? 0,
                    GroupMeetingFlag = false,
                };
                DbUtil.Db.Meetings.InsertOnSubmit(meeting);
                DbUtil.Db.SubmitChanges();
                var acr = (from s in DbUtil.Db.OrgSchedules
                           where s.OrganizationId == mpa.orgID
                           where s.SchedTime.Value.TimeOfDay == mpa.datetime.TimeOfDay
                           where s.SchedDay == (int)mpa.datetime.DayOfWeek
                           select s.AttendCreditId).SingleOrDefault();
                meeting.AttendCreditId = acr;
            }

            Attend.RecordAttendance(mpa.peopleID, meeting.MeetingId, mpa.present);

            DbUtil.Db.UpdateMeetingCounters(mpa.orgID);
            DbUtil.LogActivity("Mobile RecAtt o:{0} p:{1} u:{2} a:{3}".Fmt(meeting.OrganizationId, mpa.peopleID, Util.UserPeopleId, mpa.present));

            var br = new BaseMessage();

            br.error = 0;
            br.count = 1;
            br.type = BaseMessage.API_TYPE_ORG_RECORD_ATTEND;

            return br;
        }

        [HttpPost]
        public ActionResult AddPerson(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance role is required to take attendance for organizations.");

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_PERSON_ADD)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mpap = JsonConvert.DeserializeObject<MobilePostAddPerson>(dataIn.data);
            mpap.clean();

            var p = new Person();

            p.CreatedDate = Util.Now;
            p.CreatedBy = Util.UserId;

            p.MemberStatusId = MemberStatusCode.JustAdded;
            p.AddressTypeId = 10;

            p.FirstName = mpap.firstName;
            p.LastName = mpap.lastName;
            p.Name = "";

            if (mpap.goesBy.Length > 0)
                p.NickName = mpap.goesBy;

            if (mpap.birthday != null)
            {
                p.BirthDay = mpap.birthday.Value.Day;
                p.BirthMonth = mpap.birthday.Value.Month;
                p.BirthYear = mpap.birthday.Value.Year;
            }

            p.GenderId = mpap.genderID;
            p.MaritalStatusId = mpap.maritalStatusID;

            Family f;

            if (mpap.familyID > 0)
            {
                f = DbUtil.Db.Families.First(fam => fam.FamilyId == mpap.familyID);
            }
            else
            {
                f = new Family();

                if (mpap.homePhone.Length > 0)
                    f.HomePhone = mpap.homePhone;

                if (mpap.address.Length > 0)
                    f.AddressLineOne = mpap.address;

                if (mpap.address2.Length > 0)
                    f.AddressLineTwo = mpap.address2;

                if (mpap.city.Length > 0)
                    f.CityName = mpap.city;

                if (mpap.state.Length > 0)
                    f.StateCode = mpap.state;

                if (mpap.zipcode.Length > 0)
                    f.ZipCode = mpap.zipcode;

                if (mpap.country.Length > 0)
                    f.CountryName = mpap.country;

                DbUtil.Db.Families.InsertOnSubmit(f);
            }

            f.People.Add(p);

            p.PositionInFamilyId = PositionInFamily.Child;

            if (mpap.birthday != null && p.GetAge() >= 18)
            {
                if (f.People.Count(per => per.PositionInFamilyId == PositionInFamily.PrimaryAdult) < 2)
                    p.PositionInFamilyId = PositionInFamily.PrimaryAdult;
                else
                    p.PositionInFamilyId = PositionInFamily.SecondaryAdult;
            }

            p.OriginId = OriginCode.Visit;
            p.FixTitle();

            if (mpap.eMail.Length > 0)
                p.EmailAddress = mpap.eMail;

            if (mpap.cellPhone.Length > 0)
                p.CellPhone = mpap.cellPhone;

            if (mpap.homePhone.Length > 0)
                p.HomePhone = mpap.homePhone;

            p.MaritalStatusId = mpap.maritalStatusID;
            p.GenderId = mpap.genderID;

            DbUtil.Db.SubmitChanges();

            if (mpap.visitMeeting > 0)
            {
                var meeting = DbUtil.Db.Meetings.Single(mm => mm.MeetingId == mpap.visitMeeting);
                Attend.RecordAttendance(p.PeopleId, mpap.visitMeeting, true);
                DbUtil.Db.UpdateMeetingCounters(mpap.visitMeeting);
            }

            var br = new BaseMessage();

            br.error = 0;
            br.id = p.PeopleId;
            br.count = 1;
            br.type = BaseMessage.API_TYPE_PERSON_ADD;

            return br;
        }

        [HttpPost]
        public ActionResult JoinOrg(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance role is required to take attendance for organizations.");

            // Check to see if type matches
            var dataIn = BaseMessage.createFromString(data);
            if (dataIn.type != BaseMessage.API_TYPE_ORG_JOIN)
                return BaseMessage.createTypeErrorReturn();

            // Everything is in order, start the return
            var mpjo = JsonConvert.DeserializeObject<MobilePostJoinOrg>(dataIn.data);

            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == mpjo.peopleID && m.OrganizationId == mpjo.orgID);

            if (om != null)
            {
                if (mpjo.join)
                    om = OrganizationMember.InsertOrgMembers(DbUtil.Db, mpjo.orgID, mpjo.peopleID, MemberTypeCode.Member, DateTime.Now, null, false);
                else
                    om.Drop(DbUtil.Db, addToHistory: true);
            }

            DbUtil.Db.SubmitChanges();

            var br = new BaseMessage();

            br.error = 0;
            br.count = 1;
            br.type = BaseMessage.API_TYPE_ORG_JOIN;

            return br;
        }

        private static int MapStatusToError(UserValidationStatus status)
        {
            switch (status)
            {
                case UserValidationStatus.Success:
                    return BaseMessage.API_ERROR_SUCCESS;
                case UserValidationStatus.PinExpired:
                    return BaseMessage.API_ERROR_PIN_EXPIRED;
                case UserValidationStatus.PinInvalid:
                    return BaseMessage.API_ERROR_PIN_INVALID;
                case UserValidationStatus.SessionTokenExpired:
                    return BaseMessage.API_ERROR_SESSION_TOKEN_EXPIRED;
                default:
                    return BaseMessage.API_ERROR_SESSION_TOKEN_NOT_FOUND;
            }
        }

        private static BaseMessage AuthorizationError(UserValidationResult result)
        {
            return BaseMessage.createErrorReturn("You are not authorized!", MapStatusToError(result.Status));
        }

        private static OneTimeLink GetOneTimeLink(int orgId, int peopleId)
        {
            return new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = "{0},{1},0".Fmt(orgId, peopleId),
                Expires = DateTime.Now.AddMinutes(10),
            };
        }
    }
}
