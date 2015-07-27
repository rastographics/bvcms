using System;
using System.Linq;
using System.Net;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public User CreateAccount()
        {
            var Db = DbUtil.Db;
            if (!person.EmailAddress.HasValue())
                CannotCreateAccount = true;
            else if (person.Users.Any()) // already have account
            {
                if (org == null || org.IsMissionTrip == true)
                    return null;
                SawExistingAccount = true;
                var user = person.Users.OrderByDescending(uu => uu.LastActivityDate).First();

                var message = DbUtil.Db.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
                message = message
                    .Replace("{name}", person.Name)
                    .Replace("{host}", DbUtil.Db.CmsHost);
                Log("AlreadyHaveAccount");
                Db.Email(DbUtil.AdminMail, person, "Account information for " + Db.Host, message);
            }
            else
            {
                CreatedAccount = true;
                var user = MembershipService.CreateUser(Db, person.PeopleId);
                Log("SendNewUserEmail");
                AccountModel.SendNewUserEmail(user.Username);
                return user;
            }
            return null;
        }

        public void SendOneTimeLink(string from, string url, string subject, string body)
        {
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = $"{divid ?? orgid ?? masterorgid},{PeopleId}"
            };
            var Db = DbUtil.Db;
            Db.OneTimeLinks.InsertOnSubmit(ot);
            Db.SubmitChanges();

            var message = body.Replace("{url}", url + ot.Id.ToCode(), ignoreCase: true);
            message = message.Replace(WebUtility.UrlEncode("{url}"), url + ot.Id.ToCode(), ignoreCase: true);
            message = message.Replace("{name}", person.Name, ignoreCase: true);
            message = message.Replace("{first}", person.PreferredName, ignoreCase: true);

            Db.Email(from, person, subject, message);
        }
    }
}
