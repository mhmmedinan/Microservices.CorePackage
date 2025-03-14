# Core.ElasticSearch Package

This package provides a robust and easy-to-use interface for integrating Elasticsearch functionality into your .NET applications. It offers high-level abstractions for common Elasticsearch operations while maintaining flexibility for advanced use cases.

## Features

- Simple and intuitive API for Elasticsearch operations
- Support for:
  - Index management
  - Document CRUD operations
  - Bulk operations
  - Various search capabilities
  - Field-based searching
  - Query string searches
- Strongly-typed response models
- Asynchronous operations
- Connection management
- Authentication support

## Installation

```bash
dotnet add package Core.ElasticSearch
```

## Configuration

Add the following configuration to your `appsettings.json`:

```json
{
  "ElasticSearchConfig": {
    "ConnectionString": "http://localhost:9200",
    "UserName": "elastic",
    "Password": "your-password"
  }
}
```

Register the Elasticsearch service in your `Startup.cs` or `Program.cs`:

```csharp
services.AddSingleton<IElasticSearch, ElasticSearchManager>();
```

## Usage Examples

### Creating a New Index

```csharp
var indexModel = new IndexModel
{
    IndexName = "my-index",
    AliasName = "my-alias",
    NumberOfReplicas = 1,
    NumberOfShards = 3
};

var result = await _elasticSearch.CreateNewIndexAsync(indexModel);
```

### Inserting Documents

```csharp
// Single document
var insertModel = new ElasticSearchInsertUpdateModel
{
    IndexName = "my-index",
    Item = new MyDocument { Id = 1, Title = "Sample" }
};

var result = await _elasticSearch.InsertAsync(insertModel);

// Multiple documents
var documents = new[] 
{
    new MyDocument { Id = 1, Title = "First" },
    new MyDocument { Id = 2, Title = "Second" }
};

var result = await _elasticSearch.InsertManyAsync("my-index", documents);
```

### Searching Documents

```csharp
// Search all
var searchParams = new SearchParameters
{
    IndexName = "my-index",
    From = 0,
    Size = 10
};

var results = await _elasticSearch.GetAllSearch<MyDocument>(searchParams);

// Field-based search
var fieldParams = new SearchByFieldParameters
{
    IndexName = "my-index",
    FieldName = "title",
    Value = "sample"
};

var results = await _elasticSearch.GetSearchByField<MyDocument>(fieldParams);

// Query string search
var queryParams = new SearchByQueryParameters
{
    IndexName = "my-index",
    QueryString = "title:sample AND category:books",
    From = 0,
    Size = 10
};

var results = await _elasticSearch.GetSearchBySimpleQueryString<MyDocument>(queryParams);
```

### Updating Documents

```csharp
var updateModel = new ElasticSearchInsertUpdateModel
{
    IndexName = "my-index",
    ElasticId = "document-id",
    Item = updatedDocument
};

var result = await _elasticSearch.UpdateByElasticIdAsync(updateModel);
```

### Deleting Documents

```csharp
var deleteModel = new ElasticSearchModel
{
    IndexName = "my-index",
    ElasticId = "document-id"
};

var result = await _elasticSearch.DeleteByElasticIdAsync(deleteModel);
```

## Best Practices

1. **Index Management**
   - Use meaningful index names
   - Consider using index aliases for zero-downtime reindexing
   - Set appropriate number of shards and replicas based on your needs

2. **Search Operations**
   - Use pagination parameters (From/Size) to manage large result sets
   - Consider using field-based search for exact matches
   - Use query string search for more complex queries

3. **Performance**
   - Use bulk operations for inserting multiple documents
   - Implement appropriate retry policies
   - Monitor Elasticsearch cluster health

## Contributing

Please read our contributing guidelines and code of conduct before making a pull request.

## License

This project is licensed under the MIT License - see the LICENSE file for details. 