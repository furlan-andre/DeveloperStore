using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItem;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public sealed record UpdateSaleCommand : IRequest<UpdateSaleResponse>
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public IReadOnlyCollection<UpdateSaleItemCommand> Items { get; set; } = [];
}
