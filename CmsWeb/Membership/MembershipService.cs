using CmsData;
using Google.Authenticator;
using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using UtilityExtensions;

namespace CmsWeb.Membership
{
    public static class MembershipService
    {
        public static int MinPasswordLength (CMSDataContext db) => db.Setting("PasswordMinLength", "7").ToInt();

        public static bool RequireSpecialCharacter (CMSDataContext db) => db.Setting("PasswordRequireSpecialCharacter", "true").ToBool();

        public static bool RequireOneNumber (CMSDataContext db) => db.Setting("PasswordRequireOneNumber", "false").ToBool();

        public static bool RequireOneUpper (CMSDataContext db) => db.Setting("PasswordRequireOneUpper", "false").ToBool();

        public static bool ValidateUser(string userName, string password)
        {
            return CMSMembershipProvider.provider.ValidateUser(userName, password);
        }

        public static MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            CMSMembershipProvider.provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }
        public static User CreateUser(int peopleId, string username, string password)
        {
            CMSMembershipProvider.provider.AdminOverride = true;
            var user = CMSMembershipProvider.provider.NewUser(
                username,
                password,
                null,
                true,
                peopleId);
            CMSMembershipProvider.provider.AdminOverride = false;
            return user;
        }
        public static User CreateUser(CMSDataContext db, int peopleId)
        {
            var p = db.LoadPersonById(peopleId);
            var uname = Util2.FetchUsername(db, p.PreferredName, p.LastName);
            var pword = Guid.NewGuid().ToString();
            CMSMembershipProvider.provider.AdminOverride = true;
            var user = CMSMembershipProvider.provider.NewUser(
                uname,
                pword,
                null,
                true,
                peopleId);
            CMSMembershipProvider.provider.AdminOverride = false;
            return user;
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var currentUser = CMSMembershipProvider.provider.GetUser(userName, true /* userIsOnline */);
            return currentUser != null && currentUser.ChangePassword(oldPassword, newPassword);
        }
        public static bool ChangePassword(string userName, string newPassword)
        {
            CMSMembershipProvider.provider.AdminOverride = true;
            var mu = CMSMembershipProvider.provider.GetUser(userName, false);
            if (mu == null)
                return false;
            mu.UnlockUser();
            var ret = mu.ChangePassword(mu.ResetPassword(), newPassword);
            CMSMembershipProvider.provider.AdminOverride = false;
            return ret;
        }

        public static bool IsMFASetupRequired(User user, CMSDataContext db) =>
            !user.MFAEnabled &&
            IsTwoFactorAuthenticationEnabled(db) &&
            user.InAnyRole(db.Setting("TwoFactorAuthRequiredRoles", "").ToStringArray());

        public static bool IsTwoFactorAuthenticationEnabled(CMSDataContext db) => db.Setting("TwoFactorAuthEnabled");

        public static bool ShouldPromptForTwoFactorAuthentication(User user, CMSDataContext db, HttpRequestBase Request)
        {
            if (IsTwoFactorAuthenticationEnabled(db))
            {
                var token = Request.Cookies["_mfa"]?.Value;
                return user.MFAEnabled && !db.MFATokens.Any(
                    t => t.UserId == user.UserId &&
                    t.Key == token &&
                    t.Expires > DateTime.Now);
            }
            return false;
        }

        public static SetupCode TwoFactorAuthenticationSetupInfo(User user, CMSDataContext db)
        {
            if (!IsTwoFactorAuthenticationEnabled(db))
            {
                throw new Exception("Two-factor authentication is not enabled");
            }
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var secretKey = Get2FASecret(db);
            var churchName = db.GetSetting("NameOfChurch", "TouchPoint");
            var userSecret = Get2FAUserSecret(user, secretKey);
            var size = 300;
            var displayName = $"{user.Username}@{Util.Host}".Replace(" ", "-");
            var setupInfo = tfa.GenerateSetupCode(churchName, displayName, userSecret, size, size, true);
            return setupInfo;
        }

        private static string Get2FAUserSecret(User user, string secretKey) => $"{Util.Host}.{user.UserId}.{Util.Decrypt(user.Secret, "People")}.{secretKey}";

        private static string Get2FASecret(CMSDataContext db)
        {
            return Util.PickFirst(
                   db.Setting("TwoFactorAuthSecretKey", null),
                   Common.Configuration.Current.TwoFactorAuthSecretKey);
        }

        public static bool ValidateTwoFactorPasscode(User user, CMSDataContext db, string passcode)
        {
            TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
            var secretKey = Get2FASecret(db);

            return passcode?.Length == 6 && tfa.ValidateTwoFactorPIN(Get2FAUserSecret(user, secretKey), passcode);
        }

        public static void  DisableTwoFactorAuth(User user, CMSDataContext db, HttpResponseBase response)
        {
            user.MFAEnabled = false;
            user.Secret = null;
            db.MFATokens.DeleteAllOnSubmit(
                db.MFATokens.Where(m => m.UserId == user.UserId));
            db.SubmitChanges();
            response.SetCookie(new HttpCookie("_mfa", null) { Expires = DateTime.Now.Date });
        }

        public static void SaveTwoFactorAuthenticationToken(CMSDataContext db, HttpResponseBase response)
        {
            const string name = "_mfa";

            var expirationDays = db.Setting("TwoFactorAuthExpirationDays", "180").ToInt();
            var expires = DateTime.Now.AddDays(expirationDays);
            var key = string.Join("", "123456".Select(c => Guid.NewGuid().ToString("N")));
            var token = new MFAToken {
                Expires = expires,
                Key = key,
                UserId = db.CurrentUser.UserId
            };
            db.MFATokens.InsertOnSubmit(token);
            db.SubmitChanges();

            var cookie = new HttpCookie(name, token.Key) { Expires = expires, HttpOnly = true, Secure = !Util.IsDebug() };
            if (!cookie.Secure) // https://stackoverflow.com/questions/26627886/not-able-to-set-cookie-from-action
            {
                cookie.Domain = null;
            }
            response.SetCookie(cookie);
        }
    }
}
