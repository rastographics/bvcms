using CmsData;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Areas.Setup.Views.Gateway
{
    [Collection(Collections.Webapp)]
    public class GatewayTests : AccountTestBase
    {
        [Fact]
        public void Should_Set_Up_A_Gateway()
        {
            CMSDataContext db = CMSDataContext.Create(Host);
            var PaymentProcess = db.PaymentProcess.ToList();
            PaymentProcess.ForEach(x =>
            {
                x.GatewayAccountId = null;
            });
            db.SubmitChanges();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            Open($"{rootUrl}Gateway");

            WaitForElement(css: "tr:nth-child(1) .red-empty", maxWaitTimeInSeconds: 5);
            Find(css: "tr:nth-child(1) .red-empty").Click();

            WaitForElement(".label-gateway > input", maxWaitTimeInSeconds: 5);
            if(!Find(css: ".label-gateway > input").Selected)
            {
                Find(css: ".label-gateway > input").Click();
            }

            Find(css: ".row:nth-child(1) .form-control").Clear();
            Find(css: ".row:nth-child(1) .form-control").SendKeys("Transnational");

            Find(css: ".row:nth-child(6) .form-control").Clear();
            Find(css: ".row:nth-child(6) .form-control").SendKeys("NewUsr");

            Find(css: ".row:nth-child(8) .form-control").Clear();
            Find(css: ".row:nth-child(8) .form-control").SendKeys("NewKey");

            Find(xpath: "//input[@value='Save']").Click();

            WaitForElement(".confirm", maxWaitTimeInSeconds: 5);
            Find(css: ".confirm").Click();

            var TransnationalLabel = Find(xpath: "//a[contains(text(),'Transnational')]");
            TransnationalLabel.ShouldNotBeNull();
        }
    }
}
