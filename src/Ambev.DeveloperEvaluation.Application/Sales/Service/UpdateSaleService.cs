using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public class UpdateSaleService : IUpdateSaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleDiscountPolicy _discountPolicy;
    private readonly IMapper _mapper;
    private readonly ISalesEventPublisher _salesEventPublisher;

    public UpdateSaleService(
        ISaleRepository saleRepository,
        ISaleDiscountPolicy discountPolicy,
        IMapper mapper,
        ISalesEventPublisher salesEventPublisher)
    {
        _saleRepository = saleRepository;
        _discountPolicy = discountPolicy;
        _mapper = mapper;
        _salesEventPublisher = salesEventPublisher;
    }

    public async Task<UpdateSaleResponse> UpdateAsync(
        UpdateSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id {request.Id} was not found.");

        var wasSaleActive = sale.Active;
        var previousItems = sale.Items.ToDictionary(item => item.Id, CreateSaleItemSnapshot);
        var customer = new Customer(request.CustomerId, request.CustomerName);
        var branch = new Branch(request.BranchId, request.BranchName);
        var items = CreateSaleItemUpdateData(request.Items);

        sale.Update(
            request.SaleNumber,
            request.SaleDate,
            customer,
            branch,
            request.Active,
            items);

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await PublishEventsAsync(sale, wasSaleActive, previousItems, cancellationToken);

        return _mapper.Map<UpdateSaleResponse>(sale);
    }

    private List<SaleItemUpdateData?>? CreateSaleItemUpdateData(IEnumerable<UpdateSaleItemRequest?>? items)
    {
        return items?.Select(item => item is null ? null : CreateSaleItemUpdateData(item)).ToList();
    }

    private SaleItemUpdateData CreateSaleItemUpdateData(UpdateSaleItemRequest item)
    {
        var product = new Product(item.ProductId, item.ProductDescription);
        var saleItem = new SaleItem(product, item.Quantity, item.UnitPrice, _discountPolicy);

        return new SaleItemUpdateData(item.Id, saleItem, item.Active);
    }

    private async Task PublishEventsAsync(
        Sale sale,
        bool wasSaleActive,
        IReadOnlyDictionary<Guid, SaleItemSnapshot> previousItems,
        CancellationToken cancellationToken)
    {
        if (wasSaleActive && !sale.Active)
        {
            await _salesEventPublisher.PublishAsync(CreateSaleCancelledEvent(sale), cancellationToken);
        }
        else if (sale.Active)
        {
            await _salesEventPublisher.PublishAsync(CreateSaleModifiedEvent(sale), cancellationToken);
        }

        var cancelledItems = sale.Items
            .Where(item =>
                previousItems.TryGetValue(item.Id, out var previousItem) &&
                previousItem.Active &&
                !item.Active);

        foreach (var item in cancelledItems)
        {
            await _salesEventPublisher.PublishAsync(CreateItemCancelledEvent(sale, item), cancellationToken);
        }
    }

    private static SaleModifiedEvent CreateSaleModifiedEvent(Sale sale)
    {
        return new SaleModifiedEvent(
            sale.Id,
            sale.SaleNumber,
            sale.Customer.Id,
            sale.Customer.Name,
            sale.Branch.Id,
            sale.Branch.Name,
            sale.TotalSaleAmount,
            sale.Active,
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static SaleCancelledEvent CreateSaleCancelledEvent(Sale sale)
    {
        return new SaleCancelledEvent(
            sale.Id,
            sale.SaleNumber,
            DateTime.UtcNow,
            "Sale was cancelled by update.",
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static ItemCancelledEvent CreateItemCancelledEvent(Sale sale, SaleItem item)
    {
        return new ItemCancelledEvent(
            sale.Id,
            sale.SaleNumber,
            item.Id,
            item.Product.Id,
            item.Product.Description,
            item.Quantity,
            item.UnitPrice,
            item.Discount,
            item.TotalAmount,
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static SaleItemSnapshot CreateSaleItemSnapshot(SaleItem item)
    {
        return new SaleItemSnapshot(item.Active);
    }

    private sealed record SaleItemSnapshot(bool Active);
}
