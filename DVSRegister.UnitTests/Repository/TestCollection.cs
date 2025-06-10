namespace DVSRegister.UnitTests.Repository
{
    [CollectionDefinition("Postgres Collection")]
    public class TestCollection : ICollectionFixture<PostgresTestFixture>
    {
        // This class has no code, Itss purpose is 
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
