using CmsData;
using CmsData.Classes.Twilio;
using CmsWeb.Constants;
using CmsWeb.Membership;
using CmsWeb.Models;
using CmsWeb.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
    public class MobileAccount : IDbBinder
    {
        public enum Results
        {
            None,
            CommonCodeCreationFailed,
            CommonEmailSent,
            CommonEmailSentCode,
            CommonInvalidEmail,
            CommonInvalidDeepLink,
            CommonSMSNotConfigured,
            CommonSMSSent,
            CommonSMSSendFailed,
            CreateFoundSimiliar,
            CreateMultipleMatches,
            CreateFoundPersonNewUser,
            CreateFoundPersonExistingUser,
            CreateNewPersonAndUser,
            DeepLinkInvalidUser,
            DeepLinkNoPeopleFound,
        }

        public CMSDataContext CurrentDatabase { get; set; }
        internal CMSDataContext Db => CurrentDatabase;

        private string first { get; set; }
        private string last { get; set; }
        private string email { get; set; }
        private string phone { get; set; }
        private DateTime? birthdate { get; set; }

        private int device { get; set; }
        private string instance { get; set; }
        private string key { get; set; }
        private string build { get; set; }

        private User user { get; set; }
        private Person person { get; set; }
        private Person similiarPerson { get; set; }

        private Results results = Results.None;

        [Obsolete(Errors.ModelBindingConstructorError, true)]
        public MobileAccount() { }
        public MobileAccount(CMSDataContext Db)
        {
            CurrentDatabase = Db;
        }

        public void setCreateFields(string first, string last, string email, string phone, string dob, int device, string instance, string key, string build)
        {
            this.first = first;
            this.last = last;
            this.email = email.Trim();
            this.phone = phone.GetDigits();

            this.device = device;
            this.instance = instance;
            this.key = key;
            this.build = build;

            if (Util.DateValid(dob, out var bd))
            {
                birthdate = bd;
            }
        }

        public void create()
        {
            if (!Util.ValidEmail(email))
            {
                results = Results.CommonInvalidEmail;

                return;
            }

            int[] foundpeopleids = Db.FindPerson(first, last, birthdate, email, phone).Select(vv => vv.PeopleId.Value).ToArray();

            List<Person> foundpeople = (from pp in Db.People
                                        where foundpeopleids.Contains(pp.PeopleId)
                                        select pp).ToList();

            if (foundpeople.Count == 0)
            {
                createPersonAndUser();

                results = Results.CreateNewPersonAndUser;

                return;
            }

            if (foundpeople.Count > 1)
            {
                results = Results.CreateMultipleMatches;

                return;
            }

            // We only matched on one person
            person = foundpeople[0];

            if (person.EmailAddress.Equal(email) || person.EmailAddress2.Equal(email))
            {
                // They match on an email plus everything else
                if (person.Users.Any())
                {
                    user = person.Users.First();
                    results = Results.CreateFoundPersonExistingUser;
                }
                else
                {
                    createUser();

                    results = Results.CreateFoundPersonNewUser;
                }
            }
            else
            {
                // They match on everything except email
                similiarPerson = person;

                createPersonAndUser();

                results = Results.CreateFoundSimiliar;
            }
        }

        public void setDeepLinkFields(int device, string instance, string key, string email)
        {
            this.device = device;
            this.instance = instance;
            this.key = key;
            this.email = email?.Trim();
            if (!email.Contains("@"))
            {
                this.phone = email.GetDigits();
            }
        }

        public void sendLoginCode()
        {
            if (Db.People.Any(
                p => p.EmailAddress == email || p.EmailAddress2 == email ||
                    (p.CellPhone.Length > 0 && p.CellPhone == phone)
                ))
            {
                var code = createQuickSignInCode(device, instance, key, email, build);

                if (!code.HasValue())
                {
                    results = Results.CommonCodeCreationFailed;
                    return;
                }

                var church = Db.Setting("NameOfChurch", Db.Host);

                if (phone.HasValue())
                {
                    var systemSMSGroup = TwilioHelper.GetSystemSMSGroup(Db);
                    if (TwilioHelper.IsConfigured(Db) && systemSMSGroup?.Count > 0)
                    {
                        var index = new Random().Next(0, systemSMSGroup.Count);
                        var fromNumber = systemSMSGroup[index];
                        var message = Db.Setting("MobileQuickSignInCodeSMS", "{code} is your one-time sign in code for {church}");
                        message = message.Replace("{code}", code).Replace("{church}", church);
                        if (!TwilioHelper.SendSMS(Db, phone, fromNumber, message))
                        {
                            Db.LogActivity($"SMS send failed to {phone} from {fromNumber.Number}");
                            results = Results.CommonSMSSendFailed;
                        }
                        else
                        {
                            results = Results.CommonSMSSent;
                        }
                    }
                    else
                    {
                        results = Results.CommonSMSNotConfigured;
                    }
                }
                else
                {
                    List<MailAddress> mailAddresses = new List<MailAddress>();
                    mailAddresses.Add(new MailAddress(email));

                    var message = @"<h3>Here's your one-time mobile sign in code for {church}:</h3><h4 style=""text-align:center;font-family:monospace"">{code}</h4>";
                    var body = Db.ContentHtml("MobileQuickSignInCodeEmailBody", message);

                    body = body.Replace("{code}", code).Replace("{church}", church);

                    Db.SendEmail(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), Db.Setting("MobileQuickSignInCodeSubject", "Mobile Sign In Code"), body, mailAddresses);
                    results = Results.CommonEmailSentCode;
                }
            }
            else
            {
                results = Results.DeepLinkNoPeopleFound;
            }
        }

        public void sendDeepLink()
        {
            if (Db.People.Any(p => p.EmailAddress == email || p.EmailAddress2 == email))
            {
                string code = createQuickSignInCode(device, instance, key, email, build);
                string deepLink = Db.Setting("MobileDeepLinkURL", "");

                if (string.IsNullOrEmpty(code))
                {
                    results = Results.CommonCodeCreationFailed;

                    return;
                }

                if (string.IsNullOrEmpty(deepLink))
                {
                    results = Results.CommonInvalidDeepLink;

                    return;
                }

                List<MailAddress> mailAddresses = new List<MailAddress>();
                mailAddresses.Add(new MailAddress(email));

                string link = $"{deepLink}/signin/quick/{code}";
                string message = @"<a href=""{link}"">Click here to sign in</a>";

                string body = Db.ContentHtml("NewMobileUserDeepLink", message);
                body = body.Replace("{link}", link);

                Db.SendEmail(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), Db.Setting("MobileQuickSignInSubject", "Mobile Sign In Link"), body, mailAddresses);

                results = Results.CommonEmailSent;
            }
            else
            {
                results = Results.DeepLinkNoPeopleFound;
            }
        }

        public MobileMessage getMobileResponse(bool useMobileMesages)
        {
            MobileMessage response = new MobileMessage();

            switch (results)
            {
                case Results.CommonEmailSent:
                    response.argString = "email";
                    response.argBool = true;
                    response.setNoError();
                    break;

                case Results.CommonEmailSentCode:
                    response.argString = "code";
                    response.argBool = true;
                    response.setNoError();
                    break;

                case Results.CommonCodeCreationFailed:
                    response.setError((int)MobileMessage.Error.CREATE_CODE_FAILED);
                    break;

                case Results.CommonInvalidDeepLink:
                    response.setError((int)MobileMessage.Error.INVALID_DEEP_LINK);
                    break;

                case Results.CommonInvalidEmail:
                    response.setError((int)MobileMessage.Error.INVALID_EMAIL);
                    break;

                case Results.CreateMultipleMatches:
                    if (useMobileMesages)
                    {
                        sendDeepLink();

                        if (results == Results.CommonEmailSent)
                        {
                            response.setNoError();
                            response.argBool = true;
                        }
                        else
                        {
                            response.setError((int)MobileMessage.Error.EMAIL_NOT_SENT);
                        }
                    }
                    else
                    {
                        response.setError((int)MobileMessage.Error.CREATE_FAILED);
                    }
                    break;

                case Results.CreateFoundSimiliar:
                    if (useMobileMesages)
                    {
                        notifyAboutDuplicate();
                        notifyNewUserWithDeepLink(device, instance, key);

                        if (results == Results.CommonEmailSent)
                        {
                            response.setNoError();
                            response.argBool = true;
                        }
                        else
                        {
                            response.setError((int)MobileMessage.Error.EMAIL_NOT_SENT);
                        }
                    }
                    else
                    {
                        notifyAboutDuplicate();
                        notifyNewUser();

                        response.setNoError();
                        response.data = user.Username;
                    }
                    break;

                case Results.CreateFoundPersonExistingUser:
                    if (useMobileMesages)
                    {
                        notifyNewUserWithDeepLink(device, instance, key);

                        if (results == Results.CommonEmailSent)
                        {
                            response.setNoError();
                            response.argBool = true;
                        }
                        else
                        {
                            response.setError((int)MobileMessage.Error.EMAIL_NOT_SENT);
                        }
                    }
                    else
                    {
                        notifyAboutExisting();

                        response.setNoError();
                        response.data = user.Username;
                    }
                    break;

                case Results.CreateNewPersonAndUser:
                case Results.CreateFoundPersonNewUser:
                    if (useMobileMesages)
                    {
                        notifyNewUserWithDeepLink(device, instance, key);

                        if (results == Results.CommonEmailSent)
                        {
                            response.setNoError();
                            response.argBool = true;
                        }
                        else
                        {
                            response.setError((int)MobileMessage.Error.EMAIL_NOT_SENT);
                        }
                    }
                    else
                    {
                        notifyNewUser();
                        response.setNoError();
                        response.data = user.Username;
                    }
                    break;

                case Results.DeepLinkNoPeopleFound:
                    response.argBool = false;
                    response.setNoError();
                    break;

                case Results.CommonSMSNotConfigured:
                    response.argBool = false;
                    response.setError((int)MobileMessage.Error.SMS_NOT_CONFIGURED);
                    break;

                case Results.CommonSMSSendFailed:
                    response.argBool = false;
                    response.setError((int)MobileMessage.Error.SMS_SEND_FAILED);
                    break;

                case Results.CommonSMSSent:
                    response.argBool = true;
                    response.setNoError();
                    break;

                default:
                    response.setError((int)MobileMessage.Error.UNKNOWN);
                    break;
            }

            return response;
        }

        private void createPersonAndUser()
        {
            person = Person.Add(Db, null, first, null, last, birthdate);
            person.PositionInFamilyId = CmsData.Codes.PositionInFamily.PrimaryAdult;
            person.EmailAddress = email;
            person.HomePhone = phone;

            EntryPoint appEntryPoint = Db.EntryPoints.FirstOrDefault(ep => ep.Code == "APP");

            if (appEntryPoint != null)
            {
                person.EntryPointId = appEntryPoint.Id;
            }

            Db.SubmitChanges();

            createUser();
        }

        private void createUser()
        {
            user = MembershipService.CreateUser(Db, person.PeopleId);

            Db.SubmitChanges();
        }

        private void notifyNewUser()
        {
            AccountModel.SendNewUserEmail(Db, user.Username);
        }

        private void notifyNewUserWithDeepLink(int device, string instance, string key)
        {
            if (user == null)
            {
                results = Results.DeepLinkInvalidUser;

                return;
            }

            if (email == null)
            {
                results = Results.CommonInvalidEmail;

                return;
            }

            string code = createQuickSignInCode(device, instance, key, email, build);
            string deepLink = Db.Setting("MobileDeepLinkURL", "");
            string body = Db.ContentHtml("NewMobileUserWelcome", "");

            if (body.Length == 0)
            {
                body = Db.ContentHtml("NewUserWelcome", Resource1.AccountModel_NewUserWelcome);
            }

            if (code.Length == 0)
            {
                results = Results.CommonCodeCreationFailed;

                return;
            }

            if (deepLink.Length == 0)
            {
                results = Results.CommonInvalidDeepLink;

                return;
            }

            body = body.Replace("{first}", user.Person.PreferredName);
            body = body.Replace("{name}", user.Person.Name);
            body = body.Replace("{cmshost}", Db.Setting("DefaultHost", Db.Host));
            body = body.Replace("{username}", user.Username);
            body = body.Replace("{link}", $"{deepLink}/signin/quick/{code}");

            Db.Email(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), user.Person, null, "New Mobile User Welcome", body);

            results = Results.CommonEmailSent;
        }

        private void notifyAboutExisting()
        {
            string message = Db.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
            message = message.Replace("{name}", person.Name).Replace("{host}", Db.CmsHost);

            Db.Email(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), person, null, "Account Information for " + Db.Host, message);
        }

        private void notifyAboutDuplicate()
        {
            string message = Db.ContentHtml("NotifyDuplicateUserOnMobile", Resource1.NotifyDuplicateUserOnMobile);
            message = message.Replace("{otheremail}", email);

            Db.Email(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), person, null, "New User Account on " + Db.Host, message);
        }

        private string createQuickSignInCode(int device, string instanceID, string key, string email, string version)
        {
            var rng = new Random();
            string code = rng.Next(0, 999999).ToString("D6");
            string hash = createHash($"{code}{instanceID}");

            MobileAppDevice appDevice = new MobileAppDevice
            {
                Created = DateTime.Now,
                LastSeen = DateTime.Now,
                DeviceTypeID = device,
                InstanceID = instanceID,
                NotificationID = key,
                Authentication = "",
                Code = hash,
                CodeExpires = DateTime.Now.AddMinutes(15),
                CodeEmail = email?.ToLower(),
                AppVersion = version,
            };

            Db.MobileAppDevices.InsertOnSubmit(appDevice);
            Db.SubmitChanges();

            return code;
        }

        public static string createHash(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);

            SHA256Managed sha256Managed = new SHA256Managed();
            byte[] hash = sha256Managed.ComputeHash(bytes);

            StringBuilder hashString = new StringBuilder(64);

            foreach (byte x in hash)
            {
                hashString.Append(x.ToString("x2"));
            }

            return hashString.ToString();
        }
    }
}
