# Core.Messaging.Transport.InMemory

An in-memory implementation of the event bus transport layer for testing and development purposes. This package provides a lightweight, in-process messaging solution that implements the Core.Abstractions messaging interfaces.

## Features

- In-memory message channels for event publishing and subscription
- Diagnostic tracking for monitoring message flow
- Thread-safe message handling
- Support for multiple subscribers per event type
- Integrated logging and error handling
- No external dependencies required

## Installation

```bash
dotnet add package Core.Messaging.Transport.InMemory
```

## Usage

### Configuration

Add the in-memory transport to your service collection in `Startup.cs` or `Program.cs`:

```csharp
services.AddInMemoryTransport();
```

### Publishing Events

```csharp
public class OrderCreatedEvent : IIntegrationEvent
{
    public Guid EventId { get; set; }
    public DateTime OccurredOn { get; set; }
    public string OrderId { get; set; }
}

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
        Console.WriteLine($"Order {event.OrderId} was created at {event.OccurredOn}");
    }
}
```

## Architecture

The package consists of several key components:

- `IMessageChannel`: Interface for managing message channels
- `MessageChannel`: Implementation of message channels using System.Threading.Channels
- `InMemoryPublisher`: Publishes events to the in-memory channels
- `InMemorySubscriber`: Subscribes to events and manages handlers
- `InMemoryProducerDiagnostics`: Tracks publishing metrics
- `InMemoryConsumerDiagnostics`: Tracks subscription metrics

## Best Practices

1. **Use for Development/Testing**: This transport is primarily intended for development and testing scenarios. For production, consider using a more robust message broker.

2. **Memory Management**: Monitor memory usage when handling large numbers of messages or long-running subscriptions.

3. **Error Handling**: Always implement proper error handling in your event handlers to prevent message processing failures.

4. **Logging**: Enable logging to track message flow and diagnose issues.

## Diagnostics

The package includes built-in diagnostics for monitoring message flow:

- Message publishing metrics
- Subscription handling metrics
- Processing time tracking
- Error rate monitoring

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.