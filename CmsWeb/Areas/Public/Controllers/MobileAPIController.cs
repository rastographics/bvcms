using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.Manage.Models;
using CmsWeb.Areas.People.Models.Task;
using CmsWeb.Areas.Reports.Models;
using CmsWeb.Lifecycle;
using CmsWeb.MobileAPI;
using CmsWeb.Models;
using Dapper;
using ImageData;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;

namespace CmsWeb.Areas.Public.Controllers
{
    public class MobileAPIController : CMSBaseController
    {
        public MobileAPIController(IRequestManager requestManager) : base(requestManager)
        {
        }

        public ActionResult Exists()
        {
            return Content("1");
        }

        [HttpPost]
        public ActionResult CreateUser(string data)
        {
            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostCreate mpc = JsonConvert.DeserializeObject<MobilePostCreate>(dataIn.data);

            MobileAccount account = MobileAccount.Create(mpc.first, mpc.last, mpc.email, mpc.phone, mpc.dob);

            var br = new BaseMessage();

            // todo: notify user based on ResultCode
            if (account.Result == MobileAccount.ResultCode.BadEmailAddress || account.Result == MobileAccount.ResultCode.FoundMultipleMatches)
            {
                br.setError(BaseMessage.API_ERROR_CREATE_FAILED);
            }
            else
            {
                br.setNoError();
                br.data = account.User.Username;
            }

            return br;
        }

        private UserValidationResult AuthenticateUser(bool requirePin = false)
        {
            // Username and password checks are only necessary for the old iOS application
            var hasInvalidAuthHeaders = (string.IsNullOrEmpty(Request.Headers["Authorization"]) && (string.IsNullOrEmpty(Request.Headers["username"]) || string.IsNullOrEmpty(Request.Headers["password"])));
            var hasInvalidSessionTokenHeader = string.IsNullOrEmpty(Request.Headers["SessionToken"]);

            if (hasInvalidAuthHeaders && hasInvalidSessionTokenHeader)
            {
                //DbUtil.LogActivity("authentication headers bad");
                return UserValidationResult.Invalid(UserValidationStatus.ImproperHeaderStructure, "Either the Authorization or SessionToken headers are required.", null);
            }

            //DbUtil.LogActivity("calling authenticatemobile2");
            return AccountModel.AuthenticateMobile2(requirePin: requirePin, checkOrgLeadersOnly: true);
        }

        [HttpPost]
        public ActionResult Authenticate(string data)
        {
            var dataIn = BaseMessage.createFromString(data);

            var result = AuthenticateUser(requirePin: true);

            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            savePushID(Util.UserPeopleId ?? 0, dataIn.device, dataIn.key);

            MobileSettings ms = getUserInfo();

            var br = new BaseMessage();
            br.setNoError();
            br.data = SerializeJSON(ms, dataIn.version);
            br.token = result.User.ApiSessions.Single().SessionToken.ToString();
            return br;
        }

        [HttpPost]
        public ActionResult AuthenticatedLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            // Link in data string is path only include leading slash
            var dataIn = BaseMessage.createFromString(data);

            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = Util.UserName,
                Expires = DateTime.Now.AddMinutes(15)
            };

            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();

            var br = new BaseMessage();
            br.setNoError();
            br.data = $"{CurrentDatabase.ServerLink($"Logon?ReturnUrl={HttpUtility.UrlEncode(dataIn.argString)}&otltoken={ot.Id.ToCode()}")}";

            return br;
        }

        [HttpPost]
        public ActionResult CheckSessionToken(string data)
        {
            var result = AuthenticateUser();

            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            var br = new BaseMessage();
            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult ExpireSessionToken(string data)
        {
            var sessionToken = Request.Headers["SessionToken"];
            if (string.IsNullOrEmpty(sessionToken))
            {
                return BaseMessage.createErrorReturn("SessionToken header is required.", BaseMessage.API_ERROR_IMPROPER_HEADER_STRUCTURE);
            }

            AccountModel.ExpireSessionToken(sessionToken);

            Session.Abandon();

            var br = new BaseMessage();
            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult ResetSessionToken(string data)
        {
            var dataIn = BaseMessage.createFromString(data);

            var sessionToken = Request.Headers["SessionToken"];
            if (string.IsNullOrEmpty(sessionToken))
            {
                return BaseMessage.createErrorReturn("SessionToken header is required.", BaseMessage.API_ERROR_IMPROPER_HEADER_STRUCTURE);
            }

            var pinHeader = Request.Headers["PIN"];
            var authorizationHeader = Request.Headers["Authorization"];

            // if the user is resetting a session without their PIN, then credentials will be required
            if (string.IsNullOrEmpty(pinHeader))
            {
                // clear out the session token temporarily to ensure that authentication happens solely by credentials
                Request.Headers.Remove("SessionToken");

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return BaseMessage.createErrorReturn("Either the Authorization or PIN header is required because the session is getting reset.", BaseMessage.API_ERROR_IMPROPER_HEADER_STRUCTURE);
                }
            }

            var result = AccountModel.ResetSessionExpiration(sessionToken);

            if (!result.IsValid)
            {
                return BaseMessage.createErrorReturn("You are not authorized!", MapStatusToError(result.Status));
            }

            AuthenticateUser(requirePin: true);

            savePushID(Util.UserPeopleId ?? 0, dataIn.device, dataIn.key);

            MobileSettings ms = getUserInfo();

            var br = new BaseMessage();
            br.setNoError();
            br.data = SerializeJSON(ms, dataIn.version);
            return br;
        }

        [HttpPost]
        public ActionResult GivingLink(string data)
        {
            var dataIn = BaseMessage.createFromString(data);

            var sql = @"SELECT OrganizationId FROM dbo.Organizations WHERE RegistrationTypeId = 8 AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";
            var givingOrgId = CurrentDatabase.Connection.ExecuteScalar(sql) as int?;

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = CurrentDatabase.ServerLink($"OnlineReg/{givingOrgId}?{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = CurrentDatabase.ServerLink($"OnlineReg/{givingOrgId}");
            }

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeGivingLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            var dataIn = BaseMessage.createFromString(data);

            var sql = @"
SELECT OrganizationId FROM dbo.Organizations
WHERE RegistrationTypeId = 8
AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";
            var givingOrgId = CurrentDatabase.Connection.ExecuteScalar(sql) as int?;

            var ot = GetOneTimeLink(givingOrgId ?? 0, result.User.PeopleId.GetValueOrDefault());

            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();
            //          DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink {OrgId}, {PeopleId}");

            var br = new BaseMessage
            {
                data = CurrentDatabase.ServerLink(dataIn.version >= BaseMessage.API_VERSION_3 ? $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{dataIn.getSourceQueryString()}" : $"OnlineReg/RegisterLink/{ot.Id.ToCode()}")
            };

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeManagedGivingLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            var dataIn = BaseMessage.createFromString(data);

            var managedGivingOrgId = CurrentDatabase.Organizations
                                                    .Where(o => o.RegistrationTypeId == RegistrationTypeCode.ManageGiving)
                                                    .Select(x => x.OrganizationId).FirstOrDefault();

            var ot = GetOneTimeLink(managedGivingOrgId, result.User.PeopleId.GetValueOrDefault());

            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();
            //			DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = CurrentDatabase.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = CurrentDatabase.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            }

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeRegisterLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            var dataIn = BaseMessage.createFromString(data);
            var orgId = dataIn.argInt;

            var ot = GetOneTimeLink(orgId, result.User.PeopleId.GetValueOrDefault());

            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();
            //			DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            br.data = CurrentDatabase.ServerLink(dataIn.version >= BaseMessage.API_VERSION_3 ? $"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{dataIn.getSourceQueryString()}" : $"OnlineReg/RegisterLink/{ot.Id.ToCode()}");

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeRegisterLink2(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            var dataIn = BaseMessage.createFromString(data);
            var orgId = dataIn.argInt;

            var ot = GetOneTimeLink(orgId, result.User.PeopleId.GetValueOrDefault());

            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();
            //			DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink2 {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = CurrentDatabase.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?showfamily=true&{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = CurrentDatabase.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?showfamily=true");
            }

            br.setNoError();
            return br;
        }

        private MobileSettings getUserInfo()
        {
            var roles = from r in CurrentDatabase.UserRoles
                        where r.UserId == Util.UserId
                        orderby r.Role.RoleName
                        select r.Role.RoleName;

            MobileSettings ms = new MobileSettings
            {
                peopleID = Util.UserPeopleId ?? 0,
                userID = Util.UserId,
                userName = Util.UserFullName,
                roles = roles.ToList()
            };

            return ms;
        }

        private void savePushID(int peopleID, int device, string pushID)
        {
            if (pushID == null || pushID.Length == 0 || peopleID == 0)
            {
                return;
            }

            var registration = (from e in CurrentDatabase.MobileAppPushRegistrations
                                where e.RegistrationId == pushID
                                select e).FirstOrDefault();

            if (registration == null)
            {
                MobileAppPushRegistration register = new MobileAppPushRegistration
                {
                    Enabled = true,
                    PeopleId = peopleID,
                    Type = device,
                    RegistrationId = pushID
                };

                CurrentDatabase.MobileAppPushRegistrations.InsertOnSubmit(register);
            }
            else
            {
                registration.PeopleId = peopleID;
            }

            CurrentDatabase.SubmitChanges();
        }

        private bool enablePushID(string pushID, bool enabled)
        {
            var registration = (from e in CurrentDatabase.MobileAppPushRegistrations
                                where e.RegistrationId == pushID
                                select e).FirstOrDefault();

            if (registration != null)
            {
                registration.Enabled = true;
                CurrentDatabase.SubmitChanges();

                return true;
            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult RegisterPushID(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            BaseMessage br = new BaseMessage();

            switch (dataIn.argInt)
            {
                case 1: // Add
                    {
                        savePushID(Util.UserPeopleId ?? 0, dataIn.device, dataIn.key);
                        br.setNoError();
                        break;
                    }

                case 2: // Enable - May not be used
                    {
                        if (enablePushID(dataIn.key, true))
                        {
                            br.setNoError();
                        }

                        break;
                    }

                case 3: // Disable - May not be used
                    {
                        if (enablePushID(dataIn.key, false))
                        {
                            br.setNoError();
                        }

                        break;
                    }

                default:
                    break;
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchPeople(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostSearch mps = JsonConvert.DeserializeObject<MobilePostSearch>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var m = new PeopleSearchModel(mps.guest);
            m.m.name = mps.name;
            m.m.communication = mps.comm;
            m.m.address = mps.addr;

            br.setNoError();

            switch (dataIn.device)
            {
                case BaseMessage.API_DEVICE_ANDROID:
                    {
                        Dictionary<int, MobilePerson> mpl = new Dictionary<int, MobilePerson>();

                        MobilePerson mp;

                        foreach (var item in m.FetchPeople().OrderBy(p => p.Name2).Take(100))
                        {
                            mp = new MobilePerson().populate(item);
                            mpl.Add(mp.id, mp);
                        }

                        br.data = SerializeJSON(mpl, dataIn.version);
                        break;
                    }

                case BaseMessage.API_DEVICE_IOS:
                    {
                        List<MobilePerson> mp = new List<MobilePerson>();

                        foreach (var item in m.FetchPeople().OrderBy(p => p.Name2).Take(100))
                        {
                            mp.Add(new MobilePerson().populate(item));
                        }

                        br.data = SerializeJSON(mp, dataIn.version);
                        break;
                    }
            }
            br.count = m.Count();
            return br;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RegCategories(string id)
        {
            var a = id.Split('-');
            string val = null;
            if (a.Length > 0)
            {
                var org = CurrentDatabase.LoadOrganizationById(a[1].ToInt());
                if (org != null)
                {
                    val = org.AppCategory ?? "Other";
                }
            }
            var categories = new Dictionary<string, string>();
            var lines = CurrentDatabase.Content("AppRegistrations", "Other\tRegistrations").TrimEnd();
            var re = new Regex(@"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var line = re.Match(lines);
            while (line.Success)
            {
                var code = line.Groups[1].Value;
                var text = line.Groups[2].Value.TrimEnd();
                categories.Add(code, text);
                line = line.NextMatch();
            }
            if (!categories.ContainsKey("Other"))
            {
                categories.Add("Other", "Registrations");
            }

            if (val.HasValue())
            {
                categories.Add("selected", val);
            }

            return Json(categories);
        }

        private List<MobileRegistrationCategory> GetRegistrations()
        {
            var registrations = (from o in CurrentDatabase.ViewAppRegistrations
                                 let sort = o.PublicSortOrder == null || o.PublicSortOrder.Length == 0 ? "10" : o.PublicSortOrder
                                 select new MobileRegistration
                                 {
                                     OrgId = o.OrganizationId,
                                     Name = o.Title ?? o.OrganizationName,
                                     UseRegisterLink2 = o.UseRegisterLink2 ?? false,
                                     Description = o.Description,
                                     PublicSortOrder = sort,
                                     Category = o.AppCategory,
                                     RegStart = o.RegStart,
                                     RegEnd = o.RegEnd
                                 }).ToList();

            var categories = new Dictionary<string, string>();
            var lines = CurrentDatabase.Content("AppRegistrations", "Other\tRegistrations").TrimEnd();
            var re = new Regex(@"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var line = re.Match(lines);
            while (line.Success)
            {
                categories.Add(line.Groups[1].Value, line.Groups[2].Value.TrimEnd());
                line = line.NextMatch();
            }
            if (!categories.ContainsKey("Other"))
            {
                categories.Add("Other", "Registrations");
            }

            var list = new List<MobileRegistrationCategory>();
            var dt = Util.Now;
            foreach (var cat in categories)
            {
                var current = (from mm in registrations
                               where mm.Category == cat.Key
                               where mm.RegStart <= dt
                               orderby mm.PublicSortOrder, mm.Description
                               select mm).ToList();
                if (current.Count > 0)
                {
                    list.Add(new MobileRegistrationCategory()
                    {
                        Current = true,
                        Title = cat.Value,
                        Registrations = current
                    });
                }

                var future = (from mm in registrations
                              where mm.Category == cat.Key
                              where mm.RegStart > dt
                              orderby mm.PublicSortOrder, mm.Description
                              select mm).ToList();
                if (future.Count > 0)
                {
                    list.Add(new MobileRegistrationCategory()
                    {
                        Current = false,
                        Title = cat.Value,
                        Registrations = future.ToList()
                    });
                }
            }
            return list;
        }

        public ActionResult FetchRegistrations(string data)
        {
            var dataIn = BaseMessage.createFromString(data);

            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            var br = new BaseMessage();
            br.setNoError();
            br.data = SerializeJSON(GetRegistrations(), dataIn.version);
            return br;
        }

        [HttpPost]
        public ActionResult FetchPerson(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostFetchPerson mpfs = JsonConvert.DeserializeObject<MobilePostFetchPerson>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var person = CurrentDatabase.People.SingleOrDefault(p => p.PeopleId == mpfs.id);

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
                br.data = SerializeJSON(new MobilePerson().populate(person), dataIn.version);
            }
            else
            {
                List<MobilePerson> mp = new List<MobilePerson> { new MobilePerson().populate(person) };
                br.data = SerializeJSON(mp, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchPersonExtended(string data)
        {
            // Authenticate first
            UserValidationResult result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            BaseMessage br = new BaseMessage();

            Person person = CurrentDatabase.People.SingleOrDefault(p => p.PeopleId == dataIn.argInt);

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
                br.data = SerializeJSON(new MobilePersonExtended().populate(person, dataIn.argBool), dataIn.version);
            }
            else
            {
                List<MobilePersonExtended> mp = new List<MobilePersonExtended> { new MobilePersonExtended().populate(person, dataIn.argBool) };
                br.data = SerializeJSON(mp, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult UpdatePerson(string data)
        {
            // Authenticate first
            UserValidationResult result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            Person user = CurrentDatabase.People.FirstOrDefault(p => p.PeopleId == Util.UserPeopleId);

            if (user == null)
            {
                return BaseMessage.createErrorReturn("User not found!");
            }

            if (user.PositionInFamilyId == PositionInFamily.Child)
            {
                return BaseMessage.createErrorReturn("Childern cannot edit records");
            }

            if (user.PositionInFamilyId == PositionInFamily.SecondaryAdult && Util.UserPeopleId != dataIn.argInt)
            {
                return BaseMessage.createErrorReturn("Secondary adults can only modify themselves");
            }

            if (user.PositionInFamilyId == PositionInFamily.PrimaryAdult && user.Family.People.SingleOrDefault(fm => fm.PeopleId == dataIn.argInt) == null)
            {
                return BaseMessage.createErrorReturn("Person must be in the same family");
            }

            BaseMessage br = new BaseMessage();

            Person person = CurrentDatabase.People.SingleOrDefault(p => p.PeopleId == dataIn.argInt);

            if (person == null)
            {
                br.setError(BaseMessage.API_ERROR_PERSON_NOT_FOUND);
                br.data = "Person not found.";
                return br;
            }

            List<MobilePostEditField> fields = JsonConvert.DeserializeObject<List<MobilePostEditField>>(dataIn.data);

            List<ChangeDetail> personChangeList = new List<ChangeDetail>();
            List<ChangeDetail> familyChangeList = new List<ChangeDetail>();

            foreach (MobilePostEditField field in fields)
            {
                field.updatePerson(person, personChangeList, familyChangeList);
            }

            if (personChangeList.Count > 0)
            {
                person.LogChanges(CurrentDatabase, personChangeList);
            }

            if (familyChangeList.Count > 0)
            {
                person.Family.LogChanges(CurrentDatabase, familyChangeList, person.PeopleId);
            }

            CurrentDatabase.SubmitChanges();

            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult FetchGivingHistory(string data)
        {
            // Authenticate first
            UserValidationResult result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            if (Util.UserPeopleId != dataIn.argInt)
            {
                return BaseMessage.createErrorReturn("Giving history is not available for other people");
            }

            BaseMessage br = new BaseMessage();

            Person person = CurrentDatabase.People.SingleOrDefault(p => p.PeopleId == dataIn.argInt);

            if (person == null)
            {
                br.setError(BaseMessage.API_ERROR_PERSON_NOT_FOUND);
                br.data = "Person not found.";
                return br;
            }

            int lastYear = DateTime.Now.Year - 2;
            int thisYear = DateTime.Now.Year - 1;

            decimal lastYearTotal = (from c in CurrentDatabase.Contributions
                                     where c.PeopleId == person.PeopleId
                                             || (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                                     where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                                     where c.ContributionStatusId == ContributionStatusCode.Recorded
                                     where c.ContributionDate.Value.Year == lastYear
                                     orderby c.ContributionDate descending
                                     select c).AsEnumerable().Sum(c => c.ContributionAmount ?? 0);

            List<MobileGivingEntry> entries = (from c in CurrentDatabase.Contributions
                                               let online = c.BundleDetails.Single().BundleHeader.BundleHeaderType.Description.Contains("Online")
                                               where c.PeopleId == person.PeopleId
                                                       || (c.PeopleId == person.SpouseId && (person.ContributionOptionsId ?? StatementOptionCode.Joint) == StatementOptionCode.Joint)
                                               where !ContributionTypeCode.ReturnedReversedTypes.Contains(c.ContributionTypeId)
                                               where c.ContributionTypeId != ContributionTypeCode.Pledge
                                               where c.ContributionStatusId == ContributionStatusCode.Recorded
                                               where c.ContributionDate.Value.Year == thisYear
                                               orderby c.ContributionDate descending
                                               select new MobileGivingEntry()
                                               {
                                                   id = c.ContributionId,
                                                   date = c.ContributionDate ?? DateTime.Now,
                                                   fund = c.ContributionFund.FundName,
                                                   giver = c.Person.Name,
                                                   check = c.CheckNo,
                                                   amount = (int)(c.ContributionAmount == null ? 0 : c.ContributionAmount * 100),
                                                   type = ContributionTypeCode.SpecialTypes.Contains(c.ContributionTypeId)
                                                       ? c.ContributionType.Description
                                                       : !online
                                                           ? c.ContributionType.Description
                                                           : c.ContributionDesc == "Recurring Giving"
                                                               ? c.ContributionDesc
                                                               : "Online"
                                               }).ToList();

            MobileGivingHistory history = new MobileGivingHistory();
            history.updateEntries(thisYear, entries);
            history.setLastYearTotal(lastYear, (int)(lastYearTotal * 100));

            br.data = SerializeJSON(history, dataIn.version);
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult FetchInvolvement(string data)
        {
            // Authenticate first
            UserValidationResult result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            if (Util.UserPeopleId != dataIn.argInt)
            {
                return BaseMessage.createErrorReturn("Involvement is not available for other people");
            }

            bool limitvisibility = Util2.OrgLeadersOnly || !HttpContextFactory.Current.User.IsInRole("Access");
            int[] oids = new int[0];

            if (Util2.OrgLeadersOnly)
            {
                oids = CurrentDatabase.GetLeaderOrgIds(Util.UserPeopleId);
            }

            string[] roles = CurrentDatabase.CurrentRoles();

            List<MobileInvolvement> orgList = (from om in CurrentDatabase.OrganizationMembers
                                               let org = om.Organization
                                               where om.PeopleId == Util.UserPeopleId
                                               where (om.Pending ?? false) == false
                                               where oids.Contains(om.OrganizationId) || !(limitvisibility && om.Organization.SecurityTypeId == 3)
                                               where org.LimitToRole == null || roles.Contains(org.LimitToRole)
                                               orderby om.Organization.OrganizationType.Code ?? "z", om.Organization.OrganizationName
                                               select new MobileInvolvement
                                               {
                                                   name = om.Organization.OrganizationName,
                                                   leader = om.Organization.LeaderName ?? "",
                                                   type = om.MemberType.Description,
                                                   division = om.Organization.Division.Name,
                                                   program = om.Organization.Division.Program.Name,
                                                   @group = om.Organization.OrganizationType.Description ?? "Other",
                                                   enrolledDate = om.EnrollmentDate,
                                                   attendancePercent = (int)(om.AttendPct == null ? 0 : om.AttendPct * 100)
                                               }).ToList();

            BaseMessage br = new BaseMessage
            {
                data = SerializeJSON(orgList, dataIn.version)
            };
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult FetchImage(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostFetchImage mpfi = JsonConvert.DeserializeObject<MobilePostFetchImage>(dataIn.data);

            BaseMessage br = new BaseMessage();
            if (mpfi.id == 0)
            {
                return br.setData("The ID for the person cannot be set to zero");
            }

            br.data = "The picture was not found.";

            var person = CurrentDatabase.People.SingleOrDefault(pp => pp.PeopleId == mpfi.id);

            if (person?.PictureId == null)
            {
                return br;
            }

            Image image = null;

            switch (mpfi.size)
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

            if (image != null)
            {
                br.data = Convert.ToBase64String(image.Bits);
                br.count = 1;
                br.setNoError();
            }

            return br;
        }

        [HttpPost]
        public ActionResult SaveImage(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostSaveImage mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var imageBytes = Convert.FromBase64String(mpsi.image);

            var person = CurrentDatabase.People.SingleOrDefault(pp => pp.PeopleId == mpsi.id);

            if (person?.Picture != null)
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

            CurrentDatabase.SubmitChanges();
            person.LogPictureUpload(CurrentDatabase, Util.UserPeopleId ?? 1);

            br.setNoError();
            br.data = "Image updated.";
            br.id = mpsi.id;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult SaveFamilyImage(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostSaveImage mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var imageBytes = Convert.FromBase64String(mpsi.image);

            var family = CurrentDatabase.Families.SingleOrDefault(pp => pp.FamilyId == mpsi.id);

            if (family?.Picture != null)
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
            br.id = mpsi.id;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult FetchTasks(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var tasks = from t in CurrentDatabase.ViewIncompleteTasks
                        orderby t.CreatedOn, t.StatusId, t.OwnerId, t.CoOwnerId
                        where t.OwnerId == Util.UserPeopleId || t.CoOwnerId == Util.UserPeopleId
                        select t;

            var complete = (from c in CurrentDatabase.Tasks
                            where c.StatusId == TaskStatusCode.Complete
                            where c.OwnerId == Util.UserPeopleId || c.CoOwnerId == Util.UserPeopleId
                            orderby c.CreatedOn descending
                            select c).Take(20);

            BaseMessage br = new BaseMessage();

            switch (dataIn.device)
            {
                case BaseMessage.API_DEVICE_ANDROID:
                    {
                        Dictionary<int, MobileTask> taskList = new Dictionary<int, MobileTask>();

                        foreach (var item in tasks)
                        {
                            MobileTask task = new MobileTask().populate(item, Util.UserPeopleId ?? 0);
                            taskList.Add(task.id, task);
                        }

                        foreach (var item in complete)
                        {
                            MobileTask task = new MobileTask().populate(item, Util.UserPeopleId ?? 0);
                            taskList.Add(task.id, task);
                        }

                        br.data = SerializeJSON(taskList, dataIn.version);
                        break;
                    }

                case BaseMessage.API_DEVICE_IOS:
                    {
                        List<MobileTask> taskList = new List<MobileTask>();

                        foreach (var item in tasks)
                        {
                            MobileTask task = new MobileTask().populate(item, Util.UserPeopleId ?? 0);
                            taskList.Add(task);
                        }

                        foreach (var item in complete)
                        {
                            MobileTask task = new MobileTask().populate(item, Util.UserPeopleId ?? 0);
                            taskList.Add(task);
                        }

                        br.data = SerializeJSON(taskList, dataIn.version);
                        break;
                    }
            }

            br.count = tasks.Count();
            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult AcceptTask(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            TaskModel.AcceptTask(dataIn.argInt, CurrentDatabase.Host, CurrentDatabase);

            BaseMessage br = new BaseMessage
            {
                count = 1
            };
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult DeclineTask(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            TaskModel.DeclineTask(dataIn.argInt, dataIn.argString, CurrentDatabase.Host, CurrentDatabase);

            BaseMessage br = new BaseMessage
            {
                count = 1
            };
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult CompleteTask(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            TaskModel.CompleteTask(dataIn.argInt, CurrentDatabase.Host, CurrentDatabase);

            BaseMessage br = new BaseMessage
            {
                count = 1
            };
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult FetchCompleteWithContactLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var contactid = TaskModel.AddCompletedContact(dataIn.argInt, CurrentDatabase.Host, CurrentDatabase);

            BaseMessage br = new BaseMessage
            {
                data = GetOneTimeLoginLink($"/Contact2/{contactid}?edit=true&{dataIn.getSourceQueryString()}", Util.UserName),
                count = 1
            };
            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult FetchCompletedContactLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var task = (from t in CurrentDatabase.Tasks
                        where t.Id == dataIn.argInt
                        select t).SingleOrDefault();

            BaseMessage br = new BaseMessage();

            if (task?.CompletedContactId == null)
            {
                return br;
            }

            br.data = GetOneTimeLoginLink($"/Contact2/{task.CompletedContactId}?{dataIn.getSourceQueryString()}", Util.UserName);
            br.count = 1;
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult FetchOrgs(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
            {
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var pid = Util.UserPeopleId;
            var oids = CurrentDatabase.GetLeaderOrgIds(pid);
            var dt = DateTime.Parse("8:00 AM");

            var roles = CurrentDatabase.CurrentRoles();

            IQueryable<Organization> q = null;

            if (Util2.OrgLeadersOnly)
            {
                q = from o in CurrentDatabase.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    where oids.Contains(o.OrganizationId)
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    select o;
            }
            else
            {
                q = from o in CurrentDatabase.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    //let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where (o.OrganizationMembers.Any(om => om.PeopleId == pid // either a leader, who is not pending / inactive
                                                                       && (om.Pending ?? false) == false
                                                                       && (om.MemberTypeId != MemberTypeCode.InActive)
                                                                       && (om.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                                                                )
                            || oids.Contains(o.OrganizationId)) // or a leader of a parent org
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    select o;
            }

            var orgs = from o in q
                           //let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                           //join sch in CurrentDatabase.OrgSchedules on o.OrganizationId equals sch.OrganizationId
                       from sch in CurrentDatabase.ViewOrgSchedules2s.Where(s => o.OrganizationId == s.OrganizationId).DefaultIfEmpty()
                       from mtg in CurrentDatabase.Meetings.Where(m => o.OrganizationId == m.OrganizationId && m.MeetingDate < DateTime.Today.AddDays(1)).OrderByDescending(m => m.MeetingDate).Take(1).DefaultIfEmpty()
                       orderby sch.SchedDay, sch.SchedTime
                       select new OrganizationInfo
                       {
                           id = o.OrganizationId,
                           name = o.OrganizationName,
                           time = sch.SchedTime,
                           day = sch.SchedDay,
                           lastMeetting = mtg.MeetingDate
                       };

            BaseMessage br = new BaseMessage();
            List<MobileOrganization> mo = new List<MobileOrganization>();

            br.setNoError();
            br.count = orgs.Count();

            int tzOffset = 0;
            int.TryParse(CurrentDatabase.GetSetting("TZOffset", "0"), out tzOffset);

            foreach (var item in orgs)
            {
                MobileOrganization org = new MobileOrganization().populate(item);

                // Initial release version
                if (dataIn.version == BaseMessage.API_VERSION_2 && tzOffset != 0)
                {
                    org.changeHourOffset(-tzOffset);
                }

                mo.Add(org);
            }

            br.data = SerializeJSON(mo, dataIn.version);
            return br;
        }

        [HttpPost]
        public ActionResult FetchOrgRollList(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
            {
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");
            }

            // Convert raw post to avoid "+" being converted to space for iOS timezone info.  Remove this later when app has been updated.
            String rawPost = new StreamReader(this.Request.InputStream).ReadToEnd();
            rawPost = Server.UrlDecode(rawPost).Substring(5);

            // Check to see if type matches
            BaseMessage dataIn = BaseMessage.createFromString(rawPost);

            if (dataIn.device == BaseMessage.API_DEVICE_IOS && dataIn.version == BaseMessage.API_VERSION_2)
            {
                dataIn.data = dataIn.data.Replace(" ", "+");
            }

            MobilePostRollList mprl = JsonConvert.DeserializeObject<MobilePostRollList>(dataIn.data);

            if (dataIn.device == BaseMessage.API_DEVICE_IOS && dataIn.version == BaseMessage.API_VERSION_2)
            {
                int tzOffset = 0;
                int.TryParse(CurrentDatabase.GetSetting("TZOffset", "0"), out tzOffset);

                if (tzOffset != 0)
                {
                    mprl.changeHourOffset(tzOffset);
                }
            }

            var meetingId = CurrentDatabase.CreateMeeting(mprl.id, mprl.datetime);

            var meeting = CurrentDatabase.Meetings.SingleOrDefault(m => m.MeetingId == meetingId);
            var attendanceBySubGroup = meeting.Organization.AttendanceBySubGroups ?? false;
            var people = attendanceBySubGroup
                ? RollsheetModel.RollListFilteredBySubgroup(meetingId, mprl.id, mprl.datetime, fromMobile: true)
                : RollsheetModel.RollList(meetingId, mprl.id, mprl.datetime, fromMobile: true);

            MobileRollList mrl = new MobileRollList
            {
                attendees = new List<MobileAttendee>(),
                meetingID = meetingId,
                headcountEnabled = CurrentDatabase.Setting("RegularMeetingHeadCount", "true"),
                headcount = meeting.HeadCount ?? 0
            };

            BaseMessage br = new BaseMessage
            {
                id = meetingId
            };
            br.setNoError();
            br.count = people.Count;

            foreach (var person in people)
            {
                mrl.attendees.Add(new MobileAttendee().populate(person));
            }

            br.data = SerializeJSON(mrl, dataIn.version);
            return br;
        }

        [HttpPost]
        public ActionResult RecordAttend(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
            {
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");
            }

            // Convert raw post to avoid "+" being converted to space for iOS timezone info.  Remove this later when app has been updated.
            String rawPost = new StreamReader(this.Request.InputStream).ReadToEnd();
            rawPost = Server.UrlDecode(rawPost).Substring(5);

            BaseMessage dataIn = BaseMessage.createFromString(rawPost);

            if (dataIn.device == BaseMessage.API_DEVICE_IOS && dataIn.version == BaseMessage.API_VERSION_2)
            {
                dataIn.data = dataIn.data.Replace(" ", "+");
            }

            MobilePostAttend mpa = JsonConvert.DeserializeObject<MobilePostAttend>(dataIn.data);

            if (dataIn.device == BaseMessage.API_DEVICE_IOS && dataIn.version == BaseMessage.API_VERSION_2)
            {
                int tzOffset = 0;
                int.TryParse(CurrentDatabase.GetSetting("TZOffset", "0"), out tzOffset);

                if (tzOffset != 0)
                {
                    mpa.changeHourOffset(tzOffset);
                }
            }

            var meeting = CurrentDatabase.Meetings.SingleOrDefault(m => m.OrganizationId == mpa.orgID && m.MeetingDate == mpa.datetime);

            if (meeting == null)
            {
                CurrentDatabase.CreateMeeting(mpa.orgID, mpa.datetime);
            }

            Attend.RecordAttend(CurrentDatabase, mpa.peopleID, mpa.orgID, mpa.present, mpa.datetime);

            CurrentDatabase.UpdateMeetingCounters(mpa.orgID);
            DbUtil.LogActivity($"Mobile RecAtt o:{mpa.orgID} p:{mpa.peopleID} u:{Util.UserPeopleId} a:{mpa.present}");

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult RecordHeadcount(string data)
        {
            if (CurrentDatabase.Setting("RegularMeetingHeadCount", "true") == "disabled")
            {
                return BaseMessage.createErrorReturn("Headcounts for meetings are disabled");
            }

            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
            {
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");
            }

            // Convert raw post to avoid "+" being converted to space for iOS timezone info.  Remove this later when app has been updated.
            String rawPost = new StreamReader(this.Request.InputStream).ReadToEnd();
            rawPost = Server.UrlDecode(rawPost).Substring(5);

            BaseMessage dataIn = BaseMessage.createFromString(rawPost);

            if (dataIn.device == BaseMessage.API_DEVICE_IOS && dataIn.version == BaseMessage.API_VERSION_2)
            {
                dataIn.data = dataIn.data.Replace(" ", "+");
            }

            MobilePostHeadcount mph = JsonConvert.DeserializeObject<MobilePostHeadcount>(dataIn.data);

            if (dataIn.device == BaseMessage.API_DEVICE_IOS && dataIn.version == BaseMessage.API_VERSION_2)
            {
                int tzOffset = 0;
                int.TryParse(CurrentDatabase.GetSetting("TZOffset", "0"), out tzOffset);

                if (tzOffset != 0)
                {
                    mph.changeHourOffset(tzOffset);
                }
            }

            var meeting = CurrentDatabase.Meetings.SingleOrDefault(m => m.OrganizationId == mph.orgID && m.MeetingDate == mph.datetime);
            meeting.HeadCount = mph.headcount;

            CurrentDatabase.SubmitChanges();

            DbUtil.LogActivity($"Mobile Headcount o:{meeting.OrganizationId} m:{meeting.MeetingId} h:{mph.headcount}");

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult AddPerson(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
            {
                return BaseMessage.createErrorReturn("Attendance role is required to take attendance for organizations.");
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostAddPerson mpap = JsonConvert.DeserializeObject<MobilePostAddPerson>(dataIn.data);
            mpap.clean();

            var p = new Person
            {
                CreatedDate = Util.Now,
                CreatedBy = Util.UserId,

                MemberStatusId = MemberStatusCode.JustAdded,
                AddressTypeId = 10,

                FirstName = mpap.firstName,
                LastName = mpap.lastName,
                Name = ""
            };

            if (mpap.goesBy.Length > 0)
            {
                p.NickName = mpap.goesBy;
            }

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
                f = CurrentDatabase.Families.First(fam => fam.FamilyId == mpap.familyID);
            }
            else
            {
                f = new Family();

                if (mpap.homePhone.Length > 0)
                {
                    f.HomePhone = mpap.homePhone;
                }

                if (mpap.address.Length > 0)
                {
                    f.AddressLineOne = mpap.address;
                }

                if (mpap.address2.Length > 0)
                {
                    f.AddressLineTwo = mpap.address2;
                }

                if (mpap.city.Length > 0)
                {
                    f.CityName = mpap.city;
                }

                if (mpap.state.Length > 0)
                {
                    f.StateCode = mpap.state;
                }

                if (mpap.zipcode.Length > 0)
                {
                    f.ZipCode = mpap.zipcode;
                }

                if (mpap.country.Length > 0)
                {
                    f.CountryName = mpap.country;
                }

                CurrentDatabase.Families.InsertOnSubmit(f);
            }

            f.People.Add(p);

            p.PositionInFamilyId = CurrentDatabase.ComputePositionInFamily(mpap.getAge(), mpap.maritalStatusID == MaritalStatusCode.Married, f.FamilyId) ?? PositionInFamily.PrimaryAdult;

            p.OriginId = OriginCode.Visit;
            p.FixTitle();

            if (mpap.eMail.Length > 0 && !mpap.eMail.Equal("na"))
            {
                p.EmailAddress = mpap.eMail;
            }

            if (mpap.cellPhone.Length > 0)
            {
                p.CellPhone = mpap.cellPhone;
            }

            if (mpap.homePhone.Length > 0)
            {
                p.HomePhone = mpap.homePhone;
            }

            p.MaritalStatusId = mpap.maritalStatusID;
            p.GenderId = mpap.genderID;

            CurrentDatabase.SubmitChanges();

            if (mpap.visitMeeting > 0)
            {
                var meeting = CurrentDatabase.Meetings.Single(mm => mm.MeetingId == mpap.visitMeeting);
                Attend.RecordAttendance(p.PeopleId, mpap.visitMeeting, true);
                CurrentDatabase.UpdateMeetingCounters(mpap.visitMeeting);
                p.CampusId = meeting.Organization.CampusId;
                p.EntryPoint = meeting.Organization.EntryPoint;
                CurrentDatabase.SubmitChanges();
            }

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.id = p.PeopleId;
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult JoinOrg(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid)
            {
                return AuthorizationError(result);
            }

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
            {
                return BaseMessage.createErrorReturn("Attendance or Checkin role is required to take attendance for organizations.");
            }

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostJoinOrg mpjo = JsonConvert.DeserializeObject<MobilePostJoinOrg>(dataIn.data);

            var om = CurrentDatabase.OrganizationMembers.SingleOrDefault(m => m.PeopleId == mpjo.peopleID && m.OrganizationId == mpjo.orgID);

            if (om == null && mpjo.join)
            {
                om = OrganizationMember.InsertOrgMembers(CurrentDatabase, mpjo.orgID, mpjo.peopleID, MemberTypeCode.Member, DateTime.Today, null, false);
            }

            if (om != null && !mpjo.join)
            {
                om.Drop(CurrentDatabase, DateTime.Now);

                DbUtil.LogActivity($"Dropped {om.PeopleId} for {om.Organization.OrganizationId} via {dataIn.getSourceOS()} app", peopleid: om.PeopleId, orgid: om.OrganizationId);
            }

            CurrentDatabase.SubmitChanges();

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        private static int MapStatusToError(UserValidationStatus status)
        {
            switch (status)
            {
                case UserValidationStatus.Success:
                    return BaseMessage.API_ERROR_NONE;
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
            return BaseMessage.createErrorReturn(result.ErrorMessage ?? "You are not authorized!", MapStatusToError(result.Status));
        }

        private static OneTimeLink GetOneTimeLink(int orgId, int peopleId)
        {
            return new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = $"{orgId},{peopleId},0",
                Expires = DateTime.Now.AddMinutes(10),
            };
        }

        private string GetOneTimeLoginLink(string url, string user)
        {
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = user,
                Expires = DateTime.Now.AddMinutes(15)
            };

            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();

            return $"{CurrentDatabase.ServerLink($"Logon?ReturnUrl={HttpUtility.UrlEncode(url)}&otltoken={ot.Id.ToCode()}")}";
        }

        private string SerializeJSON(Object item, int version)
        {
            if (version == BaseMessage.API_VERSION_2)
            {
                return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:sszzz" });
            }
            else
            {
                return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss" });
            }
        }
    }
}
