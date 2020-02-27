using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace IntegrationTests.Areas.Search.Views.Query
{
    [Collection(Collections.Webapp)]
    public class ResultsTests : AccountTestBase
    {
        [Fact]
        public void Should_Update_Tag()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new[] { "Access", "Edit", "Admin" });
            Login();

            MaximizeWindow();

            Open($"{rootUrl}NewQuery");

            WaitForElement("#QueryConditionSelect .close > span");
            Find(css: "#QueryConditionSelect .close > span").Click();

            Find(id: "CancelChange").Click();
            Find(id: "Run").Click();

            Find(css: ".btn > .fa-cog").Click();
            Find(xpath: "//a[contains(text(),'Add All')]").Click();
            Find(id: "empty-dialog").Click();

            Find(id: "tagname").Clear();
            Find(id: "tagname").SendKeys("NewTagShouldBeUpdated");

            Find(css: ".modal-footer > .btn-primary").Click();

            Wait(10);
            driver.PageSource.ShouldContain("NewTagShouldBeUpdated");
        }
    }
}
