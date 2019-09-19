using IntegrationTests.Support;
using Shouldly;
using System.Drawing;
using Xunit;
using SharedTestFixtures;

namespace IntegrationTests.Areas.Finance.Views.Bundle
{
    [Collection(Collections.Webapp)]
    public class EditTests : AccountTestBase
    {
        [Theory]
        [InlineData(320)]
        [InlineData(425)]
        [InlineData(768)]
        public void Should_Open_Datepicker_On_Mobile_Resolutions(int width)
        {
            driver.Manage().Window.Size = new Size(width, 667);

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();

            Open($"{rootUrl}Bundle/Edit/{new FinanceTestUtils(db).BundleHeader.BundleHeaderId}");
            Wait(5);
            PageSource.ShouldContain("Contribution Bundle");
            Check_If_DateTimePicker_Exists();

            Open($"{rootUrl}Person2/{user.PeopleId}");
            Wait(5);
            PageSource.ShouldContain("General");

            Find(css: ".edit-basic").Click();
            Wait(2);
            ScrollTo(id: "WeddingDate");
            Check_If_DateTimePicker_Exists();

            Find(css: ".navbar-toggle").Click();
            Wait(1);
            Find(css: "#navbar>.navbar-nav>.dropdown:nth-child(5)").Click();
            ScrollTo(xpath: "//a[contains(@href, '/FinanceReports/TotalsByFundAgeRange')]");
            Wait(1);
            Find(xpath: "//a[contains(@href, '/FinanceReports/DonorTotalSummary')]").Click();
            Check_If_DateTimePicker_Exists();
        }
        
        protected void Check_If_DateTimePicker_Exists()
        {
            Find(css: "span.input-group-addon").Click();
            WaitForElement("div.bootstrap-datetimepicker-widget", 1);
            var timepicker = Find(css: "div.bootstrap-datetimepicker-widget");
            timepicker.ShouldNotBeNull();
        }
    }
}
