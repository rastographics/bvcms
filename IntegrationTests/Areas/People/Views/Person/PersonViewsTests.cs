using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;

namespace IntegrationTests.Areas.People.Views.Person
{
    [Collection(Collections.Webapp)]
    public class PersonViewsTests : AccountTestBase
    {
        [Fact]
        public void Should_Hide_Giving_Tab()
        {
            SettingUtils.UpdateSetting("HideGivingTabMyDataUsers", "false");

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password);
            Login();

            Open($"{rootUrl}Person2/{user.PeopleId}");
            WaitForElement(".active:nth-child(2) > a", 5);
            PageSource.ShouldContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            SettingUtils.UpdateSetting("HideGivingTabMyDataUsers", "true");

            Open($"{rootUrl}/Person2/Current"); //refresh page
            WaitForElement(".active:nth-child(2) > a", 5);
            PageSource.ShouldNotContain("<a href=\"#giving\" aria-controls=\"giving\" data-toggle=\"tab\">Giving</a>");

            SettingUtils.DeleteSetting("HideGivingTabMyDataUsers");
        }
    }
}
