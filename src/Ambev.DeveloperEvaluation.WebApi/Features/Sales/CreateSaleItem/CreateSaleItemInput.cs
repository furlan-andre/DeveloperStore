namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSaleItem;

public sealed record CreateSaleItemInput
{
    public Guid ProductId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
