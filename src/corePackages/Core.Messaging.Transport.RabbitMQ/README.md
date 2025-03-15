# Core.Messaging.Transport.RabbitMQ

A robust RabbitMQ transport implementation for handling message-based communication in distributed systems. This package provides a reliable and efficient way to publish and consume messages using RabbitMQ as the message broker.

## Features

- Persistent RabbitMQ connections with automatic reconnection
- Support for publish/subscribe messaging patterns
- Dead letter queue handling
- Retry mechanisms with configurable policies
- Channel pooling for improved performance
- Comprehensive logging and error handling
- Type-safe message handling
- Integration with dependency injection

## Installation

```bash
dotnet add package Core.Messaging.Transport.RabbitMQ
```

## Configuration

Configure RabbitMQ settings in your `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "RetryDelay": "00:00:05"
  }
}
```

## Usage

### Publishing Messages

```csharp
public class OrderCreatedEvent : IIntegrationEvent
{
    public string OrderId { get; set; }
    public decimal TotalAmount { get; set; }
}

// Inject IEventBusPublisher
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
            OrderId = "123",
            TotalAmount = 99.99m
        };

        await _publisher.PublishAsync(@event);
    }
}
```

### Consuming Messages

```csharp
public class OrderCreatedEventHandler : IIntegrationEventHandler<OrderCreatedEvent>
{
    public async Task Handle(OrderCreatedEvent @event)
    {
        // Handle the event
        Console.WriteLine($"Order {event.OrderId} created with amount {event.TotalAmount}");
    }
}
```

## Architecture

The package follows a clean architecture approach with the following components:

- **RabbitPersistentConnection**: Manages the RabbitMQ connection with automatic reconnection
- **RabbitMQProducer**: Handles message publishing with retry policies
- **RabbitMQConsumer**: Processes incoming messages with error handling
- **PublisherChannelPool**: Manages channel reuse for improved performance
- **QueueReferences**: Provides consistent queue and exchange naming conventions

## Best Practices

1. **Connection Management**
   - Use the provided persistent connection
   - Configure appropriate retry policies
   - Handle connection events properly

2. **Channel Usage**
   - Utilize the channel pool for publishers
   - Don't share channels between threads
   - Close channels when no longer needed

3. **Message Handling**
   - Implement idempotent handlers
   - Use dead letter queues for failed messages
   - Configure appropriate TTL for messages

4. **Error Handling**
   - Implement proper exception handling
   - Log errors with appropriate context
   - Use retry policies where appropriate

## RabbitMQ-Specific Recommendations

1. **Queue Configuration**
   - Enable queue durability for important messages
   - Configure appropriate queue TTL
   - Use dead letter exchanges for failed messages

2. **Exchange Types**
   - Use topic exchanges for flexible routing
   - Consider fanout exchanges for broadcast scenarios
   - Use direct exchanges for simple routing

3. **Performance Optimization**
   - Configure appropriate QoS settings
   - Use channel pooling for publishers
   - Configure message persistence based on requirements

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 