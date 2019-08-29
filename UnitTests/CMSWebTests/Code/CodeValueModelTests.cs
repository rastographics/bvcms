using System.Linq;
using CmsData;
using CmsWeb.Code;
using SharedTestFixtures;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace CMSWebTests
{
    [Collection(Collections.Database)]
    public class CodeValueModelTests 
    {
        [Fact]
        public void Should_be_able_to_get_BundleStatusTypes()
        {
            var b = CodeValueModel.BundleStatusTypes();
            b.Count().ShouldBe(2);
        }
        [Fact]
        public void Should_be_able_to_get_ContactReasonCodes()
        {
            using (var db = CMSDataContext.Create(Util.Host))
            {
                var cv = new CodeValueModel(db);
                var b = cv.ContactReasonCodes();
                b.Count().ShouldBe(8);
            }
        }
        [Fact]
        public void Should_be_able_to_get_MemberTypeCodes()
        {
            ContextTestUtils.FakeHttpContext();
            var b = CodeValueModel.MemberTypeCodes();
            b.Count().ShouldBe(16);
        }
    }
}
