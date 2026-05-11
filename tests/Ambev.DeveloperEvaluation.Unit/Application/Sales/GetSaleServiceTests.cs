using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.TestUtils.Domain.Sales.Builders;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleServiceTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleService _service;

    public GetSaleServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<GetSaleProfile>();
        });

        _mapper = mapperConfiguration.CreateMapper();
        _service = new GetSaleService(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Should have valid AutoMapper configuration")]
    public void Given_GetSaleProfile_When_ValidatingAutoMapper_Then_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<GetSaleProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when request is null")]
    public async Task Given_NullRequest_When_GettingSale_Then_ShouldThrowArgumentNullException()
    {
        GetSaleRequest? request = null;

        var action = async () => await _service.GetByIdAsync(request!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should get sale by id without tracking")]
    public async Task Given_ValidRequest_When_GettingSale_Then_ShouldGetSaleByIdWithoutTracking()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new GetSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsNoTrackingAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.GetByIdAsync(request);

        await _saleRepository.Received(1).GetByIdAsNoTrackingAsync(request.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Given_MissingSale_When_GettingSale_Then_ShouldThrowKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var request = new GetSaleRequestTestBuilder()
            .WithId(saleId)
            .Build();

        _saleRepository.GetByIdAsNoTrackingAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var result = await _service.GetByIdAsync(request);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be("ResourceNotFound");
    }

    [Fact(DisplayName = "Should return sale response with items")]
    public async Task Given_ExistingSale_When_GettingSale_Then_ShouldReturnSaleWithItems()
    {
        var itemQuantity = 4;
        var unitPrice = 100m;
        var expectedDiscount = 40m;
        var expectedTotalAmount = 360m;
        var item = new SaleItemTestBuilder()
            .WithQuantity(itemQuantity)
            .WithUnitPrice(unitPrice)
            .Build();
        var sale = new SaleTestBuilder()
            .WithItems([item])
            .Build();
        var request = new GetSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsNoTrackingAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var result = await _service.GetByIdAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        response.Id.Should().Be(sale.Id);
        response.SaleNumber.Should().Be(sale.SaleNumber);
        response.CustomerId.Should().Be(sale.Customer.Id);
        response.CustomerName.Should().Be(sale.Customer.Name);
        response.BranchId.Should().Be(sale.Branch.Id);
        response.BranchName.Should().Be(sale.Branch.Name);
        response.Items.Should().ContainSingle();

        var responseItem = response.Items.Single();
        responseItem.Id.Should().Be(item.Id);
        responseItem.ProductId.Should().Be(item.Product.Id);
        responseItem.ProductDescription.Should().Be(item.Product.Description);
        responseItem.Quantity.Should().Be(itemQuantity);
        responseItem.UnitPrice.Should().Be(unitPrice);
        responseItem.Discount.Should().Be(expectedDiscount);
        responseItem.TotalAmount.Should().Be(expectedTotalAmount);
        responseItem.Active.Should().BeTrue();
    }
}
