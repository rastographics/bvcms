using CmsData;
using CmsWeb.Areas.Search.Models;
using SharedTestFixtures;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace CMSWebTests
{
    [Collection(Collections.Database)]
    public class QueryModelTests
    {
        [Fact]
        public void Should_be_able_to_parse_and_execute_querycode()
        {
            const string code = "PeopleId < 4";
            var db = CMSDataContext.Create(DatabaseFixture.Host);
            var m = QueryModel.QueryCode(db, code);
            m.Count().ShouldBe(3);
            db.Dispose();
        }
    }
}
