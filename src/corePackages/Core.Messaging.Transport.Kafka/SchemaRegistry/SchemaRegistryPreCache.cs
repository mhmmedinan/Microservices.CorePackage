using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;

namespace Core.Messaging.Transport.Kafka.SchemaRegistry;

public static class SchemaRegistryPreCache
{
    public static async Task PreCacheEventAsync<TEvent>(
       ISchemaRegistryClient schemaRegistryClient,
       string topicName = "demo")
       where TEvent : ISpecificRecord, new()
    {
        var @event = new TEvent();
        var serializer = GetSchemaRegistryConfig<TEvent>(schemaRegistryClient);
        var context = new SerializationContext(MessageComponentType.Value, topicName);
        _ = await serializer.SerializeAsync(@event, context);
    }

    internal static AvroSerializer<TEvent> GetSchemaRegistryConfig<TEvent>(
        ISchemaRegistryClient schemaRegistryClient)
        where TEvent : ISpecificRecord
    {
        var serializerConfig = new AvroSerializerConfig
        {
            // ksql only runs with TopicName,
            // see https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#sr-schemas-subject-name-strategy
            SubjectNameStrategy = SubjectNameStrategy.Topic
        };

        var serializer = new AvroSerializer<TEvent>(schemaRegistryClient, serializerConfig);
        return serializer;
    }
}
