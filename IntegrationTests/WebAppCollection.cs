using Xunit;

namespace IntegrationTests
{
    [CollectionDefinition("WebApp Collection")]
    public class WebAppCollection : ICollectionFixture<WebAppFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
