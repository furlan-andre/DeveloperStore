using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleService : ICreateSaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleDiscountPolicy _discountPolicy;
    private readonly IMapper _mapper;

    public CreateSaleService(
        ISaleRepository saleRepository,
        ISaleDiscountPolicy discountPolicy,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _discountPolicy = discountPolicy;
        _mapper = mapper;
    }

    public async Task<CreateSaleResult> CreateAsync(
        CreateSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var customer = new Customer(request.CustomerId, request.CustomerName);
        var branch = new Branch(request.BranchId, request.BranchName);
        var items = request.Items?.Select(item => item is null ? null : CreateSaleItem(item)).ToList();

        var sale = Sale.Create(
            request.SaleNumber,
            request.SaleDate,
            customer,
            branch,
            items);

        await _saleRepository.AddAsync(sale, cancellationToken);

        return _mapper.Map<CreateSaleResult>(sale);
    }

    private SaleItem CreateSaleItem(CreateSaleItemRequest item)
    {
        var product = new Product(item.ProductId, item.ProductDescription);
        return new SaleItem(product, item.Quantity, item.UnitPrice, _discountPolicy);
    }
}
