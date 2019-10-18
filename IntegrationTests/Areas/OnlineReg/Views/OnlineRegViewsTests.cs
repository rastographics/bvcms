using CmsData.Codes;
using CMSWebTests;
using IntegrationTests.Support;
using SharedTestFixtures;
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
        public void Should_Change_PrefferdPaymentType()
        {
            username = RandomString();
            password = RandomString();
            string roleName = "role_" + RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Access", "Edit", "Admin" });
            Login();

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

            Open($"{rootUrl}Org/{OrgId}#tab-Registrations-tab");

            ScrollTo(xpath: "//a[contains(text(),'Fees')]");

            Find(xpath: "//a[contains(text(),'Fees')]").Click();
            Find(css: "#Fees .row .edit").Click();
            Find(id: "Fee").Clear();
            Find(id: "Fee").SendKeys("5");
            Find(css: ".pull-right:nth-child(1) > .validate").Click();

            Open($"{rootUrl}OnlineReg/{OrgId}");
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
