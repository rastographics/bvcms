using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests.Charts
{
    [Collection(Collections.Database)]
    public class GoogleChartsDataTest : DatabaseTestBase
    {
        [Fact]
        public void GetContributions_Should_Not_Fetch_ReturnedReversedPledges()
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var contributions = (from c in Contribution.GetContributions(db) select c);
                var returnedReversedPledges = from c2 in contributions
                                              where ContributionTypeCode.ReturnedReversedTypes.Contains(c2.ContributionTypeId)
                                              select c2;

                returnedReversedPledges.Count().ShouldBeLessThan(1);
            }
        }

        [Theory]
        [InlineData(new int[] { 101, 102, 201, 211, 212, 302, 306, 317, 319, 509 }, 2018)]
        [InlineData(new int[] { 211, 212 }, 2019)]
        [InlineData(new int[] { }, null)]
        public void GetContributionsPerYear_Should_Not_GetReturnedReversedPledges(int[] fundids, int? year)
        {
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var contributions = (from c in Contribution.GetContributionsPerYear(db, year, fundids) select c);
                var returnedReversedPledges = from c2 in contributions
                                              where ContributionTypeCode.ReturnedReversedTypes.Contains(c2.ContributionTypeId)
                                              select c2;

                returnedReversedPledges.Count().ShouldBeLessThan(1);
            }
        }
    }
}
