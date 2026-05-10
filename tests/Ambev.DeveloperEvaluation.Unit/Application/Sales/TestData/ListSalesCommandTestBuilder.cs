using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class ListSalesCommandTestBuilder
{
    private int _page = 1;
    private int _size = 10;
    private string? _order;
    private IReadOnlyDictionary<string, string?> _filters = new Dictionary<string, string?>();

    public ListSalesCommandTestBuilder WithPage(int page)
    {
        _page = page;
        return this;
    }

    public ListSalesCommandTestBuilder WithSize(int size)
    {
        _size = size;
        return this;
    }

    public ListSalesCommandTestBuilder WithOrder(string? order)
    {
        _order = order;
        return this;
    }

    public ListSalesCommandTestBuilder WithFilters(IReadOnlyDictionary<string, string?> filters)
    {
        _filters = filters;
        return this;
    }

    public ListSalesCommand Build()
    {
        return new ListSalesCommand
        {
            Page = _page,
            Size = _size,
            Order = _order,
            Filters = _filters
        };
    }
}
