using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IDeleteSaleService
{
    Task<DeleteSaleResponse> DeleteAsync(
        DeleteSaleRequest request,
        CancellationToken cancellationToken = default);
}
