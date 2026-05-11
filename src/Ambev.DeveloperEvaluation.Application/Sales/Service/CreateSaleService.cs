using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.Service;

public class CreateSaleService : ICreateSaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleDiscountPolicy _discountPolicy;
    private readonly IMapper _mapper;
    private readonly ISalesEventPublisher _salesEventPublisher;

    public CreateSaleService(
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

    public async Task<Result<CreateSaleResponse>> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        Sale sale;

        try
        {
            var customer = new Customer(request.CustomerId, request.CustomerName);
            var branch = new Branch(request.BranchId, request.BranchName);
            var items = request.Items?.Select(item => item is null ? null : CreateSaleItem(item)).ToList();

            sale = Sale.Create(
                request.SaleNumber,
                request.SaleDate,
                customer,
                branch,
                items);
        }
        catch (DomainException exception)
        {
            return Result<CreateSaleResponse>.Failure(
                Error.DomainRuleViolation("Sale domain rule violated", exception.Message));
        }

        await _saleRepository.AddAsync(sale, cancellationToken);
        await _salesEventPublisher.PublishAsync(CreateSaleCreatedEvent(sale), cancellationToken);

        return Result<CreateSaleResponse>.Success(_mapper.Map<CreateSaleResponse>(sale));
    }

    private SaleItem CreateSaleItem(CreateSaleItemRequest item)
    {
        var product = new Product(item.ProductId, item.ProductDescription);
        return new SaleItem(product, item.Quantity, item.UnitPrice, _discountPolicy);
    }

    private static SaleCreatedEvent CreateSaleCreatedEvent(Sale sale)
    {
        return new SaleCreatedEvent(
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
}
