using System.Linq.Expressions;
using Core.Abstractions.CQRS.Command;

namespace Core.Abstractions.Scheduler;

/// <summary>
/// Defines the contract for scheduling commands for future or recurring execution.
/// Provides functionality to schedule commands that need to be executed at a later time or on a recurring basis.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Schedule domain commands for future execution</item>
/// <item>Implement delayed command processing</item>
/// <item>Create recurring command execution patterns</item>
/// <item>Manage scheduled business operations</item>
/// </list>
/// Common scenarios:
/// <list type="bullet">
/// <item>Delayed order processing</item>
/// <item>Scheduled data cleanup</item>
/// <item>Recurring business processes</item>
/// <item>Timed workflow steps</item>
/// </list>
/// </remarks>
public interface ICommandScheduler
{
    /// <summary>
    /// Schedules a command to be executed at a specific time.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to schedule.</typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="scheduleAt">The exact time when the command should be executed.</param>
    /// <param name="description">Optional description of the scheduled command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync<TCommand>(
        TCommand command,
        DateTimeOffset scheduleAt,
        string? description = null)
        where TCommand : ICommand;

    /// <summary>
    /// Schedules a command to be executed after a specified delay.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to schedule.</typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="delay">The time to wait before executing the command.</param>
    /// <param name="description">Optional description of the scheduled command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync<TCommand>(
        TCommand command,
        TimeSpan delay,
        string? description = null)
        where TCommand : ICommand;

    /// <summary>
    /// Schedules a recurring command using a CRON expression.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to schedule.</typeparam>
    /// <param name="command">The command to be executed.</param>
    /// <param name="name">The unique name for the recurring schedule.</param>
    /// <param name="cronExpression">The CRON expression defining the recurrence pattern.</param>
    /// <param name="description">Optional description of the scheduled command.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleRecurringAsync<TCommand>(
        TCommand command,
        string name,
        string cronExpression,
        string? description = null)
        where TCommand : ICommand;
}

