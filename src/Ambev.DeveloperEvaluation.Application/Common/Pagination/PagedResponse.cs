namespace Ambev.DeveloperEvaluation.Application.Common.Pagination;

public class PagedResponse<T>
{
    public IReadOnlyCollection<T> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }
}
