using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

public sealed class GetSaleResult
{
    public Guid Id { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalSaleAmount { get; set; }
    public bool Active { get; set; }
    public IReadOnlyCollection<GetSaleItemResult> Items { get; set; } = [];
}
