using Xunit;
using CmsData;
using System.Collections.Generic;
using Shouldly;
using SharedTestFixtures;

namespace CMSWebTests.Areas.OnlineReg.Models.OnlineRegPerson
{
    [Collection(Collections.Database)]
    public class HelperTests
    {

        [Fact]
        public void Should_Use_MasterOrg_DOB_Phone_Settings()
        {
            int MasterOrgId = 0;
            int ChildOrgId = 0;
            var requestManager = FakeRequestManager.Create();
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(requestManager);
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeControllerContext(controller, routeDataValues);

            // Create Master Org
            var MasterOrgconfig = new Organization() {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = 20,
                RegSetting = XMLSettings(MasterOrgId)
            };

            var FakeMasterOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, MasterOrgconfig);
            MasterOrgId = FakeMasterOrg.org.OrganizationId;

            // Create Child Org
            var ChildOrgconfig = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = 8,
                ParentOrgId = MasterOrgId
            };

            var FakeChildOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, ChildOrgconfig);
            ChildOrgId = FakeChildOrg.org.OrganizationId;

            var MasterOnlineRegModel = FakeOrganizationUtils.GetFakeOnlineRegModel(ChildOrgId);
            var ChildOnlineRegModel = FakeOrganizationUtils.GetFakeOnlineRegModel(MasterOrgId);

            var MasterOnlineRegPersonModel = MasterOnlineRegModel.LoadExistingPerson(ChildOnlineRegModel.UserPeopleId ?? 0, 0);
            var ChildOnlineRegPersonModel = ChildOnlineRegModel.LoadExistingPerson(ChildOnlineRegModel.UserPeopleId ?? 0, 0);

            ChildOnlineRegPersonModel.ShowDOBOnFind().ShouldBe(MasterOnlineRegPersonModel.ShowDOBOnFind());
            ChildOnlineRegPersonModel.ShowPhoneOnFind().ShouldBe(MasterOnlineRegPersonModel.ShowPhoneOnFind());

            FakeOrganizationUtils.DeleteOrg(MasterOrgId);
            FakeOrganizationUtils.DeleteOrg(ChildOrgId);
        }

        private string XMLSettings(int OrgId)
        {
            string Settings = string.Format(
                @"<Settings id=""{0}"">" +
                    "<!--1 8/18/2019 10:46 PM-->" +
                    "<Fees>" +
                        "<Fee>50</Fee>" +
                        "<Deposit>15</Deposit>" +
                    "</Fees>" +
                    "<NotRequired>" +
                        "<ShowDOBOnFind>True</ShowDOBOnFind>" +
                        "<ShowPhoneOnFind>True</ShowPhoneOnFind>" +
                    "</NotRequired>" +
                "</Settings>", OrgId);

            return Settings;
        }
    }
}