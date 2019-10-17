using SharedTestFixtures;
using Shouldly;
using UtilityExtensions;
using Xunit;

namespace UtilityExtensionsTests
{
    [Collection(Collections.Miscellaneous)]
    public class UtilTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(false, false)]
        [InlineData("false", false)]
        [InlineData("False", false)]
        [InlineData("FALSE", false)]
        [InlineData("1", false)]
        [InlineData(null, false)]
        [InlineData(true, true)]
        [InlineData("true", true)]
        [InlineData("True", true)]
        [InlineData("TRUE", true)]
        public void ToBoolTest(object val, bool result)
        {
            val.ToBool().ShouldBe(result);
        }

        [Fact]
        public void ObjectToBoolTest()
        {
            object val = new { value = true };
            val.ToBool().ShouldBeFalse();

            val = new ToStringTrue();
            val.ToBool().ShouldBeTrue();
        }

        class ToStringTrue
        {
            public override string ToString()
            {
                return "True";
            }
        }

        [Theory]
        [InlineData("0", 0)]
        [InlineData("0.00", 0.00)]
        [InlineData("12.00", 12.00)]
        [InlineData("$34.00", 34.00)]
        [InlineData("   56.00", 56.00)]
        [InlineData("78.90", 78.90)]
        [InlineData("123.456", 123.456)]
        [InlineData("-789.00", -789.00)]
        [InlineData("-$0.50", -0.50)]
        [InlineData("$-1.50", -1.50)]
        [InlineData("250.00 USD", 250.00)]
        public void GetAmountTest(string value, decimal expected)
        {
            value.GetAmount().ShouldBe(expected);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void GetAmountNullTest(string value)
        {
            value.GetAmount().ShouldBeNull();
        }
    }
}
