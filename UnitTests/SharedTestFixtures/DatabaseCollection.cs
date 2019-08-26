using Xunit;

namespace SharedTestFixtures
{
    [CollectionDefinition(Collections.Database)]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
        // IMPORTANT: You must compile this class into each unit test library
    }
}
