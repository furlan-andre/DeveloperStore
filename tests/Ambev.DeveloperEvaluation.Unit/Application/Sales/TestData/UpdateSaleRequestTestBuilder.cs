using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public class UpdateSaleRequestTestBuilder
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
    private IReadOnlyCollection<UpdateSaleItemRequest> _items = [new UpdateSaleItemRequestTestBuilder().Build()];

    public UpdateSaleRequestTestBuilder WithId(Guid id)
    {
        _id = id;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSaleRequestTestBuilder WithItems(IReadOnlyCollection<UpdateSaleItemRequest> items)
    {
        _items = items;
        return this;
    }

    public UpdateSaleRequest Build()
    {
        return new UpdateSaleRequest
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
