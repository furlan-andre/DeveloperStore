using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.WebApi.Messaging;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Rebus.Bus;
using Rebus.Bus.Advanced;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.WebApi.Messaging;

public class RebusSalesEventPublisherTests
{
    private readonly IBus _bus;
    private readonly IRoutingApi _routingApi;
    private readonly RebusSalesEventPublisher _publisher;
    private readonly string _salesEventsQueue = "developerstore.sales.events";

    public RebusSalesEventPublisherTests()
    {
        _bus = Substitute.For<IBus>();
        var advancedApi = Substitute.For<IAdvancedApi>();
        _routingApi = Substitute.For<IRoutingApi>();

        _bus.Advanced.Returns(advancedApi);
        advancedApi.Routing.Returns(_routingApi);

        var options = Options.Create(new RabbitMqOptions
        {
            ConnectionString = "amqp://guest:guest@localhost:5672",
            SalesEventsQueue = _salesEventsQueue
        });

        _publisher = new RebusSalesEventPublisher(_bus, options);
    }

    [Fact(DisplayName = "Should send sale created event to configured queue")]
    public async Task Given_SaleCreatedEvent_When_Publishing_Then_ShouldSendToConfiguredQueue()
    {
        var saleEvent = CreateSaleCreatedEvent();

        await _publisher.PublishAsync(saleEvent, CancellationToken.None);

        await _routingApi.Received(1).Send(_salesEventsQueue, saleEvent);
    }

    [Fact(DisplayName = "Should send sale modified event to configured queue")]
    public async Task Given_SaleModifiedEvent_When_Publishing_Then_ShouldSendToConfiguredQueue()
    {
        var saleEvent = CreateSaleModifiedEvent();

        await _publisher.PublishAsync(saleEvent, CancellationToken.None);

        await _routingApi.Received(1).Send(_salesEventsQueue, saleEvent);
    }

    [Fact(DisplayName = "Should send sale cancelled event to configured queue")]
    public async Task Given_SaleCancelledEvent_When_Publishing_Then_ShouldSendToConfiguredQueue()
    {
        var saleEvent = CreateSaleCancelledEvent();

        await _publisher.PublishAsync(saleEvent, CancellationToken.None);

        await _routingApi.Received(1).Send(_salesEventsQueue, saleEvent);
    }

    [Fact(DisplayName = "Should send item cancelled event to configured queue")]
    public async Task Given_ItemCancelledEvent_When_Publishing_Then_ShouldSendToConfiguredQueue()
    {
        var saleEvent = CreateItemCancelledEvent();

        await _publisher.PublishAsync(saleEvent, CancellationToken.None);

        await _routingApi.Received(1).Send(_salesEventsQueue, saleEvent);
    }

    [Fact(DisplayName = "Should throw InvalidOperationException when sales events queue is not configured")]
    public async Task Given_EmptySalesEventsQueue_When_Publishing_Then_ShouldThrowInvalidOperationException()
    {
        var options = Options.Create(new RabbitMqOptions
        {
            ConnectionString = "amqp://guest:guest@localhost:5672",
            SalesEventsQueue = string.Empty
        });
        var publisher = new RebusSalesEventPublisher(_bus, options);
        var saleEvent = CreateSaleCreatedEvent();

        var action = async () => await publisher.PublishAsync(saleEvent, CancellationToken.None);

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("RabbitMQ sales events queue is not configured.");
    }

    [Fact(DisplayName = "Should throw OperationCanceledException when cancellation was requested")]
    public async Task Given_CancelledToken_When_Publishing_Then_ShouldThrowOperationCanceledException()
    {
        var saleEvent = CreateSaleCreatedEvent();
        using var cancellationTokenSource = new CancellationTokenSource();
        await cancellationTokenSource.CancelAsync();

        var action = async () => await _publisher.PublishAsync(saleEvent, cancellationTokenSource.Token);

        await action.Should().ThrowAsync<OperationCanceledException>();
        await _routingApi.DidNotReceive().Send(Arg.Any<string>(), Arg.Any<object>());
    }

    private static SaleCreatedEvent CreateSaleCreatedEvent()
    {
        return new SaleCreatedEvent(
            Guid.NewGuid(),
            "SALE-001",
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            100m,
            true,
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static SaleModifiedEvent CreateSaleModifiedEvent()
    {
        return new SaleModifiedEvent(
            Guid.NewGuid(),
            "SALE-001",
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch",
            100m,
            true,
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static SaleCancelledEvent CreateSaleCancelledEvent()
    {
        return new SaleCancelledEvent(
            Guid.NewGuid(),
            "SALE-001",
            DateTime.UtcNow,
            "Cancelled",
            Guid.NewGuid(),
            DateTime.UtcNow);
    }

    private static ItemCancelledEvent CreateItemCancelledEvent()
    {
        return new ItemCancelledEvent(
            Guid.NewGuid(),
            "SALE-001",
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Product",
            4,
            100m,
            40m,
            360m,
            Guid.NewGuid(),
            DateTime.UtcNow);
    }
}
