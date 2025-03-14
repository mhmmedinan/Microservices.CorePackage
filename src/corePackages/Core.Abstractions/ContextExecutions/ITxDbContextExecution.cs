namespace Core.Abstractions.ContextExecutions;

/// <summary>
/// Defines a contract for executing database operations within a transaction.
/// Provides functionality to ensure atomic execution of database operations by wrapping them in transactions.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Ensure atomic database operations</item>
/// <item>Manage transaction boundaries</item>
/// <item>Handle transaction rollback scenarios</item>
/// <item>Implement unit of work pattern</item>
/// </list>
/// Implementation considerations:
/// <list type="bullet">
/// <item>Handle nested transactions</item>
/// <item>Manage transaction isolation levels</item>
/// <item>Consider distributed transaction scenarios</item>
/// <item>Implement proper error handling</item>
/// </list>
/// </remarks>
public interface ITxDbContextExecution
{
    /// <summary>
    /// Executes an action within a transaction scope.
    /// </summary>
    /// <param name="action">The asynchronous action to execute within the transaction.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// The transaction will be automatically committed if the action completes successfully,
    /// or rolled back if an exception occurs.
    /// </remarks>
    Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes an action within a transaction scope and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the result returned by the action.</typeparam>
    /// <param name="action">The asynchronous action to execute within the transaction that returns a result.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation with the action result.</returns>
    /// <remarks>
    /// The transaction will be automatically committed if the action completes successfully,
    /// or rolled back if an exception occurs.
    /// </remarks>
    Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
}
