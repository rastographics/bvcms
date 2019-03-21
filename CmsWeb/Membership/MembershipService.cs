using CmsData;
using System;
using System.Linq;
using System.Web.Security;
using UtilityExtensions;

namespace CmsWeb.Membership
{
    public static class MembershipService
    {
        public static int MinPasswordLength => DbUtil.Db.Setting("PasswordMinLength", "7").ToInt();

        public static bool RequireSpecialCharacter => DbUtil.Db.Setting("PasswordRequireSpecialCharacter", "true").ToBool();

        public static bool RequireOneNumber => DbUtil.Db.Setting("PasswordRequireOneNumber", "false").ToBool();

        public static bool RequireOneUpper => DbUtil.Db.Setting("PasswordRequireOneUpper", "false").ToBool();

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
            var uname = CmsData.Util2.FetchUsername(db, p.PreferredName, p.LastName);
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

        public static string FetchPassword(CMSDataContext db)
        {
            var rnd = new Random();
            var n = db.Words.Count();
            var r1 = rnd.Next(1, n);
            var r2 = rnd.Next(1, n);
            var q = from w in db.Words
                    where w.N == r1 || w.N == r2
                    select w.WordX;
            var a = q.ToArray();
            if (a.Length == 2)
                return a[0] + "." + a[1];
            return a[0] + "." + a[0];
        }
    }
}
