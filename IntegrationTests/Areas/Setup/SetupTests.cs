using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace IntegrationTests.Areas.Manage
{
    [Collection(Collections.Webapp)]
    public class SetupTests : AccountTestBase
    {
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
            var adminRole = db.Roles.SingleOrDefault(r => r.RoleName == "Admin");
            var role = db.Roles.SingleOrDefault(r => r.RoleName == roleName);
            role.ShouldNotBeNull();
            role.Priority.GetValueOrDefault().ShouldBeGreaterThan(adminRole.Priority.GetValueOrDefault());
        }

        [Fact]
        public void Role_Setting_HideNavTabs_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(roles: new string[] { "Access", "Edit", "Admin" });
            Login();
            Toggle_NavTabs_Setting();   // test both states
            Toggle_NavTabs_Setting();
        }

        private void Toggle_NavTabs_Setting()
        {
            Open($"{rootUrl}Roles/1");

            Find(css: "button[data-target='#General']").Click();
            WaitForElement("#General");
            Find(css: "#HideNavTabs + .toggle-group").Click();

            WaitForElement(".snackbar.success");

            var status = Find(css: "#HideNavTabs + .toggle-group .btn.active").Text;    // "Hide" or "Show"

            Open($"{rootUrl}Roles");
            if (status == "Show")
            {
                PageSource.ShouldContain("#navbar .dropdown .dropdown-menu");
            }
            else
            {
                PageSource.ShouldNotContain("#navbar .dropdown .dropdown-menu");
            }
        }
    }
}
