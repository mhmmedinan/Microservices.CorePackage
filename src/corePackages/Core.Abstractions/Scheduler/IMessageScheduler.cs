using Core.Abstractions.Messaging;

namespace Core.Abstractions.Scheduler;

/// <summary>
/// Defines the contract for scheduling messages.
/// </summary>
public interface IMessageScheduler
{
    /// <summary>
    /// Schedules a message to be processed at a specific time.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to schedule.</typeparam>
    /// <param name="message">The message to schedule.</param>
    /// <param name="scheduleAt">The time at which the message should be processed.</param>
    /// <param name="description">Optional description of the scheduled message.</param>
    Task ScheduleAsync<TMessage>(TMessage message, DateTimeOffset scheduleAt, string? description = null)
        where TMessage : IMessage;

    /// <summary>
    /// Schedules a message to be processed after a specified delay.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to schedule.</typeparam>
    /// <param name="message">The message to schedule.</param>
    /// <param name="delay">The delay before the message should be processed.</param>
    /// <param name="description">Optional description of the scheduled message.</param>
    Task ScheduleAsync<TMessage>(TMessage message, TimeSpan delay, string? description = null)
        where TMessage : IMessage;

    /// <summary>
    /// Schedules a recurring message based on a cron expression.
    /// </summary>
    /// <typeparam name="TMessage">The type of message to schedule.</typeparam>
    /// <param name="message">The message to schedule.</param>
    /// <param name="name">The name of the schedule.</param>
    /// <param name="cronExpression">The cron expression for the schedule.</param>
    /// <param name="description">Optional description of the scheduled message.</param>
    Task ScheduleRecurringAsync<TMessage>(TMessage message, string name, string cronExpression, string? description = null)
        where TMessage : IMessage;
}
