using CmsWeb.Areas.Search.Models;
using CmsData;
using SharedTestFixtures;
using Xunit;
using Shouldly;
using System.Linq;

namespace CMSWebTests.Areas.Search.Models
{
    [Collection(Collections.Database)]
    public class OrgSearchModelTests
    {
        [Fact]
        public void Should_Generate_Options_Settings()
        {
            var db = DatabaseFixture.NewDbContext();
            User user = db.Users.Where(u => u.Username == "admin").FirstOrDefault();
            db.CurrentUser = user;
            OrgSearchModel m = new OrgSearchModel(db);
            var result = m.RegOptionsList();
            result.ShouldNotBeNull();
        }
    }
}
