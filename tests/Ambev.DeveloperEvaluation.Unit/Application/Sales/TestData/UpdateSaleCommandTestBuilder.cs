using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class UpdateSaleCommandTestBuilder
{
    private static readonly Faker Faker = new();

    private Guid _id = Guid.NewGuid();
    private string _saleNumber = Faker.Commerce.Ean13();
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private bool _active = true;
    private IReadOnlyCollection<UpdateSaleItemCommand> _items = [new UpdateSaleItemCommandTestBuilder().Build()];

    public UpdateSaleCommandTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSaleCommandTestBuilder WithItems(IReadOnlyCollection<UpdateSaleItemCommand> items)
    {
        _items = items;
        return this;
    }

    public UpdateSaleCommand Build()
    {
        return new UpdateSaleCommand
        {
            Id = _id,
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
