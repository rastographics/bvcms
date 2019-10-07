using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;
using System.Data.Linq;

namespace IntegrationTests.Areas.Org.Views.Org
{
    [Collection(Collections.Webapp)]
    public class SettingsViewsTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void Should_Give_Feedback_On_Save()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Admin", "Edit", "Access", "Checkin" });
            Login();

            var org = CreateOrganization();
            Open($"{rootUrl}Org/{org.OrganizationId}#tab-Settings-tab");
            WaitForElement("#Settings-tab");
            Wait(4);
            Find(css: "a[href=\"#CheckIn\"]").Click();
            Wait(4);
            WaitForElement("#CanSelfCheckin");
            PageSource.ShouldContain("<div class=\"snackbar\">");
            Find(id: "CanSelfCheckin").Click();
            Find(text: "Save").Click();
            WaitForElement(".snackbar.success");
            db.Refresh(RefreshMode.OverwriteCurrentValues, org);
            org.CanSelfCheckin.ShouldBe(true);
        }
    }
}
