using Ambev.DeveloperEvaluation.Common.Results;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed record GetSaleCommand : IRequest<Result<GetSaleResponse>>
{
    public Guid Id { get; set; }
}
