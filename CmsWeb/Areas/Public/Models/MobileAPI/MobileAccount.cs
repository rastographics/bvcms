using System;
using System.Linq;
using CmsData;
using CmsWeb.Models;
using UtilityExtensions;

namespace CmsWeb.MobileAPI
{
    public class MobileAccount
    {
        private readonly CMSDataContext db;

        public enum ResultCode
        {
            CreatedNewUser,
            FoundPersonWithDiffEmailButCreatedNewUser,
            FoundPersonWithSameEmail,
            BadEmailAddress
        }

        public string First { get; set; }
        public string Last { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthdate { get; set; }
        public User User { get; set; }
        public Person FoundPerson { get; set; }
        public Person NewPerson { get; set; }
        public ResultCode Result { get; set; }

        public MobileAccount()
        {
            db = DbUtil.Db;
        }

        public static MobileAccount Create(string first, string last, string email, string phone, string dob)
        {
            var m = new MobileAccount
            {
                First = first,
                Last = last,
                Email = email.trim(),
                Phone = phone.GetDigits()
            };
            DateTime bd;
            if (Util.DateValid(dob, out bd))
                m.Birthdate = bd;
            if (!Util.ValidEmail(m.Email))
            {
                m.Result = ResultCode.BadEmailAddress;
                return m;
            }
            m.FindPersonSendAccountInfo();
            return m;
        }

        private void FindPersonSendAccountInfo()
        {
            var foundpeopleids = db.FindPerson(First, Last, Birthdate, Email, Phone).Select(vv => vv.PeopleId.Value).ToArray();
            var foundpeople = (from p in db.People
                               where foundpeopleids.Contains(p.PeopleId)
                               select p).ToList();

            // the simplest case is that we did not find an existing person
            if (foundpeople.Count == 0)
            {
                Result = ResultCode.CreatedNewUser;
                var p = CreateNewPerson();
                CreateNewUser(p);
                return;
            }

            // notify all found matches
            foreach (var p in foundpeople)
                if (p.EmailAddress.Equal(Email) || p.EmailAddress2.Equal(Email))
                    if (p.Users.Any())
                        NotifyAboutExistingAccount(p);
                    else
                        CreateNewUser(p);
                else
                    NotifyAboutDuplicateUser(p);

            // if we did not find anybody with same email, then create a new account
            if (foundPersonWithSameEmail == null)
            {
                var p = CreateNewPerson();
                CreateNewUser(p);
                Result = ResultCode.FoundPersonWithDiffEmailButCreatedNewUser;
            }

            FoundPerson = foundPersonWithSameEmail ?? foundPersonWithDiffEmail;
        }

        private Person CreateNewPerson()
        {
            var p = Person.Add(db, null, First, null, Last, Birthdate);
            p.PositionInFamilyId = CmsData.Codes.PositionInFamily.PrimaryAdult;
            p.EmailAddress = Email;
            p.HomePhone = Phone;
            db.SubmitChanges();
            return p;
        }

        private void CreateNewUser(Person p)
        {
            User = MembershipService.CreateUser(db, p.PeopleId);
            db.SubmitChanges();
            AccountModel.SendNewUserEmail(User.Username);
        }

        private Person foundPersonWithDiffEmail;
        private Person foundPersonWithSameEmail;

        private void NotifyAboutExistingAccount(Person p)
        {
            var message = db.ContentHtml("ExistingUserConfirmation", Resource1.CreateAccount_ExistingUser);
            message = message
                .Replace("{name}", p.Name)
                .Replace("{host}", db.CmsHost);
            db.Email(DbUtil.AdminMail, p, "Account information for " + db.Host, message);
            User = p.Users.OrderByDescending(uu => uu.LastActivityDate).FirstOrDefault()
                   ?? MembershipService.CreateUser(db, p.PeopleId);
            if(foundPersonWithSameEmail == null)
                foundPersonWithSameEmail = p;
            Result = ResultCode.FoundPersonWithSameEmail;
        }

        private void NotifyAboutDuplicateUser(Person p)
        {
            var message = db.ContentHtml("DuplicateUserOnMobile", Resource1.NotifyDuplicateUserOnMobile);
            db.Email(DbUtil.AdminMail, p, "New User Account on " + db.Host, message);
            if(foundPersonWithDiffEmail == null)
                foundPersonWithDiffEmail = p;
        }
    }
}