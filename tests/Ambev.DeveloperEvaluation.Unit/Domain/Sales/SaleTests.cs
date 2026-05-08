using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class SaleTests
{
    private static readonly Customer Customer = new(Guid.NewGuid(), "John Doe");
    private static readonly Branch Branch = new(Guid.NewGuid(), "Main Branch");
    private static readonly Product Product = new(Guid.NewGuid(), "Product description");
    private static readonly SaleDiscountPolicy DiscountPolicy = new();

    [Fact(DisplayName = "Should create valid sale")]
    public void Given_ValidData_When_CreatingSale_Then_ShouldCreateSale()
    {
        var item = CreateItem();

        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [item]);

        Assert.NotNull(sale);
    }

    [Fact(DisplayName = "Should generate sale id")]
    public void Given_ValidData_When_CreatingSale_Then_ShouldGenerateId()
    {
        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        Assert.NotEqual(Guid.Empty, sale.Id);
    }

    [Fact(DisplayName = "Should keep sale number")]
    public void Given_SaleNumber_When_CreatingSale_Then_ShouldKeepSaleNumber()
    {
        var saleNumber = "SALE-001";

        var sale = Sale.Create(saleNumber, DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        Assert.Equal(saleNumber, sale.SaleNumber);
    }

    [Fact(DisplayName = "Should keep sale date")]
    public void Given_SaleDate_When_CreatingSale_Then_ShouldKeepSaleDate()
    {
        var saleDate = DateTime.UtcNow;

        var sale = Sale.Create("SALE-001", saleDate, Customer, Branch, [CreateItem()]);

        Assert.Equal(saleDate, sale.SaleDate);
    }

    [Fact(DisplayName = "Should keep customer data")]
    public void Given_Customer_When_CreatingSale_Then_ShouldKeepCustomerData()
    {
        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        Assert.Equal(Customer.Id, sale.Customer.Id);
        Assert.Equal(Customer.Name, sale.Customer.Name);
    }

    [Fact(DisplayName = "Should keep branch data")]
    public void Given_Branch_When_CreatingSale_Then_ShouldKeepBranchData()
    {
        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        Assert.Equal(Branch.Id, sale.Branch.Id);
        Assert.Equal(Branch.Name, sale.Branch.Name);
    }

    [Fact(DisplayName = "Should keep informed items")]
    public void Given_Items_When_CreatingSale_Then_ShouldKeepItems()
    {
        var item = CreateItem();

        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [item]);

        Assert.Single(sale.Items);
        Assert.Contains(item, sale.Items);
    }

    [Fact(DisplayName = "Should start active")]
    public void Given_ValidData_When_CreatingSale_Then_ShouldStartActive()
    {
        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        Assert.True(sale.Active);
    }

    [Fact(DisplayName = "Should calculate total sale amount")]
    public void Given_Items_When_CreatingSale_Then_ShouldCalculateTotalSaleAmount()
    {
        var firstItem = CreateItem(quantity: 1, unitPrice: 100m);
        var secondItem = CreateItem(quantity: 4, unitPrice: 100m);

        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [firstItem, secondItem]);

        Assert.Equal(460m, sale.TotalSaleAmount);
    }

    [Fact(DisplayName = "Should protect items collection from direct external changes")]
    public void Given_Sale_When_ModifyingExposedItemsCollection_Then_ShouldNotAllowChanges()
    {
        var sale = Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        var action = () => ((ICollection<SaleItem>)sale.Items).Add(CreateItem());

        Assert.Throws<NotSupportedException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when sale number is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidSaleNumber_When_CreatingSale_Then_ShouldThrowDomainException(string? saleNumber)
    {
        var action = () => Sale.Create(saleNumber, DateTime.UtcNow, Customer, Branch, [CreateItem()]);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when sale date is default")]
    public void Given_DefaultSaleDate_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var action = () => Sale.Create("SALE-001", default, Customer, Branch, [CreateItem()]);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when customer is null")]
    public void Given_NullCustomer_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var action = () => Sale.Create("SALE-001", DateTime.UtcNow, null, Branch, [CreateItem()]);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when branch is null")]
    public void Given_NullBranch_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var action = () => Sale.Create("SALE-001", DateTime.UtcNow, Customer, null, [CreateItem()]);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when items is null")]
    public void Given_NullItems_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var action = () => Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, null);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when items is empty")]
    public void Given_EmptyItems_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var action = () => Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, []);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when items contains null")]
    public void Given_ItemsWithNullValue_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var action = () => Sale.Create("SALE-001", DateTime.UtcNow, Customer, Branch, [CreateItem(), null]);

        Assert.Throws<DomainException>(action);
    }

    private static SaleItem CreateItem(int quantity = 1, decimal unitPrice = 100m)
    {
        return new SaleItem(Product, quantity, unitPrice, DiscountPolicy);
    }
}
