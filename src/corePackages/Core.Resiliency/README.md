# Core.Resiliency

A comprehensive resiliency package for .NET applications that provides robust error handling, retry policies, circuit breaker patterns, and fallback mechanisms.

## Features

- HTTP Client Resiliency
  - Retry policies with exponential backoff
  - Circuit breaker patterns
  - Configurable policy options
  - Custom policy handlers

- MediatR Integration
  - Retry behavior for requests
  - Fallback behavior support
  - Configurable retry attempts and delays
  - Circuit breaker integration

- Policy Configuration
  - Retry count and delay settings
  - Circuit breaker duration
  - Timeout duration
  - Exponential backoff support

## Installation

Add the package reference to your project:

```xml
<PackageReference Include="Core.Resiliency" Version="1.0.0" />
```

## Dependencies

### Core Dependencies
- Polly (8.3.0)
- Polly.Extensions.Http (3.0.0)
- MediatR (12.2.0)
- Ardalis.GuardClauses (4.5.0)

### Microsoft Dependencies
- Microsoft.Extensions.Configuration (9.0.0-preview.2)
- Microsoft.Extensions.DependencyInjection (9.0.0-preview.2)
- Microsoft.Extensions.Http (9.0.0-preview.2)
- Microsoft.Extensions.Http.Polly (9.0.0-preview.2)
- Microsoft.Extensions.Logging.Abstractions (9.0.0-preview.2)

### Additional Dependencies
- Scrutor (4.2.2)

## Usage

### HTTP Client Setup

```csharp
// Add HTTP client with retry and circuit breaker policies
services.AddHttpApiClient<IMyService, MyService>();
```

### MediatR Retry Policy Setup

```csharp
// Add MediatR retry policy
services.AddMediaterRetryPolicy(assemblies);

// Implement retry policy for a request
public class MyRequest : IRequest<MyResponse>, IRetryableRequest<MyRequest, MyResponse>
{
    public int RetryAttempts => 3;
    public int RetryDelay => 200;
    public bool RetryWithExponentialBackoff => true;
    public int ExceptionsAllowedBeforeCircuitTrip => 2;
}
```

### MediatR Fallback Policy Setup

```csharp
// Add MediatR fallback policy
services.AddMediaterFallbackPolicy(assemblies);

// Implement fallback handler
public class MyFallbackHandler : IFallbackHandler<MyRequest, MyResponse>
{
    public async Task<MyResponse> HandleFallbackAsync(
        MyRequest request, 
        CancellationToken cancellationToken)
    {
        // Fallback logic here
    }
}
```

### Policy Configuration

```json
{
  "PolicyOptions": {
    "RetryCount": 3,
    "BreakDuration": 30,
    "TimeOutDuration": 30
  }
}
```

## Best Practices

1. Configure appropriate retry counts and delays based on your service requirements
2. Use exponential backoff for retries to prevent overwhelming services
3. Implement fallback handlers for critical operations
4. Monitor circuit breaker states through logging
5. Use guard clauses for input validation
6. Configure timeout policies to prevent long-running operations

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 