using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CmsData;
using System.Text;
using UtilityExtensions;

namespace CmsWeb.Models
{
    public partial class OnlineRegPersonModel
    {
        public User CreateAccount()
        {
            var Db = DbUtil.Db;
            if (!person.EmailAddress.HasValue())
                CannotCreateAccount = true;
            else if (person.Users.Count() > 0) // already have account
            {
                SawExistingAccount = true;
                var user = person.Users.OrderByDescending(uu => uu.LastActivityDate).First();

                var message = DbUtil.Db.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
                message = message
                    .Replace("{name}", person.Name)
                    .Replace("{host}", DbUtil.Db.CmsHost);

                Db.Email(DbUtil.AdminMail, person, "Account information for " + Db.Host, message);
            }
            else
            {
                CreatedAccount = true;
                var user = MembershipService.CreateUser(Db, person.PeopleId);
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
                Querystring = "{0},{1}".Fmt(divid ?? orgid ?? masterorgid , PeopleId) 
            };
            var Db = DbUtil.Db;
            Db.OneTimeLinks.InsertOnSubmit(ot);
            Db.SubmitChanges();

            var message = body.Replace("{url}", url + ot.Id.ToCode(), ignoreCase:true);
            message = message.Replace("{name}", person.Name, ignoreCase:true);
            message = message.Replace("{first}", person.PreferredName, ignoreCase:true);

            Db.Email(from, person, subject, message);
        }
    }
}