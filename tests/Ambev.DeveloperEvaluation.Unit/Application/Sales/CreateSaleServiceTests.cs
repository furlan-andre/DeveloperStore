using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Domain.Entities.Sales;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services.Sales;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using Ambev.DeveloperEvaluation.Unit.Common;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleServiceTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalesEventPublisher _salesEventPublisher;
    private readonly IMapper _mapper;
    private readonly CreateSaleService _service;

    public CreateSaleServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _salesEventPublisher = Substitute.For<ISalesEventPublisher>();
        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<CreateSaleProfile>();
        });
        _mapper = mapperConfiguration.CreateMapper();
        _service = new CreateSaleService(
            _saleRepository,
            new SaleDiscountPolicy(),
            _mapper,
            _salesEventPublisher);
    }

    [Fact(DisplayName = "Should have valid AutoMapper configuration")]
    public void Given_CreateSaleProfile_When_ValidatingAutoMapper_Then_ShouldBeValid()
    {
        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<CreateSaleProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should create valid sale")]
    public async Task Given_ValidRequest_When_CreatingSale_Then_ShouldReturnResponse()
    {
        var request = new CreateSaleRequestTestBuilder().Build();

        var result = await _service.CreateAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        response.Should().NotBeNull();
        response.Id.Should().NotBe(Guid.Empty);
    }

    [Fact(DisplayName = "Should call sale repository once")]
    public async Task Given_ValidRequest_When_CreatingSale_Then_ShouldCallRepositoryOnce()
    {
        var request = new CreateSaleRequestTestBuilder().Build();

        await _service.CreateAsync(request);

        await _saleRepository.Received(1).AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should publish sale created event")]
    public async Task Given_ValidRequest_When_CreatingSale_Then_ShouldPublishSaleCreatedEvent()
    {
        var request = new CreateSaleRequestTestBuilder().Build();

        await _service.CreateAsync(request);

        await _salesEventPublisher.Received(1).PublishAsync(
            Arg.Is<SaleCreatedEvent>(saleEvent =>
                saleEvent.SaleId != Guid.Empty &&
                saleEvent.SaleNumber == request.SaleNumber &&
                saleEvent.CustomerId == request.CustomerId &&
                saleEvent.CustomerName == request.CustomerName &&
                saleEvent.BranchId == request.BranchId &&
                saleEvent.BranchName == request.BranchName &&
                saleEvent.Active &&
                saleEvent.EventId != Guid.Empty &&
                saleEvent.OccurredAt != default),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should not publish sale created event when repository fails")]
    public async Task Given_RepositoryFailure_When_CreatingSale_Then_ShouldNotPublishSaleCreatedEvent()
    {
        var request = new CreateSaleRequestTestBuilder().Build();
        var exception = new InvalidOperationException("Repository failure");
        _saleRepository
            .AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw exception);

        var action = async () => await _service.CreateAsync(request);

        await action.Should().ThrowAsync<InvalidOperationException>();
        await _salesEventPublisher.DidNotReceive().PublishAsync(
            Arg.Any<SaleCreatedEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send sale data to repository")]
    public async Task Given_ValidRequest_When_CreatingSale_Then_ShouldSendSaleDataToRepository()
    {
        var request = new CreateSaleRequestTestBuilder().Build();

        await _service.CreateAsync(request);

        await _saleRepository.Received(1).AddAsync(
            Arg.Is<Sale>(sale =>
                sale.SaleNumber == request.SaleNumber &&
                sale.SaleDate == request.SaleDate &&
                sale.Customer.Id == request.CustomerId &&
                sale.Customer.Name == request.CustomerName &&
                sale.Branch.Id == request.BranchId &&
                sale.Branch.Name == request.BranchName),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send sale items to repository")]
    public async Task Given_RequestWithItems_When_CreatingSale_Then_ShouldSendItemsToRepository()
    {
        var itemRequest = new CreateSaleItemRequestTestBuilder().Build();
        var request = new CreateSaleRequestTestBuilder()
            .WithItems([itemRequest])
            .Build();

        await _service.CreateAsync(request);

        await _saleRepository.Received(1).AddAsync(
            Arg.Is<Sale>(sale =>
                sale.Items.Count == 1 &&
                sale.Items.Single().Product.Id == itemRequest.ProductId &&
                sale.Items.Single().Product.Description == itemRequest.ProductDescription &&
                sale.Items.Single().Quantity == itemRequest.Quantity &&
                sale.Items.Single().UnitPrice == itemRequest.UnitPrice),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should return sale response data")]
    public async Task Given_ValidRequest_When_CreatingSale_Then_ShouldReturnSaleResponseData()
    {
        var request = new CreateSaleRequestTestBuilder().Build();

        var result = await _service.CreateAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        response.SaleNumber.Should().Be(request.SaleNumber);
        response.SaleDate.Should().Be(request.SaleDate);
        response.CustomerId.Should().Be(request.CustomerId);
        response.CustomerName.Should().Be(request.CustomerName);
        response.BranchId.Should().Be(request.BranchId);
        response.BranchName.Should().Be(request.BranchName);
        response.Active.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return calculated sale item response data")]
    public async Task Given_RequestWithItems_When_CreatingSale_Then_ShouldReturnCalculatedItemResponseData()
    {
        var quantity = 4;
        var unitPrice = 100m;
        var expectedDiscount = 40m;
        var expectedItemTotal = 360m;
        var expectedSaleTotal = 360m;
        
        var itemRequest = new CreateSaleItemRequestTestBuilder()
            .WithQuantity(quantity)
            .WithUnitPrice(unitPrice)
            .Build();
        
        var request = new CreateSaleRequestTestBuilder()
            .WithItems([itemRequest])
            .Build();

        var result = await _service.CreateAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        response.TotalSaleAmount.Should().Be(expectedSaleTotal);
        response.Items.Should().ContainSingle();
        
        var responseItem = response.Items.Single();
        responseItem.ProductId.Should().Be(itemRequest.ProductId);
        responseItem.ProductDescription.Should().Be(itemRequest.ProductDescription);
        responseItem.Quantity.Should().Be(quantity);
        responseItem.UnitPrice.Should().Be(unitPrice);
        responseItem.Discount.Should().Be(expectedDiscount);
        responseItem.TotalAmount.Should().Be(expectedItemTotal);
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when request is null")]
    public async Task Given_NullRequest_When_CreatingSale_Then_ShouldThrowArgumentNullException()
    {
        CreateSaleRequest? request = null;

        var action = async () => await _service.CreateAsync(request!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should return domain rule violation when domain rejects invalid data")]
    public async Task Given_InvalidRequest_When_CreatingSale_Then_ShouldReturnDomainRuleViolation()
    {
        var saleNumber = string.Empty;
        var request = new CreateSaleRequestTestBuilder()
            .WithSaleNumber(saleNumber)
            .Build();

        var result = await _service.CreateAsync(request);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be("DomainRuleViolation");
        await _saleRepository.DidNotReceive().AddAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}
