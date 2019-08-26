using CmsData;
using SharedTestFixtures;
using Shouldly;
using System;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class PythonModelTests
    {
        [Theory]
        [InlineData("2019-07-25", "7/22/2019 1:29:32 PM", 1, "12.00", "3214", "Slush Fund", 1)]
        [InlineData("2019-07-25", "7/22/2019 13:30:00", 1, "50", "1080", "Slush Fund", 1)]
        public void AddContributionTest(string date, string cDate, int fundid, string amount, string checkno, string description, int peopleid)
        {
            var db = CMSDataContext.Create(Util.Host);
            var model = new PythonModel(db);
            var dateValue = DateTime.Parse(date);
            var bundleHeader = model.GetBundleHeader(dateValue, DateTime.Now);
            var contributionDate = model.ParseDate(cDate).Value;
            var detail = model.AddContribution(contributionDate, fundid, amount, checkno, description, peopleid);
            bundleHeader.BundleDetails.Add(detail);
            model.FinishBundle(bundleHeader);

            detail.ContributionId.ShouldNotBe(0);
            detail.BundleHeader.ContributionDate.ShouldBe(dateValue);
            detail.Contribution.ContributionDate.ShouldBe(contributionDate);
            detail.Contribution.FundId.ShouldBe(fundid);
            detail.Contribution.ContributionAmount.ShouldBe(decimal.Parse(amount));
            detail.Contribution.CheckNo.ShouldBe(checkno);
            detail.Contribution.ContributionDesc.ShouldBe(description);
            detail.Contribution.PeopleId.ShouldBe(peopleid);
        }
    }
}
