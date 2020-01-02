using CmsData.ExtraValue;
using SharedTestFixtures;
using Shouldly;
using Xunit;

namespace CmsDataTests.Classes.ExtraValues
{
    [Collection(Collections.Database)]
    public class ViewsTests : DatabaseTestBase
    {
        [Fact]
        public void Should_GetViews()
        {
            var actual = Views.GetViews(db);
            actual.ShouldNotBeNull();
        }

        [Fact]
        public void Should_GetStandardExtraValuesOrdered()
        {
            var actual = Views.GetStandardExtraValuesOrdered(db, "People", "Entry");
            actual.ShouldNotBeNull();
        }
    }
}
