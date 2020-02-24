using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;
using System.Data.Linq;
using System.Drawing;

namespace IntegrationTests.Areas.Main.Views.Email
{
    [Collection(Collections.Webapp)]
    public class EmailViewsTests : AccountTestBase
    {
        [Fact]
        public void Should_Send_Email_On_Mobile_Resolutions()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Admin", "Edit", "Access", "Checkin" });
            Login();

            var org = CreateOrganization();
            Open($"{rootUrl}Org/{org.OrganizationId}#tab-Settings-tab");
            WaitForElement("#Settings-tab");
            driver.Manage().Window.Size = new Size(414, 768);

            WaitForElementToDisappear(loadingUI);

            Find(css: ".fa-envelope-o").Click();
            Find(xpath: "//a[contains(text(),'Individuals')]").Click();

            WaitForElement("a[template] > strong");
            Find(text: "Empty Template").Click();

            WaitForElement("#Subject");
            Find(id: "Subject").Clear();
            Find(id: "Subject").SendKeys("Email");

            Find(css: "#SendEmail > div.visible-xs-block > button").Click();

            Wait(5);

            Find(css: "body > div.sweet-alert.showSweetAlert.visible > div.sa-button-container > div > button").Click();
            Wait(5);

            driver.PageSource.ShouldContain("<h2>Email has completed.</h2>");
        }
    }
}
