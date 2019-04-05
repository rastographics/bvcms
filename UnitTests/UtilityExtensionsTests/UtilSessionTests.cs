using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UtilityExtensions;
using Xunit;

namespace UtilityExtensionsTests
{
    [Collection("Util Session Tests")]
    public class UtilSessionTests
    {
        public UtilSessionTests()
        {
            var mockContext = new Mock<HttpContextBase>();
            var mockSession = new MockHttpSessionState(MockSessionDictionary);
            mockContext.Setup(c => c.Session).Returns(mockSession);
            HttpContextFactory.SetCurrentContext(mockContext.Object);
        }

        public Dictionary<string, object> MockSessionDictionary = new Dictionary<string, object>();

        [Fact]
        public void UtilUserIdTest()
        {
            var actual = Util.UserId;
            var expected = 0;

            Assert.Equal(expected, actual);
            
            Util.UserId = 12;

            actual = Util.UserId;
            expected = 12;

            Assert.Equal(expected, actual);
            
            HttpContextFactory.SetCurrentContext(null);

            actual = Util.UserId;
            expected = 0;

            Assert.Equal(expected, actual);
        }
    }
}
