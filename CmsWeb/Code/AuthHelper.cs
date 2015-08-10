using System;
using System.Text;
using System.Web;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Code
{
    internal static class AuthHelper
    {
        public static AuthResult AuthenticateDeveloper(HttpContext context, bool shouldLog = false, string addRole = "")
        {
            var auth = context.Request.Headers["Authorization"];

            if (!auth.HasValue()) return new AuthResult {IsAuthenticated = false, Message = "!API no Authorization Header"};

            var cred = Encoding.ASCII.GetString(
                Convert.FromBase64String(auth.Substring(6))).Split(':');
            var username = cred[0];
            var password = cred[1];

            var valid = CMSMembershipProvider.provider.ValidateUser(username, password);
            if (valid)
            {
                var roles = CMSRoleProvider.provider;

                if (context.Session != null)
                    AccountModel.SetUserInfo(username, context.Session);

                if (!roles.IsUserInRole(username, "Developer"))
                    valid = false;

                if (addRole.HasValue() && !roles.IsUserInRole(username, addRole))
                    valid = false;
            }

            var message = valid ? $" API {username} authenticated" : $"!API {username} not authenticated";

            if (shouldLog)
                DbUtil.LogActivity(message.Substring(1));

            return new AuthResult {IsAuthenticated = valid, Message = message};
        }
    }

    internal class AuthResult
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
    }
}
