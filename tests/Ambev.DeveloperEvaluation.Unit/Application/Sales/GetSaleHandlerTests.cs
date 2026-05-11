using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using Ambev.DeveloperEvaluation.Unit.Common;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly IGetSaleService _getSaleService;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _getSaleService = Substitute.For<IGetSaleService>();

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<GetSaleProfile>();
        });

        var mapper = mapperConfiguration.CreateMapper();

        _handler = new GetSaleHandler(_getSaleService, mapper);
    }

    [Fact(DisplayName = "Should delegate get sale command to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldDelegateToService()
    {
        var command = new GetSaleCommandTestBuilder().Build();
        var expectedResponse = new GetSaleResponse { Id = command.Id };

        _getSaleService
            .GetByIdAsync(Arg.Any<GetSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<GetSaleResponse>.Success(expectedResponse));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().BeSameAs(expectedResponse);

        await _getSaleService.Received(1).GetByIdAsync(
            Arg.Any<GetSaleRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send mapped command data to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldSendMappedDataToService()
    {
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommandTestBuilder()
            .WithId(saleId)
            .Build();
        var expectedResponse = new GetSaleResponse { Id = saleId };

        _getSaleService
            .GetByIdAsync(Arg.Any<GetSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<GetSaleResponse>.Success(expectedResponse));

        await _handler.Handle(command, CancellationToken.None);

        await _getSaleService.Received(1).GetByIdAsync(
            Arg.Is<GetSaleRequest>(request => request.Id == saleId),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should forward cancellation token to service")]
    public async Task Given_CancellationToken_When_Handling_Then_ShouldForwardTokenToService()
    {
        var command = new GetSaleCommandTestBuilder().Build();
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResponse = new GetSaleResponse { Id = command.Id };

        _getSaleService
            .GetByIdAsync(Arg.Any<GetSaleRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<GetSaleResponse>.Success(expectedResponse));

        await _handler.Handle(command, cancellationToken);

        await _getSaleService.Received(1).GetByIdAsync(
            Arg.Any<GetSaleRequest>(),
            cancellationToken);
    }
}
