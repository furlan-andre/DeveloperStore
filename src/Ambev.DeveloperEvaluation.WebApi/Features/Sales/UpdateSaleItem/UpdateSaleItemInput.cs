namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItem;

public sealed record UpdateSaleItemInput
{
    public Guid? Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public bool Active { get; set; } = true;
}
