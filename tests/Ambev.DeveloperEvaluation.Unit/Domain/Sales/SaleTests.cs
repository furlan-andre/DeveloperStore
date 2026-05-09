using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Sales;

public class SaleTests
{
    [Fact(DisplayName = "Should create valid sale")]
    public void Given_ValidData_When_CreatingSale_Then_ShouldCreateSale()
    {
        var sale = new SaleTestBuilder().Build();

        Assert.NotNull(sale);
    }

    [Fact(DisplayName = "Should generate sale id")]
    public void Given_ValidData_When_CreatingSale_Then_ShouldGenerateId()
    {
        var sale = new SaleTestBuilder().Build();

        Assert.NotEqual(Guid.Empty, sale.Id);
    }

    [Fact(DisplayName = "Should keep sale number")]
    public void Given_SaleNumber_When_CreatingSale_Then_ShouldKeepSaleNumber()
    {
        var saleNumber = "SALE-001";

        var sale = new SaleTestBuilder()
            .WithSaleNumber(saleNumber)
            .Build();

        Assert.Equal(saleNumber, sale.SaleNumber);
    }

    [Fact(DisplayName = "Should keep sale date")]
    public void Given_SaleDate_When_CreatingSale_Then_ShouldKeepSaleDate()
    {
        var saleDate = DateTime.UtcNow;

        var sale = new SaleTestBuilder()
            .WithSaleDate(saleDate)
            .Build();

        Assert.Equal(saleDate, sale.SaleDate);
    }

    [Fact(DisplayName = "Should keep customer data")]
    public void Given_Customer_When_CreatingSale_Then_ShouldKeepCustomerData()
    {
        var customer = ReferenceDataTestBuilder.CreateCustomer();

        var sale = new SaleTestBuilder()
            .WithCustomer(customer)
            .Build();

        Assert.Equal(customer.Id, sale.Customer.Id);
        Assert.Equal(customer.Name, sale.Customer.Name);
    }

    [Fact(DisplayName = "Should keep branch data")]
    public void Given_Branch_When_CreatingSale_Then_ShouldKeepBranchData()
    {
        var branch = ReferenceDataTestBuilder.CreateBranch();

        var sale = new SaleTestBuilder()
            .WithBranch(branch)
            .Build();

        Assert.Equal(branch.Id, sale.Branch.Id);
        Assert.Equal(branch.Name, sale.Branch.Name);
    }

    [Fact(DisplayName = "Should keep informed items")]
    public void Given_Items_When_CreatingSale_Then_ShouldKeepItems()
    {
        var item = new SaleItemTestBuilder().Build();
        SaleItem?[] items = [item];

        var sale = new SaleTestBuilder()
            .WithItems(items)
            .Build();

        Assert.Single(sale.Items);
        Assert.Contains(item, sale.Items);
    }

    [Fact(DisplayName = "Should start active")]
    public void Given_ValidData_When_CreatingSale_Then_ShouldStartActive()
    {
        var sale = new SaleTestBuilder().Build();

        Assert.True(sale.Active);
    }

    [Fact(DisplayName = "Should calculate total sale amount")]
    public void Given_Items_When_CreatingSale_Then_ShouldCalculateTotalSaleAmount()
    {
        var firstItemQuantity = 1;
        var secondItemQuantity = 4;
        var unitPrice = 100m;
        var expectedTotalSaleAmount = 460m;
        
        var firstItem = new SaleItemTestBuilder()
            .WithQuantity(firstItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var secondItem = new SaleItemTestBuilder()
            .WithQuantity(secondItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        SaleItem?[] items = [firstItem, secondItem];

        var sale = new SaleTestBuilder()
            .WithItems(items)
            .Build();

        Assert.Equal(expectedTotalSaleAmount, sale.TotalSaleAmount);
    }

    [Fact(DisplayName = "Should protect items collection from direct external changes")]
    public void Given_Sale_When_ModifyingExposedItemsCollection_Then_ShouldNotAllowChanges()
    {
        var sale = new SaleTestBuilder().Build();
        var item = new SaleItemTestBuilder().Build();

        var action = () => ((ICollection<SaleItem>)sale.Items).Add(item);

        Assert.Throws<NotSupportedException>(action);
    }

    [Theory(DisplayName = "Should throw DomainException when sale number is invalid")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidSaleNumber_When_CreatingSale_Then_ShouldThrowDomainException(string? saleNumber)
    {
        var action = () => new SaleTestBuilder()
            .WithSaleNumber(saleNumber)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when sale date is default")]
    public void Given_DefaultSaleDate_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var saleDate = default(DateTime);

        var action = () => new SaleTestBuilder()
            .WithSaleDate(saleDate)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when customer is null")]
    public void Given_NullCustomer_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        Customer? customer = null;

        var action = () => new SaleTestBuilder()
            .WithCustomer(customer)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when branch is null")]
    public void Given_NullBranch_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        Branch? branch = null;

        var action = () => new SaleTestBuilder()
            .WithBranch(branch)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when items is null")]
    public void Given_NullItems_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        IEnumerable<SaleItem?>? items = null;

        var action = () => new SaleTestBuilder()
            .WithItems(items)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when items is empty")]
    public void Given_EmptyItems_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        SaleItem?[] items = [];

        var action = () => new SaleTestBuilder()
            .WithItems(items)
            .Build();

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when items contains null")]
    public void Given_ItemsWithNullValue_When_CreatingSale_Then_ShouldThrowDomainException()
    {
        var item = new SaleItemTestBuilder().Build();
        SaleItem?[] items = [item, null];

        var action = () => new SaleTestBuilder()
            .WithItems(items)
            .Build();

        Assert.Throws<DomainException>(action);
    }
}
