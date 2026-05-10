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

    [Fact(DisplayName = "Should calculate total sale amount using only active items")]
    public void Given_InactiveItems_When_CreatingSale_Then_ShouldCalculateTotalSaleAmountUsingOnlyActiveItems()
    {
        var unitPrice = 100m;
        
        var activeItemQuantity = 4;
        var inactiveItemQuantity = 10;
        
        var expectedTotalSaleAmount = 360m;

        var activeItem = new SaleItemTestBuilder()
            .WithQuantity(activeItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();

        var inactiveItem = new SaleItemTestBuilder()
            .WithQuantity(inactiveItemQuantity)
            .WithUnitPrice(unitPrice)
            .WithActive(false)
            .Build();
        SaleItem?[] items = [activeItem, inactiveItem];

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

    [Fact(DisplayName = "Should update sale data")]
    public void Given_ValidData_When_UpdatingSale_Then_ShouldUpdateSaleData()
    {
        var sale = new SaleTestBuilder().Build();
        var saleNumber = "SALE-UPDATED";
        var saleDate = DateTime.UtcNow.AddDays(1);
        var customer = ReferenceDataTestBuilder.CreateCustomer();
        var branch = ReferenceDataTestBuilder.CreateBranch();
        var active = true;
        var item = new SaleItemTestBuilder().Build();
        SaleItemUpdateData?[] items = [new(null, item)];

        sale.Update(saleNumber, saleDate, customer, branch, active, items);

        Assert.Equal(saleNumber, sale.SaleNumber);
        Assert.Equal(saleDate, sale.SaleDate);
        Assert.Equal(customer.Id, sale.Customer.Id);
        Assert.Equal(customer.Name, sale.Customer.Name);
        Assert.Equal(branch.Id, sale.Branch.Id);
        Assert.Equal(branch.Name, sale.Branch.Name);
        Assert.True(sale.Active);
    }

    [Fact(DisplayName = "Should update active flag without removing items")]
    public void Given_InactiveUpdate_When_UpdatingSale_Then_ShouldInactivateSaleAndKeepItems()
    {
        var existingItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();
        var active = false;
        
        SaleItemUpdateData?[] items = [new(existingItem.Id, existingItem)];

        sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, active, items);

        Assert.False(sale.Active);
        Assert.Single(sale.Items);
        Assert.Contains(existingItem, sale.Items);
    }

    [Fact(DisplayName = "Should update existing sale item by id")]
    public void Given_ExistingItemId_When_UpdatingSale_Then_ShouldUpdateItemAndPreserveId()
    {
        var existingItem = new SaleItemTestBuilder()
            .WithQuantity(1)
            .WithUnitPrice(100m)
            .Build();
        
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();
        
        var originalItemId = existingItem.Id;
        var updatedItem = new SaleItemTestBuilder()
            .WithQuantity(4)
            .WithUnitPrice(100m)
            .Build();
        
        var expectedDiscount = 40m;
        var expectedTotalAmount = 360m;
        SaleItemUpdateData?[] items = [new(originalItemId, updatedItem)];

        sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        var saleItem = sale.Items.Single();
        
        Assert.Equal(originalItemId, saleItem.Id);
        Assert.Equal(updatedItem.Product.Id, saleItem.Product.Id);
        Assert.Equal(updatedItem.Product.Description, saleItem.Product.Description);
        Assert.Equal(updatedItem.Quantity, saleItem.Quantity);
        Assert.Equal(updatedItem.UnitPrice, saleItem.UnitPrice);
        Assert.Equal(expectedDiscount, saleItem.Discount);
        Assert.Equal(expectedTotalAmount, saleItem.TotalAmount);
        Assert.Equal(expectedTotalAmount, sale.TotalSaleAmount);
    }

    [Fact(DisplayName = "Should inactivate existing sale item and exclude it from total")]
    public void Given_InactiveExistingItem_When_UpdatingSale_Then_ShouldInactivateItemAndExcludeItFromTotal()
    {
        var activeItemQuantity = 4;
        var inactiveItemQuantity = 10;
        var unitPrice = 100m;
        var inactive = false;
        var expectedTotalSaleAmount = 360m;
        
        var activeItem = new SaleItemTestBuilder()
            .WithQuantity(activeItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var itemToInactivate = new SaleItemTestBuilder()
            .WithQuantity(inactiveItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var sale = new SaleTestBuilder()
            .WithItems([activeItem, itemToInactivate])
            .Build();

        SaleItemUpdateData?[] items =
        [
            new(activeItem.Id, activeItem),
            new(itemToInactivate.Id, itemToInactivate, inactive)
        ];

        sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        var inactiveItem = sale.Items.Single(item => item.Id == itemToInactivate.Id);
        
        Assert.False(inactiveItem.Active);
        Assert.Equal(expectedTotalSaleAmount, sale.TotalSaleAmount);
    }

    [Fact(DisplayName = "Should add new item when item id is empty")]
    public void Given_EmptyItemId_When_UpdatingSale_Then_ShouldAddNewItem()
    {
        var existingItem = new SaleItemTestBuilder().Build();
        var newItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();
        
        SaleItemUpdateData?[] items =
        [
            new(existingItem.Id, existingItem),
            new(Guid.Empty, newItem)
        ];

        sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        Assert.Equal(2, sale.Items.Count);
        Assert.Contains(existingItem, sale.Items);
        Assert.Contains(newItem, sale.Items);
    }

    [Fact(DisplayName = "Should add inactive new item and exclude it from total")]
    public void Given_InactiveNewItem_When_UpdatingSale_Then_ShouldAddInactiveItemAndExcludeItFromTotal()
    {
        var existingItemQuantity = 4;
        var newItemQuantity = 10;
        var unitPrice = 100m;
        var inactive = false;
        var expectedTotalSaleAmount = 360m;
        
        var existingItem = new SaleItemTestBuilder()
            .WithQuantity(existingItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var newItem = new SaleItemTestBuilder()
            .WithQuantity(newItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();
        
        SaleItemUpdateData?[] items =
        [
            new(existingItem.Id, existingItem),
            new(Guid.Empty, newItem, inactive)
        ];

        sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        var inactiveItem = sale.Items.Single(item => item.Id == newItem.Id);
        
        Assert.False(inactiveItem.Active);
        Assert.Equal(expectedTotalSaleAmount, sale.TotalSaleAmount);
    }

    [Fact(DisplayName = "Should remove old item when item is absent from update")]
    public void Given_MissingExistingItem_When_UpdatingSale_Then_ShouldRemoveItem()
    {
        var keptItem = new SaleItemTestBuilder().Build();
        var removedItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([keptItem, removedItem])
            .Build();
        
        SaleItemUpdateData?[] items = [new(keptItem.Id, keptItem)];

        sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        Assert.Single(sale.Items);
        Assert.Contains(keptItem, sale.Items);
        Assert.DoesNotContain(removedItem, sale.Items);
    }

    [Fact(DisplayName = "Should throw DomainException when item id does not belong to sale")]
    public void Given_ItemIdFromAnotherSale_When_UpdatingSale_Then_ShouldThrowDomainException()
    {
        var sale = new SaleTestBuilder().Build();
        var anotherItemId = Guid.NewGuid();
        var item = new SaleItemTestBuilder().Build();
        
        SaleItemUpdateData?[] items = [new(anotherItemId, item)];

        var action = () => sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        Assert.Throws<DomainException>(action);
    }

    [Fact(DisplayName = "Should throw DomainException when update items contains null")]
    public void Given_NullUpdateItem_When_UpdatingSale_Then_ShouldThrowDomainException()
    {
        var sale = new SaleTestBuilder().Build();
        SaleItemUpdateData?[] items = [null];

        var action = () => sale.Update(sale.SaleNumber, sale.SaleDate, sale.Customer, sale.Branch, sale.Active, items);

        Assert.Throws<DomainException>(action);
    }
}
