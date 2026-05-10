using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleHandlerTests
{
    private readonly IDeleteSaleService _deleteSaleService;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _deleteSaleService = Substitute.For<IDeleteSaleService>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<DeleteSaleProfile>();
        });

        var mapper = mapperConfiguration.CreateMapper();

        _handler = new DeleteSaleHandler(_deleteSaleService, mapper);
    }

    [Fact(DisplayName = "Should delegate delete sale command to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldDelegateToService()
    {
        var command = new DeleteSaleCommandTestBuilder().Build();
        var expectedResponse = new DeleteSaleResponse { Id = command.Id, Active = false };

        _deleteSaleService
            .DeleteAsync(Arg.Any<DeleteSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(expectedResponse);

        var response = await _handler.Handle(command, CancellationToken.None);

        response.Should().BeSameAs(expectedResponse);

        await _deleteSaleService.Received(1).DeleteAsync(
            Arg.Any<DeleteSaleRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send mapped command data to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldSendMappedDataToService()
    {
        var saleId = Guid.NewGuid();
        var command = new DeleteSaleCommandTestBuilder()
            .WithId(saleId)
            .Build();

        await _handler.Handle(command, CancellationToken.None);

        await _deleteSaleService.Received(1).DeleteAsync(
            Arg.Is<DeleteSaleRequest>(request => request.Id == saleId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should forward cancellation token to service")]
    public async Task Given_CancellationToken_When_Handling_Then_ShouldForwardTokenToService()
    {
        var command = new DeleteSaleCommandTestBuilder().Build();
        var cancellationToken = new CancellationTokenSource().Token;

        await _handler.Handle(command, cancellationToken);

        await _deleteSaleService.Received(1).DeleteAsync(
            Arg.Any<DeleteSaleRequest>(),
            cancellationToken);
    }
}
