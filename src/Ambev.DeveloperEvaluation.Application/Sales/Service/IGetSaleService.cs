using Ambev.DeveloperEvaluation.Application.Sales.GetSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IGetSaleService
{
    Task<GetSaleResponse> GetByIdAsync(
        GetSaleRequest request,
        CancellationToken cancellationToken = default);
}
