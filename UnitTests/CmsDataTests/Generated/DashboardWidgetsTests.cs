using SharedTestFixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CmsDataTests.Generated
{
    [Collection(Collections.Database)]
    public class DashboardWidgetsTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(5)]
        public void CreateWidgetContent(int pythonTypeId)
        {
            var pythonContent = MockContent.CreatePythonContent(db);
            var expectedPythonTypeId = pythonTypeId;

            Assert.Equal(pythonContent.TypeID, expectedPythonTypeId);

            MockContent.DeletePythonContent(db, pythonContent);
        }
    }
}
