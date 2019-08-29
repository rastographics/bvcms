using Xunit;
using CmsData;
using System.Collections.Generic;
using Shouldly;

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

            // Create Child Org
            var ChildOrgconfig = new Organization()
            {
                OrganizationName = "MockChildName",
                RegistrationTitle = "MockChildTitle",
                Location = "MockChildLocation",
                RegistrationTypeId = 8,
                RegSettingXml = XMLSettings(MasterOrgId)
            };

            var FakeChildOrg = FakeOrganizationUtils.MakeFakeOrganization(ChildOrgconfig);
            ChildOrgId = FakeChildOrg.org.OrganizationId;

            FakeOrganizationUtils.FakeNewOrganizationModel = null;

            // Create Master Org
            var MasterOrgconfig = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = 20,
                RegSettingXml = XMLSettings(MasterOrgId, true),
                OrgPickList = ChildOrgId.ToString()
            };

            var FakeMasterOrg = FakeOrganizationUtils.MakeFakeOrganization(MasterOrgconfig);
            MasterOrgId = FakeMasterOrg.org.OrganizationId;

            var ChildOnlineRegModel = FakeOrganizationUtils.GetFakeOnlineRegModel(ChildOrgId);
            var ChildOnlineRegPersonModel = ChildOnlineRegModel.LoadExistingPerson(ChildOnlineRegModel.UserPeopleId ?? 0, 0);

            var MasterOnlineRegModel = FakeOrganizationUtils.GetFakeOnlineRegModel(MasterOrgId);
            var MasterOnlineRegPersonModel = MasterOnlineRegModel.LoadExistingPerson(MasterOnlineRegModel.UserPeopleId ?? 0, 0);

            bool ChildDOB = ChildOnlineRegPersonModel.ShowDOBOnFind();
            bool ChildPhone = ChildOnlineRegPersonModel.ShowPhoneOnFind();

            bool MasterDOB = MasterOnlineRegPersonModel.ShowDOBOnFind();
            bool MasterPhone = MasterOnlineRegPersonModel.ShowPhoneOnFind();

            ChildDOB.ShouldBe(MasterDOB);
            ChildPhone.ShouldBe(MasterPhone);

            FakeOrganizationUtils.DeleteOrg(MasterOrgId);
            FakeOrganizationUtils.DeleteOrg(ChildOrgId);
        }

        private string XMLSettings(int OrgId, bool IsMasterOrg = false)
        {
            string Settings = string.Format(
                @"<Settings id=""{0}"">" +
                    "<!--1 8/18/2019 10:46 PM-->" +
                    "<Fees>" +
                        "<Fee>50</Fee>" +
                        "<Deposit>15</Deposit>" +
                    "</Fees>" +
                    "<NotRequired>" +
                        "<ShowDOBOnFind>{1}</ShowDOBOnFind>" +
                        "<ShowPhoneOnFind>{1}</ShowPhoneOnFind>" +
                    "</NotRequired>" +
                "</Settings>", OrgId, IsMasterOrg ? "True" : "False");

            return Settings;
        }
    }
}
