using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Sales.CreateSales;

public class CreateSaleInputTestBuilder
{
    private static readonly Faker Faker = new();

    private string _saleNumber = Faker.Commerce.Ean13();
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private List<CreateSaleItemInput> _items = [new CreateSaleItemInputTestBuilder().Build()];

    public CreateSaleInputTestBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public CreateSaleInputTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public CreateSaleInputTestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public CreateSaleInputTestBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public CreateSaleInputTestBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public CreateSaleInputTestBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public CreateSaleInputTestBuilder WithItems(List<CreateSaleItemInput> items)
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
