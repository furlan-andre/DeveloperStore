namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public sealed record ItemCancelledEvent(
    Guid SaleId,
    string SaleNumber,
    Guid ItemId,
    Guid ProductId,
    string ProductDescription,
    int Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal TotalAmount,
    Guid EventId,
    DateTime OccurredAt);
