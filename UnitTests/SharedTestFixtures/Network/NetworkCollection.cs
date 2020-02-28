using Xunit;

namespace SharedTestFixtures.Network
{
    [CollectionDefinition(Collections.Network)]
    public class NetworkCollection : ICollectionFixture<NetworkFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        // IMPORTANT: You must compile this class into each unit test library
    }
}
