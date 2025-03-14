using Core.Abstractions.CQRS.Query;

namespace Core.Abstractions.Scheduler;

/// <summary>
/// Defines the contract for scheduling queries for future or recurring execution.
/// Provides functionality to schedule queries that need to be executed at a later time or on a recurring basis.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Schedule data retrieval operations for future execution</item>
/// <item>Implement delayed query processing</item>
/// <item>Create recurring data fetch patterns</item>
/// <item>Manage scheduled reporting operations</item>
/// </list>
/// Common scenarios:
/// <list type="bullet">
/// <item>Scheduled report generation</item>
/// <item>Periodic data analysis</item>
/// <item>Recurring data synchronization</item>
/// <item>Timed data exports</item>
/// </list>
/// </remarks>
public interface IQueryScheduler
{
    /// <summary>
    /// Schedules a query to be executed at a specific time.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to schedule.</typeparam>
    /// <typeparam name="TResponse">The type of response expected from the query.</typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="scheduleAt">The exact time when the query should be executed.</param>
    /// <param name="description">Optional description of the scheduled query.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync<TQuery, TResponse>(
        TQuery query,
        DateTimeOffset scheduleAt,
        string? description = null)
        where TQuery : IQuery<TResponse>;

    /// <summary>
    /// Schedules a query to be executed after a specified delay.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to schedule.</typeparam>
    /// <typeparam name="TResponse">The type of response expected from the query.</typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="delay">The time to wait before executing the query.</param>
    /// <param name="description">Optional description of the scheduled query.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync<TQuery, TResponse>(
        TQuery query,
        TimeSpan delay,
        string? description = null)
        where TQuery : IQuery<TResponse>;

    /// <summary>
    /// Schedules a recurring query using a CRON expression.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to schedule.</typeparam>
    /// <typeparam name="TResponse">The type of response expected from the query.</typeparam>
    /// <param name="query">The query to be executed.</param>
    /// <param name="name">The unique name for the recurring schedule.</param>
    /// <param name="cronExpression">The CRON expression defining the recurrence pattern.</param>
    /// <param name="description">Optional description of the scheduled query.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleRecurringAsync<TQuery, TResponse>(
        TQuery query,
        string name,
        string cronExpression,
        string? description = null)
        where TQuery : IQuery<TResponse>;
} 