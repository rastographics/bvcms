using Xunit;
using Shouldly;
using UtilityExtensions;
using CMSWebTests.SpecialContent;
using SharedTestFixtures;

namespace CmsData.QueryBuilder
{
    [Collection(Collections.Database)]
    public class QueryBuilderTest
    {
        [Theory]
        [ClassData(typeof(PythonScriptsTest))]
        public void ShouldRunPythonScriptFromString(string script)
        {
            var pe = new PythonModel(CMSDataContext.Create(DatabaseFixture.Host));

            var result = pe.RunScript(script);
            result.ShouldNotBeNull();
        }
    }
}
