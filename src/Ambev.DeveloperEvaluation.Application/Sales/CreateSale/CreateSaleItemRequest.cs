namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
