# Core.Persistence

A comprehensive persistence layer for .NET applications that provides support for multiple databases and data access patterns.

## Features

- Multi-database support:
  - SQL Server (Entity Framework Core)
  - PostgreSQL (Entity Framework Core)
  - MongoDB
- Dynamic querying capabilities
- Naming convention support
- Configuration management
- Cross-cutting concerns integration

## Installation

Add the package reference to your project:

```xml
<PackageReference Include="Core.Persistence" Version="1.0.1" />
```

## Dependencies

### Database Providers
- Microsoft.EntityFrameworkCore (9.0.0-preview.2.24128.4)
- Microsoft.EntityFrameworkCore.SqlServer (9.0.0-preview.2.24128.4)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.0-preview.1)
- MongoDB.Driver (2.24.0)

### Querying and Conventions
- System.Linq.Dynamic.Core (1.6.0)
- EFCore.NamingConventions (9.0.0-preview.1)

### Configuration
- Microsoft.Extensions.Configuration (9.0.0-preview.2.24128.5)
- Microsoft.Extensions.Configuration.EnvironmentVariables
- Microsoft.Extensions.Configuration.Json

## Usage

### Entity Framework Core Setup

```csharp
// In Program.cs or Startup.cs

services.AddDbContext<YourDbContext>(options =>
{
    // SQL Server
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    
    // PostgreSQL
    options.UseNpgsql(configuration.GetConnectionString("PostgresConnection"));
    
    // Apply naming conventions
    options.UseSnakeCaseNamingConvention();
});
```



### Dynamic Querying

```csharp
// Example of dynamic querying
var query = dbContext.Entities
    .AsQueryable()
    .Where("Property == @0", value)
    .OrderBy("Property DESC");
```

## Configuration

The package supports various configuration sources:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;",
    "PostgresConnection": "Host=...;Database=...;",
    "MongoConnection": "mongodb://..."
  }
}
```

## Naming Conventions

The package includes support for various naming conventions:

- Snake Case (e.g., `user_name`)
- Camel Case (e.g., `userName`)
- Pascal Case (e.g., `UserName`)

## Best Practices

1. Use dependency injection for database contexts
2. Implement repository pattern for data access
3. Use async/await for database operations
4. Implement proper error handling
5. Use transactions when necessary
6. Follow naming conventions consistently

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 