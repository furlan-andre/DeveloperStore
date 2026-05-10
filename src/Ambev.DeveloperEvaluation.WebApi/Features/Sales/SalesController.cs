using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSales;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.DeleteSales;
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

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale(
        [FromBody] CreateSaleInput input,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<CreateSaleCommand>(input);
        
        var result = await _mediator.Send(command, cancellationToken);
        
        var response = _mapper.Map<CreateSaleResult>(result);

        return Created($"/api/sales/{response.Id}", new ApiResponseWithData<CreateSaleResult>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = response
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale(
        [FromRoute] Guid id,
        [FromBody] UpdateSaleInput input,
        CancellationToken cancellationToken)
    {
        var command = _mapper.Map<UpdateSaleCommand>(input);
        command.Id = id;

        var result = await _mediator.Send(command, cancellationToken);
        var response = _mapper.Map<UpdateSaleResult>(result);

        return new OkObjectResult(new ApiResponseWithData<UpdateSaleResult>
        {
            Success = true,
            Message = "Sale updated successfully",
            Data = response
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<DeleteSaleResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteSale(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteSaleCommand { Id = id };

        var result = await _mediator.Send(command, cancellationToken);
        var response = _mapper.Map<DeleteSaleResult>(result);

        return new OkObjectResult(new ApiResponseWithData<DeleteSaleResult>
        {
            Success = true,
            Message = "Sale deleted successfully",
            Data = response
        });
    }
}
