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
        [Fact, FeatureTest]
        public void Create_Role_Test()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
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

        [Fact, FeatureTest]
        public void Role_Setting_HideNavTabs_Test()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();
            Toggle_NavTabs_Setting(false);   // test both states
            Toggle_NavTabs_Setting(true);
        }

        private void Toggle_NavTabs_Setting(bool shouldBeVisible)
        {
            Open($"{rootUrl}Roles/1");

            Find(css: "button[data-target=\"#General\"]").Click();
            WaitForElement("#HideNavTabs");
            Find(css: "#HideNavTabs + .toggle-group").Click();

            WaitForElement(".snackbar.success");

            Open(rootUrl);
            var navbar = @"<ul class=""nav navbar-nav"">";
            if (shouldBeVisible)
            {
                PageSource.ShouldContain(navbar);
            }
            else
            {
                PageSource.ShouldNotContain(navbar);
            }
        }
    }
}
