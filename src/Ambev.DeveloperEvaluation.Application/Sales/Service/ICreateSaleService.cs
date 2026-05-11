using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Common.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface ICreateSaleService
{
    Task<Result<CreateSaleResponse>> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken = default);
}
