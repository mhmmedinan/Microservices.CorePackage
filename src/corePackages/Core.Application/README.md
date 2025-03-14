# Core.Application

Core.Application package provides essential structures and behaviors for the application layer in microservices architecture.

## 📦 Package Contents

### 1. Pipeline Behaviors

#### 🔐 Authorization
- `AuthorizationBehavior`: Role-based authorization control
- User roles and permissions validation
- Admin and custom role checks
- Claim-based authorization support

#### 📝 Logging
- `LoggingBehavior`: Request/response logging
- Sensitive data filtering (e.g., passwords)
- Detailed method and parameter logging
- Custom log formatting

#### ⚡ Performance
- `PerformanceBehavior`: Performance monitoring and metrics collection
- Threshold monitoring and warning system
- Critical performance issue alerts
- Custom threshold configuration
- Email notifications for slow operations

#### 💾 Caching
- `CachingBehavior`: Distributed cache management
- Redis integration
- Compression support
- Group-based cache management
- Multiple cache strategies (Sliding, Absolute, NeverExpire)

#### ✅ Validation
- `ValidationBehavior`: FluentValidation integration
- Business rules validation
- Custom validation rules
- Automatic validation triggering

#### 🔄 Transaction
- `TransactionBehavior`: Database transaction management
- Unit of Work pattern support
- Distributed transaction handling
- Automatic rollback on failures

### 2. Response Models

#### `IResponse`
- Base interface for all response models
- Consistent response structure
- Type safety for responses

#### `GetListResponse<T>`
- Generic response model for list operations
- Type-safe list management
- Collection handling utilities

#### `GetListResponsePaginate<T>`
- Paginated list responses
- Total record count
- Page size and number
- Navigation properties

### 3. DTOs (Data Transfer Objects)

#### `IDto`
- Base interface for DTOs
- Data transfer standardization
- Mapping conventions

#### User DTOs
- `UserForLoginDto`: Login operations
- `UserForRegisterDto`: Registration operations
- Secure data handling

### 4. Business Rules

#### `BaseBusinessRules`
- Base class for business rules
- Domain-specific rule infrastructure
- Reusable validation logic

## 🔧 Usage Examples

### Authorization Behavior
```csharp
public class GetProductsQuery : IRequest<List<Product>>, ISecuredRequest
{
    public string[] Roles => new[] { "admin", "product.read" };
}
```

### Caching Behavior
```csharp
public class GetCategoriesQuery : IRequest<List<Category>>, ICachableRequest
{
    public string CacheKey => "categories-list";
    public string CacheGroupKey => "categories";
    public TimeSpan? SlidingExpiration => TimeSpan.FromHours(2);
}
```

### Transaction Behavior
```csharp
public class CreateOrderCommand : IRequest<Order>, ITransactionalRequest
{
    public bool RequiresNew => true;
    public IsolationLevel IsolationLevel => IsolationLevel.ReadCommitted;
}
```

## ⚙️ Configuration

### Cache Settings
```json
{
  "CacheSettings": {
    "SlidingExpiration": 2,
    "MaxRetryAttempts": 3,
    "RetryDelayMilliseconds": 300
  }
}
```

### Performance Settings
```json
{
  "PerformanceSettings": {
    "ThresholdInMilliseconds": 500,
    "EnableEmailAlerts": true,
    "AlertEmailAddress": "alerts@example.com",
    "AlertEmailDisplayName": "Performance Monitoring"
  }
}
```

## 📚 Dependencies

### Core Packages
- `FluentValidation`: Validation rules
- `MediatR`: CQRS and Mediator pattern
- `StackExchange.Redis`: Redis cache operations

### Microsoft Packages
- `AspNetCore.Http.Abstractions`
- `Extensions.Caching.Abstractions`
- `Extensions.Configuration`
- `Extensions.Logging`
- `IdentityModel.Tokens`

### Project References
- `Core.CrossCuttingConcerns`
- `Core.Mailing`
- `Core.Persistence`
- `Core.Security` 