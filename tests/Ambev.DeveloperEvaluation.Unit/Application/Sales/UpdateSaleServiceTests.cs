using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleServiceTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleService _service;

    public UpdateSaleServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<UpdateSaleProfile>();
        });

        _mapper = mapperConfiguration.CreateMapper();
        _service = new UpdateSaleService(_saleRepository, new SaleDiscountPolicy(), _mapper);
    }

    [Fact(DisplayName = "Should have valid AutoMapper configuration")]
    public void Given_UpdateSaleProfile_When_ValidatingAutoMapper_Then_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<UpdateSaleProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when request is null")]
    public async Task Given_NullRequest_When_UpdatingSale_Then_ShouldThrowArgumentNullException()
    {
        UpdateSaleRequest? request = null;

        var action = async () => await _service.UpdateAsync(request!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should get sale by id")]
    public async Task Given_ValidRequest_When_UpdatingSale_Then_ShouldGetSaleById()
    {
        var sale = new SaleTestBuilder().Build();
        var request = CreateRequestForSale(sale);

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.UpdateAsync(request);

        await _saleRepository.Received(1).GetByIdAsync(request.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Given_MissingSale_When_UpdatingSale_Then_ShouldThrowKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var request = new UpdateSaleRequestTestBuilder()
            .WithId(saleId)
            .Build();

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var action = async () => await _service.UpdateAsync(request);

        await action.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should call sale repository update once")]
    public async Task Given_ValidRequest_When_UpdatingSale_Then_ShouldCallRepositoryUpdateOnce()
    {
        var sale = new SaleTestBuilder().Build();
        var request = CreateRequestForSale(sale);

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.UpdateAsync(request);

        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should update sale data")]
    public async Task Given_ValidRequest_When_UpdatingSale_Then_ShouldUpdateSaleData()
    {
        var sale = new SaleTestBuilder().Build();
        var saleNumber = "SALE-UPDATED";
        var saleDate = DateTime.UtcNow.AddDays(1);
        var customerId = Guid.NewGuid();
        var customerName = "Updated Customer";
        var branchId = Guid.NewGuid();
        var branchName = "Updated Branch";
        var active = false;
        var existingItem = sale.Items.First();
        var itemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(existingItem.Id)
            .WithProductId(existingItem.Product.Id)
            .WithProductDescription(existingItem.Product.Description)
            .WithQuantity(existingItem.Quantity)
            .WithUnitPrice(existingItem.UnitPrice)
            .WithActive(existingItem.Active)
            .Build();

        var request = new UpdateSaleRequestTestBuilder()
            .WithId(sale.Id)
            .WithSaleNumber(saleNumber)
            .WithSaleDate(saleDate)
            .WithCustomerId(customerId)
            .WithCustomerName(customerName)
            .WithBranchId(branchId)
            .WithBranchName(branchName)
            .WithActive(active)
            .WithItems([itemRequest])
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.UpdateAsync(request);

        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(updatedSale =>
                updatedSale.SaleNumber == saleNumber &&
                updatedSale.SaleDate == saleDate &&
                updatedSale.Customer.Id == customerId &&
                updatedSale.Customer.Name == customerName &&
                updatedSale.Branch.Id == branchId &&
                updatedSale.Branch.Name == branchName &&
                updatedSale.Active == active),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should remove sale item absent from request")]
    public async Task Given_RequestWithoutExistingItem_When_UpdatingSale_Then_ShouldRemoveItem()
    {
        var keptItem = new SaleItemTestBuilder().Build();
        var removedItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([keptItem, removedItem])
            .Build();

        var keptItemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(keptItem.Id)
            .WithProductId(keptItem.Product.Id)
            .WithProductDescription(keptItem.Product.Description)
            .WithQuantity(keptItem.Quantity)
            .WithUnitPrice(keptItem.UnitPrice)
            .WithActive(keptItem.Active)
            .Build();

        var request = new UpdateSaleRequestTestBuilder()
            .WithId(sale.Id)
            .WithItems([keptItemRequest])
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var response = await _service.UpdateAsync(request);

        response.Items.Should().ContainSingle();
        response.Items.Should().Contain(item => item.Id == keptItem.Id);
        response.Items.Should().NotContain(item => item.Id == removedItem.Id);
    }

    [Fact(DisplayName = "Should add new sale item")]
    public async Task Given_RequestWithNewItem_When_UpdatingSale_Then_ShouldAddItem()
    {
        var existingItemQuantity = 4;
        var newItemQuantity = 10;
        var unitPrice = 100m;
        var expectedSaleTotal = 1160m;
        
        var existingItem = new SaleItemTestBuilder()
            .WithQuantity(existingItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();

        var existingItemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(existingItem.Id)
            .WithProductId(existingItem.Product.Id)
            .WithProductDescription(existingItem.Product.Description)
            .WithQuantity(existingItem.Quantity)
            .WithUnitPrice(unitPrice)
            .Build();

        var newItemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(Guid.Empty)
            .WithQuantity(newItemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();

        var request = new UpdateSaleRequestTestBuilder()
            .WithId(sale.Id)
            .WithItems([existingItemRequest, newItemRequest])
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var response = await _service.UpdateAsync(request);

        response.Items.Should().HaveCount(2);
        response.Items.Should().Contain(item => item.Id == existingItem.Id);

        var addedItem = response.Items.Single(item => item.Id != existingItem.Id);
        addedItem.Quantity.Should().Be(newItemQuantity);
        addedItem.Active.Should().BeTrue();
        response.TotalSaleAmount.Should().Be(expectedSaleTotal);
    }

    [Fact(DisplayName = "Should inactivate sale item and exclude it from total")]
    public async Task Given_RequestWithInactiveItem_When_UpdatingSale_Then_ShouldInactivateItem()
    {
        var activeItemQuantity = 4;
        var inactiveItemQuantity = 10;
        var unitPrice = 100m;
        var inactive = false;
        var expectedSaleTotal = 360m;
        
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

        var activeItemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(activeItem.Id)
            .WithProductId(activeItem.Product.Id)
            .WithProductDescription(activeItem.Product.Description)
            .WithQuantity(activeItem.Quantity)
            .WithUnitPrice(activeItem.UnitPrice)
            .WithActive(activeItem.Active)
            .Build();

        var inactiveItemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(itemToInactivate.Id)
            .WithProductId(itemToInactivate.Product.Id)
            .WithProductDescription(itemToInactivate.Product.Description)
            .WithQuantity(itemToInactivate.Quantity)
            .WithUnitPrice(itemToInactivate.UnitPrice)
            .WithActive(inactive)
            .Build();

        var request = new UpdateSaleRequestTestBuilder()
            .WithId(sale.Id)
            .WithItems([activeItemRequest, inactiveItemRequest])
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var response = await _service.UpdateAsync(request);

        response.Items.Should().HaveCount(2);
        response.Items.Single(item => item.Id == activeItem.Id).Active.Should().BeTrue();
        response.Items.Single(item => item.Id == itemToInactivate.Id).Active.Should().BeFalse();
        response.TotalSaleAmount.Should().Be(expectedSaleTotal);
    }

    [Fact(DisplayName = "Should return updated sale response data")]
    public async Task Given_ValidRequest_When_UpdatingSale_Then_ShouldReturnUpdatedResponseData()
    {
        var existingItem = new SaleItemTestBuilder().Build();
        var sale = new SaleTestBuilder()
            .WithItems([existingItem])
            .Build();
        var request = CreateRequestForSale(sale);

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var response = await _service.UpdateAsync(request);

        response.Id.Should().Be(sale.Id);
        response.SaleNumber.Should().Be(request.SaleNumber);
        response.SaleDate.Should().Be(request.SaleDate);
        response.CustomerId.Should().Be(request.CustomerId);
        response.CustomerName.Should().Be(request.CustomerName);
        response.BranchId.Should().Be(request.BranchId);
        response.BranchName.Should().Be(request.BranchName);
        response.Active.Should().Be(request.Active);
        response.Items.Should().ContainSingle();
    }

    private static UpdateSaleRequest CreateRequestForSale(Sale sale)
    {
        var existingItem = sale.Items.First();
        var itemRequest = new UpdateSaleItemRequestTestBuilder()
            .WithId(existingItem.Id)
            .WithProductId(existingItem.Product.Id)
            .WithProductDescription(existingItem.Product.Description)
            .WithQuantity(existingItem.Quantity)
            .WithUnitPrice(existingItem.UnitPrice)
            .WithActive(existingItem.Active)
            .Build();

        return new UpdateSaleRequestTestBuilder()
            .WithId(sale.Id)
            .WithSaleNumber(sale.SaleNumber)
            .WithSaleDate(sale.SaleDate)
            .WithCustomerId(sale.Customer.Id)
            .WithCustomerName(sale.Customer.Name)
            .WithBranchId(sale.Branch.Id)
            .WithBranchName(sale.Branch.Name)
            .WithActive(sale.Active)
            .WithItems([itemRequest])
            .Build();
    }
}
