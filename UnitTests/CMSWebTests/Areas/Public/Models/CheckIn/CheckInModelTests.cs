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
            model.SavePrintJob("TestKiosk1", null, "{\"fake\":\"test json\"}");

            db.PrintJobs.Where(m => m.Id == "TestKiosk1").Count().ShouldBeGreaterThan(0);
        }
    }
}
