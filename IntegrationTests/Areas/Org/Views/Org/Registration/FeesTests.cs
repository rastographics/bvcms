using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using Xunit;
using CMSWebTests;
using System.Data.Linq;
using CmsData;
using System.Linq;

namespace IntegrationTests.Areas.Org.Views.Org.Registration
{
    [Collection(Collections.Webapp)]
    public class FeesTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void Should_Show_PushpayFundName_Field()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Admin", "Edit", "Access", "Checkin" });
            Login();

            var org = CreateOrganization();
            org.RegistrationTypeId = 1; //Join Organization
            db.SubmitChanges();

            var gatewayAccount = MockGateways.CreateSaveGatewayAccount(db, GatewayTypes.Pushpay);
            var process = db.PaymentProcess.FirstOrDefault(p => p.ProcessId == (int)PaymentProcessTypes.OnlineRegistration);
            process.GatewayAccountId = gatewayAccount.GatewayAccountId;
            db.SubmitChanges();

            Open($"{rootUrl}Org/{org.OrganizationId}#tab-Registrations-tab");
            WaitForElement("#Registrations-tab");
            Wait(4);
            Find(text: "Fees").Click();
            WaitForElement(css: "#Fees h4:nth-child(3)");
            PageSource.ShouldContain("Pushpay Fund Name");
        }
    }
}
