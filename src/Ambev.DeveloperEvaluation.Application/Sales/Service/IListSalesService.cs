using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Common.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IListSalesService
{
    Task<Result<PagedResponse<ListSaleResponse>>> ListAsync(
        ListSalesRequest request,
        CancellationToken cancellationToken = default);
}
