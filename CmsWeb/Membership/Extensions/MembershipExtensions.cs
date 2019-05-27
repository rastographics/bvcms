using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}
