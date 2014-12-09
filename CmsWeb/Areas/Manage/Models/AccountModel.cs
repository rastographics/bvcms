using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using CmsData;
using UtilityExtensions;
using System.IO;
using System.Data.Linq;
using System.Configuration;
using System.Text;
using System.Net.Mail;

namespace CmsWeb.Models
{
    public class AccountModel
    {
        public string GetNewFileName(string path)
        {
            while (System.IO.File.Exists(path))
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
                return null;
            var guid = otltoken.ToGuid();
            if (guid == null)
                return null;
            var ot = DbUtil.Db.OneTimeLinks.SingleOrDefault(oo => oo.Id == guid.Value);
            if (ot == null)
                return null;
            if (ot.Used)
                return null;
            if (ot.Expires.HasValue && ot.Expires < DateTime.Now)
                return null;
            ot.Used = true;
            DbUtil.Db.SubmitChanges();
            return ot.Querystring;
        }
        private const string STR_UserName2 = "UserName2";
        public static string UserName2
        {
            get { return HttpContext.Current.Items[STR_UserName2] as String; }
            set { HttpContext.Current.Items[STR_UserName2] = value; }
        }
        public static bool AuthenticateMobile(string role = null, bool checkorgmembersonly = false)
        {
            string username, password;
            var auth = HttpContext.Current.Request.Headers["Authorization"];
            if (auth.HasValue())
            {
                var cred = Encoding.ASCII.GetString(
                    Convert.FromBase64String(auth.Substring(6))).SplitStr(":", 2);
                username = cred[0];
                password = cred[1];
            }
            else
            {
                username = HttpContext.Current.Request.Headers["username"];
                password = HttpContext.Current.Request.Headers["password"];
            }
            UserName2 = username;
            var u = AuthenticateLogon(username, password,
                HttpContext.Current.Request.Url.OriginalString);
            if (u is string)
                return false;
            var user = u as User;
            if (user == null)
                return false;
            var roleProvider = CMSRoleProvider.provider;
            if (role == null)
                role = "Access";
            if (roleProvider.RoleExists(role))
            {
                if (!roleProvider.IsUserInRole(user.Username, role))
                    return false;
            }
            UserName2 = user.Username;
            SetUserInfo(user.Username, HttpContext.Current.Session, deleteSpecialTags: false);
            //DbUtil.LogActivity("iphone auth " + user.Username);
            if (checkorgmembersonly && !Util2.OrgLeadersOnlyChecked)
            {
                DbUtil.LogActivity("iphone leadersonly check " + user.Username);
                if (!Util2.OrgLeadersOnly && roleProvider.IsUserInRole(username, "OrgLeadersOnly"))
                {
                    Util2.OrgLeadersOnly = true;
                    DbUtil.Db.SetOrgLeadersOnly();
                    DbUtil.LogActivity("SetOrgLeadersOnly");
                }
                Util2.OrgLeadersOnlyChecked = true;
            }
            return true;
        }

        public static object AuthenticateLogon(string userName, string password, string url)
        {
            var q = DbUtil.Db.Users.Where(uu =>
                uu.Username == userName ||
                uu.Person.EmailAddress == userName ||
                uu.Person.EmailAddress2 == userName
                );

            var impersonating = false;
            User user = null;
            int n = 0;
            try
            {
                n = q.Count();
            }
            catch (Exception)
            {
                return "bad database";
            }
            int failedpasswordcount = 0;
            foreach (var u in q.ToList())
                if (u.TempPassword != null && password == u.TempPassword)
                {
                    u.TempPassword = null;
                    if (password == "bvcms") // set this up so Admin/bvcms works until password is changed
                    {
                        u.Password = "";
                        u.MustChangePassword = true;
                    }
                    u.IsLockedOut = false;
                    DbUtil.Db.SubmitChanges();
                    user = u;
                    break;
                }
                else if (password == DbUtil.Db.Setting("ImpersonatePassword", Guid.NewGuid().ToString()))
                {
                    user = u;
                    impersonating = true;
                    break;
                }
                else if (Membership.Provider.ValidateUser(u.Username, password))
                {
                    DbUtil.Db.Refresh(RefreshMode.OverwriteCurrentValues, u);
                    user = u;
                    break;
                }
                else
                {
                    failedpasswordcount = Math.Max(failedpasswordcount, u.FailedPasswordAttemptCount);
                }


            var max = CMSMembershipProvider.provider.MaxInvalidPasswordAttempts;
            string problem = "There is a problem with your username and password combination. If you are using your email address, it must match the one we have on record. Try again or use one of the links below.";
            if (user == null && n > 0)
            {
                if (n > 3)
                    DbUtil.LogActivity("failed password #{1} by {0}".Fmt(userName, failedpasswordcount));
                if (failedpasswordcount == max)
                    return "Your account has been locked out for too many failed attempts, use the forgot password link, or notify an Admin";
                return problem;
            }
            else if (user == null)
            {
                DbUtil.LogActivity("attempt to login by non-user " + userName);
                return problem;
            }
            else if (user.IsLockedOut)
            {
                NotifyAdmins("{0} locked out #{2} on {1}"
                    .Fmt(userName, url, user.FailedPasswordAttemptCount),
                        "{0} tried to login at {1} but is locked out"
                            .Fmt(userName, Util.Now));
                return "Your account has been locked out for {0} failed attempts in a short window of time, please use the forgot password link or notify an Admin".Fmt(max);
            }
            else if (!user.IsApproved)
            {
                NotifyAdmins("unapproved user {0} logging in on {1}"
                    .Fmt(userName, url),
                        "{0} tried to login at {1} but is not approved"
                            .Fmt(userName, Util.Now));
                return problem;
            }
            if (impersonating == true)
            {
                if (user.Roles.Contains("Finance"))
                {
                    NotifyAdmins("cannot impersonate Finance user {0} on {1}"
                        .Fmt(userName, url),
                            "{0} tried to login at {1}".Fmt(userName, Util.Now));
                    return problem;
                }
            }
            return user;
        }
        public static object AuthenticateLogon(string userName, string password, HttpSessionStateBase Session, HttpRequestBase Request)
        {
            var o = AuthenticateLogon(userName, password, Request.Url.OriginalString);
            if (o is User)
            {
                var user = o as User;
                FormsAuthentication.SetAuthCookie(user.Username, false);
                SetUserInfo(user.Username, Session);
                DbUtil.LogActivity("User {0} logged in".Fmt(user.Username));
                return user;
            }
            return o;
        }
        private static void NotifyAdmins(string subject, string message)
        {
            IEnumerable<Person> notify = null;
            if (Roles.GetAllRoles().Contains("NotifyLogin"))
                notify = CMSRoleProvider.provider.GetRoleUsers("NotifyLogin").Select(u => u.Person).Distinct();
            else
                notify = CMSRoleProvider.provider.GetRoleUsers("Admin").Select(u => u.Person).Distinct();
            DbUtil.Db.EmailRedacted(DbUtil.AdminMail, notify, subject, message);
        }
        public static void SetUserInfo(string username, System.Web.SessionState.HttpSessionState Session, bool deleteSpecialTags = true)
        {
            var u = SetUserInfo(username);
            if (u == null)
                return;
            Session["ActivePerson"] = u.Name;
            if (deleteSpecialTags)
                DbUtil.Db.DeleteSpecialTags(u.PeopleId);
        }
        public static User SetUserInfo(string username, HttpSessionStateBase Session)
        {
            var u = SetUserInfo(username);
            if (u == null)
                return null;
            Session["ActivePerson"] = u.Name;
            return u;
        }
        private static User SetUserInfo(string username)
        {
            var i = (from u in DbUtil.Db.Users
                     where u.Username == username
                     select new { u, u.Person.PreferredName }).SingleOrDefault();
            if (i == null)
                return null;
            //var u = DbUtil.Db.Users.SingleOrDefault(us => us.Username == username);
            if (i.u != null)
            {
                Util.UserId = i.u.UserId;
                Util.UserPeopleId = i.u.PeopleId;
                Util.UserEmail = i.u.EmailAddress;
                Util2.CurrentPeopleId = i.u.PeopleId.Value;
                Util.UserPreferredName = i.PreferredName;
                Util.UserFullName = i.u.Name;
            }
            return i.u;
        }
        public static string CheckAccessRole(string name)
        {
            if (!Roles.IsUserInRole(name, "Access") && !Roles.IsUserInRole(name, "OrgMembersOnly"))
            {
                if (Util.UserPeopleId > 0)
                    return "/Person2/" + Util.UserPeopleId;

                if (name.HasValue())
                    DbUtil.LogActivity("user {0} loggedin without a role ".Fmt(name));
                FormsAuthentication.SignOut();
                return "/Error/401";
            }
            if (Roles.IsUserInRole(name, "NoRemoteAccess") && DbUtil.CheckRemoteAccessRole)
            {
                NotifyAdmins("NoRemoteAccess", string.Format("{0} tried to login from {1}", name, DbUtil.Db.Host));
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
            var link = DbUtil.Db.ServerLink("/Account/SetPassword/" + user.ResetPasswordCode.ToString());
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
                        var url = DbUtil.Db.ServerLink("/Account/CreateAccount/{0}".Fmt(ot.Id.ToCode()));
                        msg = DbUtil.Db.ContentHtml("ForgotPasswordReset", Resource1.AccountModel_ForgotPasswordReset);
                        msg = msg.Replace("{name}", p.Name);
                        msg = msg.Replace("{first}", p.PreferredName);
                        msg = msg.Replace("{email}", username);
                        msg = msg.Replace("{resetlink}", url);
                        Util.SendMsg(ConfigurationManager.AppSettings["sysfromemail"],
                            DbUtil.Db.CmsHost, Util.FirstAddress(DbUtil.AdminMail),
                            "bvcms new password link", msg, Util.ToMailAddressList(p.EmailAddress ?? p.EmailAddress2), 0, null);
                    }
                    DbUtil.LogActivity("ForgotPassword ('{0}', {1})".Fmt(username, path));
                    return;
                }
                path.Append("p0");
                if (!Util.ValidEmail(username))
                {
                    DbUtil.LogActivity("ForgotPassword ('{0}', {1})".Fmt(username, path));
                    return;
                }
                path.Append("n0");

                msg = DbUtil.Db.ContentHtml("ForgotPasswordBadEmail", Resource1.AccountModel_ForgotPasswordBadEmail);
                msg = msg.Replace("{email}", username);
                Util.SendMsg(ConfigurationManager.AppSettings["sysfromemail"],
                    DbUtil.Db.CmsHost, Util.FirstAddress(DbUtil.AdminMail),
                    "Forgot password request for " + DbUtil.Db.Setting("NameOfChurch", "bvcms"),
                    msg, Util.ToMailAddressList(username), 0, null);
                DbUtil.LogActivity("ForgotPassword ('{0}', {1})".Fmt(username, path));
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
                var link = DbUtil.Db.ServerLink("/Account/SetPassword/" + user.ResetPasswordCode.ToString());
                sb.AppendFormat(@"{0}, <a href=""{1}"">{2}</a><br>", user.Name, link, user.Username);
                DbUtil.Db.SubmitChanges();
            }
            msg = DbUtil.Db.ContentHtml("ForgotPasswordReset2", Resource1.AccountModel_ForgotPasswordReset2);
            msg = msg.Replace("{email}", username);
            msg = msg.Replace("{resetlink}", sb.ToString());
            Util.SendMsg(ConfigurationManager.AppSettings["sysfromemail"],
                DbUtil.Db.CmsHost, Util.FirstAddress(DbUtil.AdminMail),
                "bvcms password reset link", msg, addrlist, 0, null);
            DbUtil.LogActivity("ForgotPassword ('{0}', {1})".Fmt(username, path));
        }
    }
}
