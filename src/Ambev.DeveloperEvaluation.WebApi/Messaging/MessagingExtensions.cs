using Ambev.DeveloperEvaluation.Application.Messaging;
using Microsoft.Extensions.Options;
using Rebus.Config;

namespace Ambev.DeveloperEvaluation.WebApi.Messaging;

public static class MessagingExtensions
{
    public static WebApplicationBuilder AddRabbitMqMessaging(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection(RabbitMqOptions.SectionName));

        builder.Services.AddScoped<ISalesEventPublisher, RebusSalesEventPublisher>();

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
