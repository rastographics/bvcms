using IntegrationTests.Support;
using OpenQA.Selenium;
using Xunit;

namespace IntegrationTests.Areas.Manage
{
    public class AccountTests : AccountTestBase
    {
        [Fact]
        public void MyData_User_Logon_Fail_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser();

            Login(withPassword: "bad password");

            Assert.Contains("Logon Error!", PageSource);
        }

        [Fact]
        public void MyData_User_Logon_Success_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser();

            Login();

            Assert.Contains(user.Person.Name, PageSource);
            Assert.Contains(user.Person.EmailAddress, PageSource);
        }

        [Fact]
        public void MyData_User_Logout_Success_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser();

            Login();

            Logout();
        }

        [Fact]
        public void MyData_User_ChangePassword_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser();

            Login();
            var newPassword = RandomString() + "1!";
            Find(css: profileMenu).Click();
            Find(text: "Change Password").Click();

            Assert.Equal($"{rootUrl}Account/ChangePassword/", CurrentUrl);

            Find(id: "currentPassword").SendKeys(password);
            Find(id: "newPassword").SendKeys(newPassword);
            Find(id: "confirmPassword").SendKeys(newPassword);
            Find(css: "input[type=submit]").Click();

            Assert.Contains("Password Changed", PageSource);

            Find(text: "Return to Home").Click();

            Logout();
            Login(withPassword: newPassword);

            Assert.Contains(user.Person.Name, PageSource);
            Assert.Contains(user.Person.EmailAddress, PageSource);
        }
    }
}
