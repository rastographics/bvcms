using CmsWeb.Membership;
using SharedTestFixtures;
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

        protected CmsData.User CreateUser()
        {
            return base.CreateUser(username, password);
        }

        protected void Login(string withPassword = null)
        {
            Open(rootUrl);

            WaitForElement("#inputEmail", 30);

            Find(name: "UsernameOrEmail").SendKeys(username);
            Find(name: "Password").SendKeys(withPassword ?? password);
            Find(css: "input[type=submit]").Click();
        }
        protected CmsData.User LoginAsAdmin()
        {
            username = RandomString();
            password = RandomString();
            var person = CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin" });
            Login();
            return person;
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
