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
            var contributionFund = CreateContributionFund();
            var givingPage = CreateGivingPage(givingPageName, contributionFund.FundId);
            var givingPageFund = CreateGivingPageFund(givingPage.GivingPageId, contributionFund.FundId);

            var expectedName = givingPageName;
            var actualName = givingPage.PageName;
            Assert.Equal(expectedName, actualName);

            DeleteGivingPageFund(givingPageFund);
            DeleteGivingPage(givingPage);
            DeleteContributionFund(contributionFund);
        }
    }
}
