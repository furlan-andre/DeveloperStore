using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Common;

[CollectionDefinition(nameof(FunctionalTestCollection))]
public sealed class FunctionalTestCollection : ICollectionFixture<PostgreSqlFunctionalFixture>
{
}
