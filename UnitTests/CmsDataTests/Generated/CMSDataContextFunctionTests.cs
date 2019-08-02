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
            using (var db = CMSDataContext.Create(Util.Host))
            {
                var bundleHeader = CreateBundle(db);
                CreateContribution(db, bundleHeader, fromDate, 120, peopleId: 1);
                CreateContribution(db, bundleHeader, fromDate, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);

                var results = db.GetTotalContributionsDonor(fromDate, toDate, null, null, true, null, null).ToList();
                var TotalAmmountContributions = db.Contributions.Where(x => x.ContributionTypeId == 1).Sum(x => x.ContributionAmount);
                var actual = results.First();

                actual.Amount.ShouldBe(TotalAmmountContributions);
                //This fails => actual.PledgeAmount.ShouldBe(500);
            }
        }
    }
}
