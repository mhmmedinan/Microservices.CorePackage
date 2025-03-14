# Core.Messaging.InMemory

This package provides an in-memory implementation of the outbox pattern for handling message persistence and reliable message delivery in distributed systems.

## Features

- In-memory message storage for outbox pattern implementation
- Support for integration events
- Message serialization and deserialization
- Automatic message publishing
- Message cleanup for processed messages
- Thread-safe operations

## Installation

```bash
dotnet add package Core.Messaging.InMemory
```

## Configuration

Add the following to your `Startup.cs` or program configuration:

```csharp
services.AddInMemoryOutbox();
```

## Usage

### Basic Usage

```csharp
// Inject the service
public class MyService
{
    private readonly IOutboxService _outboxService;

    public MyService(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    // Save a message to outbox
    public async Task SaveMessage(MyIntegrationEvent event)
    {
        await _outboxService.SaveAsync(event);
    }

    // Publish unsent messages
    public async Task PublishMessages()
    {
        await _outboxService.PublishUnsentOutboxMessagesAsync();
    }
}
```

### Advanced Usage

```csharp
// Get all messages
var allMessages = await _outboxService.GetAllOutboxMessagesAsync();

// Get only unsent messages
var unsentMessages = await _outboxService.GetAllUnsentOutboxMessagesAsync();

// Clean up processed messages
await _outboxService.CleanProcessedAsync();
```

## Best Practices

1. Regularly clean up processed messages to prevent memory leaks
2. Handle exceptions during message publishing
3. Implement retry mechanisms for failed message publishing
4. Monitor message processing status
5. Use appropriate logging levels for debugging

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 