using CmsData;
using CmsData.Codes;
using SharedTestFixtures;
using Shouldly;
using System;
using System.Linq;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class CMSDataContextFunctionTests : FinanceTestBase
    {
        [Fact]
        public void GetTotalContributionsDonorTest()
        {
            var fromDate = new DateTime(2019, 1, 1);
            var toDate = new DateTime(2019, 7, 31);
            using (var db = CMSDataContext.Create(DatabaseFixture.Host))
            {
                var TotalAmmountContributions = db.Contributions
                    .Where(x => x.ContributionTypeId == ContributionTypeCode.CheckCash)
                    .Where(x => x.ContributionDate >= fromDate)
                    .Where(x => x.ContributionDate < toDate.AddDays(1))
                    .Sum(x => x.ContributionAmount) ?? 0;
                var TotalPledgeAmountContributions = db.Contributions
                    .Where(x => x.ContributionTypeId == ContributionTypeCode.Pledge)
                    .Where(x => x.ContributionDate >= fromDate)
                    .Where(x => x.ContributionDate < toDate.AddDays(1))
                    .Sum(x => x.ContributionAmount) ?? 0;

                var bundleHeader = CreateBundle(db);
                var FirstContribution = CreateContribution(db, bundleHeader, fromDate, 120, peopleId: 1);
                var SecondContribution = CreateContribution(db, bundleHeader, fromDate, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);

                var results = db.GetTotalContributionsDonor(fromDate, toDate, null, null, true, null, null, true).ToList();
                var actualContributionsAmount = results.Where(x => x.ContributionTypeId == ContributionTypeCode.CheckCash).Sum(x => x.Amount);
                var actualPledgesAmount = results.Where(x => x.ContributionTypeId == ContributionTypeCode.Pledge).Sum(x => x.PledgeAmount);

                actualContributionsAmount.ShouldBe(TotalAmmountContributions + 120);
                actualPledgesAmount.ShouldBe(TotalPledgeAmountContributions + 500);

                DeleteAllFromBundle(db, bundleHeader);
            }
        }

        [Fact]
        public void PledgesSummaryTest()
        {
            var fromDate = new DateTime(2019, 1, 1);
            using (var db = CMSDataContext.Create(Util.Host))
            {
                var bundleHeader = CreateBundle(db);
                var FirstContribution = CreateContribution(db, bundleHeader, fromDate, 100, peopleId: 1);
                var SecondContribution = CreateContribution(db, bundleHeader, fromDate, 20, peopleId: 1);
                var Pledges = CreateContribution(db, bundleHeader, fromDate, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);

                //Get amount contributed to the pledge
                var TotalAmmountContributions = db.Contributions
                    .Where(x => x.ContributionTypeId == ContributionTypeCode.CheckCash)
                    .Sum(x => x.ContributionAmount) ?? 0;

                //Get Pledge amount
                var TotalPledgeAmount = db.Contributions
                    .Where(x => x.ContributionTypeId == ContributionTypeCode.Pledge && x.PeopleId == 1 && x.FundId == 1)
                    .Sum(x => x.ContributionAmount) ?? 0;

                var results = db.PledgesSummary(1);
                var actual = results.ToList().First();

                actual.AmountContributed.ShouldBe(TotalAmmountContributions);
                actual.AmountPledged.ShouldBe(TotalPledgeAmount);
                actual.Balance.ShouldBe((TotalPledgeAmount) - (TotalAmmountContributions) < 0 ? 0 : (TotalPledgeAmount) - (TotalAmmountContributions));

                DeleteAllFromBundle(db, bundleHeader);              
            }
        }
    }
}
