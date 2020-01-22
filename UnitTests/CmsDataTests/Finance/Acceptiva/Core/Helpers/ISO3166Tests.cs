using Xunit;
using Shouldly;
using CmsData.Finance.Acceptiva.Core;
using System.Linq;

namespace CmsDataTests.Finance.Acceptiva.Core
{
    public class ISO3166Tests
    {
        [Theory]
        [InlineData("United States")]
        [InlineData("Canada")]
        [InlineData("Mexico")]
        public void Should_Return_Country_Code(string country)
        {
            var code = ISO3166.Alpha3FromName(country);
            code.ToCharArray().Count().ShouldBe(3);
        }
    }
}
