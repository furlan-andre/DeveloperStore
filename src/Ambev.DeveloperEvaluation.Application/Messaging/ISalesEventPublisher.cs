using Ambev.DeveloperEvaluation.Application.Sales.Events;

namespace Ambev.DeveloperEvaluation.Application.Messaging;

public interface ISalesEventPublisher
{
    Task PublishAsync(SaleCreatedEvent saleEvent, CancellationToken cancellationToken);
    Task PublishAsync(SaleModifiedEvent saleEvent, CancellationToken cancellationToken);
    Task PublishAsync(SaleCancelledEvent saleEvent, CancellationToken cancellationToken);
    Task PublishAsync(ItemCancelledEvent saleEvent, CancellationToken cancellationToken);
}
