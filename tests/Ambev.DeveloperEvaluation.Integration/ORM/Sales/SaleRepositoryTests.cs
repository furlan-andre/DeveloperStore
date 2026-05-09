using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
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

    private async Task<Sale?> GetPersistedSaleAsync(Guid saleId)
    {
        await using var dbContext = _fixture.CreateDbContext();

        return await dbContext.Sales
            .AsNoTracking()
            .Include(sale => sale.Items)
            .FirstOrDefaultAsync(sale => sale.Id == saleId);
    }
}
