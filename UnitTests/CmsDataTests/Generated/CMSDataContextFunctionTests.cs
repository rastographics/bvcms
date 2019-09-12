using CmsData;
using CmsData.Codes;
using CmsDataTests.Support;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests
{
    [Collection("Database collection")]
    public class CMSDataContextFunctionTests : FinanceTestBase
    {
        //TODO: clean up contributions in the database
        [Fact]
        public void GetTotalContributionsDonorTest()
        {
            var fromDate = new DateTime(2019, 1, 1);
            var toDate = new DateTime(2019, 7, 31);
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var TotalAmmountContributions = db.Contributions.Where(x => x.ContributionTypeId == 1).Sum(x => x.ContributionAmount) ?? 0;
                var TotalPledgeAmountContributions = db.Contributions.Where(x => x.ContributionTypeId == 8).Sum(x => x.ContributionAmount) ?? 0;

                var bundleHeader = CreateBundle(db);
                var FirstContribution = CreateContribution(db, bundleHeader, fromDate, 120, peopleId: 1);
                var SecondContribution = CreateContribution(db, bundleHeader, fromDate, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);

                var results = db.GetTotalContributionsDonor(fromDate, toDate, null, null, true, null, null, true).ToList();
                var actual = results.First();

                actual.Amount.ShouldBe(TotalAmmountContributions + 120);
                actual.PledgeAmount.ShouldBe(TotalPledgeAmountContributions + 500);

                DeleteAllFromBundle(db, bundleHeader);
            }
        }
    }
}
