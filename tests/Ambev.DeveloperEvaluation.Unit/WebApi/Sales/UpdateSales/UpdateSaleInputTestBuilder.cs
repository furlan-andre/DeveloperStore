using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Sales.UpdateSales;

public class UpdateSaleInputTestBuilder
{
    private static readonly Faker Faker = new();

    private string _saleNumber = Faker.Commerce.Ean13();
    private DateTime _saleDate = DateTime.UtcNow;
    private Guid _customerId = Guid.NewGuid();
    private string _customerName = Faker.Person.FullName;
    private Guid _branchId = Guid.NewGuid();
    private string _branchName = Faker.Company.CompanyName();
    private bool _active = true;
    private List<UpdateSaleItemInput> _items = [new UpdateSaleItemInputTestBuilder().Build()];

    public UpdateSaleInputTestBuilder WithSaleNumber(string saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public UpdateSaleInputTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public UpdateSaleInputTestBuilder WithCustomerId(Guid customerId)
    {
        _customerId = customerId;
        return this;
    }

    public UpdateSaleInputTestBuilder WithCustomerName(string customerName)
    {
        _customerName = customerName;
        return this;
    }

    public UpdateSaleInputTestBuilder WithBranchId(Guid branchId)
    {
        _branchId = branchId;
        return this;
    }

    public UpdateSaleInputTestBuilder WithBranchName(string branchName)
    {
        _branchName = branchName;
        return this;
    }

    public UpdateSaleInputTestBuilder WithActive(bool active)
    {
        _active = active;
        return this;
    }

    public UpdateSaleInputTestBuilder WithItems(List<UpdateSaleItemInput> items)
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
