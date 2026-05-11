using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging;

[ExcludeFromCodeCoverage]
public sealed class RabbitMqTopologyInitializer : IHostedService
{
    private readonly IConnection _connection;
    private readonly RabbitMqOptions _options;

    public RabbitMqTopologyInitializer(
        IConnection connection,
        IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.SalesEventsQueue))
            throw new InvalidOperationException("RabbitMQ sales events queue is not configured.");

        await using var channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);
        await channel.QueueDeclareAsync(
            queue: _options.SalesEventsQueue,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
