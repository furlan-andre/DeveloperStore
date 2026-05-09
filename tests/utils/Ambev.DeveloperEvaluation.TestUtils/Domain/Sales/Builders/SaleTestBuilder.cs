using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Bogus;

namespace Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;

public class SaleTestBuilder
{
    private static readonly Faker Faker = new();

    private string? _saleNumber = Faker.Commerce.Ean13();
    private DateTime _saleDate = DateTime.UtcNow;
    private Customer? _customer = ReferenceDataTestBuilder.CreateCustomer();
    private Branch? _branch = ReferenceDataTestBuilder.CreateBranch();
    private IEnumerable<SaleItem?>? _items = [new SaleItemTestBuilder().Build()];

    public SaleTestBuilder WithSaleNumber(string? saleNumber)
    {
        _saleNumber = saleNumber;
        return this;
    }

    public SaleTestBuilder WithSaleDate(DateTime saleDate)
    {
        _saleDate = saleDate;
        return this;
    }

    public SaleTestBuilder WithCustomer(Customer? customer)
    {
        _customer = customer;
        return this;
    }

    public SaleTestBuilder WithBranch(Branch? branch)
    {
        _branch = branch;
        return this;
    }

    public SaleTestBuilder WithItems(IEnumerable<SaleItem?>? items)
    {
        _items = items;
        return this;
    }

    public Sale Build()
    {
        return Sale.Create(_saleNumber, _saleDate, _customer, _branch, _items);
    }
}
