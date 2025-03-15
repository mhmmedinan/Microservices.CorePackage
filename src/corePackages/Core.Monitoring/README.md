# Core.Monitoring

A comprehensive monitoring solution for .NET applications that integrates health checks and Prometheus metrics.

## Features

- Health check endpoints with customizable responses
- Prometheus metrics integration for both HTTP and gRPC
- Health check UI with in-memory storage
- Extensible health check builder
- JSON formatted health check responses

## Installation

Add the package reference to your project:

```xml
<PackageReference Include="Core.Monitoring" Version="1.0.0" />
```

## Usage

### Basic Setup

```csharp
// In Program.cs or Startup.cs

// Add monitoring services
services.AddMonitoring();

// Configure the application
app.UseMonitoring();
```

### Advanced Setup with Custom Health Checks

```csharp
services.AddMonitoring(healthChecks =>
{
    healthChecks
        .AddSqlServer(connectionString)
        .AddRedis(redisConnectionString)
        .AddUrlGroup(new Uri("https://api.example.com/health"));
});
```

## Endpoints

The package provides several endpoints for monitoring:

- `/healthz` - Basic health check with UI-friendly response
- `/health` - Detailed health check excluding service checks
- `/health/ready` - Readiness check for all components
- `/healthcheck` - Health check API endpoint
- `/healthcheck-ui` - Health check UI dashboard
- `/metrics` - Prometheus metrics endpoint

## Health Check UI

The health check UI is available at `/healthcheck-ui` and provides:

- Visual status of all health checks
- Historical health check data
- 60-second refresh interval
- Configurable endpoints

## Prometheus Integration

The package automatically exposes metrics for:

- HTTP requests
- gRPC calls
- Health check status

## Response Format

Health check endpoints return JSON responses in the following format:

```json
{
    "status": "Healthy",
    "results": {
        "database": {
            "status": "Healthy"
        },
        "redis": {
            "status": "Healthy"
        }
    }
}
```

## Status Codes

- `200 OK` - System is healthy
- `500 Internal Server Error` - System is degraded
- `503 Service Unavailable` - System is unhealthy

## Dependencies

- AspNetCore.HealthChecks.UI (8.0.0)
- prometheus-net.AspNetCore (8.2.1)
- prometheus-net.AspNetCore.Grpc (8.2.1)
- prometheus-net.AspNetCore.HealthChecks (8.2.1)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request. 