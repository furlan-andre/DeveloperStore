using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Sales.TestData;

public sealed class UpdateSalePayloadBuilder
{
    private static readonly Faker Faker = new();

    private string _saleNumber = $"SALE-UPD-{Guid.NewGuid():N}";
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private bool _active = true;
    private List<UpdateSaleItemInput> _items = [new UpdateSaleItemPayloadBuilder().Build()];

    public UpdateSalePayloadBuilder FromCreatePayload(CreateSaleInput input, Guid? itemId = null)
    {
        _saleNumber = input.SaleNumber;
        _saleDate = input.SaleDate;
        _customerId = input.CustomerId;
        _customerName = input.CustomerName;
        _branchId = input.BranchId;
        _branchName = input.BranchName;
        _active = true;
        _items = input.Items
            .Select(item => new UpdateSaleItemPayloadBuilder()
                .FromCreateItem(item)
                .WithId(itemId)
                .Build())
            .ToList();

        return this;
    }

    public UpdateSalePayloadBuilder FromGetResult(GetSaleResult result)
    {
        _saleNumber = result.SaleNumber;
        _saleDate = result.SaleDate;
        _customerId = result.CustomerId;
        _customerName = result.CustomerName;
        _branchId = result.BranchId;
        _branchName = result.BranchName;
        _active = result.Active;
        _items = result.Items
            .Select(item => new UpdateSaleItemPayloadBuilder()
                .FromGetItem(item)
                .Build())
            .ToList();

        return this;
    }

    public UpdateSalePayloadBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public UpdateSalePayloadBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public UpdateSalePayloadBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public UpdateSalePayloadBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSalePayloadBuilder WithItems(List<UpdateSaleItemInput> items)
    {
        _items = items;
        return this;
    }

    public UpdateSaleInput Build()
    {
        return new UpdateSaleInput
        {
            SaleNumber = _saleNumber,
            SaleDate = _saleDate,
            CustomerId = _customerId,
            CustomerName = _customerName,
            BranchId = _branchId,
            BranchName = _branchName,
            Active = _active,
            Items = _items
        };
    }
}
