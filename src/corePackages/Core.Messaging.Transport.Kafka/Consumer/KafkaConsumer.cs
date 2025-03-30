using Ardalis.GuardClauses;
using Avro.Generic;
using Confluent.Kafka;
using Confluent.Kafka.SyncOverAsync;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Core.Abstractions.Events;
using Core.Abstractions.Messaging.Transport;
using Core.Messaging.Transport.Kafka.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks.Dataflow;

namespace Core.Messaging.Transport.Kafka.Consumer;

/// <summary>
/// Implements the event bus subscriber interface using Apache Kafka
/// </summary>
public class KafkaConsumer : IEventBusSubscriber, IDisposable
{
    private readonly KafkaConsumerConfig _config;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly CachedSchemaRegistryClient _schemaRegistry;
    private readonly IConsumer<string, GenericRecord> _consumer;
    private readonly ActionBlock<ConsumeResult<string, GenericRecord>> _processingBlock;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the KafkaConsumer class
    /// </summary>
    /// <param name="configuration">Application configuration</param>
    /// <param name="serviceScopeFactory">Service scope factory for dependency injection</param>
    /// <param name="logger">Logger instance</param>
    public KafkaConsumer(
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<KafkaConsumer> logger)
    {
        _serviceScopeFactory = Guard.Against.Null(serviceScopeFactory, nameof(serviceScopeFactory));
        _logger = Guard.Against.Null(logger, nameof(logger));
        Guard.Against.Null(configuration, nameof(configuration));

        _config = configuration.GetKafkaConsumerConfig();

        _schemaRegistry = new CachedSchemaRegistryClient(new SchemaRegistryConfig
        {
            Url = _config.SchemaRegistryUrl
        });

        _consumer = new ConsumerBuilder<string, GenericRecord>(_config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Error}", e.Reason))
            .SetStatisticsHandler((_, json) => _logger.LogDebug("Kafka consumer statistics: {Statistics}", json))
            .SetValueDeserializer(new AvroDeserializer<GenericRecord>(_schemaRegistry).AsSyncOverAsync())
            .Build();

        _processingBlock = new ActionBlock<ConsumeResult<string, GenericRecord>>(
            ProcessMessageAsync,
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = _config.MaxParallelism
            });
    }

    /// <summary>
    /// Starts consuming messages from Kafka topics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation(
                "Starting Kafka consumer for topics: {Topics}",
                string.Join(", ", _config.Topics));

            _consumer.Subscribe(_config.Topics);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));
                    if (consumeResult == null) continue;

                    await _processingBlock.SendAsync(consumeResult, cancellationToken);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex,
                        "Error consuming message from Kafka: {ErrorReason}",
                        ex.Error.Reason);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumer stopping due to cancellation");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in Kafka consumer");
            throw;
        }
        finally
        {
            _consumer.Unsubscribe();
        }
    }

    private async Task ProcessMessageAsync(ConsumeResult<string, GenericRecord> consumeResult)
    {
        try
        {
            var fullSchemaName = consumeResult.Message.Value.Schema.SchemaName.Fullname;
            var genericRecord = consumeResult.Message.Value;
            var bytes = await genericRecord.SerializeAsync(_schemaRegistry);

            _logger.LogDebug(
                "Processing message with key {MessageKey} from topic {Topic}",
                consumeResult.Message.Key,
                consumeResult.Topic);

            if (_config.EventResolver != null)
            {
                var @event = await _config.EventResolver(fullSchemaName, bytes, _schemaRegistry);
                if (@event is IEvent eventMessage)
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var eventProcessor = scope.ServiceProvider.GetRequiredService<IEventProcessor>();
                    await eventProcessor.DispatchAsync(eventMessage);

                    _logger.LogInformation(
                        "Successfully processed event {EventType} from topic {Topic}",
                        @event.GetType().Name,
                        consumeResult.Topic);

                    _consumer.Commit(consumeResult);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error processing message from topic {Topic}: {ErrorMessage}",
                consumeResult.Topic,
                ex.Message);
        }
    }

    /// <summary>
    /// Stops the Kafka consumer
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Stopping Kafka consumer");
        _processingBlock.Complete();
        await _processingBlock.Completion;
        Dispose();
    }

    /// <summary>
    /// Disposes of resources used by the Kafka consumer
    /// </summary>
   public void Dispose()
    {
        if (_disposed) return;

        _consumer?.Close();
        _consumer?.Dispose();
        _schemaRegistry?.Dispose();
        _processingBlock.Complete();

        _disposed = true;
    }
}
