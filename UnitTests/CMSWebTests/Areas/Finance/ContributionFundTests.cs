using CmsWeb.Models;
using System;
using System.Linq;
using Xunit;
using SharedTestFixtures;
using CMSWebTests.Support;
using Shouldly;
using CmsData;
using CmsData.Codes;

namespace CMSWebTests.Areas.Finance
{
    [Collection(Collections.Database)]
    public class ContributionFundTests : DatabaseTestBase
    {
        [Fact]
        public void ToggleContributionFundEndDate()
        {
            var contributionFund = (from c in db.ContributionFunds where c.EndDateFlag == false || c.EndDateFlag == null select c).FirstOrDefault();
            TurnEndDateToFundOn(contributionFund);
            contributionFund.EndDateFlag.ShouldBe(true);
            TurnEndDateToFundOff(contributionFund);
            contributionFund.EndDateFlag.ShouldBe(false);
        }

        public ContributionFund TurnEndDateToFundOn(ContributionFund contributionFund)
        {
            contributionFund.EndDateFlag = true;
            db.SubmitChanges();
            return contributionFund;
        }

        public ContributionFund TurnEndDateToFundOff(ContributionFund contributionFund)
        {
            contributionFund.EndDateFlag = false;
            db.SubmitChanges();
            return contributionFund;
        }
    }
}
