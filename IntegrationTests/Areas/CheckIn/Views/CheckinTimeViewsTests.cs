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

    [Collection(Collections.Webapp)]
    public class CheckInTests : AccountTestBase
    {
        [Fact]
        public void Should_Allow_Login_and_Logout()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Checkin" });
            Open($"{rootUrl}CheckIn/");

            WaitForElement("#CheckInApp", 30);

            Find(name: "username").SendKeys(username);
            Find(name: "password").SendKeys(password);
            Find(name: "kiosk").SendKeys("test");
            Find(css: "input[type=submit]").Click();
            Wait(10);

            Open($"{rootUrl}CheckIn/");
            WaitForElement("#CheckInApp", 30);

            PageSource.ShouldContain("Enter your phone number");
            
            Open($"{rootUrl}CheckIn/Logout", true);
            Open($"{rootUrl}CheckIn/");
            WaitForElement("#CheckInApp", 30);

            PageSource.ShouldNotContain("Enter your phone number");
        }
    }
}
