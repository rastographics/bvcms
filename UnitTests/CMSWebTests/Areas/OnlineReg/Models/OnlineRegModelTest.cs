using CmsData;
using CmsData.Codes;
using CmsWeb.Areas.OnlineReg.Models;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace CMSWebTests.Areas.OnlineReg.Models
{
    [Collection(Collections.Database)]
    public class OnlineRegModelTest
    {
        [Fact]
        public void ShouldProcessZeroLimitOrgsAsFilled()
        {
            var requestManager = FakeRequestManager.Create();
            var orgRegLimitConfig = new Organization()
            {
                OrganizationName = "MockMasterName",
                RegistrationTitle = "MockMasterTitle",
                Location = "MockLocation",
                RegistrationTypeId = RegistrationTypeCode.JoinOrganization,
                Limit = 0
            };

            var fakeOrg = FakeOrganizationUtils.MakeFakeOrganization(requestManager, orgRegLimitConfig);

            OnlineRegModel om = FakeOrganizationUtils.GetFakeOnlineRegModel((int)fakeOrg.org.OrganizationId);
            om.Filled().ShouldBe("registration is full");

            FakeOrganizationUtils.DeleteOrg(fakeOrg.org.OrganizationId);

        }
    }
}
