using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class CreateSaleCommandTestBuilder
{
    private static readonly Faker Faker = new();

    private string _saleNumber = Faker.Commerce.Ean13();
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private IReadOnlyCollection<CreateSaleItemCommand> _items = [new CreateSaleItemCommandTestBuilder().Build()];

    public CreateSaleCommandTestBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public CreateSaleCommandTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public CreateSaleCommandTestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public CreateSaleCommandTestBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public CreateSaleCommandTestBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public CreateSaleCommandTestBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public CreateSaleCommandTestBuilder WithItems(IReadOnlyCollection<CreateSaleItemCommand> items)
    {
        _items = items;
        return this;
    }

    public CreateSaleCommand Build()
    {
        return new CreateSaleCommand
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
