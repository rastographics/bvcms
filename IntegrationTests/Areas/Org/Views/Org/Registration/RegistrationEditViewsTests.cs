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

namespace IntegrationTests.Areas.Org.Views.Org.Registration
{
    [Collection(Collections.Webapp)]
    public class RegistrationEditViewsTests : AccountTestBase
    {
        private int OrgId { get; set; }

        [Fact]
        public void Relaxed_Questions_Should_Not_Be_Visible()
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

            SettingUtils.UpdateSetting("RelaxedReqAdminOnly", "true");

            Open($"{rootUrl}Org/{OrgId}#tab-Registrations-tab");
            WaitForElementToDisappear(loadingUI);

            Find(css: "#Registration > form > div.row > div:nth-child(2) > div > a.btn.edit.ajax.btn-primary").Click();
            WaitForElementToDisappear(loadingUI);

            var inputDOB = Find(id: "ShowDOBOnFind");
            inputDOB.ShouldBeNull();
        }

        public override void Dispose()
        {
            FakeOrganizationUtils.DeleteOrg(OrgId);
        }
    }
}
