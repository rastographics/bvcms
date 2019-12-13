using CmsWeb.Models;
using System.Linq;
using Xunit;
using SharedTestFixtures;
using Shouldly;

namespace CmsWeb.ModelsTests
{
    [Collection(Collections.Database)]
    public class CheckInModelTests : DatabaseTestBase
    {
        [Fact]
        public void SavePrintJobTest()
        {
            var model = new CheckInModel(db);
            model.SavePrintJob("Kiosk1", null, "{\"fake\":\"json string test\"}");

            db.PrintJobs.Count().ShouldBeGreaterThan(0);
        }
    }
}
