using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Core.Messaging.Transport.Kafka.Producers;

/// <summary>
/// Configuration settings for Kafka producer
/// </summary>
public class KafkaProducerConfig
{
    /// <summary>
    /// Gets or sets the Confluent.Kafka producer configuration
    /// </summary>
    public ProducerConfig ProducerConfig { get; set; }

    /// <summary>
    /// Gets or sets the Kafka topic name
    /// </summary>
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets the schema registry URL for Avro serialization
    /// </summary>
    public string SchemaRegistryUrl { get; set; }
}

/// <summary>
/// Extension methods for Kafka producer configuration
/// </summary>
public static class KafkaProducerConfigExtensions
{
    /// <summary>
    /// Gets Kafka producer configuration from IConfiguration
    /// </summary>
    /// <param name="configuration">The configuration instance</param>
    /// <returns>Kafka producer configuration</returns>
    public static KafkaProducerConfig GetKafkaProducerConfig(this IConfiguration configuration)
    {
        var config = configuration.GetSection(nameof(KafkaProducerConfig)).Get<KafkaProducerConfig>();
        if (config == null)
            throw new InvalidOperationException($"Section '{nameof(KafkaProducerConfig)}' is not found in configuration.");
        
        if (string.IsNullOrEmpty(config.Topic))
            throw new InvalidOperationException("Kafka topic must be specified in configuration.");
            
        if (config.ProducerConfig == null)
            throw new InvalidOperationException("Kafka producer configuration must be specified.");
            
        return config;
    }
}
