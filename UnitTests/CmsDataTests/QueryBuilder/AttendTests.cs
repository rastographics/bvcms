using System;
using System.Linq;
using CmsData;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace CmsDataTests.QueryBuilder
{
    [Collection(Collections.Database)]
    public class AttendTests : DatabaseTestBase
    {
        [Fact]
        public void RecentAttendCount_Should_Work()
        {
            var code = "RecentAttendCount( Days=1, Org=36[App Testing Org] ) = 1";
            
            var org = 36;
            var pid = 2;
            var dt = DateTime.Today.AddHours(7);

            Attend.RecordAttend(db, pid, org, true, dt);
            db.SubmitChanges();
            var qb = db.PeopleQuery2(code);
            var count = qb.Count();
            db.ExecuteCommand("DELETE dbo.attend where OrganizationId = {0} AND MeetingDate = {1}", org, dt);
            db.ExecuteCommand("DELETE dbo.meetings where OrganizationId = {0} AND MeetingDate = {1}", org, dt);

            count.ShouldBe(1);
        }
    }
}
