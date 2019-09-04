using UtilityExtensions;

namespace SharedTestFixtures
{
    public class ContextTestUtils
    {
        public static MockHttpContext CreateMockHttpContext()
        {
            var mock = new MockHttpContext();
            HttpContextFactory.SetCurrentContext(mock.Object);
            return mock;
        }
    }
}
