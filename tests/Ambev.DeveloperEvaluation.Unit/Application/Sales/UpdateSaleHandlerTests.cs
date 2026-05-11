using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly IUpdateSaleService _updateSaleService;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _updateSaleService = Substitute.For<IUpdateSaleService>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<UpdateSaleProfile>();
        });

        var mapper = mapperConfiguration.CreateMapper();

        _handler = new UpdateSaleHandler(_updateSaleService, mapper);
    }

    [Fact(DisplayName = "Should delegate update sale command to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldDelegateToService()
    {
        var command = new UpdateSaleCommandTestBuilder().Build();
        var expectedResponse = new UpdateSaleResponse { Id = command.Id };

        _updateSaleService
            .UpdateAsync(Arg.Any<UpdateSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<UpdateSaleResponse>.Success(expectedResponse));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().BeSameAs(expectedResponse);

        await _updateSaleService.Received(1).UpdateAsync(
            Arg.Any<UpdateSaleRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send mapped command data to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldSendMappedDataToService()
    {
        var itemActive = false;
        var itemCommand = new UpdateSaleItemCommandTestBuilder()
            .WithActive(itemActive)
            .Build();

        var command = new UpdateSaleCommandTestBuilder()
            .WithItems([itemCommand])
            .Build();
        var expectedResponse = new UpdateSaleResponse { Id = command.Id };

        _updateSaleService
            .UpdateAsync(Arg.Any<UpdateSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<UpdateSaleResponse>.Success(expectedResponse));

        await _handler.Handle(command, CancellationToken.None);

        await _updateSaleService.Received(1).UpdateAsync(
            Arg.Is<UpdateSaleRequest>(request =>
                request.Id == command.Id &&
                request.SaleNumber == command.SaleNumber &&
                request.SaleDate == command.SaleDate &&
                request.CustomerId == command.CustomerId &&
                request.CustomerName == command.CustomerName &&
                request.BranchId == command.BranchId &&
                request.BranchName == command.BranchName &&
                request.Active == command.Active &&
                request.Items.Count == 1 &&
                request.Items.Single().Id == itemCommand.Id &&
                request.Items.Single().ProductId == itemCommand.ProductId &&
                request.Items.Single().ProductDescription == itemCommand.ProductDescription &&
                request.Items.Single().Quantity == itemCommand.Quantity &&
                request.Items.Single().UnitPrice == itemCommand.UnitPrice &&
                request.Items.Single().Active == itemActive),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should forward cancellation token to service")]
    public async Task Given_CancellationToken_When_Handling_Then_ShouldForwardTokenToService()
    {
        var command = new UpdateSaleCommandTestBuilder().Build();
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResponse = new UpdateSaleResponse { Id = command.Id };

        _updateSaleService
            .UpdateAsync(Arg.Any<UpdateSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<UpdateSaleResponse>.Success(expectedResponse));

        await _handler.Handle(command, cancellationToken);

        await _updateSaleService.Received(1).UpdateAsync(
            Arg.Any<UpdateSaleRequest>(),
            cancellationToken);
    }
}
