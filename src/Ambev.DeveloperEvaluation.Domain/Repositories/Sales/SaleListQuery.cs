namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;

public sealed class SaleListQuery
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? Order { get; init; }
    public IReadOnlyDictionary<string, string?> Filters { get; init; } = new Dictionary<string, string?>();
}
