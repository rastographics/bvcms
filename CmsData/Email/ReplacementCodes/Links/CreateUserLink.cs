using System;
using System.Linq;
using UtilityExtensions;

namespace CmsData
{
    public partial class EmailReplacements
    {
        private string CreateUserLinkReplacement(EmailQueueTo emailqueueto)
        {
            var user = (from u in db.Users
                        where u.PeopleId == emailqueueto.PeopleId
                        select u).FirstOrDefault();
            if (user != null)
            {
                user.ResetPasswordCode = Guid.NewGuid();
                user.ResetPasswordExpires = Util.Now.AddHours(db.Setting("ResetPasswordExpiresHours", "24").ToInt());
                string link = db.ServerLink("/Account/SetPassword/" + user.ResetPasswordCode.ToString());
                db.SubmitChanges();
                return $@"<a href=""{link}"">Set password for {user.Username}</a>";
            }
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = emailqueueto.PeopleId.ToString()
            };
            db.OneTimeLinks.InsertOnSubmit(ot);
            db.SubmitChanges();
            var url = db.ServerLink($"/Account/CreateAccount/{ot.Id.ToCode()}");
            return $@"<a href=""{url}"">Create Account</a>";
        }

    }
}
