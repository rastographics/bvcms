using CmsData;
using CmsWeb.Areas.Search.Models;
using CmsWeb.Models;
using SharedTestFixtures;
using Xunit;
using Shouldly;

namespace CMSWebTests.Areas.Search.Controllers
{
    [Collection(Collections.Database)]
    public class QueryControllerTests
    {
        [Theory]
        [InlineData ("")]
        [InlineData ("simpletagname")]
        public void Should_Add_Peopple_To_Tag(string tagname)
        {
            string TagName = Util2.GetValidTagName(tagname);
            TagName.ShouldNotBeNullOrEmpty();
        }
    }
}
