using Xunit;
using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTestFixtures;
using Shouldly;

namespace CmsData.Tests
{
    [Collection(Collections.Database)]
    public class ContributionFundTests : DatabaseTestBase
    {
        [Theory]
        [InlineData("Contribution Fund Notes Test 1", true)]
        [InlineData("Contribution Fund Notes Test 2", true)]
        public void ContributionFundNotesEnabled(string name, bool notes)
        {
            var contributionFund = MockFunds.CreateContributionFund(db, name, notes);
            var newContributionFund = db.ContributionFunds.Where(c => c.FundId == contributionFund.FundId).FirstOrDefault();
            newContributionFund.Notes.ShouldBeTrue();
        }
    }
}
