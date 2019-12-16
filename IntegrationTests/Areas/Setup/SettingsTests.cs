using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Setup
{
    [Collection(Collections.Webapp)]
    public class SettingsTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void Settings_Should_Display_By_Category_And_Type()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            Open($"{rootUrl}Settings");
            var general = "a[href=\"#general\"]";
            var system = "a[href=\"#system\"]";
            var features = "a[href=\"#features\"]";
            var integrations = "a[href=\"#integrations\"]";
            Find(css: general).ShouldNotBeNull();
            Find(css: system).ShouldNotBeNull();
            Find(css: features).ShouldNotBeNull();
            Find(css: integrations).ShouldNotBeNull();

            Find(css: system).Click();
            Find(css: "#system .accordion div:nth-child(1) button").Click();
            Find(id: "AdminCoupon").ShouldNotBeNull();
        }
    }
}
