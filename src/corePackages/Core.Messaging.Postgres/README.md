# Core.Messaging.Postgres

This package provides a PostgreSQL implementation of the outbox pattern for reliable message delivery in distributed systems. It uses Entity Framework Core for data access and supports automatic migrations.

## Features

- PostgreSQL-based outbox pattern implementation
- Entity Framework Core integration
- Automatic database migrations
- Support for integration events
- Message serialization and deserialization
- Automatic message publishing
- Message cleanup for processed messages
- Retry mechanism for database operations
- Snake case naming convention support

## Installation

```bash
dotnet add package Core.Messaging.Postgres
```

## Configuration

Add the following to your `appsettings.json`:

```json
{
  "OutboxOptions": {
    "Enabled": true,
    "ConnectionString": "Host=localhost;Database=outbox;Username=postgres;Password=password"
  }
}
```

Add the following to your `Program.cs` or `Startup.cs`:

```csharp
// Add services
services.AddPostgresMessaging(configuration);

// Configure the application
app.UsePostgresMessaging(logger);
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

## Database Schema

The package creates a `messaging` schema in your PostgreSQL database with the following structure:

- Table: `outbox_messages`
  - `id` (UUID): Primary key
  - `event_id` (UUID): Event identifier
  - `occurred_on` (timestamp): Event occurrence time
  - `type` (varchar): Event type name
  - `name` (varchar): Event name
  - `data` (jsonb): Serialized event data
  - `processed_on` (timestamp): Processing timestamp
  - `error` (text): Error message if processing failed
  - `correlation_id` (UUID): Optional correlation identifier

## Best Practices

1. Regularly clean up processed messages
2. Handle exceptions during message publishing
3. Monitor database performance
4. Use appropriate transaction isolation levels
5. Implement retry mechanisms for failed operations
6. Keep the outbox table size under control
7. Use appropriate indexes for better performance

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 