using CmsData;
using CmsData.Codes;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;

namespace IntegrationTests.Areas.OnlineReg
{
    [Collection(Collections.Webapp)]
    public class OneTimeGivingTests : AccountTestBase
    {
        [Fact, FeatureTest]
        public void OneTimeGiving_Processes_Payment()
        {
            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password);
            var org = db.Organizations.First(o => o.RegistrationTypeId == RegistrationTypeCode.OnlineGiving);
            org.ShouldNotBeNull();
            var orgId = org.OrganizationId;
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.OneTimeGiving, GatewayTypes.Transnational);
            Login();

            Open($"{rootUrl}Person2/{user.PeopleId}");
            WaitForPageLoad();
            Find(css: @"a[href=""#giving""]").Click();
            WaitForElementToDisappear(loadingUI);
            Wait(2);

            Find(text: "Make a One Time Gift").Click();
            Wait(3);
            SwitchToWindow(d => d.Title == "Online Registration");

            CurrentUrl.ShouldBe($"{rootUrl}OnlineReg/{orgId}");
        }
    }
}
