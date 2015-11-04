using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using Newtonsoft.Json.Converters;
using System.IO;
using UtilityExtensions;
using DbUtil = CmsData.DbUtil;
using System.Web;
using Dapper;

namespace CmsWeb.Areas.Public.Controllers
{
    public class MobileAPIController : Controller
    {
        public ActionResult Exists()
        {
            return Content("1");
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
                return AuthorizationError(result);

            savePushID(Util.UserPeopleId ?? 0, dataIn.device, dataIn.key);

            MobileSettings ms = getUserInfo();

            var br = new BaseMessage();
            br.setNoError();
            br.data = SerializeJSON(ms, dataIn.version);
            br.token = result.User.ApiSessions.Single().SessionToken.ToString();
            return br;
        }

        [HttpPost]
        public ActionResult CheckSessionToken(string data)
        {
            var result = AuthenticateUser();

            if (!result.IsValid)
                return AuthorizationError(result);

            var br = new BaseMessage();
            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult ExpireSessionToken(string data)
        {
            var sessionToken = Request.Headers["SessionToken"];
            if (string.IsNullOrEmpty(sessionToken))
                return BaseMessage.createErrorReturn("SessionToken header is required.", BaseMessage.API_ERROR_IMPROPER_HEADER_STRUCTURE);

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
                return BaseMessage.createErrorReturn("SessionToken header is required.", BaseMessage.API_ERROR_IMPROPER_HEADER_STRUCTURE);

            var pinHeader = Request.Headers["PIN"];
            var authorizationHeader = Request.Headers["Authorization"];

            // if the user is resetting a session without their PIN, then credentials will be required
            if (string.IsNullOrEmpty(pinHeader))
            {
                // clear out the session token temporarily to ensure that authentication happens solely by credentials
                Request.Headers.Remove("SessionToken");

                if (string.IsNullOrEmpty(authorizationHeader))
                    return BaseMessage.createErrorReturn("Either the Authorization or PIN header is required because the session is getting reset.", BaseMessage.API_ERROR_IMPROPER_HEADER_STRUCTURE);
            }

            var result = AccountModel.ResetSessionExpiration(sessionToken);

            if (!result.IsValid)
                return BaseMessage.createErrorReturn("You are not authorized!", MapStatusToError(result.Status));

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

            var givingOrgId = DbUtil.Db.Organizations
                 .Where(o => o.RegistrationTypeId == RegistrationTypeCode.OnlineGiving)
                 .Select(x => x.OrganizationId).FirstOrDefault();

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/{givingOrgId}?{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/{givingOrgId}");
            }

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeGivingLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            
            var sql = @"
SELECT OrganizationId FROM dbo.Organizations
WHERE RegistrationTypeId = 8
AND RegSettingXml.value('(/Settings/Fees/DonationFundId)[1]', 'int') IS NULL";
            var givingOrgId = DbUtil.Db.Connection.ExecuteScalar(sql) as int?;

            var ot = GetOneTimeLink(givingOrgId ?? 0, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //          DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            }

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeManagedGivingLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);

            var managedGivingOrgId = DbUtil.Db.Organizations
                 .Where(o => o.RegistrationTypeId == RegistrationTypeCode.ManageGiving)
                 .Select(x => x.OrganizationId).FirstOrDefault();

            var ot = GetOneTimeLink(managedGivingOrgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            }

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeRegisterLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            var orgId = dataIn.argInt;

            var ot = GetOneTimeLink(orgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}");
            }

            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult OneTimeRegisterLink2(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            var dataIn = BaseMessage.createFromString(data);
            var orgId = dataIn.argInt;

            var ot = GetOneTimeLink(orgId, result.User.PeopleId.GetValueOrDefault());

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();
            //			DbUtil.LogActivity($"APIPerson GetOneTimeRegisterLink2 {OrgId}, {PeopleId}");

            var br = new BaseMessage();

            if (dataIn.version >= BaseMessage.API_VERSION_3)
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?showfamily=true&{dataIn.getSourceQueryString()}");
            }
            else
            {
                br.data = DbUtil.Db.ServerLink($"OnlineReg/RegisterLink/{ot.Id.ToCode()}?showfamily=true");
            }

            br.setNoError();
            return br;
        }

        private MobileSettings getUserInfo()
        {
            var roles = from r in DbUtil.Db.UserRoles
                        where r.UserId == Util.UserId
                        orderby r.Role.RoleName
                        select r.Role.RoleName;

            MobileSettings ms = new MobileSettings();

            ms.peopleID = Util.UserPeopleId ?? 0;
            ms.userID = Util.UserId;
            ms.userName = Util.UserFullName;
            ms.roles = roles.ToList();

            return ms;
        }

        private void savePushID(int peopleID, int device, string pushID)
        {
            if (pushID == null || pushID.Length == 0 || peopleID == 0) return;

            var registration = (from e in DbUtil.Db.MobileAppPushRegistrations
                                where e.RegistrationId == pushID
                                select e).FirstOrDefault();

            if (registration == null)
            {
                MobileAppPushRegistration register = new MobileAppPushRegistration();
                register.Enabled = true;
                register.PeopleId = peopleID;
                register.Type = device;
                register.RegistrationId = pushID;

                DbUtil.Db.MobileAppPushRegistrations.InsertOnSubmit(register);
            }
            else
            {
                registration.PeopleId = peopleID;
            }

            DbUtil.Db.SubmitChanges();
        }

        private bool enablePushID(string pushID, bool enabled)
        {
            var registration = (from e in DbUtil.Db.MobileAppPushRegistrations
                                where e.RegistrationId == pushID
                                select e).FirstOrDefault();

            if (registration != null)
            {
                registration.Enabled = true;
                DbUtil.Db.SubmitChanges();

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
            if (!result.IsValid) return AuthorizationError(result);

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
                        br.setNoError();

                    break;
                }

                case 3: // Disable - May not be used
                {
                    if (enablePushID(dataIn.key, false))
                        br.setNoError();

                    break;
                }

                default: break;
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchPeople(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostSearch mps = JsonConvert.DeserializeObject<MobilePostSearch>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var m = new SearchModel(mps.name, mps.comm, mps.addr);

            br.setNoError();
            br.count = m.Count;

            switch (dataIn.device)
            {
                case BaseMessage.API_DEVICE_ANDROID:
                    {
                        Dictionary<int, MobilePerson> mpl = new Dictionary<int, MobilePerson>();

                        MobilePerson mp;

                        foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(100))
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

                        foreach (var item in m.ApplySearch().OrderBy(p => p.Name2).Take(100))
                        {
                            mp.Add(new MobilePerson().populate(item));
                        }

                    br.data = SerializeJSON(mp, dataIn.version);
                        break;
                    }
            }

            return br;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult RegCategories(string id)
        {
            var a = id.Split('-');
            string val = null;
            if (a.Length > 0)
            {
                var org = DbUtil.Db.LoadOrganizationById(a[1].ToInt());
                if (org != null)
                    val = org.AppCategory ?? "Other";
            }
            var categories = new Dictionary<string, string>();
            var lines = DbUtil.Db.Content("AppRegistrations", "Other\tRegistrations").TrimEnd();
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
                categories.Add("Other", "Registrations");
            if (val.HasValue())
                categories.Add("selected", val);
            return Json(categories);
        }

        private List<MobileRegistrationCategory> GetRegistrations()
        {
            var registrations = (from o in DbUtil.Db.ViewAppRegistrations
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
            var lines = DbUtil.Db.Content("AppRegistrations", "Other\tRegistrations").TrimEnd();
            var re = new Regex(@"^(\S*)\s+(.*)$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            var line = re.Match(lines);
            while (line.Success)
            {
                categories.Add(line.Groups[1].Value, line.Groups[2].Value.TrimEnd());
                line = line.NextMatch();
            }
            if (!categories.ContainsKey("Other"))
                categories.Add("Other", "Registrations");
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
                    list.Add(new MobileRegistrationCategory()
                    {
                        Current = true,
                        Title = cat.Value,
                        Registrations = current
                    });

                var future = (from mm in registrations
                              where mm.Category == cat.Key
                              where mm.RegStart > dt
                              orderby mm.PublicSortOrder, mm.Description
                              select mm).ToList();
                if (future.Count > 0)
                    list.Add(new MobileRegistrationCategory()
                    {
                        Current = false,
                        Title = cat.Value,
                        Registrations = future.ToList()
                    });
            }
            return list;
        }

        public ActionResult FetchRegistrations(string data)
        {
            var dataIn = BaseMessage.createFromString(data);

            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

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
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostFetchPerson mpfs = JsonConvert.DeserializeObject<MobilePostFetchPerson>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var person = DbUtil.Db.People.SingleOrDefault(p => p.PeopleId == mpfs.id);

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
                List<MobilePerson> mp = new List<MobilePerson>();
                mp.Add(new MobilePerson().populate(person));
                br.data = SerializeJSON(mp, dataIn.version);
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchImage(string data)
        {
            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostFetchImage mpfi = JsonConvert.DeserializeObject<MobilePostFetchImage>(dataIn.data);

            BaseMessage br = new BaseMessage();
            if (mpfi.id == 0) return br.setData("The ID for the person cannot be set to zero");

            br.data = "The picture was not found.";

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
                    br.setNoError();
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

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostSaveImage mpsi = JsonConvert.DeserializeObject<MobilePostSaveImage>(dataIn.data);

            BaseMessage br = new BaseMessage();

            var imageBytes = Convert.FromBase64String(mpsi.image);

            var person = DbUtil.Db.People.SingleOrDefault(pp => pp.PeopleId == mpsi.id);

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
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var tasks = from t in DbUtil.Db.ViewIncompleteTasks
                        orderby t.CreatedOn, t.StatusId, t.OwnerId, t.CoOwnerId
                        where t.OwnerId == Util.UserPeopleId || t.CoOwnerId == Util.UserPeopleId
                        select t;

            var complete = (from c in DbUtil.Db.Tasks
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
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var task = new TaskModel();
            task.AcceptTask(dataIn.argInt);

            BaseMessage br = new BaseMessage();
            br.count = 1;
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult DeclineTask(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var task = new TaskModel();
            task.DeclineTask(dataIn.argInt, dataIn.argString);

            BaseMessage br = new BaseMessage();
            br.count = 1;
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult CompleteTask(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var task = new TaskModel();
            task.CompleteTask(dataIn.argInt);

            BaseMessage br = new BaseMessage();
            br.count = 1;
            br.setNoError();

            return br;
        }

        [HttpPost]
        public ActionResult FetchCompleteWithContactLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var tasks = new TaskModel();
            var contactid = tasks.AddCompletedContact(dataIn.argInt);

            BaseMessage br = new BaseMessage();
            br.data = GetOneTimeLoginLink($"/Contact2/{contactid}?edit=true&{dataIn.getSourceQueryString()}", Util.UserName);
            br.count = 1;
            br.setNoError();
            return br;
        }

        [HttpPost]
        public ActionResult FetchCompletedContactLink(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            BaseMessage dataIn = BaseMessage.createFromString(data);

            var task = (from t in DbUtil.Db.Tasks
                        where t.Id == dataIn.argInt
                        select t).SingleOrDefault();

            BaseMessage br = new BaseMessage();

            if (task != null && task.CompletedContactId != null)
            {
                br.data = GetOneTimeLoginLink($"/Contact2/{task.CompletedContactId}?{dataIn.getSourceQueryString()}", Util.UserName);
                br.count = 1;
                br.setNoError();
            }

            return br;
        }

        [HttpPost]
        public ActionResult FetchOrgs(string data)
        {
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

            BaseMessage dataIn = BaseMessage.createFromString(data);

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
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    where o.SecurityTypeId != 3
                    select o;
            }
            else
            {
                q = from o in DbUtil.Db.Organizations
                    where o.LimitToRole == null || roles.Contains(o.LimitToRole)
                    //let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                    where (o.OrganizationMembers.Any(om => om.PeopleId == pid // either a leader, who is not pending / inactive
                                       && (om.Pending ?? false) == false
                                       && (om.MemberTypeId != MemberTypeCode.InActive)
                                       && (om.MemberType.AttendanceTypeId == AttendTypeCode.Leader)
                               )
                       || oids.Contains(o.OrganizationId)) // or a leader of a parent org
                    where o.SecurityTypeId != 3
                    where o.OrganizationStatusId == OrgStatusCode.Active
                    select o;
            }

            var orgs = from o in q
                       //let sc = o.OrgSchedules.FirstOrDefault() // SCHED
                           //join sch in DbUtil.Db.OrgSchedules on o.OrganizationId equals sch.OrganizationId
                       from sch in DbUtil.Db.OrgSchedules.Where(s => o.OrganizationId == s.OrganizationId).DefaultIfEmpty()
                       from mtg in DbUtil.Db.Meetings.Where(m => o.OrganizationId == m.OrganizationId).OrderByDescending(m => m.MeetingDate).Take(1).DefaultIfEmpty()
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
            int.TryParse(DbUtil.Db.GetSetting("TZOffset", "0"), out tzOffset);

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
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

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
                int.TryParse(DbUtil.Db.GetSetting("TZOffset", "0"), out tzOffset);

                if (tzOffset != 0)
                {
                    mprl.changeHourOffset(tzOffset);
                }
            }

            var meetingId = DbUtil.Db.CreateMeeting(mprl.id, mprl.datetime);
            var people = RollsheetModel.RollList(meetingId, mprl.id, mprl.datetime);

            var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.MeetingId == meetingId);

            MobileRollList mrl = new MobileRollList();
            mrl.attendees = new List<MobileAttendee>();
            mrl.meetingID = meetingId;
            mrl.headcountEnabled = DbUtil.Db.Setting("RegularMeetingHeadCount", "true");
            mrl.headcount = meeting.HeadCount ?? 0;

            BaseMessage br = new BaseMessage();
            br.id = meetingId;
            br.setNoError();
            br.count = people.Count();

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
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

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
                int.TryParse(DbUtil.Db.GetSetting("TZOffset", "0"), out tzOffset);

                if (tzOffset != 0)
                {
                    mpa.changeHourOffset(tzOffset);
                }
            }

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
            DbUtil.LogActivity($"Mobile RecAtt o:{meeting.OrganizationId} p:{mpa.peopleID} u:{Util.UserPeopleId} a:{mpa.present}");

            BaseMessage br = new BaseMessage();
            br.setNoError();
            br.count = 1;

            return br;
        }

        [HttpPost]
        public ActionResult RecordHeadcount(string data)
        {
            if (DbUtil.Db.Setting("RegularMeetingHeadCount", "true") == "disabled")
            {
                return BaseMessage.createErrorReturn("Headcounts for meetings are disabled");
            }

            // Authenticate first
            var result = AuthenticateUser();
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance roles is required to take attendance for organizations");

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
                int.TryParse(DbUtil.Db.GetSetting("TZOffset", "0"), out tzOffset);

                if (tzOffset != 0)
                {
                    mph.changeHourOffset(tzOffset);
                }
            }

            var meeting = DbUtil.Db.Meetings.SingleOrDefault(m => m.OrganizationId == mph.orgID && m.MeetingDate == mph.datetime);
            meeting.HeadCount = mph.headcount;

            DbUtil.Db.SubmitChanges();

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
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance role is required to take attendance for organizations.");

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostAddPerson mpap = JsonConvert.DeserializeObject<MobilePostAddPerson>(dataIn.data);
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
                p.CampusId = meeting.Organization.CampusId;
                DbUtil.Db.SubmitChanges();
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
            if (!result.IsValid) return AuthorizationError(result);

            // Check Role
            if (!CMSRoleProvider.provider.IsUserInRole(AccountModel.UserName2, "Attendance"))
                return BaseMessage.createErrorReturn("Attendance role is required to take attendance for organizations.");

            BaseMessage dataIn = BaseMessage.createFromString(data);
            MobilePostJoinOrg mpjo = JsonConvert.DeserializeObject<MobilePostJoinOrg>(dataIn.data);

            var om = DbUtil.Db.OrganizationMembers.SingleOrDefault(m => m.PeopleId == mpjo.peopleID && m.OrganizationId == mpjo.orgID);

            if (om == null && mpjo.join)
                om = OrganizationMember.InsertOrgMembers(DbUtil.Db, mpjo.orgID, mpjo.peopleID, MemberTypeCode.Member, DateTime.Now, null, false);

            if (om != null && !mpjo.join)
            {
                om.Drop(DbUtil.Db, DateTime.Today);

                DbUtil.LogActivity($"Dropped {om.PeopleId} for {om.Organization.OrganizationId} via {dataIn.getSourceOS()} app");
            }

            DbUtil.Db.SubmitChanges();

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

        private static string GetOneTimeLoginLink(string url, string user)
        {
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = user,
                Expires = DateTime.Now.AddMinutes(15)
            };

            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();

            return $"{DbUtil.Db.ServerLink($"Logon?ReturnUrl={HttpUtility.UrlEncode(url)}&otltoken={ot.Id.ToCode()}")}";
        }

        private static string SerializeJSON(Object item, int version)
        {
            if (version == BaseMessage.API_VERSION_2)
            {
                return JsonConvert.SerializeObject(item);
            }
            else
            {
                return JsonConvert.SerializeObject(item, new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss" });
            }
        }
    }
}
