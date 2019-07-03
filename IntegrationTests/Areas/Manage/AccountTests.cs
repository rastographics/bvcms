using IntegrationTests.Support;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Manage
{
    [Collection("WebApp Collection")]
    public class AccountTests : AccountTestBase
    {
        [Fact]
        public void MyData_User_Logon_Fail_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser();

            Login(withPassword: "bad password");

            PageSource.ShouldContain("Logon Error!");
        }

        [Fact]
        public void MyData_User_Logon_Success_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser();

            Login();

            PageSource.ShouldContain(user.Person.Name);
            PageSource.ShouldContain(user.Person.EmailAddress);
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

            CurrentUrl.ShouldBe($"{rootUrl}Account/ChangePassword/");

            Find(id: "currentPassword").SendKeys(password);
            Find(id: "newPassword").SendKeys(newPassword);
            Find(id: "confirmPassword").SendKeys(newPassword);
            Find(css: "input[type=submit]").Click();

            PageSource.ShouldContain("Password Changed");

            Find(text: "Return to Home").Click();

            Logout();
            Login(withPassword: newPassword);

            PageSource.ShouldContain(user.Person.Name);
            PageSource.ShouldContain(user.Person.EmailAddress);
        }

        [Fact]
        public void MyData_User_ForgotPassword_Test()
        {
            username = RandomString();
            password = RandomString();

            var newPassword = RandomString() + "1!";
            var user = CreateUser();

            Open(rootUrl);
            WaitForElement("#inputEmail", 30);

            Find(text: "Forgot?").Click();
            CurrentUrl.ShouldBe($"{rootUrl}Account/ForgotPassword");

            Find(name: "UsernameOrEmail").SendKeys(username);
            Find(css: "input[type=submit]").Click();

            PageSource.ShouldContain("Password Sent");

            db.Refresh(System.Data.Linq.RefreshMode.OverwriteCurrentValues, user);
            user.ResetPasswordCode.ShouldNotBeNull();

            Open($"{rootUrl}Account/SetPassword/{user.ResetPasswordCode}");
            PageSource.ShouldContain("Confirm Password Reset");
            Find(css: "button[type=submit]").Click();
            CurrentUrl.ShouldBe($"{rootUrl}Account/SetPasswordConfirm");

            Find(id: "newPassword").SendKeys(newPassword);
            Find(id: "confirmPassword").SendKeys(newPassword);
            Find(css: "input[type=submit]").Click();
            PageSource.ShouldContain("Password Changed");

            Find(text: "Return to Home").Click();

            Logout();
            Login(withPassword: newPassword);

            PageSource.ShouldContain(user.Person.Name);
            PageSource.ShouldContain(user.Person.EmailAddress);
        }
    }
}
