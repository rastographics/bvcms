using UtilityExtensions;

namespace SharedTestFixtures
{
    public class ContextTestUtils
    {
        public static MockHttpContext CreateMockHttpContext(bool isAuthenticated = true)
        {
            var mock = new MockHttpContext(isAuthenticated);
            HttpContextFactory.SetCurrentContext(mock.Object);
            return mock;
        }
    }
}
