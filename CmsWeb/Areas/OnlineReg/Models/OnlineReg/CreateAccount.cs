using CmsData;
using CmsWeb.Membership;
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

                var message = CurrentDatabase.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
                message = message
                    .Replace("{name}", person.Name)
                    .Replace("{host}", CurrentDatabase.CmsHost);
                Log("AlreadyHaveAccount");
                CurrentDatabase.Email(DbUtil.AdminMail, person, "Account information for " + CurrentDatabase.Host, message);
            }
            else
            {
                CreatedAccount = true;
                var user = MembershipService.CreateUser(CurrentDatabase, person.PeopleId);
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
            CurrentDatabase.OneTimeLinks.InsertOnSubmit(ot);
            CurrentDatabase.SubmitChanges();

            url = $"{url}{ot.Id.ToCode()}{(!string.IsNullOrWhiteSpace(appendQueryString) ? $"?{appendQueryString}" : string.Empty)}";

            var message = body.Replace("{url}", url, ignoreCase: true);
            message = message.Replace(WebUtility.UrlEncode("{url}"), url, ignoreCase: true);
            message = message.Replace("{name}", person.Name, ignoreCase: true);
            message = message.Replace("{first}", person.PreferredName, ignoreCase: true);

            CurrentDatabase.Email(from, person, subject, message);
        }
    }
}
