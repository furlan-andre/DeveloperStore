using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Common.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IGetSaleService
{
    Task<Result<GetSaleResponse>> GetByIdAsync(
        GetSaleRequest request,
        CancellationToken cancellationToken = default);
}
