# Core.CrossCuttingConcerns Package

This package contains cross-cutting concerns that are used across the microservices architecture. It provides common functionality for exception handling, logging, and file operations.

## Features

### Exception Handling
- Centralized exception middleware
- Custom exception types
- HTTP problem details formatting
- Standardized error responses

### Logging
- Structured logging with Serilog
- Multiple sink support:
  - Elasticsearch
  - Graylog
  - MongoDB
  - MS SQL Server
  - PostgreSQL
  - RabbitMQ
- Detailed log parameters
- Exception logging

### File Operations
- File upload/download helpers
- File type validation
- File size validation
- Safe file name generation

## Installation

```bash
dotnet add package Core.CrossCuttingConcerns
```

## Configuration

### Logging Configuration
```json
{
  "SeriLogConfigurations": {
    "PostgreConfiguration": {
      "ConnectionString": "Host=localhost;Port=5432;Database=TestDb;Username=postgres;Password=test;",
      "TableName": "Logs",
      "NeedAutoCreateTable": true
    },
    "MsSqlConfiguration": {
      "ConnectionString": "Server=(localdb)\\MSSQLLocalDB;Database=TestDb;Trusted_Connection=True;",
      "TableName": "Logs",
      "NeedAutoCreateTable": true
    },
    "OracleConfiguration": {
      "ConnectionString": "Data Source=localhost:1521;User Id=SYSTEM;Password=test;",
      "TableName": "Logs",
      "NeedAutoCreateTable": true
    },
    "FileLogConfiguration": {
      "FolderPath": "/logs/"
    },
    "MongoDbConfiguration": {
      "ConnectionString": "mongodb://localhost:27017/TestDb",
      "Collection": "logs"
    },
    "ElasticSearchConfiguration": {
      "ConnectionString": "http://localhost:9200",
      "TemplateName": "TestDb",
      "IndexFormat": "TestDb-{0:yyyy.MM.dd}"
    },
    "GraylogConfiguration": {
      "HostnameOrAddress": "localhost",
      "Port": 12201
    },
    "RabbitMQConfiguration": {
      "Exchange": "TestExchange",
      "ExchangeType": "fanout",
      "Hostnames": ["localhost"],
      "Username": "guest",
      "Password": "guest",
      "Port": 5672
    }
  }
}
```

## Usage Examples

### Exception Middleware Setup
```csharp
public void Configure(IApplicationBuilder app)
{
    app.UseMiddleware<ExceptionMiddleware>();
}
```


### File Operations
```csharp
public class FileOperation
{
    private readonly FileHelper _fileHelper;

    public FileOperation(FileHelper fileHelper)
    {
        _fileHelper = fileHelper;
    }

    public async Task<string> UploadFile(IFormFile file)
    {
        return await _fileHelper.Upload(file, "/uploads/");
    }
}
```

## Contributing

Please read our contributing guidelines and code of conduct before making a pull request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 