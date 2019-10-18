using CmsData;
using CmsData.Codes;
using CmsData.Finance;
using CmsWeb.Areas.Dialog.Models;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests.Areas.OnlineReg.Views
{
    [Collection(Collections.Webapp)]
    public class OnlineRegViewsTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Should_Change_Payment_Methods()
        {
            MaximizeWindow();

            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

            createOrgWithFee();

            Open($"{rootUrl}Org/{OrgId}#tab-Registrations-tab");

            ScrollTo(css: "#Fees-tab>a");
            Wait(10);

            Find(css: "#Fees-tab>a").Click();
            Wait(7);

            Find(css: "#Fees .row .edit").Click();
            Wait(7);

            Find(id: "Fee").Clear();
            Find(id: "Fee").SendKeys("5");
            Find(css: ".pull-right:nth-child(1) > .validate").Click();
            Wait(7);

            Open($"{rootUrl}OnlineReg/{OrgId}");

            Find(id: "otheredit").Click();
            WaitForElement("#submitit", 3);
            Find(id: "submitit").Click();

            Find(id: "First").Clear();
            Find(id: "First").SendKeys("FName");

            Find(id: "Last").Clear();
            Find(id: "Last").SendKeys("LName");

            Find(id: "Address").Clear();
            Find(id: "Address").SendKeys("St 12");

            Find(id: "City").Clear();
            Find(id: "City").SendKeys("City");

            Find(id: "State").Clear();
            Find(id: "State").SendKeys("State");

            Find(id: "Country").Click();
            Find(css: ".form-group:nth-child(10)").Click();

            Find(id: "Zip").Clear();
            Find(id: "Zip").SendKeys("01000");

            Find(id: "Phone").Clear();
            Find(id: "Phone").SendKeys("1234567890");

            Find(css: ".btn-group > .btn:nth-child(2)").Click();

            Find(id: "Routing").Clear();
            Find(id: "Routing").SendKeys("123456780");

            Find(id: "Account").Clear();
            Find(id: "Account").SendKeys("111110");

            Find(id: "SavePayInfo").Click();

            WaitForElement("#submitit", maxWaitTimeInSeconds: 5);
            Find(id: "submitit").Click();
            Wait(5);

            var startNewTransaction = Find(xpath: "//a[contains(text(),'Start a New Transaction')]");
            startNewTransaction.ShouldNotBeNull();

            var paymentInfo = db.PaymentInfos.SingleOrDefault(x => x.PeopleId == user.PeopleId);
            paymentInfo.PreferredPaymentType.ShouldBe("C");
        }

        private void createOrgWithFee()
        {
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            var FakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, new CmsData.Organization()
            {
                OrganizationName = "MockName",
                RegistrationTitle = "MockTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization
            });

            OrgId = FakeOrg.org.OrganizationId;
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
