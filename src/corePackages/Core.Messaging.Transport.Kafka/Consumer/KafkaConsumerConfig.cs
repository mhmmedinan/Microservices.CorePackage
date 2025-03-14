using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;

namespace Core.Messaging.Transport.Kafka.Consumer;

/// <summary>
/// Configuration settings for Kafka consumer
/// </summary>
public class KafkaConsumerConfig : ConsumerConfig
{
    /// <summary>
    /// The name identifier for Kafka configuration
    /// </summary>
    public const string Name = "Kafka";

    /// <summary>
    /// Gets or sets the list of Kafka topics to subscribe to
    /// </summary>
    public IReadOnlyList<string> Topics { get; set; }

    /// <summary>
    /// Gets or sets the Schema Registry URL for Avro deserialization
    /// </summary>
    public string SchemaRegistryUrl { get; set; } = "http://localhost:8081";

    /// <summary>
    /// Gets or sets the event resolver function for Avro deserialization
    /// </summary>
    public Func<string, byte[], ISchemaRegistryClient, Task<ISpecificRecord>> EventResolver { get; set; }

    /// <summary>
    /// Gets or sets the consumer group ID
    /// </summary>
    public string GroupId { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of messages to process in parallel
    /// </summary>
    public int MaxParallelism { get; set; } = 10;

    /// <summary>
    /// Initializes a new instance of the KafkaConsumerConfig class with default settings
    /// </summary>
    public KafkaConsumerConfig()
    {
        AutoOffsetReset = AutoOffsetReset.Earliest;
        EnableAutoOffsetStore = false;
        EnableAutoCommit = false;
        AllowAutoCreateTopics = true;
    }
}

/// <summary>
/// Extension methods for Kafka consumer configuration
/// </summary>
public static class KafkaConsumerConfigExtensions
{
    /// <summary>
    /// Gets Kafka consumer configuration from IConfiguration
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>Kafka consumer configuration</returns>
    public static KafkaConsumerConfig GetKafkaConsumerConfig(this IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(KafkaConsumerConfig)).Get<KafkaConsumerConfig>();
        if (config == null)
            throw new InvalidOperationException($"Section '{nameof(KafkaConsumerConfig)}' is not found in configuration.");

        if (config.Topics == null || config.Topics.Count == 0)
            throw new InvalidOperationException("At least one Kafka topic must be specified in configuration.");

        if (string.IsNullOrEmpty(config.GroupId))
            throw new InvalidOperationException("Consumer group ID must be specified in configuration.");

        return config;
    }
}
