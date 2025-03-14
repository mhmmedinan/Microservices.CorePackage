using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;

namespace Core.Messaging.Transport.Kafka.Consumer;

public class KafkaConsumerConfig:ConsumerConfig
{
    public const string Name = "Kafka";
    public IReadOnlyList<string> Topics { get; set; }

    public string SchemaRegistryUrl { get; set; } = "http://localhost:8081";

    public Func<string, byte[], ISchemaRegistryClient, Task<ISpecificRecord>> EventResolver { get; set; }

    public KafkaConsumerConfig()
    {
        AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        EnableAutoOffsetStore = false;
    }
}

public static class KafkaConsumerConfigExtensions
{
    public static KafkaConsumerConfig GetKafkaConsumerConfig(this IConfiguration configuration)
    {
        return configuration.GetSection(nameof(KafkaConsumerConfig)).Get<KafkaConsumerConfig>();
    }
}
