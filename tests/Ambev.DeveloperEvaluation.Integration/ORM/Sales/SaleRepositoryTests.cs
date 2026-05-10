using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories.Sales;
using Ambev.DeveloperEvaluation.Integration.ORM.Sales.Testcontainers;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.ORM.Sales;

[Collection(nameof(PostgreSqlTestContainerCollection))]
public class SaleRepositoryTests
{
    private readonly PostgreSqlTestContainerFixture _fixture;

    public SaleRepositoryTests(PostgreSqlTestContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Should persist sale data")]
    public async Task Given_ValidSale_When_AddingSale_Then_ShouldPersistSaleData()
    {
        await _fixture.ResetDatabaseAsync();
        
        var saleNumber = "SALE-001";
        var saleDate = DateTime.UtcNow;
        var customer = ReferenceDataTestBuilder.CreateCustomer();
        var branch = ReferenceDataTestBuilder.CreateBranch();
        
        var sale = new SaleTestBuilder()
            .WithSaleNumber(saleNumber)
            .WithSaleDate(saleDate)
            .WithCustomer(customer)
            .WithBranch(branch)
            .Build();
        
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        await repository.AddAsync(sale);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);
        
        persistedSale.Should().NotBeNull();
        persistedSale!.SaleNumber.Should().Be(saleNumber);
        persistedSale.SaleDate.Should().BeCloseTo(saleDate, TimeSpan.FromMilliseconds(1));
        persistedSale.Customer.Id.Should().Be(customer.Id);
        persistedSale.Customer.Name.Should().Be(customer.Name);
        persistedSale.Branch.Id.Should().Be(branch.Id);
        persistedSale.Branch.Name.Should().Be(branch.Name);
        persistedSale.TotalSaleAmount.Should().Be(sale.TotalSaleAmount);
        persistedSale.Active.Should().BeTrue();
    }

    [Fact(DisplayName = "Should persist sale items data")]
    public async Task Given_ValidSaleWithItems_When_AddingSale_Then_ShouldPersistItemsData()
    {
        await _fixture.ResetDatabaseAsync();
        var product = ReferenceDataTestBuilder.CreateProduct();
        var quantity = 4;
        var unitPrice = 100m;
        var expectedDiscount = 40m;
        var expectedTotalAmount = 360m;
        
        var item = new SaleItemTestBuilder()
            .WithProduct(product)
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();
        SaleItem?[] items = [item];
        
        var sale = new SaleTestBuilder()
            .WithItems(items)
            .Build();
        
        await using var dbContext = _fixture.CreateDbContext();
        
        var repository = new SaleRepository(dbContext);

        await repository.AddAsync(sale);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);
        
        persistedSale.Should().NotBeNull();
        persistedSale!.Items.Should().ContainSingle();
        var persistedItem = persistedSale.Items.Single();
        persistedItem.Id.Should().Be(item.Id);
        persistedItem.Product.Id.Should().Be(product.Id);
        persistedItem.Product.Description.Should().Be(product.Description);
        persistedItem.Quantity.Should().Be(quantity);
        persistedItem.UnitPrice.Should().Be(unitPrice);
        persistedItem.Discount.Should().Be(expectedDiscount);
        persistedItem.TotalAmount.Should().Be(expectedTotalAmount);
        persistedItem.Active.Should().BeTrue();
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when sale is null")]
    public async Task Given_NullSale_When_AddingSale_Then_ShouldThrowArgumentNullException()
    {
        await _fixture.ResetDatabaseAsync();
        Sale? sale = null;
        
        await using var dbContext = _fixture.CreateDbContext();
        
        var repository = new SaleRepository(dbContext);

        var action = async () => await repository.AddAsync(sale!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should persist updated sale data")]
    public async Task Given_ExistingSale_When_UpdatingSale_Then_ShouldPersistSaleData()
    {
        await _fixture.ResetDatabaseAsync();

        var sale = new SaleTestBuilder().Build();
        await AddSaleAsync(sale);

        var saleNumber = "SALE-UPDATED";
        var saleDate = DateTime.UtcNow.AddDays(1);
        var customer = ReferenceDataTestBuilder.CreateCustomer();
        var branch = ReferenceDataTestBuilder.CreateBranch();
        var active = false;

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToUpdate = await repository.GetByIdAsync(sale.Id);
        saleToUpdate.Should().NotBeNull();

        var updateItems = CreateUpdateDataFromExistingItems(saleToUpdate!);
        saleToUpdate!.Update(saleNumber, saleDate, customer, branch, active, updateItems);

        await repository.UpdateAsync(saleToUpdate);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);

        persistedSale.Should().NotBeNull();
        persistedSale!.SaleNumber.Should().Be(saleNumber);
        persistedSale.SaleDate.Should().BeCloseTo(saleDate, TimeSpan.FromMilliseconds(1));
        persistedSale.Customer.Id.Should().Be(customer.Id);
        persistedSale.Customer.Name.Should().Be(customer.Name);
        persistedSale.Branch.Id.Should().Be(branch.Id);
        persistedSale.Branch.Name.Should().Be(branch.Name);
        persistedSale.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should persist updated sale item preserving id")]
    public async Task Given_ExistingSaleItem_When_UpdatingSale_Then_ShouldPersistItemData()
    {
        await _fixture.ResetDatabaseAsync();

        var item = new SaleItemTestBuilder()
            .WithQuantity(1)
            .WithUnitPrice(100m)
            .Build();
        var sale = new SaleTestBuilder()
            .WithItems([item])
            .Build();
        await AddSaleAsync(sale);

        var updatedQuantity = 4;
        var updatedUnitPrice = 100m;
        var expectedDiscount = 40m;
        var expectedTotalAmount = 360m;

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToUpdate = await repository.GetByIdAsync(sale.Id);
        saleToUpdate.Should().NotBeNull();

        var updatedItem = new SaleItemTestBuilder()
            .WithQuantity(updatedQuantity)
            .WithUnitPrice(updatedUnitPrice)
            .Build();
        SaleItemUpdateData?[] updateItems = [new(item.Id, updatedItem)];

        saleToUpdate!.Update(
            saleToUpdate.SaleNumber,
            saleToUpdate.SaleDate,
            saleToUpdate.Customer,
            saleToUpdate.Branch,
            saleToUpdate.Active,
            updateItems);

        await repository.UpdateAsync(saleToUpdate);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);
        var persistedItem = persistedSale!.Items.Single();

        persistedItem.Id.Should().Be(item.Id);
        persistedItem.Quantity.Should().Be(updatedQuantity);
        persistedItem.UnitPrice.Should().Be(updatedUnitPrice);
        persistedItem.Discount.Should().Be(expectedDiscount);
        persistedItem.TotalAmount.Should().Be(expectedTotalAmount);
        persistedSale.TotalSaleAmount.Should().Be(expectedTotalAmount);
    }

    [Fact(DisplayName = "Should persist added sale item")]
    public async Task Given_NewSaleItem_When_UpdatingSale_Then_ShouldPersistAddedItem()
    {
        await _fixture.ResetDatabaseAsync();

        var existingItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();
        await AddSaleAsync(sale);

        var newItemQuantity = 4;
        var unitPrice = 100m;
        var expectedNewItemTotalAmount = 360m;
        var newItem = new SaleItemTestBuilder()
            .WithQuantity(newItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToUpdate = await repository.GetByIdAsync(sale.Id);
        saleToUpdate.Should().NotBeNull();

        SaleItemUpdateData?[] updateItems =
        [
            new(existingItem.Id, saleToUpdate!.Items.Single(item => item.Id == existingItem.Id)),
            new(Guid.Empty, newItem)
        ];

        saleToUpdate.Update(
            saleToUpdate.SaleNumber,
            saleToUpdate.SaleDate,
            saleToUpdate.Customer,
            saleToUpdate.Branch,
            saleToUpdate.Active,
            updateItems);

        await repository.UpdateAsync(saleToUpdate);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);
        var persistedNewItem = persistedSale!.Items.Single(item => item.Id == newItem.Id);

        persistedSale.Items.Should().HaveCount(2);
        persistedNewItem.Quantity.Should().Be(newItemQuantity);
        persistedNewItem.TotalAmount.Should().Be(expectedNewItemTotalAmount);
        persistedNewItem.Active.Should().BeTrue();
    }

    [Fact(DisplayName = "Should remove sale item absent from update")]
    public async Task Given_MissingSaleItem_When_UpdatingSale_Then_ShouldRemoveItem()
    {
        await _fixture.ResetDatabaseAsync();

        var keptItem = new SaleItemTestBuilder().Build();
        var removedItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([keptItem, removedItem])
            .Build();
        await AddSaleAsync(sale);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToUpdate = await repository.GetByIdAsync(sale.Id);
        saleToUpdate.Should().NotBeNull();

        SaleItemUpdateData?[] updateItems =
        [
            new(keptItem.Id, saleToUpdate!.Items.Single(item => item.Id == keptItem.Id))
        ];

        saleToUpdate.Update(
            saleToUpdate.SaleNumber,
            saleToUpdate.SaleDate,
            saleToUpdate.Customer,
            saleToUpdate.Branch,
            saleToUpdate.Active,
            updateItems);

        await repository.UpdateAsync(saleToUpdate);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);

        persistedSale!.Items.Should().ContainSingle();
        persistedSale.Items.Should().Contain(item => item.Id == keptItem.Id);
        persistedSale.Items.Should().NotContain(item => item.Id == removedItem.Id);
    }

    [Fact(DisplayName = "Should persist inactive sale item and total using active items")]
    public async Task Given_InactiveSaleItem_When_UpdatingSale_Then_ShouldPersistInactiveItemAndTotal()
    {
        await _fixture.ResetDatabaseAsync();

        var activeItem = new SaleItemTestBuilder()
            .WithQuantity(4)
            .WithUnitPrice(100m)
            .Build();
        var itemToInactivate = new SaleItemTestBuilder()
            .WithQuantity(10)
            .WithUnitPrice(100m)
            .Build();
        var sale = new SaleTestBuilder()
            .WithItems([activeItem, itemToInactivate])
            .Build();
        await AddSaleAsync(sale);

        var inactive = false;
        var expectedTotalSaleAmount = 360m;

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToUpdate = await repository.GetByIdAsync(sale.Id);
        saleToUpdate.Should().NotBeNull();

        SaleItemUpdateData?[] updateItems =
        [
            new(activeItem.Id, saleToUpdate!.Items.Single(item => item.Id == activeItem.Id)),
            new(itemToInactivate.Id, saleToUpdate.Items.Single(item => item.Id == itemToInactivate.Id), inactive)
        ];

        saleToUpdate.Update(
            saleToUpdate.SaleNumber,
            saleToUpdate.SaleDate,
            saleToUpdate.Customer,
            saleToUpdate.Branch,
            saleToUpdate.Active,
            updateItems);

        await repository.UpdateAsync(saleToUpdate);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);
        var persistedInactiveItem = persistedSale!.Items.Single(item => item.Id == itemToInactivate.Id);

        persistedInactiveItem.Active.Should().BeFalse();
        persistedSale.Items.Single(item => item.Id == activeItem.Id).Active.Should().BeTrue();
        persistedSale.TotalSaleAmount.Should().Be(expectedTotalSaleAmount);
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when updating null sale")]
    public async Task Given_NullSale_When_UpdatingSale_Then_ShouldThrowArgumentNullException()
    {
        await _fixture.ResetDatabaseAsync();
        Sale? sale = null;

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var action = async () => await repository.UpdateAsync(sale!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should persist logical sale delete")]
    public async Task Given_ExistingSale_When_DeletingSale_Then_ShouldPersistInactiveSale()
    {
        await _fixture.ResetDatabaseAsync();

        var sale = new SaleTestBuilder().Build();
        await AddSaleAsync(sale);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToDelete = await repository.GetByIdAsync(sale.Id);
        saleToDelete.Should().NotBeNull();

        saleToDelete!.Delete();

        await repository.DeleteAsync(saleToDelete);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);

        persistedSale.Should().NotBeNull();
        persistedSale!.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should keep sale items and total when deleting sale")]
    public async Task Given_ExistingSaleWithItems_When_DeletingSale_Then_ShouldKeepItemsAndTotal()
    {
        await _fixture.ResetDatabaseAsync();

        var itemQuantity = 4;
        var unitPrice = 100m;
        var expectedTotalSaleAmount = 360m;
        
        var item = new SaleItemTestBuilder()
            .WithQuantity(itemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var sale = new SaleTestBuilder()
            .WithItems([item])
            .Build();
        
        await AddSaleAsync(sale);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);
        var saleToDelete = await repository.GetByIdAsync(sale.Id);
        saleToDelete.Should().NotBeNull();

        saleToDelete!.Delete();

        await repository.DeleteAsync(saleToDelete);

        var persistedSale = await GetPersistedSaleAsync(sale.Id);

        persistedSale.Should().NotBeNull();
        persistedSale!.Items.Should().ContainSingle();
        persistedSale.Items.Single().Id.Should().Be(item.Id);
        persistedSale.TotalSaleAmount.Should().Be(expectedTotalSaleAmount);
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when deleting null sale")]
    public async Task Given_NullSale_When_DeletingSale_Then_ShouldThrowArgumentNullException()
    {
        await _fixture.ResetDatabaseAsync();
        Sale? sale = null;

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var action = async () => await repository.DeleteAsync(sale!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should get sale by id without tracking with items")]
    public async Task Given_ExistingSale_When_GettingByIdAsNoTracking_Then_ShouldReturnSaleWithItems()
    {
        await _fixture.ResetDatabaseAsync();

        var item = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([item])
            .Build();
        await AddSaleAsync(sale);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.GetByIdAsNoTrackingAsync(sale.Id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(sale.Id);
        result.Items.Should().ContainSingle();
        result.Items.Single().Id.Should().Be(item.Id);
    }

    [Fact(DisplayName = "Should return null when getting missing sale by id without tracking")]
    public async Task Given_MissingSale_When_GettingByIdAsNoTracking_Then_ShouldReturnNull()
    {
        await _fixture.ResetDatabaseAsync();

        var saleId = Guid.NewGuid();
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.GetByIdAsNoTrackingAsync(saleId);

        result.Should().BeNull();
    }

    [Fact(DisplayName = "Should paginate sales")]
    public async Task Given_Sales_When_ListingSales_Then_ShouldPaginateResults()
    {
        await _fixture.ResetDatabaseAsync();

        var firstSale = CreateSale("SALE-001", DateTime.UtcNow.AddDays(-3), 1);
        var secondSale = CreateSale("SALE-002", DateTime.UtcNow.AddDays(-2), 1);
        var thirdSale = CreateSale("SALE-003", DateTime.UtcNow.AddDays(-1), 1);
        await AddSalesAsync([firstSale, secondSale, thirdSale]);

        var page = 2;
        var size = 1;
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery { Page = page, Size = size });

        result.CurrentPage.Should().Be(page);
        result.PageSize.Should().Be(size);
        result.TotalItems.Should().Be(3);
        result.TotalPages.Should().Be(3);
        result.Items.Should().ContainSingle();
        result.Items.Single().Id.Should().Be(secondSale.Id);
    }

    [Fact(DisplayName = "Should filter sales by sale number")]
    public async Task Given_Sales_When_FilteringBySaleNumber_Then_ShouldReturnMatchingSale()
    {
        await _fixture.ResetDatabaseAsync();

        var expectedSale = CreateSale("SALE-ABC", DateTime.UtcNow, 1);
        var otherSale = CreateSale("SALE-XYZ", DateTime.UtcNow.AddDays(1), 1);
        await AddSalesAsync([expectedSale, otherSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Filters = new Dictionary<string, string?> { ["saleNumber"] = "SALE-ABC" }
        });

        result.TotalItems.Should().Be(1);
        result.Items.Single().Id.Should().Be(expectedSale.Id);
    }

    [Fact(DisplayName = "Should filter sales by partial sale number")]
    public async Task Given_Sales_When_FilteringByPartialSaleNumber_Then_ShouldReturnMatchingSales()
    {
        await _fixture.ResetDatabaseAsync();

        var firstSale = CreateSale("ONLINE-001", DateTime.UtcNow, 1);
        var secondSale = CreateSale("ONLINE-002", DateTime.UtcNow.AddDays(1), 1);
        var otherSale = CreateSale("STORE-001", DateTime.UtcNow.AddDays(2), 1);
        await AddSalesAsync([firstSale, secondSale, otherSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Filters = new Dictionary<string, string?> { ["saleNumber"] = "ONLINE*" }
        });

        result.TotalItems.Should().Be(2);
        result.Items.Select(sale => sale.Id).Should().BeEquivalentTo([firstSale.Id, secondSale.Id]);
    }

    [Fact(DisplayName = "Should filter sales by active status")]
    public async Task Given_Sales_When_FilteringByActive_Then_ShouldReturnMatchingSales()
    {
        await _fixture.ResetDatabaseAsync();

        var activeSale = CreateSale("SALE-ACTIVE", DateTime.UtcNow, 1);
        var inactiveSale = CreateSale("SALE-INACTIVE", DateTime.UtcNow.AddDays(1), 1);
        inactiveSale.Delete();
        await AddSalesAsync([activeSale, inactiveSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Filters = new Dictionary<string, string?> { ["active"] = "false" }
        });

        result.TotalItems.Should().Be(1);
        result.Items.Single().Id.Should().Be(inactiveSale.Id);
        result.Items.Single().Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should filter sales by sale date range")]
    public async Task Given_Sales_When_FilteringBySaleDateRange_Then_ShouldReturnMatchingSales()
    {
        await _fixture.ResetDatabaseAsync();

        var oldSale = CreateSale("SALE-OLD", new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc), 1);
        var expectedSale = CreateSale("SALE-MID", new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc), 1);
        var futureSale = CreateSale("SALE-FUTURE", new DateTime(2026, 12, 1, 0, 0, 0, DateTimeKind.Utc), 1);
        await AddSalesAsync([oldSale, expectedSale, futureSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Filters = new Dictionary<string, string?>
            {
                ["_minSaleDate"] = "2026-05-01",
                ["_maxSaleDate"] = "2026-05-31"
            }
        });

        result.TotalItems.Should().Be(1);
        result.Items.Single().Id.Should().Be(expectedSale.Id);
    }

    [Fact(DisplayName = "Should filter sales by total sale amount range")]
    public async Task Given_Sales_When_FilteringByTotalAmountRange_Then_ShouldReturnMatchingSales()
    {
        await _fixture.ResetDatabaseAsync();

        var lowSale = CreateSale("SALE-LOW", DateTime.UtcNow, 1);
        var expectedSale = CreateSale("SALE-MID", DateTime.UtcNow.AddDays(1), 4);
        var highSale = CreateSale("SALE-HIGH", DateTime.UtcNow.AddDays(2), 10);
        await AddSalesAsync([lowSale, expectedSale, highSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Filters = new Dictionary<string, string?>
            {
                ["_minTotalSaleAmount"] = "300",
                ["_maxTotalSaleAmount"] = "500"
            }
        });

        result.TotalItems.Should().Be(1);
        result.Items.Single().Id.Should().Be(expectedSale.Id);
    }

    [Fact(DisplayName = "Should order sales by sale date descending")]
    public async Task Given_Sales_When_OrderingBySaleDateDescending_Then_ShouldReturnOrderedSales()
    {
        await _fixture.ResetDatabaseAsync();

        var firstSale = CreateSale("SALE-001", DateTime.UtcNow.AddDays(-2), 1);
        var secondSale = CreateSale("SALE-002", DateTime.UtcNow.AddDays(-1), 1);
        await AddSalesAsync([firstSale, secondSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Order = "saleDate desc"
        });

        result.Items.Select(sale => sale.Id).Should().ContainInOrder([secondSale.Id, firstSale.Id]);
    }

    [Fact(DisplayName = "Should order sales by multiple fields")]
    public async Task Given_Sales_When_OrderingByMultipleFields_Then_ShouldReturnOrderedSales()
    {
        await _fixture.ResetDatabaseAsync();

        var activeSale = CreateSale("SALE-B", DateTime.UtcNow, 1);
        var firstInactiveSale = CreateSale("SALE-A", DateTime.UtcNow.AddDays(1), 1);
        var secondInactiveSale = CreateSale("SALE-C", DateTime.UtcNow.AddDays(2), 1);
        firstInactiveSale.Delete();
        secondInactiveSale.Delete();
        await AddSalesAsync([activeSale, firstInactiveSale, secondInactiveSale]);

        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        var result = await repository.ListAsync(new SaleListQuery
        {
            Page = 1,
            Size = 10,
            Order = "active desc, saleNumber asc"
        });

        result.Items.Select(sale => sale.Id)
            .Should()
            .ContainInOrder([activeSale.Id, firstInactiveSale.Id, secondInactiveSale.Id]);
    }

    private async Task<Sale?> GetPersistedSaleAsync(Guid saleId)
    {
        await using var dbContext = _fixture.CreateDbContext();

        return await dbContext.Sales
            .AsNoTracking()
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == saleId);
    }

    private async Task AddSaleAsync(Sale sale)
    {
        await using var dbContext = _fixture.CreateDbContext();
        var repository = new SaleRepository(dbContext);

        await repository.AddAsync(sale);
    }

    private async Task AddSalesAsync(IEnumerable<Sale> sales)
    {
        foreach (var sale in sales)
        {
            await AddSaleAsync(sale);
        }
    }

    private static Sale CreateSale(string saleNumber, DateTime saleDate, int itemQuantity)
    {
        var item = new SaleItemTestBuilder()
            .WithQuantity(itemQuantity)
            .WithUnitPrice(100m)
            .Build();

        return new SaleTestBuilder()
            .WithSaleNumber(saleNumber)
            .WithSaleDate(saleDate)
            .WithItems([item])
            .Build();
    }

    private static SaleItemUpdateData?[] CreateUpdateDataFromExistingItems(Sale sale)
    {
        return sale.Items
            .Select(item => (SaleItemUpdateData?)new SaleItemUpdateData(item.Id, item, item.Active))
            .ToArray();
    }
}
