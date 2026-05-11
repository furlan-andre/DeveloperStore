using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Results;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public interface IUpdateSaleService
{
    Task<Result<UpdateSaleResponse>> UpdateAsync(
        UpdateSaleRequest request,
        CancellationToken cancellationToken = default);
}
