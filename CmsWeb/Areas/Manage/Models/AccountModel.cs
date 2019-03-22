using CmsData;
using CmsData.API;
using CmsWeb.Areas.Manage.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public class AccountModel
    {
        public AccountModel() { }
        public string GetNewFileName(string path)
        {
            while (File.Exists(path))
            {
                var ext = Path.GetExtension(path);
                var fn = Path.GetFileNameWithoutExtension(path) + "a" + ext;
                var dir = Path.GetDirectoryName(path);
                path = Path.Combine(dir, fn);
            }
            return path;
        }

        public string CleanFileName(string fn)
        {
            fn = fn.Replace(' ', '_');
            fn = fn.Replace('(', '-');
            fn = fn.Replace(')', '-');
            fn = fn.Replace(',', '_');
            fn = fn.Replace("#", "");
            fn = fn.Replace("!", "");
            fn = fn.Replace("$", "");
            fn = fn.Replace("%", "");
            fn = fn.Replace("&", "_");
            fn = fn.Replace("'", "");
            fn = fn.Replace("+", "-");
            fn = fn.Replace("=", "-");
            return fn;
        }

        public static string GetValidToken(string otltoken)
        {
            if (!otltoken.HasValue())
            {
                return null;
            }

            var guid = otltoken.ToGuid();
            if (guid == null)
            {
                return null;
            }

            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
            {
                return null;
            }

            if (ot.Used)
            {
                return null;
            }

            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
            {
                return null;
            }

            ot.Used = true;
            DbUtil.Db.SubmitChanges();
            return ot.Querystring;
        }

        private const string STR_UserName2 = "UserName2";

        public static string UserName2
        {
            get { return HttpContextFactory.Current.Items[STR_UserName2] as string; }
            set { HttpContextFactory.Current.Items[STR_UserName2] = value; }
        }

        public static UserValidationResult AuthenticateMobile(string requiredRole = null, bool checkOrgLeadersOnly = false, bool requirePin = false)
        {
            var userStatus = GetUserViaCredentials() ?? GetUserViaSessionToken(requirePin);

            if (userStatus == null)
            {
                return UserValidationResult.Invalid(UserValidationStatus.ImproperHeaderStructure, "Could not authenticate user, Authorization or SessionToken headers likely missing.", null);
            }

            if (!userStatus.IsValid)
            {
                return userStatus;
            }

            var user = userStatus.User;

            var roleProvider = CMSRoleProvider.provider;

            if (requiredRole != null)
            {
                if (!roleProvider.RoleExists("Checkin"))
                {
                    requiredRole = "Access";
                }

                if (!roleProvider.IsUserInRole(user.Username, requiredRole))
                {
                    userStatus.Status = UserValidationStatus.UserNotInRole;
                    return userStatus;
                }
            }

            UserName2 = user.Username;
            SetUserInfo(user.Username, HttpContextFactory.Current.Session, deleteSpecialTags: false);
            //DbUtil.LogActivity("iphone auth " + user.Username);

            if (checkOrgLeadersOnly && !Util2.OrgLeadersOnlyChecked)
            {
                var db = DbUtil.Db;
                DbUtil.LogActivity("iphone leadersonly check " + user.Username);
                if (!Util2.OrgLeadersOnly && roleProvider.IsUserInRole(user.Username, "OrgLeadersOnly", db))
                {
                    Util2.OrgLeadersOnly = true;
                    DbUtil.Db.SetOrgLeadersOnly();
                    DbUtil.LogActivity("SetOrgLeadersOnly");
                }
                Util2.OrgLeadersOnlyChecked = true;
            }

            ApiSessionModel.SaveApiSession(userStatus.User, requirePin, HttpContextFactory.Current.Request.Headers["PIN"].ToInt2());

            return userStatus;
        }

        public static UserValidationResult AuthenticateMobile2(bool checkOrgLeadersOnly = false, bool requirePin = false)
        {
            var userStatus = GetUserViaCredentials() ?? GetUserViaSessionToken(requirePin);

            if (userStatus == null)
            {
                //DbUtil.LogActivity("userStatus==null");
                return UserValidationResult.Invalid(UserValidationStatus.ImproperHeaderStructure, "Could not authenticate user, Authorization or SessionToken headers likely missing.", null);
                //throw new Exception("Could not authenticate user, Authorization or SessionToken headers likely missing.");
            }

            if (!userStatus.IsValid)
            {
                return userStatus;
            }

            var user = userStatus.User;

            var roleProvider = CMSRoleProvider.provider;

            UserName2 = user.Username;
            SetUserInfo(user.Username, HttpContextFactory.Current.Session, deleteSpecialTags: false);

            if (checkOrgLeadersOnly && !Util2.OrgLeadersOnlyChecked)
            {
                var db = DbUtil.Db;
                DbUtil.LogActivity("iphone leadersonly check " + user.Username);
                if (!Util2.OrgLeadersOnly && roleProvider.IsUserInRole(user.Username, "OrgLeadersOnly", db))
                {
                    Util2.OrgLeadersOnly = true;
                    DbUtil.Db.SetOrgLeadersOnly();
                    DbUtil.LogActivity("SetOrgLeadersOnly");
                }
                Util2.OrgLeadersOnlyChecked = true;
            }

            FormsAuthentication.SetAuthCookie(user.Username, false);
            ApiSessionModel.SaveApiSession(userStatus.User, requirePin, HttpContextFactory.Current.Request.Headers["PIN"].ToInt2());

            return userStatus;
        }

        public static UserValidationResult ResetSessionExpiration(string sessionToken)
        {
            if (string.IsNullOrEmpty(sessionToken))
            {
                return UserValidationResult.Invalid(UserValidationStatus.ImproperHeaderStructure, "Could not authenticate user, Authorization or SessionToken headers likely missing.", null);
            }
            //throw new ArgumentNullException("sessionToken");

            var userStatus = AuthenticateMobile(requirePin: true);

            if (userStatus.Status == UserValidationStatus.Success
                || userStatus.Status == UserValidationStatus.PinExpired
                || userStatus.Status == UserValidationStatus.SessionTokenExpired)
            {
                var result = ApiSessionModel.ResetSessionExpiration(userStatus.User, HttpContextFactory.Current.Request.Headers["PIN"].ToInt2());
                if (!result)
                {
                    return UserValidationResult.Invalid(UserValidationStatus.PinInvalid);
                }

                userStatus.Status = UserValidationStatus.Success;
            }

            return userStatus;
        }

        public static void ExpireSessionToken(string sessionToken)
        {
            ApiSessionModel.ExpireSession(Guid.Parse(sessionToken));
        }

        private static UserValidationResult GetUserViaSessionToken(bool requirePin)
        {
            var sessionToken = HttpContextFactory.Current.Request.Headers["SessionToken"];
            if (string.IsNullOrEmpty(sessionToken))
            {
                //DbUtil.LogActivity("GetUserViaSession==null");
                return null;
            }

            var result = ApiSessionModel.DetermineApiSessionStatus(Guid.Parse(sessionToken), requirePin, HttpContextFactory.Current.Request.Headers["PIN"].ToInt2());

            //DbUtil.LogActivity("GetUserViaSession==" + result.Status.ToString());

            switch (result.Status)
            {
                case ApiSessionStatus.SessionTokenNotFound:
                    return UserValidationResult.Invalid(UserValidationStatus.SessionTokenNotFound);
                case ApiSessionStatus.SessionTokenExpired:
                    return UserValidationResult.Invalid(UserValidationStatus.SessionTokenExpired, user: result.User);
                case ApiSessionStatus.PinExpired:
                    return UserValidationResult.Invalid(UserValidationStatus.PinExpired, user: result.User);
                case ApiSessionStatus.PinInvalid:
                    return UserValidationResult.Invalid(UserValidationStatus.PinInvalid);
            }

            return ValidateUserBeforeLogin(result.User.Username, HttpContextFactory.Current.Request.Url.OriginalString, result.User, userExists: true);
        }

        private static UserValidationResult GetUserViaCredentials()
        {
            string username;
            string password;

            var auth = HttpContextFactory.Current.Request.Headers["Authorization"];
            if (auth.HasValue())
            {
                var cred = Encoding.ASCII.GetString(
                    Convert.FromBase64String(auth.Substring(6))).SplitStr(":", 2);
                username = cred[0];
                password = cred[1];
            }
            else
            {
                // NOTE: this is necessary only for the old iOS application
                username = HttpContextFactory.Current.Request.Headers["username"];
                password = HttpContextFactory.Current.Request.Headers["password"];
            }

            if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password))
            {
                var creds = new NetworkCredential(username, password);
                UserName2 = creds.UserName;
                //DbUtil.LogActivity("GetUserViaCreds");
                return AuthenticateLogon(creds.UserName, creds.Password, HttpContextFactory.Current.Request.Url.OriginalString);
            }

            //DbUtil.LogActivity("GetUserViaCreds==null");
            return null;
        }

        public static UserValidationResult AuthenticateLogon(string userName, string password, string url)
        {
            var db = DbUtil.Db;
            var userQuery = DbUtil.Db.Users.Where(uu =>
                uu.Username == userName ||
                uu.Person.EmailAddress == userName ||
                uu.Person.EmailAddress2 == userName
                );

            var impersonating = false;
            User user = null;
            var userExists = false;
            try
            {
                userExists = userQuery.Any();
            }
            catch
            {
                return UserValidationResult.Invalid(UserValidationStatus.BadDatabase, "bad database");
            }

            var failedPasswordCount = 0;
            foreach (var u in userQuery.ToList())
            {
                if (u.TempPassword != null && password == u.TempPassword)
                {
                    u.TempPassword = null;
                    if (password == "bvcms") // set this up so Admin/bvcms works until password is changed
                    {
                        u.Password = "";
                        u.MustChangePassword = true;
                    }
                    else
                    {
                        var mu = CMSMembershipProvider.provider.GetUser(userName, false);
                        mu?.UnlockUser();
                        CMSMembershipProvider.provider.AdminOverride = true;
                        mu?.ChangePassword(mu.ResetPassword(), password);
                        CMSMembershipProvider.provider.AdminOverride = false;
                        u.MustChangePassword = true;
                    }
                    u.IsLockedOut = false;
                    DbUtil.Db.SubmitChanges();
                    user = u;
                    break;
                }

                if (password == DbUtil.Db.Setting("ImpersonatePassword", Guid.NewGuid().ToString()))
                {
                    user = u;
                    impersonating = true;
                    HttpContextFactory.Current.Session["IsNonFinanceImpersonator"] = "true";
                    break;
                }

                if (Membership.Provider.ValidateUser(u.Username, password))
                {
                    DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, u);
                    user = u;
                    break;
                }

                failedPasswordCount = Math.Max(failedPasswordCount, u.FailedPasswordAttemptCount);
            }

            return ValidateUserBeforeLogin(userName, url, user, userExists, failedPasswordCount, impersonating);
        }

        private static UserValidationResult ValidateUserBeforeLogin(string userName, string url, User user, bool userExists, int failedPasswordCount = 0, bool impersonating = false)
        {
            var maxInvalidPasswordAttempts = CMSMembershipProvider.provider.MaxInvalidPasswordAttempts;
            const string DEFAULT_PROBLEM = "There is a problem with your username and password combination. If you are using your email address, it must match the one we have on record. Try again or use one of the links below.";

            if (user == null && userExists)
            {
                DbUtil.LogActivity($"failed password #{failedPasswordCount} by {userName}");

                if (failedPasswordCount == maxInvalidPasswordAttempts)
                {
                    return UserValidationResult.Invalid(UserValidationStatus.TooManyFailedPasswordAttempts,
                        "Your account has been locked out for too many failed attempts, use the forgot password link, or notify an Admin");
                }

                return UserValidationResult.Invalid(UserValidationStatus.IncorrectPassword, DEFAULT_PROBLEM);
            }

            if (user == null)
            {
                DbUtil.LogActivity("attempt to login by non-user " + userName);
                return UserValidationResult.Invalid(UserValidationStatus.NoUserFound, DEFAULT_PROBLEM);
            }

            if (user.IsLockedOut)
            {
                NotifyAdmins($"{userName} locked out #{user.FailedPasswordAttemptCount} on {url}",
                    $"{userName} tried to login at {Util.Now} but is locked out");

                return UserValidationResult.Invalid(UserValidationStatus.LockedOut,
                    $"Your account has been locked out for {maxInvalidPasswordAttempts} failed attempts in a short window of time, please use the forgot password link or notify an Admin");
            }

            if (!user.IsApproved)
            {
                NotifyAdmins($"unapproved user {userName} logging in on {url}",
                    $"{userName} tried to login at {Util.Now} but is not approved");

                return UserValidationResult.Invalid(UserValidationStatus.UserNotApproved, DEFAULT_PROBLEM);
            }

            if (impersonating)
            {
                if (user.Roles.Contains("Finance"))
                {
                    NotifyAdmins($"cannot impersonate Finance user {userName} on {url}",
                        $"{userName} tried to login at {Util.Now}");

                    return UserValidationResult.Invalid(UserValidationStatus.CannotImpersonateFinanceUser, DEFAULT_PROBLEM);
                }
            }

            if (user.Roles.Contains("APIOnly"))
            {
                return UserValidationResult.Invalid(UserValidationStatus.NoUserFound,
                    "Api User is limited to API use only, no interactive login allowed.");
            }

            return UserValidationResult.Valid(user);
        }

        public static object AuthenticateLogon(string userName, string password, HttpSessionStateBase Session, HttpRequestBase Request)
        {
            var status = AuthenticateLogon(userName, password, Request.Url.OriginalString);
            if (status.IsValid)
            {
                SetUserInfo(status.User.Username, Session);
                FormsAuthentication.SetAuthCookie(status.User.Username, false);
                DbUtil.LogActivity($"User {status.User.Username} logged in");
                return status.User;
            }
            return status.ErrorMessage;
        }


        public static object AutoLogin(string userName, HttpSessionStateBase Session, HttpRequestBase Request)
        {
#if DEBUG
            SetUserInfo(userName, Session);
            FormsAuthentication.SetAuthCookie(userName, false);
#endif
            return null;
        }

        private static void NotifyAdmins(string subject, string message)
        {
            IEnumerable<Person> notify = null;
            if (Roles.GetAllRoles().Contains("NotifyLogin"))
            {
                notify = CMSRoleProvider.provider.GetRoleUsers("NotifyLogin").Select(u => u.Person).Distinct();
            }
            else
            {
                notify = CMSRoleProvider.provider.GetRoleUsers("Admin").Select(u => u.Person).Distinct();
            }

            DbUtil.Db.EmailRedacted(DbUtil.AdminMail, notify, subject, message);
        }

        public static void SetUserInfo(string username, HttpSessionStateBase Session, bool deleteSpecialTags = true)
        {
            var u = SetUserInfo(username);
            if (u == null)
            {
                return;
            }

            Session["ActivePerson"] = u.Name;
            if (deleteSpecialTags)
            {
                DbUtil.Db.DeleteSpecialTags(Util.UserPeopleId);
            }
        }

        public static User SetUserInfo(string username, HttpSessionStateBase Session)
        {
            var u = SetUserInfo(username);
            if (u == null)
            {
                return null;
            }

            Session["ActivePerson"] = u.Name;
            return u;
        }

        private static User SetUserInfo(string username)
        {
            var i = (from u in DbUtil.Db.Users
                     where u.Username == username
                     select new { u, u.Person.PreferredName }).SingleOrDefault();
            if (i == null)
            {
                return null;
            }
            //var u = DbUtil.Db.Users.SingleOrDefault(us => us.Username == username);
            if (i.u != null)
            {
                Util.UserId = i.u.UserId;
                Util.UserPeopleId = i.u.PeopleId;

                Util.UserThumbPictureBgPosition = "top";
                if (i.u.Person?.Picture != null)
                {
                    var picture = i.u.Person.Picture;
                    Util.UserThumbPictureUrl = picture.ThumbUrl;
                    Util.UserThumbPictureBgPosition = picture.X.HasValue || picture.Y.HasValue ?
                        $"{picture.X.GetValueOrDefault()}% {picture.Y.GetValueOrDefault()}%"
                        : "top";
                }

                Util.UserEmail = i.u.EmailAddress;
                Util2.CurrentPeopleId = i.u.PeopleId.Value;
                Util.UserPreferredName = i.PreferredName;
                Util.UserFullName = i.u.Name;
                Util.UserFirstName = i.u.Person.FirstName;
            }
            return i.u;
        }

        public static string CheckAccessRole(string name)
        {
            if (!Roles.IsUserInRole(name, "Access") && !Roles.IsUserInRole(name, "OrgMembersOnly"))
            {
                if (Util.UserPeopleId > 0)
                {
                    return $"/Person2/{Util.UserPeopleId}";
                }

                if (name.HasValue())
                {
                    DbUtil.LogActivity($"user {name} loggedin without a role ");
                }

                FormsAuthentication.SignOut();
                return "/Errors/AccessDenied.htm";
            }

            if (Roles.IsUserInRole(name, "NoRemoteAccess") && DbUtil.CheckRemoteAccessRole)
            {
                NotifyAdmins("NoRemoteAccess", $"{name} tried to login from {DbUtil.Db.Host}");
                return "NoRemoteAccess.htm";
            }

            return null;
        }

        public static User AddUser(int id)
        {
            var p = DbUtil.Db.People.Single(pe => pe.PeopleId == id);
            CMSMembershipProvider.provider.AdminOverride = true;
            var user = MembershipService.CreateUser(DbUtil.Db, id);
            CMSMembershipProvider.provider.AdminOverride = false;
            user.MustChangePassword = false;
            DbUtil.Db.SubmitChanges();
            return user;
        }

        public static void SendNewUserEmail(string username)
        {
            var user = DbUtil.Db.Users.First(u => u.Username == username);
            var body = DbUtil.Db.ContentHtml("NewUserWelcome", Resource1.AccountModel_NewUserWelcome);
            body = body.Replace("{first}", user.Person.PreferredName);
            body = body.Replace("{name}", user.Person.Name);
            body = body.Replace("{cmshost}", DbUtil.Db.Setting("DefaultHost", DbUtil.Db.Host));
            body = body.Replace("{username}", user.Username);
            user.ResetPasswordCode = Guid.NewGuid();
            user.ResetPasswordExpires = DateTime.Now.AddHours(DbUtil.Db.Setting("ResetPasswordExpiresHours", "24").ToInt());
            var link = DbUtil.Db.ServerLink("/Account/SetPassword/" + user.ResetPasswordCode);
            body = body.Replace("{link}", link);
            DbUtil.Db.SubmitChanges();
            DbUtil.Db.EmailRedacted(DbUtil.AdminMail, user.Person, "New user welcome", body);
        }

        public static void ForgotPassword(string username)
        {
            // first find a user with the email address or username
            string msg = null;
            var path = new StringBuilder();

            username = username.Trim();
            var q = DbUtil.Db.Users.Where(uu =>
                uu.Username == username ||
                uu.Person.EmailAddress == username ||
                uu.Person.EmailAddress2 == username
                );
            if (!q.Any())
            {
                path.Append("u0");
                // could not find a user to match
                // so we look for a person without an account, to match the email address

                var minage = DbUtil.Db.Setting("MinimumUserAge", "16").ToInt();
                var q2 = from uu in DbUtil.Db.People
                         where uu.EmailAddress == username || uu.EmailAddress2 == username
                         where uu.Age == null || uu.Age >= minage
                         select uu;
                if (q2.Any())
                {
                    path.Append("p+");
                    // we found person(s), not a user
                    // we will compose an email for each of them to create an account
                    foreach (var p in q2)
                    {
                        var ot = new OneTimeLink
                        {
                            Id = Guid.NewGuid(),
                            Querystring = p.PeopleId.ToString()
                        };
                        DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
                        DbUtil.Db.SubmitChanges();
                        var url = DbUtil.Db.ServerLink($"/Account/CreateAccount/{ot.Id.ToCode()}");
                        msg = DbUtil.Db.ContentHtml("ForgotPasswordReset", Resource1.AccountModel_ForgotPasswordReset);
                        msg = msg.Replace("{name}", p.Name);
                        msg = msg.Replace("{first}", p.PreferredName);
                        msg = msg.Replace("{email}", username);
                        msg = msg.Replace("{resetlink}", url);
                        DbUtil.Db.SendEmail(Util.FirstAddress(DbUtil.AdminMail),
                            "touchpointsoftware new password link", msg, Util.ToMailAddressList(p.EmailAddress ?? p.EmailAddress2));
                    }
                    DbUtil.LogActivity($"ForgotPassword ('{username}', {path})");
                    return;
                }
                path.Append("p0");
                if (!Util.ValidEmail(username))
                {
                    DbUtil.LogActivity($"ForgotPassword ('{username}', {path})");
                    return;
                }
                path.Append("n0");

                msg = DbUtil.Db.ContentHtml("ForgotPasswordBadEmail", Resource1.AccountModel_ForgotPasswordBadEmail);
                msg = msg.Replace("{email}", username);
                DbUtil.Db.SendEmail(Util.FirstAddress(DbUtil.AdminMail),
                    "Forgot password request for " + DbUtil.Db.Setting("NameOfChurch", "bvcms"),
                    msg, Util.ToMailAddressList(username));
                DbUtil.LogActivity($"ForgotPassword ('{username}', {path})");
                return;
            }
            path.Append("u+");

            // we found users who match,
            // so now we send the users who match the username or email a set of links to all their usernames

            var sb = new StringBuilder();
            var addrlist = new List<MailAddress>();
            foreach (var user in q)
            {
                Util.AddGoodAddress(addrlist, user.EmailAddress);
                user.ResetPasswordCode = Guid.NewGuid();
                user.ResetPasswordExpires = DateTime.Now.AddHours(DbUtil.Db.Setting("ResetPasswordExpiresHours", "24").ToInt());
                var link = DbUtil.Db.ServerLink($"/Account/SetPassword/{user.ResetPasswordCode}");
                sb.Append($@"{user.Name}, <a href=""{link}"">{user.Username}</a><br>");
                DbUtil.Db.SubmitChanges();
            }
            msg = DbUtil.Db.ContentHtml("ForgotPasswordReset2", Resource1.AccountModel_ForgotPasswordReset2);
            msg = msg.Replace("{email}", username);
            msg = msg.Replace("{resetlink}", sb.ToString());
            DbUtil.Db.SendEmail(Util.FirstAddress(DbUtil.AdminMail),
                "TouchPoint password reset link", msg, addrlist);
            DbUtil.LogActivity($"ForgotPassword ('{username}', {path})");
        }
    }
}
