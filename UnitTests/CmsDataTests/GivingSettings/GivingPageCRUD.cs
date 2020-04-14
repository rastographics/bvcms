using CmsData.View;
using SharedTestFixtures;
using System.Linq;
using Xunit;

namespace CmsDataTests.GivingSettings
{
    [Collection(Collections.Database)]
    public class GivingPageCRUD : DatabaseTestBase
    {
        [Theory]
        [InlineData("Giving Page One")]
        [InlineData("Giving Page Two")]
        [InlineData("Giving Page Three")]
        public void GivingPageCRUDTest(string givingPageName)
        {
            var contributionFund = MockFunds.CreateContributionFund(db, null);
            var givingPage = MockGivingPage.CreateGivingPage(db, givingPageName, contributionFund.FundId);
            var givingPageFund = MockGivingPage.CreateGivingPageFund(db, givingPage.GivingPageId, contributionFund.FundId);

            var expectedName = givingPageName;
            var actualName = givingPage.PageName;
            Assert.Equal(expectedName, actualName);

            MockGivingPage.DeleteGivingPageFund(db, givingPageFund);
            MockGivingPage.DeleteGivingPage(db, givingPage);
            MockFunds.DeleteFund(db, contributionFund.FundId);
        }
    }
}
