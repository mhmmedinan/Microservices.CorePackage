using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Core.Abstractions.ContextExecutions;

/// <summary>
/// Defines the contract for database context operations with enhanced transaction and retry capabilities.
/// Extends Entity Framework Core's DbContext functionality with additional features for robust data access.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Manage database operations with transaction support</item>
/// <item>Handle entity persistence with retry mechanisms</item>
/// <item>Control transaction isolation levels</item>
/// <item>Implement unit of work pattern</item>
/// </list>
/// Key features:
/// <list type="bullet">
/// <item>Transaction management with isolation level control</item>
/// <item>Automatic retry on transient failures</item>
/// <item>Entity tracking and persistence</item>
/// <item>Asynchronous operation support</item>
/// </list>
/// </remarks>
public interface IDbContext : ITxDbContextExecution, IRetryDbContextExecution
{
    /// <summary>
    /// Gets a DbSet instance for access to entities of the given type in the context.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity for which a set should be returned.</typeparam>
    /// <returns>A DbSet instance for the given entity type.</returns>
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    /// <summary>
    /// Begins a new database transaction with the specified isolation level.
    /// </summary>
    /// <param name="isolationLevel">The isolation level to use for the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task BeginTransactionAsync(IsolationLevel isolationLevel, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current database transaction.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database with additional entity-specific logic.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains true if the operation succeeded.</returns>
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves all changes made in this context to the database.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous save operation. The task result contains the number of state entries written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
