using Ardalis.GuardClauses;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Core.Messaging.Transport.Kafka.Producers;

/// <summary>
/// Implements the event bus publisher interface using Apache Kafka
/// </summary>
public class KafkaProducer : IEventBusPublisher
{
    private readonly KafkaProducerConfig _config;
    private readonly ILogger<KafkaProducer> _logger;
    private readonly ISchemaRegistryClient _schemaRegistry;
    private readonly IProducer<string, string> _producer;

    /// <summary>
    /// Initializes a new instance of the KafkaProducer class
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="logger">Logger instance</param>
    public KafkaProducer(IConfiguration configuration, ILogger<KafkaProducer> logger)
    {
        Guard.Against.Null(configuration, nameof(configuration));
        Guard.Against.Null(logger, nameof(logger));

        _config = configuration.GetKafkaProducerConfig();
        _logger = logger;

        if (!string.IsNullOrEmpty(_config.SchemaRegistryUrl))
        {
            _schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
            {
                Url = _config.SchemaRegistryUrl
            });
        }

        _producer = new ProducerBuilder<string, string>(_config.ProducerConfig)
            .SetErrorHandler((_, e) => 
                _logger.LogError("Kafka producer error: {Error}", e.Reason))
            .Build();
    }

    /// <summary>
    /// Publishes an integration event to Kafka
    /// </summary>
    /// <typeparam name="TEvent">Type of the integration event</typeparam>
    /// <param name="integrationEvent">The event to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        try
        {
            Guard.Against.Null(integrationEvent, nameof(integrationEvent));

            var eventType = integrationEvent.GetType();
            var data = JsonSerializer.Serialize(integrationEvent);

            _logger.LogInformation(
                "Publishing event {EventType} with ID {EventId} to topic {Topic}",
                eventType.Name,
                integrationEvent.EventId,
                _config.Topic);

            var message = new Message<string, string>
            {
                Key = eventType.Name,
                Value = data,
                Headers = new Headers
                {
                    { "event-id", System.Text.Encoding.UTF8.GetBytes(integrationEvent.EventId.ToString()) },
                    { "occurred-on", System.Text.Encoding.UTF8.GetBytes(integrationEvent.OccurredOn.ToString("O")) }
                }
            };

            var deliveryResult = await _producer.ProduceAsync(
                _config.Topic,
                message,
                cancellationToken);

            _logger.LogInformation(
                "Successfully published event {EventType} with ID {EventId} to partition {Partition} at offset {Offset}",
                eventType.Name,
                integrationEvent.EventId,
                deliveryResult.Partition.Value,
                deliveryResult.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex,
                "Failed to publish event {EventType} with ID {EventId}: {ErrorReason}",
                integrationEvent.GetType().Name,
                integrationEvent.EventId,
                ex.Error.Reason);
            throw;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _producer?.Dispose();
        _schemaRegistry?.Dispose();
    }
}
