namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public sealed record SaleCreatedEvent(
    Guid SaleId,
    string SaleNumber,
    Guid CustomerId,
    string CustomerName,
    Guid BranchId,
    string BranchName,
    decimal TotalSaleAmount,
    bool Active,
    Guid EventId,
    DateTime OccurredAt);
