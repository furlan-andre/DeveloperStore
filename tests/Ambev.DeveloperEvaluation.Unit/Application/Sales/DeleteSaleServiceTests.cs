using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
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

public class DeleteSaleServiceTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISalesEventPublisher _salesEventPublisher;
    private readonly IMapper _mapper;
    private readonly DeleteSaleService _service;

    public DeleteSaleServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _salesEventPublisher = Substitute.For<ISalesEventPublisher>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<DeleteSaleProfile>();
        });

        _mapper = mapperConfiguration.CreateMapper();
        _service = new DeleteSaleService(
            _saleRepository,
            _mapper,
            _salesEventPublisher);
    }

    [Fact(DisplayName = "Should have valid AutoMapper configuration")]
    public void Given_DeleteSaleProfile_When_ValidatingAutoMapper_Then_ShouldBeValid()
    {
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<DeleteSaleProfile>();
        });

        mapperConfiguration.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should throw ArgumentNullException when request is null")]
    public async Task Given_NullRequest_When_DeletingSale_Then_ShouldThrowArgumentNullException()
    {
        DeleteSaleRequest? request = null;

        var action = async () => await _service.DeleteAsync(request!);

        await action.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact(DisplayName = "Should get sale by id")]
    public async Task Given_ValidRequest_When_DeletingSale_Then_ShouldGetSaleById()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.DeleteAsync(request);

        await _saleRepository.Received(1).GetByIdAsync(request.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should throw KeyNotFoundException when sale does not exist")]
    public async Task Given_MissingSale_When_DeletingSale_Then_ShouldThrowKeyNotFoundException()
    {
        var saleId = Guid.NewGuid();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(saleId)
            .Build();

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        var result = await _service.DeleteAsync(request);

        result.IsFailure.Should().BeTrue();
        result.Error.Type.Should().Be("ResourceNotFound");
        await _saleRepository.DidNotReceive().DeleteAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _salesEventPublisher.DidNotReceive().PublishAsync(
            Arg.Any<SaleCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should logically delete sale")]
    public async Task Given_ActiveSale_When_DeletingSale_Then_ShouldInactivateSale()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var result = await _service.DeleteAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        sale.Active.Should().BeFalse();
        response.Id.Should().Be(sale.Id);
        response.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should keep items and total sale amount when deleting sale")]
    public async Task Given_SaleWithItems_When_DeletingSale_Then_ShouldKeepItemsAndTotal()
    {
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
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.DeleteAsync(request);

        sale.Items.Should().ContainSingle();
        sale.Items.Should().Contain(item);
        sale.TotalSaleAmount.Should().Be(expectedTotalSaleAmount);
    }

    [Fact(DisplayName = "Should allow deleting inactive sale")]
    public async Task Given_InactiveSale_When_DeletingSale_Then_ShouldKeepSaleInactive()
    {
        var sale = new SaleTestBuilder().Build();
        sale.Delete();
        
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var result = await _service.DeleteAsync(request);
        var response = result.Value;

        result.IsSuccess.Should().BeTrue();
        sale.Active.Should().BeFalse();
        response.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should call sale repository delete once")]
    public async Task Given_ValidRequest_When_DeletingSale_Then_ShouldCallRepositoryDeleteOnce()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.DeleteAsync(request);

        await _saleRepository.Received(1).DeleteAsync(sale, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should publish sale cancelled event when active sale is deleted")]
    public async Task Given_ActiveSale_When_DeletingSale_Then_ShouldPublishSaleCancelledEvent()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.DeleteAsync(request);

        await _salesEventPublisher.Received(1).PublishAsync(
            Arg.Is<SaleCancelledEvent>(saleEvent =>
                saleEvent.SaleId == sale.Id &&
                saleEvent.SaleNumber == sale.SaleNumber &&
                saleEvent.EventId != Guid.Empty &&
                saleEvent.OccurredAt != default &&
                saleEvent.CancelledAt != default),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should not publish sale cancelled event when sale was already inactive")]
    public async Task Given_InactiveSale_When_DeletingSale_Then_ShouldNotPublishSaleCancelledEvent()
    {
        var sale = new SaleTestBuilder().Build();
        sale.Delete();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        await _service.DeleteAsync(request);

        await _salesEventPublisher.DidNotReceive().PublishAsync(
            Arg.Any<SaleCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should not publish sale cancelled event when repository delete fails")]
    public async Task Given_RepositoryFailure_When_DeletingSale_Then_ShouldNotPublishSaleCancelledEvent()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();
        var exception = new InvalidOperationException("Repository failure");

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        
        _saleRepository
            .DeleteAsync(sale, Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw exception);

        var action = async () => await _service.DeleteAsync(request);

        await action.Should().ThrowAsync<InvalidOperationException>();
        
        await _salesEventPublisher.DidNotReceive().PublishAsync(
            Arg.Any<SaleCancelledEvent>(),
            Arg.Any<CancellationToken>());
    }
}
