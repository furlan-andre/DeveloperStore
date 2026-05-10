using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.WebApi.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Sales;

public class SalesControllerTests
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly SalesController _controller;

    public SalesControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _mapper = Substitute.For<IMapper>();
        _controller = new SalesController(_mediator, _mapper);
    }

    [Fact(DisplayName = "Should map input to command and send through MediatR")]
    public async Task Given_ValidInput_When_CreatingSale_Then_ShouldSendCommandThroughMediator()
    {
        var input = new CreateSaleInputTestBuilder().Build();
        var command = new CreateSaleCommand { SaleNumber = input.SaleNumber };
        var applicationResponse = new CreateSaleResponse { Id = Guid.NewGuid(), SaleNumber = input.SaleNumber };
        var apiResult = new CreateSaleResult { Id = applicationResponse.Id, SaleNumber = applicationResponse.SaleNumber };
        
        _mapper.Map<CreateSaleCommand>(input).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(applicationResponse);
        _mapper.Map<CreateSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.CreateSale(input, CancellationToken.None);

        _mapper.Received(1).Map<CreateSaleCommand>(input);
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<CreateSaleResult>(applicationResponse);
    }

    [Fact(DisplayName = "Should forward cancellation token to MediatR")]
    public async Task Given_CancellationToken_When_CreatingSale_Then_ShouldForwardTokenToMediator()
    {
        var input = new CreateSaleInputTestBuilder().Build();
        var command = new CreateSaleCommand();
        var applicationResponse = new CreateSaleResponse { Id = Guid.NewGuid() };
        var apiResult = new CreateSaleResult { Id = applicationResponse.Id };
        var cancellationToken = new CancellationTokenSource().Token;
        
        _mapper.Map<CreateSaleCommand>(input).Returns(command);
        _mediator.Send(command, cancellationToken).Returns(applicationResponse);
        _mapper.Map<CreateSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.CreateSale(input, cancellationToken);

        await _mediator.Received(1).Send(command, cancellationToken);
    }

    [Fact(DisplayName = "Should return created sale response")]
    public async Task Given_ValidInput_When_CreatingSale_Then_ShouldReturnCreatedResponse()
    {
        var input = new CreateSaleInputTestBuilder().Build();
        var command = new CreateSaleCommand();
        var applicationResponse = new CreateSaleResponse { Id = Guid.NewGuid() };
        var apiResult = new CreateSaleResult { Id = applicationResponse.Id };
        var expectedLocation = $"/api/sales/{apiResult.Id}";
        var expectedStatusCode = StatusCodes.Status201Created;
        var expectedMessage = "Sale created successfully";
        
        _mapper.Map<CreateSaleCommand>(input).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(applicationResponse);
        _mapper.Map<CreateSaleResult>(applicationResponse).Returns(apiResult);

        var actionResult = await _controller.CreateSale(input, CancellationToken.None);

        var createdResult = actionResult.Should().BeOfType<CreatedResult>().Subject;
        createdResult.Location.Should().Be(expectedLocation);
        createdResult.StatusCode.Should().Be(expectedStatusCode);
        
        var apiResponse = createdResult.Value.Should().BeOfType<ApiResponseWithData<CreateSaleResult>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be(expectedMessage);
        apiResponse.Data.Should().BeSameAs(apiResult);
    }
}
