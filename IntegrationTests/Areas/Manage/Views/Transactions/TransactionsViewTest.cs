using CmsData;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;

namespace IntegrationTests.Areas.Manage.Views.Transactions
{
    [Collection(Collections.Webapp)]
    public class TransactionsViewTest : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Should_Find_Transacionts()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.OnlineRegistration, GatewayTypes.Transnational);
            Login();

            OrgId = CreateOrgWithFee();

            SettingUtils.UpdateSetting("UseRecaptcha", "false");

            PayRegistration(OrgId, true);

            Open($"{rootUrl}Transactions/");

            var people = db.People.FirstOrDefault(p => p.PeopleId == user.PeopleId);

            Find(xpath: "//input[@id='name']").Clear();
            Find(xpath: "//input[@id='name']").SendKeys(people.FirstName);

            Find(xpath: "//form[@id='form']/div/div[2]/div[6]/div/div[2]/label").Click();   
            Find(xpath: "//a[@id='filter']").Click();   

            var element = Find(xpath: "//table[@id='resultsTable']/tbody/tr/td[7]");

            element.ShouldNotBeNull();
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
