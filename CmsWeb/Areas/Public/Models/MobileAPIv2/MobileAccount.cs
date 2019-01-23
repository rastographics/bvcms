using CmsData;
using CmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Areas.Public.Models.MobileAPIv2
{
    public class MobileAccount
    {
        public enum Results
        {
            None,
            CommonCodeCreationFailed,
            CommonEmailSent,
            CommonInvalidEmail,
            CommonInvalidDeepLink,
            CreateFoundSimiliar,
            CreateMultipleMatches,
            CreateFoundPersonNewUser,
            CreateFoundPersonExistingUser,
            CreateNewPersonAndUser,
            DeepLinkInvalidUser,
            DeepLinkNoPeopleFound,
        }

        private readonly CMSDataContext db;

        private string first { get; set; }
        private string last { get; set; }
        private string email { get; set; }
        private string phone { get; set; }
        private DateTime? birthdate { get; set; }

        private int device { get; set; }
        private string instance { get; set; }
        private string key { get; set; }

        private User user { get; set; }
        private Person person { get; set; }
        private Person similiarPerson { get; set; }

        private Results results = Results.None;

        public MobileAccount(CMSDataContext db)
        {
            this.db = db;
        }

        public void setCreateFields(string first, string last, string email, string phone, string dob, int device, string instance, string key)
        {
            this.first = first;
            this.last = last;
            this.email = email.Trim();
            this.phone = phone.GetDigits();

            this.device = device;
            this.instance = instance;
            this.key = key;

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

            int[] foundpeopleids = DbUtil.Db.FindPerson(first, last, birthdate, email, phone).Select(vv => vv.PeopleId.Value).ToArray();

            List<Person> foundpeople = (from pp in DbUtil.Db.People
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
        }

        public void sendDeepLink()
        {
            if (DbUtil.Db.People.Any(p => p.EmailAddress == email || p.EmailAddress2 == email))
            {
                string code = createQuickSignInCode(device, instance, key, email);
                string deepLink = DbUtil.Db.Setting("MobileDeepLinkURL", "");

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
                string message = $"<a href='{link}'>Click here to sign in</a>";

                string body = DbUtil.Db.ContentHtml("NewMobileUserDeepLink", message);
                body = body.Replace("{link}", link);

                DbUtil.Db.SendEmail(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), DbUtil.Db.Setting("MobileQuickSignInSubject", "Quick Sign In Link"), body, mailAddresses);

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
                    {
                        response.argBool = true;
                        response.setNoError();

                        break;
                    }

                case Results.CommonCodeCreationFailed:
                    {
                        response.setError((int)MobileMessage.Error.CREATE_CODE_FAILED);

                        break;
                    }

                case Results.CommonInvalidDeepLink:
                    {
                        response.setError((int)MobileMessage.Error.INVALID_DEEP_LINK);

                        break;
                    }

                case Results.CommonInvalidEmail:
                    {
                        response.setError((int)MobileMessage.Error.INVALID_EMAIL);

                        break;
                    }

                case Results.CreateMultipleMatches:
                    {
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
                    }

                case Results.CreateFoundSimiliar:
                    {
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
                    }

                case Results.CreateFoundPersonExistingUser:
                    {
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
                    }

                case Results.CreateNewPersonAndUser:
                case Results.CreateFoundPersonNewUser:
                    {
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
                    }

                case Results.DeepLinkNoPeopleFound:
                    {
                        response.argBool = false;
                        response.setNoError();

                        break;
                    }

                default:
                    {
                        response.setError((int)MobileMessage.Error.UNKNOWN);

                        break;
                    }
            }

            return response;
        }

        private void createPersonAndUser()
        {
            person = Person.Add(db, null, first, null, last, birthdate);
            person.PositionInFamilyId = CmsData.Codes.PositionInFamily.PrimaryAdult;
            person.EmailAddress = email;
            person.HomePhone = phone;

            EntryPoint appEntryPoint = DbUtil.Db.EntryPoints.FirstOrDefault(ep => ep.Code == "APP");

            if (appEntryPoint != null)
            {
                person.EntryPointId = appEntryPoint.Id;
            }

            DbUtil.Db.SubmitChanges();

            createUser();
        }

        private void createUser()
        {
            user = MembershipService.CreateUser(db, person.PeopleId);

            DbUtil.Db.SubmitChanges();
        }

        private void notifyNewUser()
        {
            AccountModel.SendNewUserEmail(user.Username);
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

            string code = createQuickSignInCode(device, instance, key, email);
            string deepLink = DbUtil.Db.Setting("MobileDeepLinkURL", "");
            string body = DbUtil.Db.ContentHtml("NewMobileUserWelcome", "");

            if (body.Length == 0)
            {
                body = DbUtil.Db.ContentHtml("NewUserWelcome", Resource1.AccountModel_NewUserWelcome);
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
            body = body.Replace("{cmshost}", DbUtil.Db.Setting("DefaultHost", DbUtil.Db.Host));
            body = body.Replace("{username}", user.Username);
            body = body.Replace("{link}", $"{deepLink}/signin/quick/{code}");

            DbUtil.Db.Email(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), user.Person, null, "New Mobile User Welcome", body);

            results = Results.CommonEmailSent;
        }

        private void notifyAboutExisting()
        {
            string message = DbUtil.Db.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
            message = message.Replace("{name}", person.Name).Replace("{host}", DbUtil.Db.CmsHost);

            DbUtil.Db.Email(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), person, null, "Account Information for " + DbUtil.Db.Host, message);
        }

        private void notifyAboutDuplicate()
        {
            string message = DbUtil.Db.ContentHtml("NotifyDuplicateUserOnMobile", Resource1.NotifyDuplicateUserOnMobile);
            message = message.Replace("{otheremail}", email);

            DbUtil.Db.Email(new MailAddress(DbUtil.AdminMail, DbUtil.AdminMailName), person, null, "New User Account on " + DbUtil.Db.Host, message);
        }

        private string createQuickSignInCode(int device, string instanceID, string key, string email)
        {
            string hash = createHash(DateTime.Now + email);

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
                CodeEmail = email
            };

            DbUtil.Db.MobileAppDevices.InsertOnSubmit(appDevice);
            DbUtil.Db.SubmitChanges();

            return appDevice.Id > 0 ? hash : "";
        }

        private static string createHash(string value)
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
