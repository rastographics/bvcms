using CmsData;
using CmsWeb.Models;
using System;
using System.Linq;
using System.Net;
using UtilityExtensions;

namespace CmsWeb.Areas.OnlineReg.Models
{
    public partial class OnlineRegPersonModel
    {
        public User CreateAccount()
        {
            //var Db = Db;
            if (!person.EmailAddress.HasValue())
            {
                CannotCreateAccount = true;
            }
            else if (person.Users.Any()) // already have account
            {
                if (org == null || org.IsMissionTrip == true)
                {
                    return null;
                }

                SawExistingAccount = true;
                var user = person.Users.OrderByDescending(uu => uu.LastActivityDate).First();

                var message = DbUtil.Db.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
                message = message
                    .Replace("{name}", person.Name)
                    .Replace("{host}", DbUtil.Db.CmsHost);
                Log("AlreadyHaveAccount");
                DbUtil.Db.Email(DbUtil.AdminMail, person, "Account information for " + DbUtil.Db.Host, message);
            }
            else
            {
                CreatedAccount = true;
                var user = MembershipService.CreateUser(DbUtil.Db, person.PeopleId);
                Log("SendNewUserEmail");
                AccountModel.SendNewUserEmail(user.Username);
                return user;
            }
            return null;
        }

        public void SendOneTimeLink(string from, string url, string subject, string body, string appendQueryString = "")
        {
            var ot = new OneTimeLink
            {
                Id = Guid.NewGuid(),
                Querystring = $"{divid ?? orgid ?? masterorgid},{PeopleId}"
            };
            //var Db = Db;
            DbUtil.Db.OneTimeLinks.InsertOnSubmit(ot);
            DbUtil.Db.SubmitChanges();

            url = $"{url}{ot.Id.ToCode()}{(!string.IsNullOrWhiteSpace(appendQueryString) ? $"?{appendQueryString}" : string.Empty)}";

            var message = body.Replace("{url}", url, ignoreCase: true);
            message = message.Replace(WebUtility.UrlEncode("{url}"), url, ignoreCase: true);
            message = message.Replace("{name}", person.Name, ignoreCase: true);
            message = message.Replace("{first}", person.PreferredName, ignoreCase: true);

            DbUtil.Db.Email(from, person, subject, message);
        }
    }
}
