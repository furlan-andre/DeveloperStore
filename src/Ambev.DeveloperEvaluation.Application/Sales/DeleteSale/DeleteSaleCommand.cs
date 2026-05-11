using Ambev.DeveloperEvaluation.Common.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public sealed record DeleteSaleCommand : IRequest<Result<DeleteSaleResponse>>
{
    public Guid Id { get; set; }
}
