using System;
using System.Text;
using System.Web;
using CmsData;
using CmsWeb.Membership;
using CmsWeb.Models;
using ImageData;
using UtilityExtensions;

namespace CmsWeb.Code
{
    internal static class AuthHelper
    {
        public static AuthResult AuthenticateDeveloper(HttpContextBase context, bool shouldLog = false, string additionalRole = "", string altrole = "")
        {
            var auth = context.Request.Headers["Authorization"];
            var db = CMSDataContext.Create(context);
            var idb = CMSImageDataContext.Create(context);

            if (!auth.HasValue()) return new AuthResult {IsAuthenticated = false, Message = "!API no Authorization Header"};

            var cred = Encoding.ASCII.GetString(
                Convert.FromBase64String(auth.Substring(6))).Split(':');
            var username = cred[0];
            var password = cred[1];

            User user = null;
            var valid = CMSMembershipProvider.provider.ValidateUser(username, password);
            if (valid)
            {
                var roles = CMSRoleProvider.provider;
                user = AccountModel.SetUserInfo(db, idb, username, context.Session);

                var isdev = roles.IsUserInRole(username, "Developer");
                var isalt = altrole.HasValue() && roles.IsUserInRole(username, altrole);
                if (!isdev && !isalt)
                    valid = false;

                if (additionalRole.HasValue() && !roles.IsUserInRole(username, additionalRole))
                    valid = false;
            }

            var message = valid ? $" API {username} authenticated" : $"!API {username} not authenticated";

            if (shouldLog)
                CmsData.DbUtil.LogActivity(message.Substring(1));

            return new AuthResult {IsAuthenticated = valid, User = user, Message = message};
        }
    }

    internal class AuthResult
    {
        public bool IsAuthenticated { get; set; }
        public User User { get; set; }
        public string Message { get; set; }
    }
}
