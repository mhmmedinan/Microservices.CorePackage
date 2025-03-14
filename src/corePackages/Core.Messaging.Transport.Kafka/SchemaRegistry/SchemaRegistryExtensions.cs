using Avro.Generic;
using Avro.Specific;
using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Messaging.Transport.Kafka.SchemaRegistry;

/// <summary>
/// Extension methods for Schema Registry operations
/// </summary>
public static class SchemaRegistryExtensions
{
    /// <summary>
    /// Adds Schema Registry client to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="config">The configuration instance</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSchemeRegistry(this IServiceCollection services, IConfiguration config)
    {
        var registryUrl = config.GetValue("Kafka:SchemaRegistryUrl", "http://localhost:8081");
        if (string.IsNullOrEmpty(registryUrl))
            throw new InvalidOperationException("Schema Registry URL must be specified in configuration.");

        services.AddSingleton<ISchemaRegistryClient>(x => new CachedSchemaRegistryClient(
            new SchemaRegistryConfig { Url = registryUrl }));
        return services;
    }

    /// <summary>
    /// Serializes an Avro-specific record using Schema Registry
    /// </summary>
    /// <typeparam name="TEvent">Type of the event to serialize</typeparam>
    /// <param name="event">The event instance to serialize</param>
    /// <param name="schemaRegistryClient">Schema Registry client</param>
    /// <param name="topicName">Optional topic name for context</param>
    /// <returns>Serialized bytes of the event</returns>
    public static async Task<byte[]> SerializeAsync<TEvent>(
        this TEvent @event,
        ISchemaRegistryClient schemaRegistryClient,
        string topicName = "demo")
        where TEvent : ISpecificRecord
    {
        if (@event == null)
            throw new ArgumentNullException(nameof(@event));
        if (schemaRegistryClient == null)
            throw new ArgumentNullException(nameof(schemaRegistryClient));

        var serializer = SchemaRegistryPreCache.GetSchemaRegistryConfig<TEvent>(schemaRegistryClient);
        var context = new SerializationContext(MessageComponentType.Value, topicName);
        return await serializer.SerializeAsync(@event, context);
    }

    /// <summary>
    /// Deserializes bytes into an Avro-specific record using Schema Registry
    /// </summary>
    /// <typeparam name="TEvent">Type of the event to deserialize into</typeparam>
    /// <param name="eventBytes">The bytes to deserialize</param>
    /// <param name="schemaRegistryClient">Schema Registry client</param>
    /// <param name="topicName">Optional topic name for context</param>
    /// <returns>Deserialized event instance</returns>
    public static async Task<TEvent> DeserializeAsync<TEvent>(
        this byte[] eventBytes,
        ISchemaRegistryClient schemaRegistryClient,
        string topicName = "demo")
        where TEvent : ISpecificRecord
    {
        if (eventBytes == null || eventBytes.Length == 0)
            throw new ArgumentException("Event bytes cannot be null or empty", nameof(eventBytes));
        if (schemaRegistryClient == null)
            throw new ArgumentNullException(nameof(schemaRegistryClient));

        var context = new SerializationContext(MessageComponentType.Value, topicName);
        var deserializer = new AvroDeserializer<TEvent>(schemaRegistryClient);
        return await deserializer.DeserializeAsync(eventBytes, false, context);
    }

    /// <summary>
    /// Serializes a generic Avro record using Schema Registry
    /// </summary>
    /// <param name="genericRecord">The generic record to serialize</param>
    /// <param name="schemaRegistryClient">Schema Registry client</param>
    /// <returns>Serialized bytes of the record</returns>
    public static async Task<byte[]> SerializeAsync(
        this GenericRecord genericRecord,
        ISchemaRegistryClient schemaRegistryClient)
    {
        if (genericRecord == null)
            throw new ArgumentNullException(nameof(genericRecord));
        if (schemaRegistryClient == null)
            throw new ArgumentNullException(nameof(schemaRegistryClient));

        var serializer = new AvroSerializer<GenericRecord>(schemaRegistryClient);
        var context = new SerializationContext(MessageComponentType.Value, genericRecord.Schema.Name);
        return await serializer.SerializeAsync(genericRecord, context);
    }

    /// <summary>
    /// Deserializes bytes into an Avro-specific record using Schema Registry without topic context
    /// </summary>
    /// <typeparam name="TEvent">Type of the event to deserialize into</typeparam>
    /// <param name="eventBytes">The bytes to deserialize</param>
    /// <param name="schemaRegistryClient">Schema Registry client</param>
    /// <returns>Deserialized event instance</returns>
    public static async Task<TEvent> DeserializeAsync<TEvent>(
        this byte[] eventBytes,
        ISchemaRegistryClient schemaRegistryClient)
        where TEvent : ISpecificRecord
    {
        if (eventBytes == null || eventBytes.Length == 0)
            throw new ArgumentException("Event bytes cannot be null or empty", nameof(eventBytes));
        if (schemaRegistryClient == null)
            throw new ArgumentNullException(nameof(schemaRegistryClient));

        var context = new SerializationContext(MessageComponentType.Value, typeof(TEvent).Name);
        var deserializer = new AvroDeserializer<TEvent>(schemaRegistryClient);
        return await deserializer.DeserializeAsync(eventBytes, false, context);
    }
}
