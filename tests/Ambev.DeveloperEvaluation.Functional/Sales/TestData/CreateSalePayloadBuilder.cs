using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.Sales.TestData;

public sealed class CreateSalePayloadBuilder
{
    private static readonly Faker Faker = new();

    private string _saleNumber = $"SALE-{Guid.NewGuid():N}";
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private List<CreateSaleItemInput> _items = [new CreateSaleItemPayloadBuilder().Build()];

    public CreateSalePayloadBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public CreateSalePayloadBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public CreateSalePayloadBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public CreateSalePayloadBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public CreateSalePayloadBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public CreateSalePayloadBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public CreateSalePayloadBuilder WithItems(List<CreateSaleItemInput> items)
    {
        _items = items;
        return this;
    }

    public CreateSaleInput Build()
    {
        return new CreateSaleInput
        {
            SaleNumber = _saleNumber,
            SaleDate = _saleDate,
            CustomerId = _customerId,
            CustomerName = _customerName,
            BranchId = _branchId,
            BranchName = _branchName,
            Items = _items
        };
    }
}
