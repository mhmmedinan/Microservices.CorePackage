namespace Core.Abstractions.Scheduler;

/// <summary>
/// Defines the contract for scheduling and managing scheduled tasks.
/// The scheduler is responsible for managing delayed and recurring task execution in the system.
/// </summary>
/// <remarks>
/// Use this interface when you need to:
/// <list type="bullet">
/// <item>Schedule tasks for future execution</item>
/// <item>Implement recurring job patterns</item>
/// <item>Manage delayed processing</item>
/// <item>Handle distributed scheduling</item>
/// </list>
/// Key features:
/// <list type="bullet">
/// <item>One-time scheduled tasks</item>
/// <item>Recurring tasks with CRON expressions</item>
/// <item>Delayed task execution</item>
/// <item>Schedule management and tracking</item>
/// </list>
/// </remarks>
public interface IScheduler : ICommandScheduler, IMessageScheduler
{
    /// <summary>
    /// Schedules a task to be executed at a specific time.
    /// </summary>
    /// <param name="scheduleSerializedObject">The serialized object containing the task details.</param>
    /// <param name="scheduleAt">The exact time when the task should be executed.</param>
    /// <param name="description">Optional description of the scheduled task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        DateTimeOffset scheduleAt,
        string? description = null);

    /// <summary>
    /// Schedules a task to be executed after a specified delay.
    /// </summary>
    /// <param name="scheduleSerializedObject">The serialized object containing the task details.</param>
    /// <param name="delay">The time to wait before executing the task.</param>
    /// <param name="description">Optional description of the scheduled task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        TimeSpan delay,
        string? description = null);

    /// <summary>
    /// Schedules a recurring task using a CRON expression.
    /// </summary>
    /// <param name="scheduleSerializedObject">The serialized object containing the task details.</param>
    /// <param name="name">The unique name for the recurring schedule.</param>
    /// <param name="cronExpression">The CRON expression defining the recurrence pattern.</param>
    /// <param name="description">Optional description of the scheduled task.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ScheduleRecurringAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        string name,
        string cronExpression,
        string? description = null);

    /// <summary>
    /// Removes a scheduled task from the system.
    /// </summary>
    /// <param name="scheduleId">The unique identifier of the schedule to remove.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveScheduleAsync(string scheduleId);

    /// <summary>
    /// Checks if a schedule exists in the system.
    /// </summary>
    /// <param name="scheduleId">The unique identifier of the schedule to check.</param>
    /// <returns>True if the schedule exists; otherwise, false.</returns>
    Task<bool> ExistsAsync(string scheduleId);
}