using CmsData;
using System.Linq;
using System.Security.Principal;

namespace CmsWeb.Membership.Extensions
{
    public static class MembershipExtensions
    {
        public static void ChangePassword(this User user, string newpassword)
        {
            CMSMembershipProvider.provider.AdminOverride = true;
            var mu = CMSMembershipProvider.provider.GetUser(user.Username, false);
            if (mu != null)
            {
                mu.UnlockUser();
                mu.ChangePassword(mu.ResetPassword(), newpassword);
                user.TempPassword = newpassword;
            }
            CMSMembershipProvider.provider.AdminOverride = false;
        }

        public static bool InAnyRole(this IPrincipal principal, params string[] roles)
        {
            return roles.Any(role => principal.IsInRole(role) == true); 
        }
    }
}
