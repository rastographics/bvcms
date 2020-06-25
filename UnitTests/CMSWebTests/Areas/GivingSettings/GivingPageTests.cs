using SharedTestFixtures;
using Shouldly;
using System.Linq;
using Xunit;
using CMSWebTests.Support;
using CMSWebTests;
using CmsWeb.Lifecycle;
using CmsWeb.Membership;
using CmsWeb.Areas.Giving.Controllers;

namespace CmsDataTests.GivingSettings
{
    [Collection(Collections.Database)]
    public class GivingPageTests : ControllerTestBase
    {
        [Theory]
        [InlineData("Giving Page One", 1)]
        [InlineData("Giving Page Two", 1)]
        [InlineData("Giving Page Three", 1)]
        [InlineData("Giving Page Four", 1)]
        public void GivingPageCRUDTest(string givingPageName, int pageType)
        {
            var contributionFund = MockFunds.CreateContributionFund(db, null);
            var givingPage = MockGivingPage.CreateGivingPage(db, givingPageName, contributionFund.FundId, pageType);
            var givingPageFund = MockGivingPage.CreateGivingPageFund(db, givingPage.GivingPageId, contributionFund.FundId);

            var expectedName = givingPageName;
            db.Copy().GivingPages
                .Count(p => p.PageName == givingPageName)
                .ShouldBeGreaterThan(0);

            MockGivingPage.DeleteGivingPageFund(db, givingPageFund);
            MockGivingPage.DeleteGivingPage(db, givingPage);
            MockFunds.DeleteFund(db, contributionFund.FundId);
        }

        [Fact()]
        public void OnlyOneDefaultGivingPage()
        {
            var requestManager = SetupRequestManager();
            var controller = new GivingManagementController(requestManager);
            var newGivingPage = MockGivingPage.CreateGivingPage(db, "Giving Page One", null, 1);
            controller.SetGivingDefaultPage(true, newGivingPage.GivingPageId);
            var defaultGivingPageList = (from g in db.GivingPages where g.DefaultPage == true select g).ToList();
            defaultGivingPageList.Count.ShouldBe(1);
        }

        private IRequestManager SetupRequestManager()
        {
            var username = RandomString();
            var password = RandomString();
            var user = CreateUser(username, password);
            var requestManager = FakeRequestManager.Create();
            var membershipProvider = new MockCMSMembershipProvider { ValidUser = true };
            var roleProvider = new MockCMSRoleProvider();
            CMSMembershipProvider.SetCurrentProvider(membershipProvider);
            CMSRoleProvider.SetCurrentProvider(roleProvider);
            requestManager.CurrentHttpContext.Request.Headers["Authorization"] = BasicAuthenticationString(username, password);
            return requestManager;
        }
    }
}
