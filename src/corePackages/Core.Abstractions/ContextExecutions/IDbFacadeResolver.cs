using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Abstractions.ContextExecutions;

/// <summary>
/// Defines a contract for resolving the database facade in Entity Framework Core.
/// Provides access to low-level database operations and configuration through EF Core's DatabaseFacade.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Access low-level database operations</item>
/// <item>Manage database connections</item>
/// <item>Execute raw SQL commands</item>
/// <item>Handle database transactions</item>
/// </list>
/// Common scenarios:
/// <list type="bullet">
/// <item>Direct database access</item>
/// <item>Connection management</item>
/// <item>Transaction coordination</item>
/// <item>Database migrations</item>
/// </list>
/// </remarks>
public interface IDbFacadeResolver
{
    /// <summary>
    /// Gets the DatabaseFacade instance that provides access to database-related information and operations.
    /// </summary>
    /// <remarks>
    /// The DatabaseFacade provides functionality for:
    /// <list type="bullet">
    /// <item>Database connection management</item>
    /// <item>Transaction handling</item>
    /// <item>Raw SQL execution</item>
    /// <item>Database creation and deletion</item>
    /// </list>
    /// </remarks>
    DatabaseFacade Database { get; }
}
