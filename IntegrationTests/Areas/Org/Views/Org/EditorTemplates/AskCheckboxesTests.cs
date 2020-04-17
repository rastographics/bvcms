using CmsData;
using CmsData.Codes;
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

namespace IntegrationTests.Areas.Org.Views.Org.EditorTemplates
{
    [Collection(Collections.Webapp)]
    public class AskCheckboxesTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Column_Number_Out_Of_Range_Should_Show_Error()
        {
            CMSDataContext db = CMSDataContext.Create(DatabaseFixture.Host);
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

            username = RandomString();
            password = RandomString();
            var user = CreateUser(username, password, roles: new string[] { "Edit", "Access" });
            Login();

            Open($"{rootUrl}Org/{OrgId}#tab-Registrations-tab");
            WaitForElementToDisappear(loadingUI);

            Find(css: "#Questions-tab > .ajax").Click();
            WaitForElementToDisappear(loadingUI);

            Find(css: ".row:nth-child(1) > .col-sm-12 .edit").Click();
            WaitForElementToDisappear(loadingUI);

            Find(text: "Add Question").Click();
            Wait(2);
            Find(text: "Checkboxes").Click();
            Wait(2);
            Find(text: "Done").Click();
            Wait(2);
            Find(css: ".confirm").Click();
            Wait(2);

            Find(xpath: "//div[4]/div/div/input").SendKeys("4");
            Find(text: "Save").Click();
            Wait(2);

            PageSource.ShouldContain("The field Columns must be between 0 and 3.");
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
