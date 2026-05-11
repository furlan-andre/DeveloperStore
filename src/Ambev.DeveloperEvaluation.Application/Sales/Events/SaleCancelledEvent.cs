namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

public sealed record SaleCancelledEvent(
    Guid SaleId,
    string SaleNumber,
    DateTime CancelledAt,
    string? Reason,
    Guid EventId,
    DateTime OccurredAt);
