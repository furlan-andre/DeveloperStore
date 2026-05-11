using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class CreateSaleHandlerTests
{
    private readonly ICreateSaleService _createSaleService;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _createSaleService = Substitute.For<ICreateSaleService>();
        
        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<CreateSaleProfile>();
        });
        
        var mapper = mapperConfiguration.CreateMapper();
        
        _handler = new CreateSaleHandler(_createSaleService, mapper);
    }

    [Fact(DisplayName = "Should delegate create sale command to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldDelegateToService()
    {
        var command = new CreateSaleCommandTestBuilder().Build();
        
        var expectedResponse = new CreateSaleResponse { Id = Guid.NewGuid() };
        
        _createSaleService
            .CreateAsync(Arg.Any<CreateSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreateSaleResponse>.Success(expectedResponse));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().BeSameAs(expectedResponse);
        
        await _createSaleService.Received(1).CreateAsync(
            Arg.Any<CreateSaleRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send mapped command data to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldSendMappedDataToService()
    {
        var itemCommand = new CreateSaleItemCommandTestBuilder().Build();
        
        var command = new CreateSaleCommandTestBuilder()
            .WithItems([itemCommand])
            .Build();
        var expectedResponse = new CreateSaleResponse { Id = Guid.NewGuid() };

        _createSaleService
            .CreateAsync(Arg.Any<CreateSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreateSaleResponse>.Success(expectedResponse));

        await _handler.Handle(command, CancellationToken.None);

        await _createSaleService.Received(1).CreateAsync(
            Arg.Is<CreateSaleRequest>(request =>
                request.SaleNumber == command.SaleNumber &&
                request.SaleDate == command.SaleDate &&
                request.CustomerId == command.CustomerId &&
                request.CustomerName == command.CustomerName &&
                request.BranchId == command.BranchId &&
                request.BranchName == command.BranchName &&
                request.Items.Count == 1 &&
                request.Items.Single().ProductId == itemCommand.ProductId &&
                request.Items.Single().ProductDescription == itemCommand.ProductDescription &&
                request.Items.Single().Quantity == itemCommand.Quantity &&
                request.Items.Single().UnitPrice == itemCommand.UnitPrice),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should forward cancellation token to service")]
    public async Task Given_CancellationToken_When_Handling_Then_ShouldForwardTokenToService()
    {
        var command = new CreateSaleCommandTestBuilder().Build();
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResponse = new CreateSaleResponse { Id = Guid.NewGuid() };

        _createSaleService
            .CreateAsync(Arg.Any<CreateSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<CreateSaleResponse>.Success(expectedResponse));

        await _handler.Handle(command, cancellationToken);

        await _createSaleService.Received(1).CreateAsync(
            Arg.Any<CreateSaleRequest>(),
            cancellationToken);
    }
}
