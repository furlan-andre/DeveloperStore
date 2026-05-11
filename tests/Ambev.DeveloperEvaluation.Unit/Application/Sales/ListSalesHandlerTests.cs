using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.Mappings;
using Ambev.DeveloperEvaluation.Application.Sales.Service;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesHandlerTests
{
    private readonly IListSalesService _listSalesService;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _listSalesService = Substitute.For<IListSalesService>();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<ListSalesProfile>();
        });

        var mapper = mapperConfiguration.CreateMapper();

        _handler = new ListSalesHandler(_listSalesService, mapper);
    }

    [Fact(DisplayName = "Should delegate list sales command to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldDelegateToService()
    {
        var command = new ListSalesCommandTestBuilder().Build();
        var expectedResponse = new PagedResponse<ListSaleResponse>();

        _listSalesService
            .ListAsync(Arg.Any<ListSalesRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<ListSaleResponse>>.Success(expectedResponse));

        var response = await _handler.Handle(command, CancellationToken.None);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().BeSameAs(expectedResponse);

        await _listSalesService.Received(1).ListAsync(
            Arg.Any<ListSalesRequest>(),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should send mapped command data to service")]
    public async Task Given_ValidCommand_When_Handling_Then_ShouldSendMappedDataToService()
    {
        var page = 2;
        var size = 20;
        var order = "saleDate desc";
        IReadOnlyDictionary<string, string?> filters = new Dictionary<string, string?>
        {
            ["active"] = "true"
        };
        
        var command = new ListSalesCommandTestBuilder()
            .WithPage(page)
            .WithSize(size)
            .WithOrder(order)
            .WithFilters(filters)
            .Build();
        var expectedResponse = new PagedResponse<ListSaleResponse>();

        _listSalesService
            .ListAsync(Arg.Any<ListSalesRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<ListSaleResponse>>.Success(expectedResponse));

        await _handler.Handle(command, CancellationToken.None);

        await _listSalesService.Received(1).ListAsync(
            Arg.Is<ListSalesRequest>(request =>
                request.Page == page &&
                request.Size == size &&
                request.Order == order &&
                request.Filters.Count == filters.Count &&
                request.Filters["active"] == filters["active"]),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Should forward cancellation token to service")]
    public async Task Given_CancellationToken_When_Handling_Then_ShouldForwardTokenToService()
    {
        var command = new ListSalesCommandTestBuilder().Build();
        var cancellationToken = new CancellationTokenSource().Token;
        var expectedResponse = new PagedResponse<ListSaleResponse>();

        _listSalesService
            .ListAsync(Arg.Any<ListSalesRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<ListSaleResponse>>.Success(expectedResponse));

        await _handler.Handle(command, cancellationToken);

        await _listSalesService.Received(1).ListAsync(
            Arg.Any<ListSalesRequest>(),
            cancellationToken);
    }
}
