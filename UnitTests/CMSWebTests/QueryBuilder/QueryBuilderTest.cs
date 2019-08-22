using Xunit;
using Shouldly;
using UtilityExtensions;
using CMSWebTests.SpecialContent;

namespace CmsData.QueryBuilder
{
    [Collection("Database collection")]
    public class QueryBuilderTest
    {
        [Theory]
        [ClassData(typeof(PythonScriptsTest))]
        public void ShouldRunPythonScriptFromString(string script)
        {
            var pe = new PythonModel(CMSDataContext.Create(Util.Host));

            var result = pe.RunScript(script);
            result.ShouldNotBeNull();
        }
    }
}
