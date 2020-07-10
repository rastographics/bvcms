using CmsData;
using CmsData.Codes;
using CMSWebTests;
using IntegrationTests.Support;
using OpenQA.Selenium.Support.UI;
using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;

namespace IntegrationTests.Areas.OnlineReg
{
    [Collection(Collections.Webapp)]
    public class ManageGivingTests : AccountTestBase
    {
        [Fact]
        public void Should_Update_Vault_When_Change_Address()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin", "Developer"});
            FinanceTestUtils.CreateMockPaymentProcessor(db, PaymentProcessTypes.RecurringGiving, GatewayTypes.Transnational);
            Login();
            Wait(3);

            var recurringGivingOrg = (from o in db.Organizations
                       where o.RegistrationTypeId == RegistrationTypeCode.ManageGiving
                       select o.OrganizationId).FirstOrDefault();

            Open($"{rootUrl}OnlineReg/{recurringGivingOrg}");

            Find(id: "myAddFundLink").Click();

            var dropdown = Find(css: "#special-funds-list");
            var selectElement = new SelectElement(dropdown);
            selectElement.SelectByText("General Operation");

            Find(xpath: "//input[@name='FundItem[0].Value']").Clear();
            Find(xpath: "//input[@name='FundItem[0].Value']").SendKeys("5");

            Find(id: "CreditCard").Clear();
            Find(id: "CreditCard").SendKeys("4111111111111111");

            Find(id: "Expires").Clear();
            Find(id: "Expires").SendKeys("0225");

            Find(id: "CVV").Clear();
            Find(id: "CVV").SendKeys("123");

            Find(id: "Middle").Clear();
            Find(id: "Middle").SendKeys("M");

            Find(id: "Suffix").Clear();
            Find(id: "Suffix").SendKeys("S");

            Find(id: "Address").Clear();
            Find(id: "Address").SendKeys("Addr");

            Find(id: "Address2").Clear();
            Find(id: "Address2").SendKeys("Addr2");

            Find(id: "City").Clear();
            Find(id: "City").SendKeys("City");

            Find(id: "State").Clear();
            Find(id: "State").SendKeys("TX");

            Find(id: "Zip").Clear();
            Find(id: "Zip").SendKeys("01000");

            Find(id: "Phone").Clear();
            Find(id: "Phone").SendKeys("1234567890");

            Find(id: "submitit").Click();
            Wait(5);

            Open($"{rootUrl}OnlineReg/{recurringGivingOrg}");

            Find(id: "Address").Clear();
            Find(id: "Address").SendKeys("AddrUpdated");

            Find(id: "submitit").Click();
            Wait(5);

            var log = db.ActivityLogs.OrderByDescending(l => l.ActivityDate).Where(l => l.OrgId == recurringGivingOrg).Skip(1).First();
            log.Activity.ShouldBe("OnlineReg ManageGiving Gateway Vault Updated");
        }

        public override void Dispose()
        {            
        }
    }
}
