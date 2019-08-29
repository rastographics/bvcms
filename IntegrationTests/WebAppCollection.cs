using SharedTestFixtures;
using Xunit;

namespace IntegrationTests
{
    [CollectionDefinition(Collections.Webapp)]
    public class WebAppCollection : ICollectionFixture<WebAppFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
