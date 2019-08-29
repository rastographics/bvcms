using UtilityExtensions;

namespace SharedTestFixtures
{
    public static class ContextTestUtils
    {
        public static MockHttpContext CreateMockHttpContext()
        {
            var mock = new MockHttpContext();
            HttpContextFactory.SetCurrentContext(mock.Object);
            return mock;
        }
    }
}
