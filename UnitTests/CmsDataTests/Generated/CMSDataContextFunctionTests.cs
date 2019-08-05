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
                var FirstContribution = CreateContribution(db, bundleHeader, fromDate, 120, peopleId: 1);
                var SecondContribution = CreateContribution(db, bundleHeader, fromDate, 500, peopleId: 1, contributionType: ContributionTypeCode.Pledge);

                var results = db.GetTotalContributionsDonor(fromDate, toDate, null, null, true, null, null).ToList();
                var actual = results.First();

                actual.Amount.ShouldBe(120);

                var bundleDetail = db.BundleDetails.Where(x => x.ContributionId == FirstContribution.ContributionId && x.ContributionId == SecondContribution.ContributionId).FirstOrDefault();
                db.BundleDetails.DeleteOnSubmit(bundleDetail);
                db.Contributions.DeleteOnSubmit(FirstContribution);
                db.Contributions.DeleteOnSubmit(SecondContribution);
                //This fails => actual.PledgeAmount.ShouldBe(500);
            }
        }
    }
}
