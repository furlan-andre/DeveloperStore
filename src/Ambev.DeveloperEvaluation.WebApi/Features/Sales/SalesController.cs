using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Common.Errors;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<ListSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ListSales(CancellationToken cancellationToken)
    {
        var queryParameters = GetQueryParameters();
        var command = new ListSalesCommand
        {
            Page = queryParameters.Page,
            Size = queryParameters.Size,
            Order = queryParameters.Order,
            Filters = queryParameters.Filters
        };
        
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.ToActionResult(response =>
        {
            var apiResult = _mapper.Map<IReadOnlyCollection<ListSaleResult>>(response.Items);

            return new OkObjectResult(new PaginatedResponse<ListSaleResult>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = apiResult,
                CurrentPage = response.CurrentPage,
                TotalPages = response.TotalPages,
                TotalItems = response.TotalItems
            });
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new GetSaleCommand { Id = id };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult(response =>
        {
            var apiResult = _mapper.Map<GetSaleResult>(response);

            return new OkObjectResult(new ApiResponseWithData<GetSaleResult>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = apiResult
            });
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleInput input,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateSaleCommand>(input);
        
        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult(response =>
        {
            var apiResult = _mapper.Map<CreateSaleResult>(response);

            return Created($"/api/sales/{apiResult.Id}", new ApiResponseWithData<CreateSaleResult>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = apiResult
            });
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateSale(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleInput input,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateSaleCommand>(input);
        command.Id = id;

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult(response =>
        {
            var apiResult = _mapper.Map<UpdateSaleResult>(response);

            return new OkObjectResult(new ApiResponseWithData<UpdateSaleResult>
            {
                Success = true,
                Message = "Sale updated successfully",
                Data = apiResult
            });
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSaleCommand { Id = id };

        var result = await _mediator.Send(command, cancellationToken);

        return result.ToActionResult(response =>
        {
            var apiResult = _mapper.Map<DeleteSaleResult>(response);

            return new OkObjectResult(new ApiResponseWithData<DeleteSaleResult>
            {
                Success = true,
                Message = "Sale deleted successfully",
                Data = apiResult
            });
        });
    }
}
