using Xunit;
using CmsData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTestFixtures;
using Shouldly;
using CmsData.Codes;

namespace CmsDataTests
{
    [Collection(Collections.Database)]
    public class QueryFunctionsTests : DatabaseTestBase
    {
        [Fact]
        public void ContributionCountTest()
        {
            var q = new QueryFunctions(db);
            var count = q.ContributionCount(7, "1");
            count.ShouldBe(db.Contributions
                .Where(c => c.ContributionStatusId == ContributionStatusCode.Recorded)
                .Where(c => new[] { 6, 7, 8 }.Contains(c.ContributionTypeId) == false)
                .Where(c => c.CreatedDate > DateTime.Now.AddDays(-7).Date && c.FundId == 1)
                .Count());
        }
        [Fact]
        public void ContributionCount2Test()
        {
            var q = new QueryFunctions(db);
            var count = q.ContributionCount(7, 6, "1");
            count.ShouldBe(db.Contributions
                .Where(c => c.ContributionStatusId == ContributionStatusCode.Recorded)
                .Where(c => new[] { 6, 7, 8 }.Contains(c.ContributionTypeId) == false)
                .Where(c => c.FundId == 1)
                .Where(c => c.CreatedDate > DateTime.Now.AddDays(-7).Date)
                .Where(c => c.CreatedDate <= DateTime.Now.AddDays(-6).Date)
                .Count());
        }
    }
}
