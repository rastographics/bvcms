/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */

using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.Security;
using CmsData.API;
using UtilityExtensions;

namespace CmsData
{

    public sealed class CMSMembershipProvider : MembershipProvider
    {
        public static CMSMembershipProvider provider => Membership.Provider as CMSMembershipProvider;

        private int newPasswordLength = 8;

        private MachineKeySection machineKey;

        CMSDataContext GetDb()
        {
            return DbUtil.Create(Util.Host);
        }
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
                name = "CMSMembershipProvider";

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "CMS Membership provider");
            }
            base.Initialize(name, config);

            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));

            var Db = GetDb();
            pMinRequiredPasswordLength = Db.Setting("PasswordMinLength", "7").ToInt();

            pMinRequiredNonAlphanumericCharacters = DbUtil.Db.Setting("PasswordRequireSpecialCharacter", "true").ToBool() ? 1 : 0;
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
                temp_format = "Hashed";

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }

//		public bool pRequireOneNonAlphaNum { get; set; }
//		public bool pRequireOneNumber { get; set; }
//		public bool pRequireOneUpper { get; set; }

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (string.IsNullOrEmpty(configValue))
                return defaultValue;
            return configValue;
        }

        public override string ApplicationName { get { return "cms"; } set { } }

        private bool pEnablePasswordReset;
        public override bool EnablePasswordReset => pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        public override bool EnablePasswordRetrieval => pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        public override bool RequiresQuestionAndAnswer => pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        public override bool RequiresUniqueEmail => pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        public override int MaxInvalidPasswordAttempts => pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        public override int PasswordAttemptWindow => pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;
        public override MembershipPasswordFormat PasswordFormat => pPasswordFormat;
        private int pMinRequiredNonAlphanumericCharacters;
        public override int MinRequiredNonAlphanumericCharacters => pMinRequiredNonAlphanumericCharacters;
        private int pMinRequiredPasswordLength;
        public override int MinRequiredPasswordLength => pMinRequiredPasswordLength;
        private string pPasswordStrengthRegularExpression;
        public override string PasswordStrengthRegularExpression => pPasswordStrengthRegularExpression;
        public bool AdminOverride = false;
        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            username = Util.GetUserName(username);
            if (!ValidateUser(username, oldPwd))
                return false;
            var args = new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);
            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            if (!AdminOverride)
            {
                if (newPwd.Length < MinRequiredPasswordLength)
                    throw new ArgumentException($"Password must contain at least {MinRequiredPasswordLength} chars");
                if (MembershipService.RequireSpecialCharacter)
                    if (newPwd.All(char.IsLetterOrDigit))
                        throw new ArgumentException("Password needs at least 1 non-alphanumeric character");
                if (MembershipService.RequireOneNumber)
                    if (!newPwd.Any(char.IsDigit))
                        throw new ArgumentException("Password needs at least 1 number");
                if (MembershipService.RequireOneUpper)
                    if (!newPwd.Any(char.IsUpper))
                        throw new ArgumentException("Password needs at least 1 uppercase letter");
            }

            var db = GetDb();
            var user = db.Users.Single(u => u.Username == username);
            user.Password = EncodePassword(newPwd);
            user.MustChangePassword = false;
            user.LastPasswordChangedDate = Util.Now;
            ApiSessionModel.DeleteSession(db, user);
            db.SubmitChanges();
            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username,
                      string password,
                      string newPwdQuestion,
                      string newPwdAnswer)
        {
            username = Util.GetUserName(username);
            if (!ValidateUser(username, password))
                return false;
            var Db = GetDb();
            var user = Db.Users.Single(u => u.Username == username);
            user.PasswordQuestion = newPwdQuestion;
            user.PasswordAnswer = newPwdAnswer;
            Db.SubmitChanges();
            return true;
        }

        public override MembershipUser CreateUser(string username,
                 string password,
                 string email,
                 string passwordQuestion,
                 string passwordAnswer,
                 bool isApproved,
                 object providerUserKey,
                 out MembershipCreateStatus status)
        {
            username = Util.GetUserName(username);
            var args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);
            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }
            var Db = GetDb();
            var u = GetUser(username, false);
            if (u == null)
            {
                int? pid = null;
                Person per = null;
                if (providerUserKey != null && providerUserKey is int)
                    per = Db.People.SingleOrDefault(p => p.PeopleId == (int)providerUserKey);
                else
                    per = Db.People.SingleOrDefault(p => p.EmailAddress == email);

                if (per != null)
                    pid = per.PeopleId;

                var createDate = Util.Now;
                var user = new User
                {
                    PeopleId = pid,
                    Username = username,
                    //EmailAddress = email,
                    Password = EncodePassword(password),
                    PasswordQuestion = passwordQuestion,
                    PasswordAnswer = EncodePassword(passwordAnswer),
                    IsApproved = isApproved,
                    Comment = "",
                    CreationDate = createDate,
                    LastPasswordChangedDate = createDate,
                    LastActivityDate = createDate,
                    IsLockedOut = false,
                    LastLockedOutDate = createDate,
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = createDate,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = createDate,
                };
                Db.Users.InsertOnSubmit(user);
                Db.SubmitChanges();
                status = MembershipCreateStatus.Success;
                return GetUser(username, false);
            }
            else
                status = MembershipCreateStatus.DuplicateUserName;
            return null;
        }

        public User NewUser(
                string username,
                string password,
                string email,
                bool isApproved,
                int? PeopleId)
        {
            username = Util.GetUserName(username);
            var args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);
            if (args.Cancel)
                return null;
            var u = GetUser(username, false);
            if (u == null)
                return MakeNewUser(username, EncodePassword(password), email, isApproved, PeopleId);
            return null;
        }
        public static User MakeNewUser(string username, string password, string email, bool isApproved, int? PeopleId)
        {
            var createDate = DateTime.Now;
            var user = new User
            {
                PeopleId = PeopleId,
                Username = username,
                Password = password,
                MustChangePassword = false,
                IsApproved = isApproved,
                Comment = "",
                CreationDate = createDate,
                LastPasswordChangedDate = createDate,
                LastActivityDate = createDate,
                IsLockedOut = false,
                LastLockedOutDate = createDate,
                FailedPasswordAttemptCount = 0,
                FailedPasswordAttemptWindowStart = createDate,
                FailedPasswordAnswerAttemptCount = 0,
                FailedPasswordAnswerAttemptWindowStart = createDate,
            };
            DbUtil.Db.Users.InsertOnSubmit(user);
            DbUtil.Db.SubmitChanges();
            return user;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            username = Util.GetUserName(username);
            var Db = GetDb();
            var user = Db.Users.SingleOrDefault(u => u.Username == username);
            Db.UserRoles.DeleteAllOnSubmit(user.UserRoles);
            Db.Users.DeleteOnSubmit(user);
            Db.SubmitChanges();
            return true;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var Db = GetDb();
            var q = Db.Users.AsQueryable();
            totalRecords = q.Count();
            q = q.OrderBy(u => u.Username).Skip(pageIndex * pageSize).Take(pageSize);
            foreach (var u in q)
                users.Add(GetMu(u));
            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            var onlineSpan = new TimeSpan(0, Membership.UserIsOnlineTimeWindow, 0);
            var compareTime = Util.Now.Subtract(onlineSpan);
            var Db = GetDb();
            return Db.Users.Count(u => u.LastActivityDate > compareTime);
        }

        public override string GetPassword(string username, string answer)
        {
            username = Util.GetUserName(username);
            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve Hashed passwords.");

            var Db = GetDb();
            var user = Db.Users.SingleOrDefault(u => u.Username == username);
            if (user != null)
            {
                if (user.IsLockedOut)
                    throw new MembershipPasswordException("The supplied user is locked out.");
            }
            else
                throw new MembershipPasswordException("The supplied user name is not found.");

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer))
            {
                UpdateFailureCount(Db, user, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }
            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
                return UnEncodePassword(user.Password);
            return user.Password;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            username = Util.GetUserName(username);
            var Db = GetDb();
            var q = Db.Users.Where(user => user.Username == username);
            if (q.Count() > 1)
                throw new Exception("duplicate user: " + username);
            var u = q.SingleOrDefault();
            if (u != null)
            {
                MembershipUser mu = GetMu(u);
                if (userIsOnline)
                {
                    u.LastActivityDate = Util.Now;
                    string host = HttpContextFactory.Current.Request.Url.Host;
                    if (host.Length > 98)
                        host = host.Substring(0, 98);
                    u.Host = host;
                    Db.SubmitChanges();
                }
                return mu;
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var Db = GetDb();
            var u = Db.Users.SingleOrDefault(user =>
                user.UserId == providerUserKey.ToInt());
            if (u != null)
            {
                MembershipUser mu = GetMu(u);
                if (userIsOnline)
                {
                    u.LastActivityDate = Util.Now;
                    string host = HttpContextFactory.Current.Request.Url.Host;
                    if (host.Length > 98)
                        host = host.Substring(0, 98);
                    u.Host = host;
                    Db.SubmitChanges();
                }
                return mu;
            }
            return null;
        }

        private MembershipUser GetMu(User u)
        {
            return new MembershipUser(this.Name,
            u.Username,
            u.UserId,
            u.Person.EmailAddress,
            u.PasswordQuestion,
            u.Comment,
            u.IsApproved,
            u.IsLockedOut,
            u.CreationDate ?? new DateTime(),
            u.LastLoginDate ?? new DateTime(),
            u.LastActivityDate ?? new DateTime(),
            u.LastPasswordChangedDate ?? new DateTime(),
            u.LastLockedOutDate ?? new DateTime());
        }
        public override bool UnlockUser(string username)
        {
            var Db = GetDb();
            username = Util.GetUserName(username);
            var u = Db.Users.SingleOrDefault(user => user.Username == username);
            if (u != null)
            {
                u.LastLockedOutDate = Util.Now;
                u.IsLockedOut = false;
                Db.SubmitChanges();
            }
            return u != null;
        }

        public override string GetUserNameByEmail(string email)
        {
            var Db = GetDb();
            return Db.Users.Single(u => u.Person.EmailAddress == email).Username;
        }

        public override string ResetPassword(string username, string answer)
        {
            username = Util.GetUserName(username);
            if (!EnablePasswordReset)
                throw new NotSupportedException("Password reset is not enabled.");

            var db = GetDb();
            var user = db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
                throw new MembershipPasswordException("The supplied user name is not found.");

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(db, user, "passwordAnswer");
                throw new ProviderException("Password answer required for password reset.");
            }
            var newPassword = Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            var args = new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");
            }

            if (user.IsLockedOut)
                throw new MembershipPasswordException("The supplied user is locked out.");

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, user.PasswordAnswer))
            {
                UpdateFailureCount(db, user, "passwordAnswer");
                throw new MembershipPasswordException("Incorrect password answer.");
            }

            user.Password = EncodePassword(newPassword);
            user.LastPasswordChangedDate = Util.Now;
            ApiSessionModel.DeleteSession(db, user);
            db.SubmitChanges();
            return newPassword;
        }

        public override void UpdateUser(MembershipUser user)
        {
            var Db = GetDb();
            var u = Db.Users.SingleOrDefault(us => us.Username == user.UserName);
            u.IsApproved = user.IsApproved;
            //u.Person.EmailAddress = user.Email;
            u.Comment = user.Comment;
            Db.SubmitChanges();
        }

        public override bool ValidateUser(string username, string password)
        {
            try
            {
                var Db = GetDb();
                username = Util.GetUserName(username);
                var user = Db.Users.FirstOrDefault(u =>
                    u.Username == username
                    || u.EmailAddress == username
                    || u.Person.EmailAddress2 == username
                    );
                if (user == null)
                    return false;
                if (CheckPassword(password, user.Password))
                    if (user.IsApproved)
                    {
                        user.LastLoginDate = Util.Now;
                        user.FailedPasswordAttemptCount = 0;
                        if (user.IsLockedOut && user.FailedPasswordAttemptWindowStart.HasValue)
                            if ((DateTime.Now - user.FailedPasswordAttemptWindowStart.Value).TotalMinutes > 3)
                                user.IsLockedOut = false;
                        Db.SubmitChanges();
                        return true;
                    }
                UpdateFailureCount(Db, user, "password");
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void UpdateFailureCount(CMSDataContext Db, User user, string failureType)
        {
            var windowStart = new DateTime();
            int failureCount = 0;
            if (failureType == "password")
            {
                failureCount = user.FailedPasswordAttemptCount;
                windowStart = user.FailedPasswordAttemptWindowStart ?? Util.Now;
            }
            else if (failureType == "passwordAnswer")
            {
                failureCount = user.FailedPasswordAnswerAttemptCount;
                windowStart = user.FailedPasswordAnswerAttemptWindowStart ?? Util.Now;
            }
            var windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);
            if (failureCount == 0 || Util.Now > windowEnd)
            {
                if (failureType == "password")
                {
                    user.FailedPasswordAttemptCount = 1;
                    user.FailedPasswordAttemptWindowStart = Util.Now;
                }
                else if (failureType == "passwordAnswer")
                {
                    user.FailedPasswordAnswerAttemptCount = 1;
                    user.FailedPasswordAnswerAttemptWindowStart = Util.Now;
                }
            }
            else if (failureCount++ >= MaxInvalidPasswordAttempts)
            {
                user.IsLockedOut = true;
                user.LastLockedOutDate = DateTime.Now;
            }
            else if (failureType == "password")
                user.FailedPasswordAttemptCount = failureCount;
            else if (failureType == "passwordAnswer")
                user.FailedPasswordAnswerAttemptCount = failureCount;
            Db.SubmitChanges();
        }

        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;
            if (dbpassword == "" && password == "bvcms")
                return true;
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }
            return pass1 == pass2;
        }

        private string EncodePassword(string password)
        {
            if (!password.HasValue())
                return "";
            string encodedPassword = password;
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    var hash = new HMACSHA1();
                    hash.Key = HexToByte(machineKey.ValidationKey);
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return encodedPassword;
        }

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;
            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }
            return password;
        }

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var Db = GetDb();
            var q = from u in Db.Users select u;

            bool left = usernameToMatch.StartsWith("%");
            bool right = usernameToMatch.EndsWith("%");
            usernameToMatch = usernameToMatch.Trim('%');
            if (left && right)
                q = q.Where(u => u.Username.Contains(usernameToMatch));
            else if (left)
                q = q.Where(u => u.Username.EndsWith(usernameToMatch));
            else if (right)
                q = q.Where(u => u.Username.StartsWith(usernameToMatch));

            totalRecords = q.Count();
            q = q.OrderBy(u => u.Username).Skip(pageIndex * pageSize).Take(pageSize);
            foreach (var u in q)
                users.Add(GetMu(u));
            return users;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            var Db = GetDb();
            var q = from u in Db.Users select u;

            bool left = emailToMatch.StartsWith("%");
            bool right = emailToMatch.EndsWith("%");
            emailToMatch = emailToMatch.Trim('%');
            if (left && right)
                q = q.Where(u => u.Person.EmailAddress.Contains(emailToMatch));
            else if (left)
                q = q.Where(u => u.Person.EmailAddress.EndsWith(emailToMatch));
            else if (right)
                q = q.Where(u => u.Person.EmailAddress.StartsWith(emailToMatch));

            totalRecords = q.Count();
            q = q.OrderBy(u => u.Username).Skip(pageIndex * pageSize).Take(pageSize);
            foreach (var u in q)
                users.Add(GetMu(u));
            return users;
        }
        public void MustChangePassword(string username, bool tf)
        {
            username = Util.GetUserName(username);
            var Db = GetDb();
            var user = Db.Users.Single(u => u.Username == username);
            user.MustChangePassword = tf;
            Db.SubmitChanges();
        }
        public bool MustChangePassword(string username)
        {
            username = Util.GetUserName(username);
            var Db = GetDb();
            var user = Db.Users.SingleOrDefault(u => u.Username == username);
            if (user == null)
                return false;
            return user.MustChangePassword;
        }
        public bool UserMustChangePassword
        {
            get { return MustChangePassword(Util.UserName); }
            set { MustChangePassword(Util.UserName, value); }
        }
    }
}
