using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class ListSalesRequestTestBuilder
{
    private int _page = 1;
    private int _size = 10;
    private string? _order;
    private IReadOnlyDictionary<string, string?> _filters = new Dictionary<string, string?>();

    public ListSalesRequestTestBuilder WithPage(int page)
    {
        _page = page;
        return this;
    }

    public ListSalesRequestTestBuilder WithSize(int size)
    {
        _size = size;
        return this;
    }

    public ListSalesRequestTestBuilder WithOrder(string? order)
    {
        _order = order;
        return this;
    }

    public ListSalesRequestTestBuilder WithFilters(IReadOnlyDictionary<string, string?> filters)
    {
        _filters = filters;
        return this;
    }

    public ListSalesRequest Build()
    {
        return new ListSalesRequest
        {
            Page = _page,
            Size = _size,
            Order = _order,
            Filters = _filters
        };
    }
}
