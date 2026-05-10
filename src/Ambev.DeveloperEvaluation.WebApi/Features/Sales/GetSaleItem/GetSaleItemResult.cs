namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSaleItem;

public sealed class GetSaleItemResult
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool Active { get; set; }
}
