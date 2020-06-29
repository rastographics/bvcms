using SharedTestFixtures;
using Xunit;

namespace CmsDataTests.Generated
{
    [Collection(Collections.Database)]
    public class DashboardWidgetsTests : DatabaseTestBase
    {
        [Theory]
        [InlineData(5)]
        public void CreateWidgetPythonContent(int pythonTypeId)
        {
            var pythonContent = MockContent.CreatePythonContent(db);
            var expectedPythonTypeId = pythonTypeId;

            Assert.Equal(pythonContent.TypeID, expectedPythonTypeId);

            MockContent.DeletePythonContent(db, pythonContent);
        }
        [Theory]
        [InlineData(1)]
        public void CreateWidgetHTMLContent(int htmlTypeId)
        {
            var htmlContent = MockContent.CreateHTMLContent(db);
            var expectedHtmlTypeId = htmlTypeId;

            Assert.Equal(htmlContent.TypeID, expectedHtmlTypeId);

            MockContent.DeleteHTMLContent(db, htmlContent);
        }
        [Theory]
        [InlineData(4)]
        public void CreateWidgetSqlContent(int sqlTypeId)
        {
            var sqlContent = MockContent.CreateSqlContent(db);
            var expectedSqlTypeId = sqlTypeId;

            Assert.Equal(sqlContent.TypeID, expectedSqlTypeId);

            MockContent.DeleteHTMLContent(db, sqlContent);
        }
    }
}
