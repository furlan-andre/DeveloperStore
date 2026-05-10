using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface ICreateSaleService
{
    Task<CreateSaleResponse> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken = default);
}
