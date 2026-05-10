using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IListSalesService
{
    Task<PagedResponse<ListSaleResponse>> ListAsync(
        ListSalesRequest request,
        CancellationToken cancellationToken = default);
}
