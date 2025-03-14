# Core.Messaging.Transport.Kafka

A Kafka implementation of the event bus transport layer that provides reliable message delivery using Apache Kafka and Confluent Schema Registry. This package implements the Core.Abstractions messaging interfaces and adds Avro serialization support.

## Features

- Apache Kafka integration for reliable message delivery
- Avro schema support with Confluent Schema Registry
- Parallel message processing with configurable concurrency
- Automatic schema registration and validation
- Comprehensive error handling and logging
- Support for multiple topics and consumer groups
- Message headers for correlation and tracing
- Performance optimizations with schema pre-caching

## Installation

```bash
dotnet add package Core.Messaging.Transport.Kafka
```

## Configuration

### Producer Configuration

Add the following section to your `appsettings.json`:

```json
{
  "KafkaProducerConfig": {
    "ProducerConfig": {
      "BootstrapServers": "localhost:9092",
      "ClientId": "your-producer-client",
      "Acks": "All"
    },
    "Topic": "your-topic-name",
    "SchemaRegistryUrl": "http://localhost:8081"
  }
}
```

### Consumer Configuration

```json
{
  "KafkaConsumerConfig": {
    "BootstrapServers": "localhost:9092",
    "GroupId": "your-consumer-group",
    "AutoOffsetReset": "Earliest",
    "EnableAutoOffsetStore": false,
    "Topics": ["topic1", "topic2"],
    "SchemaRegistryUrl": "http://localhost:8081",
    "MaxParallelism": 10
  }
}
```

## Usage

### Publishing Events

```csharp
public class OrderService
{
    private readonly IEventBusPublisher _publisher;

    public OrderService(IEventBusPublisher publisher)
    {
        _publisher = publisher;
    }

    public async Task CreateOrder()
    {
        var @event = new OrderCreatedEvent
        {
            EventId = Guid.NewGuid(),
            OccurredOn = DateTime.UtcNow,
            OrderId = "123"
        };

        await _publisher.PublishAsync(@event);
    }
}
```

### Subscribing to Events

```csharp
public class OrderEventHandler
{
    private readonly IEventBusSubscriber _subscriber;

    public OrderEventHandler(IEventBusSubscriber subscriber)
    {
        _subscriber.Subscribe<OrderCreatedEvent>(HandleOrderCreated);
    }

    private async Task HandleOrderCreated(OrderCreatedEvent @event)
    {
        // Handle the event
        Console.WriteLine($"Order {@event.OrderId} was created at {@event.OccurredOn}");
    }
}
```

### Schema Registry Integration

```csharp
// Pre-cache schemas for better performance
public class SchemaInitializer
{
    private readonly ISchemaRegistryClient _schemaRegistry;

    public SchemaInitializer(ISchemaRegistryClient schemaRegistry)
    {
        _schemaRegistry = schemaRegistry;
    }

    public async Task InitializeSchemas()
    {
        await SchemaRegistryPreCache.PreCacheEventAsync<OrderCreatedEvent>(_schemaRegistry);
        await SchemaRegistryPreCache.PreCacheEventAsync<OrderUpdatedEvent>(_schemaRegistry);
    }
}
```

## Architecture

The package consists of several key components:

### Producer
- `KafkaProducer`: Implements `IEventBusPublisher` for publishing events to Kafka
- `KafkaProducerConfig`: Configuration settings for the producer

### Consumer
- `KafkaConsumer`: Implements `IEventBusSubscriber` for consuming events from Kafka
- `KafkaConsumerConfig`: Configuration settings for the consumer
- Parallel message processing using TPL Dataflow

### Schema Registry
- `SchemaRegistryExtensions`: Extension methods for Schema Registry operations
- `SchemaRegistryPreCache`: Performance optimization through schema pre-caching
- Support for Avro serialization and deserialization

## Best Practices

1. **Configuration**
   - Use appropriate acknowledgment settings for your reliability needs
   - Configure consumer group IDs to manage message distribution
   - Set appropriate parallelism levels based on your workload

2. **Error Handling**
   - Implement proper error handling in your event handlers
   - Use dead letter queues for failed messages
   - Monitor consumer group lag

3. **Performance**
   - Pre-cache schemas for frequently used event types
   - Configure appropriate batch sizes and compression
   - Monitor and adjust parallel processing settings

4. **Monitoring**
   - Enable logging for troubleshooting
   - Monitor consumer group health
   - Track message processing metrics

## Kafka-Specific Recommendations

1. **Topic Configuration**
   - Use appropriate partition counts based on throughput
   - Configure retention policies
   - Set replication factor for reliability

2. **Consumer Groups**
   - Design consumer group strategy
   - Monitor consumer group lag
   - Handle rebalancing properly

3. **Schema Evolution**
   - Follow Avro schema evolution rules
   - Test schema compatibility
   - Version your schemas appropriately

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 