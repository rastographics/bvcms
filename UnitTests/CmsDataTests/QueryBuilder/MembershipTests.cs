using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedTestFixtures;
using Xunit;
using Dapper;
using Shouldly;

namespace CmsDataTests.QueryBuilder
{
    [Collection(Collections.Database)]
    public class MembershipTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(-2, 1)]
        [InlineData(-1, 0)]
        public void JoinDateMonthsAgo(int addmonths, int shouldbe)
        {
            var dt = DateTime.Today.AddMonths(addmonths);
            db.Connection.Execute("update dbo.People set JoinDate = @dt where PeopleId = 2", new {dt});
            var code = $"JoinDateMonthsAgo = 2";
            var qb = db.PeopleQuery2(code);
            var count = qb.Count();
            // clean up
            db.Connection.Execute("update dbo.People set JoinDate = null where PeopleId = 2");
            count.ShouldBe(shouldbe);
        }
    }
}
