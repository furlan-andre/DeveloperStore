using System.Diagnostics.CodeAnalysis;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Rebus.Config;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging;

[ExcludeFromCodeCoverage]
public static class MessagingExtensions
{
    public static WebApplicationBuilder AddRabbitMqMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection(RabbitMqOptions.SectionName));

        builder.Services.AddSingleton<IConnection>(provider =>
        {
            var rabbitMqOptions = provider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            if (string.IsNullOrWhiteSpace(rabbitMqOptions.ConnectionString))
                throw new InvalidOperationException("RabbitMQ connection string is not configured.");

            var connectionFactory = new ConnectionFactory
            {
                Uri = new Uri(rabbitMqOptions.ConnectionString)
            };

            return connectionFactory.CreateConnectionAsync().GetAwaiter().GetResult();
        });

        builder.Services.AddScoped<ISalesEventPublisher, RebusSalesEventPublisher>();
        builder.Services.AddHostedService<RabbitMqTopologyInitializer>();

        builder.Services
            .AddHealthChecks()
            .AddRabbitMQ(
                provider => provider.GetRequiredService<IConnection>(),
                name: "rabbitmq",
                tags: ["readiness"]);

        builder.Services.AddRebus((configure, provider) =>
        {
            var rabbitMqOptions = provider.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

            if (string.IsNullOrWhiteSpace(rabbitMqOptions.ConnectionString))
                throw new InvalidOperationException("RabbitMQ connection string is not configured.");

            return configure.Transport(transport =>
                transport.UseRabbitMqAsOneWayClient(rabbitMqOptions.ConnectionString));
        });

        return builder;
    }
}
