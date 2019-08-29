using CmsWeb.Membership;
using System;
using System.Linq;
using Xunit;

namespace IntegrationTests.Support
{
    /// <summary>
    /// Any feature test that needs a logged in account should inherit this class
    /// </summary>
    public class AccountTestBase : FeatureTestBase
    {
        protected const string profileMenu = "#navbar #me, #bluebar-menu > div.btn-group.btn-group-lg > button";

        //some commonly used variables
        protected string username;
        protected string password;

        public string loginUrl => rootUrl + "Logon?ReturnUrl=%2f";

        protected CmsData.User CreateUser(CmsData.Family family = null, params string[] roles)
        {
            if (family == null)
            {
                family = new CmsData.Family();
                db.Families.InsertOnSubmit(family);
                db.SubmitChanges();
            }
            var person = new CmsData.Person
            {
                Family = family,
                FirstName = RandomString(),
                LastName = RandomString(),
                EmailAddress = RandomString() + "@example.com",
                MemberStatusId = CmsData.Codes.MemberStatusCode.Member,
                PositionInFamilyId = CmsData.Codes.PositionInFamily.PrimaryAdult,
            };
            db.People.InsertOnSubmit(person);
            db.SubmitChanges();

            var createDate = DateTime.Now;
            var machineKey = GetValidationKeyFromWebConfig();
            var passwordhash = CMSMembershipProvider.EncodePassword(password, System.Web.Security.MembershipPasswordFormat.Hashed, machineKey);
            var user = new CmsData.User
            {
                PeopleId = person.PeopleId,
                Username = username,
                Password = passwordhash,
                MustChangePassword = false,
                IsApproved = true,
                CreationDate = createDate,
                LastPasswordChangedDate = createDate,
                LastActivityDate = createDate,
                IsLockedOut = false,
                LastLockedOutDate = createDate,
                FailedPasswordAttemptWindowStart = createDate,
                FailedPasswordAnswerAttemptWindowStart = createDate,
            };
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();

            if (roles.Any())
            {
                user.AddRoles(db, roles);
                db.SubmitChanges();
            }
            return user;
        }

        private string GetValidationKeyFromWebConfig()
        {
            var config = LoadWebConfig();
            var machineKey = config.SelectSingleNode("configuration/system.web/machineKey");
            return machineKey.Attributes["validationKey"].Value;
        }

        protected void Login(string withPassword = null)
        {
            Open(rootUrl);

            WaitForElement("#inputEmail", 30);

            Find(name: "UsernameOrEmail").SendKeys(username);
            Find(name: "Password").SendKeys(withPassword ?? password);
            Find(css: "input[type=submit]").Click();
        }

        protected void Logout()
        {
            Find(css: profileMenu).Click();
            Find(text: "Log Out").Click();

            Assert.Contains("Sign In", PageSource);

            Assert.True(driver.Url == loginUrl);
        }
    }
}
