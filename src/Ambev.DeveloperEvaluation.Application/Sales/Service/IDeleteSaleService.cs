using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Common.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IDeleteSaleService
{
    Task<Result<DeleteSaleResponse>> DeleteAsync(
        DeleteSaleRequest request,
        CancellationToken cancellationToken = default);
}
