# Microservices Core Package

## Core.Abstractions

Core.Abstractions is a foundational package that provides essential building blocks and abstractions for microservices architecture. This package offers core components that enable reliable, scalable, and maintainable microservice development.

### 📦 Package Structure

#### 1. Types
Core type management and system information structures:

- **ITypeResolver & TypeResolver**: 
  - Dynamic type resolution at runtime
  - Infrastructure for plugin and module systems
  - Thread-safe type caching
  - Assembly-based type resolution
  - Support for dynamic loading of types

- **ISystemInfo**: 
  - System-wide configuration and information
  - Client ID and group management
  - Publish-only mode control
  - Instance tracking in distributed systems

- **ITypeList**: 
  - Generic type list management
  - Type-safe collection operations
  - Base type constraints
  - Dynamic type registration

#### 2. Messaging
Inter-service communication and messaging infrastructure:

##### Core Components

- **IMessage**:
  ```csharp
  public interface IMessage
  {
      string MessageId { get; }        // Unique message identifier
      string CorrelationId { get; }    // Related message tracking
      DateTime CreatedAt { get; }      // Creation timestamp
      string MessageType { get; }      // Message type
  }
  ```

- **IMessageHandler**:
  ```csharp
  public interface IMessageHandler<TMessage> where TMessage : IMessage
  {
      Task HandleAsync(
          TMessage message,
          IMessageContext messageContext,
          CancellationToken cancellationToken = default);
  }
  ```

- **IMessageProcessor**:
  - Message processing pipeline management
  - Middleware coordination
  - Error handling and retry logic
  - Transaction management

##### Subsystems

- **Transport**: 
  - Message transport infrastructure
  - Protocol support (RabbitMQ, Azure Service Bus, Kafka)
  - Reliable message delivery
  - Message routing and topology

- **Outbox**: 
  - Reliable message sending
  - Transactional consistency
  - At-least-once delivery guarantee
  - Message persistence

- **Serialization**: 
  - Message serialization/deserialization
  - Format support (JSON, XML, Protobuf)
  - Custom serializer integration

- **BackgroundServices**: 
  - Background processing services
  - Message consumption and processing
  - Outbox message handling
  - Scheduled message processing

#### 3. Events
Event-driven architecture components:

- **IEvent & Event**: 
  - Base event structure
  - Event sourcing infrastructure
  - Event metadata management
  - Event versioning support

- **IEventHandler & EventProcessor**: 
  - Event processing logic
  - Event-driven architecture support
  - Event processing pipeline
  - Event routing and distribution

- **External Events**: 
  - Integration events for cross-service communication
  - Event publishing and subscription
  - Event correlation and tracking
  - Distributed event handling

#### 4. ContextExecutions
Reliable database operations infrastructure:

- **IDbContext**: 
  - EF Core integration
  - Transaction management
  - Retry mechanisms
  - Unit of Work pattern implementation
  - Entity tracking and state management

- **IRetryDbContextExecution**: 
  - Automatic retry handling
  - Transient fault handling
  - Retry policies and strategies
  - Circuit breaker pattern support

- **ITxDbContextExecution**: 
  - Transaction management
  - Atomic operation guarantee
  - Distributed transaction support
  - Transaction isolation level control

- **IDbFacadeResolver**: 
  - Database facade resolution
  - Low-level database operations
  - Connection management
  - Raw SQL execution support

#### 5. CQRS
Command Query Responsibility Segregation implementation:

- **Command**: 
  - Data modification operations
  - Command validation
  - Command handling pipeline
  - Command result handling

- **Query**: 
  - Data retrieval operations
  - Query optimization
  - Caching support
  - Query result mapping

#### 6. Scheduler
Task scheduling and management:

- **IScheduler**: 
  - Delayed task execution
  - CRON-based recurring tasks
  - Schedule management
  - Task tracking and monitoring

- **ICommandScheduler**: 
  - Command scheduling
  - Delayed command execution
  - Recurring command scheduling
  - Command execution tracking

- **IQueryScheduler**: 
  - Query scheduling
  - Periodic data retrieval
  - Report generation scheduling
  - Query result caching

### 🌟 Features

1. **Loose Coupling**
   - Minimized service dependencies
   - Independent development and modification
   - Interface-based design
   - Plugin architecture support

2. **Scalability**
   - Asynchronous processing support
   - Horizontal and vertical scaling compatibility
   - Load balancing readiness
   - Distributed processing support

3. **Reliability**
   - Message loss prevention
   - Atomic operation guarantees
   - Automatic retry mechanisms
   - Circuit breaker patterns

4. **Traceability**
   - Unique message identifiers
   - Correlation ID tracking
   - Detailed logging infrastructure
   - Distributed tracing support

5. **Extensibility**
   - Plugin system support
   - Middleware architecture
   - Customizable components
   - Custom implementation support

### 📚 Usage Examples

1. **Order Processing**
```csharp
public class OrderCreatedMessage : IMessage
{
    public string OrderId { get; set; }
    public decimal TotalAmount { get; set; }
    public string CustomerId { get; set; }
}

public class OrderCreatedHandler : IMessageHandler<OrderCreatedMessage>
{
    public async Task HandleAsync(OrderCreatedMessage message, IMessageContext context)
    {
        // Order processing logic
        // Inventory update
        // Customer notification
    }
}
```

2. **Payment Processing**
```csharp
public class PaymentProcessedMessage : IMessage
{
    public string PaymentId { get; set; }
    public string OrderId { get; set; }
    public PaymentStatus Status { get; set; }
}
```

3. **Email Notification**
```csharp
public class SendEmailMessage : IMessage
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}
```

### 🔧 Installation

```xml
<PackageReference Include="Core.Abstractions" Version="1.0.0" />
```

### 📋 Requirements

- .NET 9.0
- Entity Framework Core
- MediatR

### 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

### 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. 