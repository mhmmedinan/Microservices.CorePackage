namespace Core.Abstractions.ContextExecutions;

/// <summary>
/// Defines a contract for executing database operations with retry capabilities.
/// Provides functionality to handle transient failures in database operations by automatically retrying failed operations.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Handle transient database failures</item>
/// <item>Implement resilient database operations</item>
/// <item>Manage retry policies for database access</item>
/// <item>Ensure operation reliability</item>
/// </list>
/// Implementation considerations:
/// <list type="bullet">
/// <item>Define appropriate retry intervals</item>
/// <item>Handle maximum retry attempts</item>
/// <item>Log retry attempts</item>
/// <item>Consider circuit breaker patterns</item>
/// </list>
/// </remarks>
public interface IRetryDbContextExecution
{
    /// <summary>
    /// Executes a database operation with retry capability.
    /// </summary>
    /// <param name="operation">The asynchronous operation to execute.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// The operation will be retried according to the configured retry policy if it fails due to transient errors.
    /// </remarks>
    Task RetryOnExceptionAsync(Func<Task> operation);

    /// <summary>
    /// Executes a database operation with retry capability and returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result returned by the operation.</typeparam>
    /// <param name="operation">The asynchronous operation to execute that returns a result.</param>
    /// <returns>A task representing the asynchronous operation with the operation result.</returns>
    /// <remarks>
    /// The operation will be retried according to the configured retry policy if it fails due to transient errors.
    /// </remarks>
    Task<TResult> RetryOnExceptionAsync<TResult>(Func<Task<TResult>> operation);
}
