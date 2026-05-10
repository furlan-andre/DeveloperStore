using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSales;

public sealed record UpdateSaleInput
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public bool Active { get; set; }
    public List<UpdateSaleItemInput> Items { get; set; } = [];
}
