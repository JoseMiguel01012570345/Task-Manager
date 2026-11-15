namespace TaskManagement.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<ClusterApi>
{
    public const string Name = "api";
}
