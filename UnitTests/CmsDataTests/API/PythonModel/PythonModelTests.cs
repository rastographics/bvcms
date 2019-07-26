using CmsData;
using Shouldly;
using System;
using UtilityExtensions;
using Xunit;

namespace CmsDataTests
{
    [Collection("Database collection")]
    public class PythonModelTests
    {
        [Theory]
        [InlineData("2019-07-25", 1, "12.00", "3214", "Slush Fund", 1)]
        public void AddContributionTest(string date, int fundid, string amount, string checkno, string description, int peopleid)
        {
            var db = CMSDataContext.Create(Util.Host);
            var model = new PythonModel(db);
            var dateValue = DateTime.Parse(date);
            var bundleHeader = model.GetBundleHeader(dateValue, DateTime.Now);
            var detail = model.AddContribution(dateValue, fundid, amount, checkno, description, peopleid);
            bundleHeader.BundleDetails.Add(detail);
            model.FinishBundle(bundleHeader);

            detail.ContributionId.ShouldNotBe(0);
            detail.Contribution.ContributionDate.ShouldBe(dateValue);
            detail.Contribution.FundId.ShouldBe(fundid);
            detail.Contribution.ContributionAmount.ShouldBe(decimal.Parse(amount));
            detail.Contribution.CheckNo.ShouldBe(checkno);
            detail.Contribution.ContributionDesc.ShouldBe(description);
            detail.Contribution.PeopleId.ShouldBe(peopleid);
        }
    }
}
