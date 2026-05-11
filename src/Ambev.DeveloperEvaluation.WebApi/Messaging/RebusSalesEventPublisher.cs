using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Microsoft.Extensions.Options;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging;

public sealed class RebusSalesEventPublisher : ISalesEventPublisher
{
    private readonly IBus _bus;
    private readonly RabbitMqOptions _options;

    public RebusSalesEventPublisher(
        IBus bus,
        IOptions<RabbitMqOptions> options)
    {
        _bus = bus;
        _options = options.Value;
    }

    public Task PublishAsync(SaleCreatedEvent saleEvent, CancellationToken cancellationToken)
    {
        return SendAsync(saleEvent, cancellationToken);
    }

    public Task PublishAsync(SaleModifiedEvent saleEvent, CancellationToken cancellationToken)
    {
        return SendAsync(saleEvent, cancellationToken);
    }

    public Task PublishAsync(SaleCancelledEvent saleEvent, CancellationToken cancellationToken)
    {
        return SendAsync(saleEvent, cancellationToken);
    }

    public Task PublishAsync(ItemCancelledEvent saleEvent, CancellationToken cancellationToken)
    {
        return SendAsync(saleEvent, cancellationToken);
    }

    private Task SendAsync(object saleEvent, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(_options.SalesEventsQueue))
            throw new InvalidOperationException("RabbitMQ sales events queue is not configured.");

        return _bus.Advanced.Routing.Send(_options.SalesEventsQueue, saleEvent);
    }
}
