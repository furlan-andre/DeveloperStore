namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSaleItem;

public sealed record CreateSaleItemCommand
{
    public Guid ProductId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
