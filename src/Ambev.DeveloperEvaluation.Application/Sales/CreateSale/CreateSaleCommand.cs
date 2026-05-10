using Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public sealed record CreateSaleCommand : IRequest<CreateSaleResponse>
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public IReadOnlyCollection<CreateSaleItemCommand> Items { get; set; } = [];
}
