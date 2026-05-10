using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;

public sealed record DeleteSaleCommand : IRequest<DeleteSaleResponse>
{
    public Guid Id { get; set; }
}
