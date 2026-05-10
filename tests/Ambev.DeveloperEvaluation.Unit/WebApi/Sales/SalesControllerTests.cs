using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Unit.WebApi.Sales.CreateSales;
using Ambev.DeveloperEvaluation.Unit.WebApi.Sales.UpdateSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using WebApiDeleteSaleProfile = Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings.DeleteSaleProfile;
using WebApiUpdateSaleProfile = Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings.UpdateSaleProfile;

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

    [Fact(DisplayName = "Should map update input to command, set route id and send through MediatR")]
    public async Task Given_ValidInput_When_UpdatingSale_Then_ShouldSendCommandThroughMediator()
    {
        var saleId = Guid.NewGuid();
        var input = new UpdateSaleInputTestBuilder().Build();
        var command = new UpdateSaleCommand();
        var applicationResponse = new UpdateSaleResponse { Id = saleId, SaleNumber = input.SaleNumber };
        var apiResult = new UpdateSaleResult { Id = applicationResponse.Id, SaleNumber = applicationResponse.SaleNumber };

        _mapper.Map<UpdateSaleCommand>(input).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(applicationResponse);
        _mapper.Map<UpdateSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.UpdateSale(saleId, input, CancellationToken.None);

        command.Id.Should().Be(saleId);
        _mapper.Received(1).Map<UpdateSaleCommand>(input);
        await _mediator.Received(1).Send(command, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<UpdateSaleResult>(applicationResponse);
    }

    [Fact(DisplayName = "Should forward cancellation token to MediatR when updating sale")]
    public async Task Given_CancellationToken_When_UpdatingSale_Then_ShouldForwardTokenToMediator()
    {
        var saleId = Guid.NewGuid();
        var input = new UpdateSaleInputTestBuilder().Build();
        var command = new UpdateSaleCommand();
        var applicationResponse = new UpdateSaleResponse { Id = saleId };
        var apiResult = new UpdateSaleResult { Id = applicationResponse.Id };
        var cancellationToken = new CancellationTokenSource().Token;

        _mapper.Map<UpdateSaleCommand>(input).Returns(command);
        _mediator.Send(command, cancellationToken).Returns(applicationResponse);
        _mapper.Map<UpdateSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.UpdateSale(saleId, input, cancellationToken);

        await _mediator.Received(1).Send(command, cancellationToken);
    }

    [Fact(DisplayName = "Should return ok update sale response")]
    public async Task Given_ValidInput_When_UpdatingSale_Then_ShouldReturnOkResponse()
    {
        var saleId = Guid.NewGuid();
        var input = new UpdateSaleInputTestBuilder().Build();
        var command = new UpdateSaleCommand();
        var itemResult = new UpdateSaleItemResult { Id = Guid.NewGuid(), Active = false };
        var applicationResponse = new UpdateSaleResponse { Id = saleId };
        var apiResult = new UpdateSaleResult
        {
            Id = applicationResponse.Id,
            Active = false,
            Items = [itemResult]
        };
        var expectedStatusCode = StatusCodes.Status200OK;
        var expectedMessage = "Sale updated successfully";

        _mapper.Map<UpdateSaleCommand>(input).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>()).Returns(applicationResponse);
        _mapper.Map<UpdateSaleResult>(applicationResponse).Returns(apiResult);

        var actionResult = await _controller.UpdateSale(saleId, input, CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(expectedStatusCode);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponseWithData<UpdateSaleResult>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be(expectedMessage);
        apiResponse.Data.Should().BeSameAs(apiResult);
        apiResponse.Data!.Active.Should().BeFalse();
        apiResponse.Data.Items.Single().Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should map update sale input including active flags")]
    public void Given_UpdateSaleInput_When_MappingToCommand_Then_ShouldMapActiveFlags()
    {
        var saleActive = false;
        var itemActive = false;
        var itemInput = new UpdateSaleItemInputTestBuilder()
            .WithActive(itemActive)
            .Build();
        
        var input = new UpdateSaleInputTestBuilder()
            .WithActive(saleActive)
            .WithItems([itemInput])
            .Build();

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiUpdateSaleProfile>();
        });
        var mapper = mapperConfiguration.CreateMapper();

        var command = mapper.Map<UpdateSaleCommand>(input);

        command.Active.Should().BeFalse();
        command.Items.Should().ContainSingle();
        command.Items.Single().Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should map update sale response including active flags")]
    public void Given_UpdateSaleResponse_When_MappingToResult_Then_ShouldMapActiveFlags()
    {
        var saleActive = false;
        var itemActive = false;
        var response = new UpdateSaleResponse
        {
            Active = saleActive,
            Items =
            [
                new UpdateSaleItemResponse
                {
                    Id = Guid.NewGuid(),
                    Active = itemActive
                }
            ]
        };

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiUpdateSaleProfile>();
        });
        var mapper = mapperConfiguration.CreateMapper();

        var result = mapper.Map<UpdateSaleResult>(response);

        result.Active.Should().BeFalse();
        result.Items.Should().ContainSingle();
        result.Items.Single().Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should create delete command with route id and send through MediatR")]
    public async Task Given_ValidId_When_DeletingSale_Then_ShouldSendCommandThroughMediator()
    {
        var saleId = Guid.NewGuid();
        var applicationResponse = new DeleteSaleResponse { Id = saleId, Active = false };
        var apiResult = new DeleteSaleResult { Id = saleId, Active = false };

        _mediator
            .Send(Arg.Any<DeleteSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(applicationResponse);
        _mapper.Map<DeleteSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.DeleteSale(saleId, CancellationToken.None);

        await _mediator.Received(1).Send(
            Arg.Is<DeleteSaleCommand>(command => command.Id == saleId),
            Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<DeleteSaleResult>(applicationResponse);
    }

    [Fact(DisplayName = "Should forward cancellation token to MediatR when deleting sale")]
    public async Task Given_CancellationToken_When_DeletingSale_Then_ShouldForwardTokenToMediator()
    {
        var saleId = Guid.NewGuid();
        var applicationResponse = new DeleteSaleResponse { Id = saleId, Active = false };
        var apiResult = new DeleteSaleResult { Id = saleId, Active = false };
        var cancellationToken = new CancellationTokenSource().Token;

        _mediator
            .Send(Arg.Any<DeleteSaleCommand>(), cancellationToken)
            .Returns(applicationResponse);
        _mapper.Map<DeleteSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.DeleteSale(saleId, cancellationToken);

        await _mediator.Received(1).Send(
            Arg.Any<DeleteSaleCommand>(),
            cancellationToken);
    }

    [Fact(DisplayName = "Should return ok delete sale response")]
    public async Task Given_ValidId_When_DeletingSale_Then_ShouldReturnOkResponse()
    {
        var saleId = Guid.NewGuid();
        var applicationResponse = new DeleteSaleResponse { Id = saleId, Active = false };
        var apiResult = new DeleteSaleResult { Id = saleId, Active = false };
        var expectedStatusCode = StatusCodes.Status200OK;
        var expectedMessage = "Sale deleted successfully";

        _mediator
            .Send(Arg.Any<DeleteSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(applicationResponse);
        _mapper.Map<DeleteSaleResult>(applicationResponse).Returns(apiResult);

        var actionResult = await _controller.DeleteSale(saleId, CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(expectedStatusCode);

        var apiResponse = okResult.Value.Should().BeOfType<ApiResponseWithData<DeleteSaleResult>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be(expectedMessage);
        apiResponse.Data.Should().BeSameAs(apiResult);
        apiResponse.Data!.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should map delete sale response to result")]
    public void Given_DeleteSaleResponse_When_MappingToResult_Then_ShouldMapData()
    {
        var saleId = Guid.NewGuid();
        var active = false;
        var response = new DeleteSaleResponse
        {
            Id = saleId,
            Active = active
        };

        var mapperConfiguration = new MapperConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiDeleteSaleProfile>();
        });
        var mapper = mapperConfiguration.CreateMapper();

        var result = mapper.Map<DeleteSaleResult>(response);

        result.Id.Should().Be(saleId);
        result.Active.Should().BeFalse();
    }
}
