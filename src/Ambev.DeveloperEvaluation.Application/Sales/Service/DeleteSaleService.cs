using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public class DeleteSaleService : IDeleteSaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ISalesEventPublisher _salesEventPublisher;

    public DeleteSaleService(
        ISaleRepository saleRepository,
        IMapper mapper,
        ISalesEventPublisher salesEventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _salesEventPublisher = salesEventPublisher;
    }

    public async Task<Result<DeleteSaleResponse>> DeleteAsync(
        DeleteSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale is null)
        {
            return Result<DeleteSaleResponse>.Failure(CreateSaleNotFoundError(request.Id));
        }

        var wasSaleActive = sale.Active;
        sale.Delete();

        await _saleRepository.DeleteAsync(sale, cancellationToken);

        if (wasSaleActive && !sale.Active)
            await _salesEventPublisher.PublishAsync(CreateSaleCancelledEvent(sale), cancellationToken);

        return Result<DeleteSaleResponse>.Success(_mapper.Map<DeleteSaleResponse>(sale));
    }

    private static Error CreateSaleNotFoundError(Guid saleId)
    {
        return Error.ResourceNotFound(
            "Sale not found",
            $"The sale with ID {saleId} does not exist.");
    }

    private static SaleCancelledEvent CreateSaleCancelledEvent(Sale sale)
    {
        return new SaleCancelledEvent(
            sale.Id,
            sale.SaleNumber,
            DateTime.UtcNow,
            "Sale was cancelled by delete.",
            Guid.NewGuid(),
            DateTime.UtcNow);
    }
}
