namespace Ambev.DeveloperEvaluation.WebApi.Messaging;

public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    public string ConnectionString { get; set; } = string.Empty;
    public string SalesEventsQueue { get; set; } = string.Empty;
}
