# Core.Abstractions

Core.Abstractions package provides foundational abstractions and interfaces for building microservices. It defines common contracts and patterns used across the application.

## 📦 Package Contents

### 1. CQRS
- Command and Query separation interfaces
- Command handling and validation
- Query processing and optimization
- CQRS extensions and utilities
- Structured folder organization:
  - `Command/`: Command handling interfaces and base types
  - `Query/`: Query handling interfaces and base types
  - `CqrsExtensions.cs`: Extension methods for CQRS operations

### 2. Events
- `IEvent.cs`: Base event interface with metadata
- `IEventHandler.cs`: Event handling contracts
- `IEventProcessor.cs`: Event processing pipeline
- `Event.cs`: Base event implementation
- `EventProcessor.cs`: Default event processor implementation
- External event handling support
- Event sourcing capabilities

### 3. Messaging
- Message handling infrastructure:
  - `IMessage.cs`: Base message contract
  - `IMessageHandler.cs`: Message handling interface
  - `IMessageProcessor.cs`: Message processing pipeline
  - `IMessageDispatcher.cs`: Message routing and dispatch
  - `IMessageMiddleware.cs`: Message processing middleware
- Transport layer abstractions
- Message outbox pattern
- Serialization contracts
- Background processing services
- Transactional messaging support

### 4. Scheduler
- Job scheduling interfaces
- Task execution contracts
- Scheduling pattern abstractions
- Background job definitions
- Recurring job support

### 5. Context Executions
- Execution context interfaces
- Context management
- Scope handling
- Context propagation
- Ambient context support

### 6. Extensions
- Common extension methods
- Utility abstractions
- Helper interfaces
- Shared extension points

### 7. Types
- Common type definitions
- Base type abstractions
- Type conversion contracts
- Custom type handlers

## 🔧 Usage Examples

### CQRS Implementation
```csharp
public interface ICommand<TResponse> : IRequest<TResponse>
{
    Guid TransactionId { get; }
    DateTime Timestamp { get; }
}

public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}
```

### Event Handling
```csharp
public interface IEvent
{
    Guid EventId { get; }
    DateTime OccurredOn { get; }
    string EventType { get; }
    string AggregateId { get; }
    int Version { get; }
}

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    Task HandleAsync(TEvent @event, CancellationToken cancellationToken);
}
```

### Message Processing
```csharp
public interface IMessageProcessor
{
    Task ProcessAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;
    
    Task ProcessAsync<TMessage>(TMessage message, MessageContext context, CancellationToken cancellationToken = default)
        where TMessage : class, IMessage;
}
```

## 📋 Dependencies

- MediatR (12.2.0)
- Ardalis.GuardClauses (5.0.0)
- Core.CrossCuttingConcerns

## 🔍 Best Practices

1. Keep interfaces focused and cohesive (ISP)
2. Use meaningful and consistent naming
3. Document interface contracts thoroughly
4. Follow async/await patterns
5. Handle cancellation tokens appropriately
6. Use guard clauses for preconditions
7. Implement proper exception contracts
8. Version interfaces when needed
9. Follow SOLID principles
10. Use dependency injection

## 🔄 Integration

### 1. Service Registration
```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ICommand<>).Assembly));
services.AddScoped<IEventProcessor, EventProcessor>();
services.AddScoped<IMessageProcessor, MessageProcessor>();
```

### 2. Event Handler Registration
```csharp
services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
services.AddScoped<IEventHandler<PaymentProcessedEvent>, PaymentProcessedEventHandler>();
```

### 3. Message Processing Pipeline
```csharp
services.AddScoped<IMessageMiddleware, LoggingMiddleware>();
services.AddScoped<IMessageMiddleware, ValidationMiddleware>();
services.AddScoped<IMessageDispatcher, MessageDispatcher>();
```

## 📚 Additional Resources

- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [Domain Events Pattern](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-events-design-implementation)
- [Message Processing Pipeline](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PipesAndFilters.html)
- [Outbox Pattern](https://microservices.io/patterns/data/transactional-outbox.html) 