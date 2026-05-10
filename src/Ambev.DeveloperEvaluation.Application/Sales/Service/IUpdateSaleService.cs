using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IUpdateSaleService
{
    Task<UpdateSaleResponse> UpdateAsync(
        UpdateSaleRequest request,
        CancellationToken cancellationToken = default);
}
