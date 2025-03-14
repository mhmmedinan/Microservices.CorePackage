using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;

namespace Core.Messaging.Transport.Kafka.SchemaRegistry;

/// <summary>
/// Provides pre-caching functionality for Schema Registry to improve performance
/// </summary>
public static class SchemaRegistryPreCache
{
    /// <summary>
    /// Pre-caches the schema for a specific event type to improve serialization performance
    /// </summary>
    /// <typeparam name="TEvent">Type of the event to pre-cache</typeparam>
    /// <param name="schemaRegistryClient">Schema Registry client</param>
    /// <param name="topicName">Optional topic name for context</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public static async Task PreCacheEventAsync<TEvent>(
       ISchemaRegistryClient schemaRegistryClient,
       string topicName = "demo")
       where TEvent : ISpecificRecord, new()
    {
        if (schemaRegistryClient == null)
            throw new ArgumentNullException(nameof(schemaRegistryClient));

        if (string.IsNullOrEmpty(topicName))
            throw new ArgumentException("Topic name cannot be null or empty", nameof(topicName));

        try
        {
            var @event = new TEvent();
            var serializer = GetSchemaRegistryConfig<TEvent>(schemaRegistryClient);
            var context = new SerializationContext(MessageComponentType.Value, topicName);
            _ = await serializer.SerializeAsync(@event, context);
        }
        catch (Exception ex)
        {
            throw new SchemaRegistryException(
                $"Failed to pre-cache schema for event type {typeof(TEvent).Name}",
                ex);
        }
    }

    /// <summary>
    /// Gets an Avro serializer configured for Schema Registry
    /// </summary>
    /// <typeparam name="TEvent">Type of the event to serialize</typeparam>
    /// <param name="schemaRegistryClient">Schema Registry client</param>
    /// <returns>Configured Avro serializer</returns>
    internal static AvroSerializer<TEvent> GetSchemaRegistryConfig<TEvent>(
        ISchemaRegistryClient schemaRegistryClient)
        where TEvent : ISpecificRecord
    {
        if (schemaRegistryClient == null)
            throw new ArgumentNullException(nameof(schemaRegistryClient));

        var serializerConfig = new AvroSerializerConfig
        {
            // ksql only runs with TopicName strategy
            // See: https://docs.confluent.io/platform/current/schema-registry/serdes-develop/index.html#sr-schemas-subject-name-strategy
            SubjectNameStrategy = SubjectNameStrategy.Topic,
            AutoRegisterSchemas = true
        };

        return new AvroSerializer<TEvent>(schemaRegistryClient, serializerConfig);
    }
}

/// <summary>
/// Exception thrown when Schema Registry operations fail
/// </summary>
public class SchemaRegistryException : Exception
{
    /// <summary>
    /// Initializes a new instance of the SchemaRegistryException class
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public SchemaRegistryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
