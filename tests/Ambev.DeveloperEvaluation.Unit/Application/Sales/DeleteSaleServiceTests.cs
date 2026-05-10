using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
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
    private readonly IMapper _mapper;
    private readonly DeleteSaleService _service;

    public DeleteSaleServiceTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<DeleteSaleProfile>();
        });

        _mapper = mapperConfiguration.CreateMapper();
        _service = new DeleteSaleService(_saleRepository, _mapper);
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

        var action = async () => await _service.DeleteAsync(request);

        await action.Should().ThrowAsync<KeyNotFoundException>();
        await _saleRepository.DidNotReceive().DeleteAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should logically delete sale")]
    public async Task Given_ActiveSale_When_DeletingSale_Then_ShouldInactivateSale()
    {
        var sale = new SaleTestBuilder().Build();
        var request = new DeleteSaleRequestTestBuilder()
            .WithId(sale.Id)
            .Build();

        _saleRepository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(sale);

        var response = await _service.DeleteAsync(request);

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

        var response = await _service.DeleteAsync(request);

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
}
