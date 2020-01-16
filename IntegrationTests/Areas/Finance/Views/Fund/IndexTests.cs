using IntegrationTests.Support;
using Shouldly;
using System.Drawing;
using Xunit;
using SharedTestFixtures;
using OpenQA.Selenium.Support.UI;

namespace IntegrationTests.Areas.Finance.Views.Fund
{
    [Collection(Collections.Webapp)]
    public class IndexTests : AccountTestBase
    {
        [Fact]
        public void Should_Edit_ShowList_Fund()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Finance" });
            Login();

            Open($"{rootUrl}Funds/");

            Find(css: "#sl1").Click();
            Wait(2);
            var dropdown = Find(tag: "select");
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText("Secondary");
            Find(css: ".editable-submit").Click();
            Wait(2);
            var showList = Find(css: "#sl1").GetAttribute("innerHTML");
            showList.ShouldBe("Secondary");

            Find(css: "#sl1").Click();
            Wait(2);
            var dropdown1 = Find(tag: "select");
            var selectElement1 = new SelectElement(dropdown1);
            selectElement1.SelectByText("Primary");
            Find(css: ".editable-submit").Click();
            Wait(2);
            var showList1 = Find(css: "#sl1").GetAttribute("innerHTML");
            showList1.ShouldBe("Primary");
        }
    }
}
