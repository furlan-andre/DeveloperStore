namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public interface ICreateSaleService
{
    Task<CreateSaleResult> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken = default);
}
