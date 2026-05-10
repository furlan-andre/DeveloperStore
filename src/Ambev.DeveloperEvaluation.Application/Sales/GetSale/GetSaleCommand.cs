using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSale;

public sealed record GetSaleCommand : IRequest<GetSaleResponse>
{
    public Guid Id { get; set; }
}
