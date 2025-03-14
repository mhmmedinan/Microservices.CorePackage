# Core.Messaging.SqlServer

This package provides a SQL Server implementation of the outbox pattern for reliable message delivery in distributed systems. It uses Entity Framework Core for data access and supports automatic migrations.

## Features

- SQL Server-based outbox pattern implementation
- Entity Framework Core integration
- Automatic database migrations
- Support for integration events
- Message serialization and deserialization
- Automatic message publishing
- Message cleanup for processed messages
- Retry mechanism for database operations
- Optimized for SQL Server performance

## Installation

```bash
dotnet add package Core.Messaging.SqlServer
```

## Configuration

Add the following to your `appsettings.json`:

```json
{
  "OutboxOptions": {
    "Enabled": true,
    "ConnectionString": "Server=.;Database=OutboxDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Add the following to your `Program.cs` or `Startup.cs`:

```csharp
// Add services
services.AddSqlServerMessaging(configuration);

// Configure the application
app.UseSqlServerMessaging(logger);
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

The package creates a `messaging` schema in your SQL Server database with the following structure:

- Table: `OutboxMessages`
  - `Id` (uniqueidentifier): Primary key
  - `EventId` (uniqueidentifier): Event identifier
  - `OccurredOn` (datetime2): Event occurrence time
  - `Type` (nvarchar(500)): Event type name
  - `Name` (nvarchar(100)): Event name
  - `Data` (nvarchar(max)): Serialized event data
  - `ProcessedOn` (datetime2): Processing timestamp
  - `Error` (nvarchar(max)): Error message if processing failed
  - `CorrelationId` (uniqueidentifier): Optional correlation identifier

## Best Practices

1. Regularly clean up processed messages
2. Handle exceptions during message publishing
3. Monitor database performance
4. Use appropriate transaction isolation levels
5. Implement retry mechanisms for failed operations
6. Keep the outbox table size under control
7. Use appropriate indexes for better performance
8. Consider table partitioning for large-scale systems

## SQL Server Specific Recommendations

1. Use appropriate collation for string columns
2. Consider using compression for large tables
3. Monitor and maintain index fragmentation
4. Use proper transaction isolation levels
5. Consider using memory-optimized tables for high-throughput scenarios

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 