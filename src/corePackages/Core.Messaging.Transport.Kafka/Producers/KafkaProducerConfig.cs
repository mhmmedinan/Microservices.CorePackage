using Confluent.Kafka;
using Microsoft.Extensions.Configuration;

namespace Core.Messaging.Transport.Kafka.Producers;

public class KafkaProducerConfig
{
    public ProducerConfig ProducerConfig { get; set; }
    public string Topic { get; set; }
}

public static class KafkaProducerConfigExtensions
{
    public static KafkaProducerConfig GetKafkaProducerConfig(this IConfiguration configuration)
    {
        return configuration.GetSection(nameof(KafkaProducerConfig)).Get<KafkaProducerConfig>();
    }
}
