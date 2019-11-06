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
        [Theory, FeatureTest]
        [InlineData(320)]
        [InlineData(425)]
        [InlineData(768)]
        public void Should_Open_Datepicker_On_Mobile_Resolutions(int width)
        {
            var window = driver.Manage().Window;
            window.Size = new Size(width, window.Size.Height);

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();

            Open($"{rootUrl}Bundle/Edit/{new FinanceTestUtils(db).BundleHeader.BundleHeaderId}");
            PageSource.ShouldContain("Contribution Bundle");
            Check_If_DateTimePicker_Exists("Bundle_ContributionDate");

            Open($"{rootUrl}Person2/{user.PeopleId}");
            PageSource.ShouldContain("General");

            Find(css: ".edit-basic").Click();
            Wait(2);
            ScrollTo(id: "WeddingDate");
            Check_If_DateTimePicker_Exists("WeddingDate");
        }
        
        protected void Check_If_DateTimePicker_Exists(string id)
        {
            WaitForElementToDisappear(loadingUI);
            Find(css: $"#{id} + span.input-group-addon").Click();
            WaitForElement("div.bootstrap-datetimepicker-widget", 1);
            var timepicker = Find(css: "div.bootstrap-datetimepicker-widget");
            timepicker.ShouldNotBeNull();
        }
    }
}
