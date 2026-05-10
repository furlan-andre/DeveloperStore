namespace Ambev.DeveloperEvaluation.Domain.Repositories.Sales;

public sealed class PagedResult<T>
{
    public IReadOnlyCollection<T> Items { get; init; } = [];
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int TotalItems { get; init; }
    public int PageSize { get; init; }
}
