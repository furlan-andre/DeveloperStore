using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM.Sales.Testcontainers;

[CollectionDefinition(nameof(PostgreSqlTestContainerCollection))]
public sealed class PostgreSqlTestContainerCollection : ICollectionFixture<PostgreSqlTestContainerFixture>
{
}
