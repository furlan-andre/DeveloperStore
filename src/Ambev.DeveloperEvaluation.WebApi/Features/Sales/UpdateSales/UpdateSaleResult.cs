using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;

public class UpdateSaleResult
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
    public IReadOnlyCollection<UpdateSaleItemResult> Items { get; set; } = [];
}
