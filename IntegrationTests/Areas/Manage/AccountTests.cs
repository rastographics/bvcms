using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IntegrationTests.Areas.Manage
{
    [Collection(Collections.Webapp)]
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
        public void Create_Role_Test()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            Open($"{rootUrl}Lookups/");
            PageSource.ShouldContain("Lookup Codes");

            Find(text: "Roles").Click();
            CurrentUrl.ShouldBe($"{rootUrl}Roles");

            Find(css: ".box-tools button[type=submit]").Click();
            Find(id: "RoleName.NEW").Click();
            WaitForElement(".editable-input input[type=text]");
            Find(css: ".editable-input input[type=text]").Clear();
            Find(css: ".editable-input input[type=text]").SendKeys(roleName);
            Find(css: ".editable-buttons button[type=submit]").Click();
            Open($"{rootUrl}Roles");
            PageSource.ShouldContain(roleName);

            var role = db.Roles.SingleOrDefault(r => r.RoleName == roleName);
            role.ShouldNotBeNull();
        }
        
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
