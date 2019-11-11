using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.CheckIn.Views
{
    [Collection(Collections.Webapp)]
    public class CheckinTimeViewsTests : AccountTestBase
    {
        [Fact]
        public void Should_Display_No_CheckIn_Times()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            Open($"{rootUrl}CheckinTimes/");

            PageSource.ShouldContain("no checkin times available");
        }
    }
}
