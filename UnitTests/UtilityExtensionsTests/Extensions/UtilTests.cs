using Shouldly;
using UtilityExtensions;
using Xunit;

namespace UtilityExtensionsTests
{
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
    }
}
