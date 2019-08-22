using Xunit;
using System.Collections;
using System.Net.Http;
using CmsData;
using CmsWeb.Areas.OnlineReg.Models;
using System.Collections.Generic;
using Shouldly;
using UtilityExtensions;
using CmsWeb.Areas.OnlineReg.Models;

namespace CMSWebTests.Areas.OnlineReg.Models.OnlineRegPerson
{
    [Collection("Database collection")]
    public class HelperTests
    {
        private int MasterOrgId { get; set; }
        private int ChildOrgId { get; set; }

        [Fact]
        public void Should_Use_MasterOrg_DOB_Phone_Settings()
        {
            var controller = new CmsWeb.Areas.OnlineReg.Controllers.OnlineRegController(FakeRequestManager.FakeRequest());
            var routeDataValues = new Dictionary<string, string> { { "controller", "OnlineReg" } };
            controller.ControllerContext = ControllerTestUtils.FakeContextController(controller, routeDataValues);

            // Create Master Org
            var MasterOrgconfig = new Organization() {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = 20,
                RegSetting = XMLSettings(MasterOrgId)
            };

            var FakeMasterOrg = FakeOrganizationUtils.MakeFakeOrganization(MasterOrgconfig);
            MasterOrgId = FakeMasterOrg.org.OrganizationId;

            FakeOrganizationUtils.FakeNewOrganizationModel = null;

            // Create Child Org
            var ChildOrgconfig = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = 8,
                ParentOrgId = MasterOrgId
            };

            var FakeChildOrg = FakeOrganizationUtils.MakeFakeOrganization(ChildOrgconfig);
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
