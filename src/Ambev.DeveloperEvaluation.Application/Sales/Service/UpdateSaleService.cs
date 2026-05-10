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

    public UpdateSaleService(
        ISaleRepository saleRepository,
        ISaleDiscountPolicy discountPolicy,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _discountPolicy = discountPolicy;
        _mapper = mapper;
    }

    public async Task<UpdateSaleResponse> UpdateAsync(
        UpdateSaleRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);

        if (sale is null)
            throw new KeyNotFoundException($"Sale with id {request.Id} was not found.");

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
}
