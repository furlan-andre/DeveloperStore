using Ambev.DeveloperEvaluation.Application.Common.Pagination;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.Common.Results;
using Ambev.DeveloperEvaluation.Unit.WebApi.Sales.CreateSales;
using Ambev.DeveloperEvaluation.Unit.WebApi.Sales.UpdateSales;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using AutoMapper;
using Ambev.DeveloperEvaluation.Unit.Common;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Xunit;
using WebApiCreateSaleProfile = Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings.CreateSaleProfile;
using WebApiDeleteSaleProfile = Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings.DeleteSaleProfile;
using WebApiGetSaleProfile = Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings.GetSaleProfile;
using WebApiListSalesProfile = Ambev.DeveloperEvaluation.WebApi.Features.Sales.Mappings.ListSalesProfile;
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

    [Fact(DisplayName = "Should create list sales command from query and send through MediatR")]
    public async Task Given_QueryParameters_When_ListingSales_Then_ShouldSendCommandThroughMediator()
    {
        var page = 2;
        var size = 20;
        var order = "saleDate desc";
        var active = "true";
        var saleNumber = "SALE*";
        var applicationResponse = new PagedResponse<ListSaleResponse>
        {
            Items = [],
            CurrentPage = page,
            TotalPages = 1,
            TotalItems = 0,
            PageSize = size
        };
        IReadOnlyCollection<ListSaleResult> apiResult = [];
        ConfigureQueryString($"?_page={page}&_size={size}&_order={Uri.EscapeDataString(order)}&active={active}&saleNumber={saleNumber}");

        _mediator
            .Send(Arg.Any<ListSalesCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<ListSaleResponse>>.Success(applicationResponse));
        _mapper.Map<IReadOnlyCollection<ListSaleResult>>(applicationResponse.Items).Returns(apiResult);

        await _controller.ListSales(CancellationToken.None);

        await _mediator.Received(1).Send(
            Arg.Is<ListSalesCommand>(command =>
                command.Page == page &&
                command.Size == size &&
                command.Order == order &&
                command.Filters.Count == 2 &&
                command.Filters["active"] == active &&
                command.Filters["saleNumber"] == saleNumber),
            Arg.Any<CancellationToken>());
        
        _mapper.Received(1).Map<IReadOnlyCollection<ListSaleResult>>(applicationResponse.Items);
    }

    [Fact(DisplayName = "Should forward cancellation token to MediatR when listing sales")]
    public async Task Given_CancellationToken_When_ListingSales_Then_ShouldForwardTokenToMediator()
    {
        var cancellationToken = new CancellationTokenSource().Token;
        var applicationResponse = new PagedResponse<ListSaleResponse>();
        IReadOnlyCollection<ListSaleResult> apiResult = [];
        ConfigureQueryString("?_page=1&_size=10");

        _mediator
            .Send(Arg.Any<ListSalesCommand>(), cancellationToken)
            .Returns(Result<PagedResponse<ListSaleResponse>>.Success(applicationResponse));
        _mapper.Map<IReadOnlyCollection<ListSaleResult>>(applicationResponse.Items).Returns(apiResult);

        await _controller.ListSales(cancellationToken);

        await _mediator.Received(1).Send(
            Arg.Any<ListSalesCommand>(),
            cancellationToken);
    }

    [Fact(DisplayName = "Should return paginated list sales response")]
    public async Task Given_QueryParameters_When_ListingSales_Then_ShouldReturnPaginatedResponse()
    {
        var page = 2;
        var totalPages = 3;
        var totalItems = 25;
        var saleResult = new ListSaleResult { Id = Guid.NewGuid() };
        
        var applicationResponse = new PagedResponse<ListSaleResponse>
        {
            Items = [new ListSaleResponse { Id = saleResult.Id }],
            CurrentPage = page,
            TotalPages = totalPages,
            TotalItems = totalItems,
            PageSize = 10
        };
        
        IReadOnlyCollection<ListSaleResult> apiResult = [saleResult];
        var expectedStatusCode = StatusCodes.Status200OK;
        var expectedMessage = "Sales retrieved successfully";
        ConfigureQueryString("?_page=2&_size=10");

        _mediator
            .Send(Arg.Any<ListSalesCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<ListSaleResponse>>.Success(applicationResponse));
        
        _mapper.Map<IReadOnlyCollection<ListSaleResult>>(applicationResponse.Items).Returns(apiResult);

        var actionResult = await _controller.ListSales(CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<PaginatedResponse<ListSaleResult>>().Subject;
        
        okResult.StatusCode.Should().Be(expectedStatusCode);
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be(expectedMessage);
        apiResponse.Data.Should().BeSameAs(apiResult);
        apiResponse.CurrentPage.Should().Be(page);
        apiResponse.TotalPages.Should().Be(totalPages);
        apiResponse.TotalItems.Should().Be(totalItems);
    }

    [Fact(DisplayName = "Should create get sale command with route id and send through MediatR")]
    public async Task Given_ValidId_When_GettingSale_Then_ShouldSendCommandThroughMediator()
    {
        var saleId = Guid.NewGuid();
        var applicationResponse = new GetSaleResponse { Id = saleId };
        var apiResult = new GetSaleResult { Id = saleId };

        _mediator
            .Send(Arg.Any<GetSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<GetSaleResponse>.Success(applicationResponse));
        _mapper.Map<GetSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.GetSale(saleId, CancellationToken.None);

        await _mediator.Received(1).Send(
            Arg.Is<GetSaleCommand>(command => command.Id == saleId),
            Arg.Any<CancellationToken>());
        
        _mapper.Received(1).Map<GetSaleResult>(applicationResponse);
    }

    [Fact(DisplayName = "Should forward cancellation token to MediatR when getting sale")]
    public async Task Given_CancellationToken_When_GettingSale_Then_ShouldForwardTokenToMediator()
    {
        var saleId = Guid.NewGuid();
        var applicationResponse = new GetSaleResponse { Id = saleId };
        var apiResult = new GetSaleResult { Id = saleId };
        var cancellationToken = new CancellationTokenSource().Token;

        _mediator
            .Send(Arg.Any<GetSaleCommand>(), cancellationToken)
            .Returns(Result<GetSaleResponse>.Success(applicationResponse));
        _mapper.Map<GetSaleResult>(applicationResponse).Returns(apiResult);

        await _controller.GetSale(saleId, cancellationToken);

        await _mediator.Received(1).Send(
            Arg.Any<GetSaleCommand>(),
            cancellationToken);
    }

    [Fact(DisplayName = "Should return ok get sale response")]
    public async Task Given_ValidId_When_GettingSale_Then_ShouldReturnOkResponse()
    {
        var saleId = Guid.NewGuid();
        var applicationResponse = new GetSaleResponse { Id = saleId };
        var apiResult = new GetSaleResult { Id = saleId };
        var expectedStatusCode = StatusCodes.Status200OK;
        var expectedMessage = "Sale retrieved successfully";

        _mediator
            .Send(Arg.Any<GetSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<GetSaleResponse>.Success(applicationResponse));
        
        _mapper.Map<GetSaleResult>(applicationResponse).Returns(apiResult);

        var actionResult = await _controller.GetSale(saleId, CancellationToken.None);

        var okResult = actionResult.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponseWithData<GetSaleResult>>().Subject;
        
        okResult.StatusCode.Should().Be(expectedStatusCode);
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be(expectedMessage);
        apiResponse.Data.Should().BeSameAs(apiResult);
    }

    [Fact(DisplayName = "Should map input to command and send through MediatR")]
    public async Task Given_ValidInput_When_CreatingSale_Then_ShouldSendCommandThroughMediator()
    {
        var input = new CreateSaleInputTestBuilder().Build();
        var command = new CreateSaleCommand { SaleNumber = input.SaleNumber };
        var applicationResponse = new CreateSaleResponse { Id = Guid.NewGuid(), SaleNumber = input.SaleNumber };
        var apiResult = new CreateSaleResult { Id = applicationResponse.Id, SaleNumber = applicationResponse.SaleNumber };
        
        _mapper.Map<CreateSaleCommand>(input).Returns(command);
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(Result<CreateSaleResponse>.Success(applicationResponse));
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
        _mediator.Send(command, cancellationToken).Returns(Result<CreateSaleResponse>.Success(applicationResponse));
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
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(Result<CreateSaleResponse>.Success(applicationResponse));
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

    [Fact(DisplayName = "Should map create sale input to command")]
    public void Given_CreateSaleInput_When_MappingToCommand_Then_ShouldMapData()
    {
        var itemInput = new CreateSaleItemInputTestBuilder().Build();
        var input = new CreateSaleInputTestBuilder()
            .WithItems([itemInput])
            .Build();

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiCreateSaleProfile>();
        });
        var mapper = mapperConfiguration.CreateMapper();

        var command = mapper.Map<CreateSaleCommand>(input);

        command.SaleNumber.Should().Be(input.SaleNumber);
        command.SaleDate.Should().Be(input.SaleDate);
        command.CustomerId.Should().Be(input.CustomerId);
        command.CustomerName.Should().Be(input.CustomerName);
        command.BranchId.Should().Be(input.BranchId);
        command.BranchName.Should().Be(input.BranchName);
        command.Items.Should().ContainSingle();
        command.Items.Single().ProductId.Should().Be(itemInput.ProductId);
        command.Items.Single().ProductDescription.Should().Be(itemInput.ProductDescription);
        command.Items.Single().Quantity.Should().Be(itemInput.Quantity);
        command.Items.Single().UnitPrice.Should().Be(itemInput.UnitPrice);
    }

    [Fact(DisplayName = "Should map create sale response to result")]
    public void Given_CreateSaleResponse_When_MappingToResult_Then_ShouldMapData()
    {
        var itemResponse = new CreateSaleItemResponse
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ProductDescription = "Product",
            Quantity = 4,
            UnitPrice = 100m,
            Discount = 40m,
            TotalAmount = 360m
        };
        
        var response = new CreateSaleResponse
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-001",
            Items = [itemResponse]
        };

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiCreateSaleProfile>();
        });
        
        var mapper = mapperConfiguration.CreateMapper();

        var result = mapper.Map<CreateSaleResult>(response);

        result.Id.Should().Be(response.Id);
        result.SaleNumber.Should().Be(response.SaleNumber);
        result.Items.Should().ContainSingle();
        
        var itemResult = result.Items.Single();
        
        itemResult.Id.Should().Be(itemResponse.Id);
        itemResult.ProductId.Should().Be(itemResponse.ProductId);
        itemResult.ProductDescription.Should().Be(itemResponse.ProductDescription);
        itemResult.Quantity.Should().Be(itemResponse.Quantity);
        itemResult.UnitPrice.Should().Be(itemResponse.UnitPrice);
        itemResult.Discount.Should().Be(itemResponse.Discount);
        itemResult.TotalAmount.Should().Be(itemResponse.TotalAmount);
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
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(Result<UpdateSaleResponse>.Success(applicationResponse));
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
        _mediator.Send(command, cancellationToken).Returns(Result<UpdateSaleResponse>.Success(applicationResponse));
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
        _mediator.Send(command, Arg.Any<CancellationToken>())
            .Returns(Result<UpdateSaleResponse>.Success(applicationResponse));
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

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
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

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
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
            .Returns(Result<DeleteSaleResponse>.Success(applicationResponse));
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
            .Returns(Result<DeleteSaleResponse>.Success(applicationResponse));
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
            .Returns(Result<DeleteSaleResponse>.Success(applicationResponse));
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

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiDeleteSaleProfile>();
        });
        var mapper = mapperConfiguration.CreateMapper();

        var result = mapper.Map<DeleteSaleResult>(response);

        result.Id.Should().Be(saleId);
        result.Active.Should().BeFalse();
    }

    [Fact(DisplayName = "Should map get sale response to result including items")]
    public void Given_GetSaleResponse_When_MappingToResult_Then_ShouldMapData()
    {
        var itemResponse = new GetSaleItemResponse
        {
            Id = Guid.NewGuid(),
            ProductId = Guid.NewGuid(),
            ProductDescription = "Product",
            Quantity = 4,
            UnitPrice = 100m,
            Discount = 40m,
            TotalAmount = 360m,
            Active = true
        };
        
        var response = new GetSaleResponse
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-001",
            Active = true,
            Items = [itemResponse]
        };

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiGetSaleProfile>();
        });
        
        var mapper = mapperConfiguration.CreateMapper();

        var result = mapper.Map<GetSaleResult>(response);

        result.Id.Should().Be(response.Id);
        result.SaleNumber.Should().Be(response.SaleNumber);
        result.Active.Should().BeTrue();
        result.Items.Should().ContainSingle();

        var itemResult = result.Items.Single();
        itemResult.Id.Should().Be(itemResponse.Id);
        itemResult.ProductId.Should().Be(itemResponse.ProductId);
        itemResult.ProductDescription.Should().Be(itemResponse.ProductDescription);
        itemResult.Quantity.Should().Be(itemResponse.Quantity);
        itemResult.UnitPrice.Should().Be(itemResponse.UnitPrice);
        itemResult.Discount.Should().Be(itemResponse.Discount);
        itemResult.TotalAmount.Should().Be(itemResponse.TotalAmount);
        itemResult.Active.Should().BeTrue();
    }

    [Fact(DisplayName = "Should map list sale response to result")]
    public void Given_ListSaleResponse_When_MappingToResult_Then_ShouldMapData()
    {
        var response = new ListSaleResponse
        {
            Id = Guid.NewGuid(),
            SaleNumber = "SALE-001",
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = "Customer",
            BranchId = Guid.NewGuid(),
            BranchName = "Branch",
            TotalSaleAmount = 360m,
            Active = true
        };

        var mapperConfiguration = AutoMapperTestHelper.CreateConfiguration(configuration =>
        {
            configuration.AddProfile<WebApiListSalesProfile>();
        });
        var mapper = mapperConfiguration.CreateMapper();

        var result = mapper.Map<ListSaleResult>(response);

        result.Id.Should().Be(response.Id);
        result.SaleNumber.Should().Be(response.SaleNumber);
        result.SaleDate.Should().Be(response.SaleDate);
        result.CustomerId.Should().Be(response.CustomerId);
        result.CustomerName.Should().Be(response.CustomerName);
        result.BranchId.Should().Be(response.BranchId);
        result.BranchName.Should().Be(response.BranchName);
        result.TotalSaleAmount.Should().Be(response.TotalSaleAmount);
        result.Active.Should().BeTrue();
    }

    [Fact(DisplayName = "Should return error response when listing sales fails")]
    public async Task Given_FailureResult_When_ListingSales_Then_ShouldReturnErrorResponse()
    {
        var error = Error.Validation("Invalid input data", "Order field is unsupported.");
        ConfigureQueryString("?_order=unsupported");

        _mediator
            .Send(Arg.Any<ListSalesCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<PagedResponse<ListSaleResponse>>.Failure(error));

        var actionResult = await _controller.ListSales(CancellationToken.None);

        AssertErrorResponse(actionResult, StatusCodes.Status400BadRequest, error);
    }

    [Fact(DisplayName = "Should return error response when getting sale fails")]
    public async Task Given_FailureResult_When_GettingSale_Then_ShouldReturnErrorResponse()
    {
        var saleId = Guid.NewGuid();
        var error = Error.ResourceNotFound("Sale not found", $"The sale with ID {saleId} does not exist.");

        _mediator
            .Send(Arg.Any<GetSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<GetSaleResponse>.Failure(error));

        var actionResult = await _controller.GetSale(saleId, CancellationToken.None);

        AssertErrorResponse(actionResult, StatusCodes.Status404NotFound, error);
    }

    [Fact(DisplayName = "Should return error response when creating sale fails")]
    public async Task Given_FailureResult_When_CreatingSale_Then_ShouldReturnErrorResponse()
    {
        var input = new CreateSaleInputTestBuilder().Build();
        var command = new CreateSaleCommand();
        var error = Error.DomainRuleViolation("Sale domain rule violated", "Invalid sale item quantity.");

        _mapper.Map<CreateSaleCommand>(input).Returns(command);
        _mediator
            .Send(command, Arg.Any<CancellationToken>())
            .Returns(Result<CreateSaleResponse>.Failure(error));

        var actionResult = await _controller.CreateSale(input, CancellationToken.None);

        AssertErrorResponse(actionResult, StatusCodes.Status400BadRequest, error);
    }

    [Fact(DisplayName = "Should return error response when updating sale fails")]
    public async Task Given_FailureResult_When_UpdatingSale_Then_ShouldReturnErrorResponse()
    {
        var saleId = Guid.NewGuid();
        var input = new UpdateSaleInputTestBuilder().Build();
        var command = new UpdateSaleCommand();
        var error = Error.ResourceNotFound("Sale not found", $"The sale with ID {saleId} does not exist.");

        _mapper.Map<UpdateSaleCommand>(input).Returns(command);
        _mediator
            .Send(command, Arg.Any<CancellationToken>())
            .Returns(Result<UpdateSaleResponse>.Failure(error));

        var actionResult = await _controller.UpdateSale(saleId, input, CancellationToken.None);

        AssertErrorResponse(actionResult, StatusCodes.Status404NotFound, error);
    }

    [Fact(DisplayName = "Should return error response when deleting sale fails")]
    public async Task Given_FailureResult_When_DeletingSale_Then_ShouldReturnErrorResponse()
    {
        var saleId = Guid.NewGuid();
        var error = Error.ResourceNotFound("Sale not found", $"The sale with ID {saleId} does not exist.");

        _mediator
            .Send(Arg.Any<DeleteSaleCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<DeleteSaleResponse>.Failure(error));

        var actionResult = await _controller.DeleteSale(saleId, CancellationToken.None);

        AssertErrorResponse(actionResult, StatusCodes.Status404NotFound, error);
    }

    private void ConfigureQueryString(string queryString)
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                Request =
                {
                    QueryString = new QueryString(queryString)
                }
            }
        };
    }

    private static void AssertErrorResponse(IActionResult actionResult, int expectedStatusCode, Error error)
    {
        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        var errorResponse = objectResult.Value.Should().BeOfType<ErrorResponse>().Subject;

        objectResult.StatusCode.Should().Be(expectedStatusCode);
        errorResponse.Type.Should().Be(error.Type);
        errorResponse.Error.Should().Be(error.ErrorMessage);
        errorResponse.Detail.Should().Be(error.Detail);
    }
}
