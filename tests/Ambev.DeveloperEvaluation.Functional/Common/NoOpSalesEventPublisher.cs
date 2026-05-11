using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.Events;

namespace Ambev.DeveloperEvaluation.Functional.Common;

public sealed class NoOpSalesEventPublisher : ISalesEventPublisher
{
    public Task PublishAsync(SaleCreatedEvent saleEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync(SaleModifiedEvent saleEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync(SaleCancelledEvent saleEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync(ItemCancelledEvent saleEvent, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
