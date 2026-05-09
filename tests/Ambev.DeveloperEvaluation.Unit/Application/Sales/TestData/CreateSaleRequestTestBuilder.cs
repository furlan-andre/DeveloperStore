using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class CreateSaleRequestTestBuilder
{
    private static readonly Faker Faker = new();

    private string _saleNumber = Faker.Commerce.Ean13();
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private IReadOnlyCollection<CreateSaleItemRequest> _items = [new CreateSaleItemRequestTestBuilder().Build()];

    public CreateSaleRequestTestBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public CreateSaleRequestTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public CreateSaleRequestTestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public CreateSaleRequestTestBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public CreateSaleRequestTestBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public CreateSaleRequestTestBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public CreateSaleRequestTestBuilder WithItems(IReadOnlyCollection<CreateSaleItemRequest> items)
    {
        _items = items;
        return this;
    }

    public CreateSaleRequest Build()
    {
        return new CreateSaleRequest
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
