using System;
using UtilityExtensions;
using Xunit;

namespace UtilityExtensionsTests
{
    [Collection("ConnectionString Tests")]
    public class UtilConnectionStringsTests
    {
        [Fact]
        public void GetConnectionStringTest()
        {
            var actual = Util.GetConnectionString("mytesthost");
            var expected = "Data Source=(local);Initial Catalog=CMS_mytesthost;Integrated Security=True";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetConnectionStringFromUrlTest()
        {
            var actual = Util.GetConnectionString("mytest.tpsdb.com");
            var expected = "Data Source=(local);Initial Catalog=CMS_mytest;Integrated Security=True";

            Assert.Equal(expected, actual);
        }
    }
}
