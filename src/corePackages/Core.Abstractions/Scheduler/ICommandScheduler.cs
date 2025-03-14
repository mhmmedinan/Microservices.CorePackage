using System.Linq.Expressions;

namespace Core.Abstractions.Scheduler;

public interface ICommandScheduler
{
    Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default);
    Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default);
    Task ScheduleAsync(Expression<Func<Task>> methodCall, DateTime scheduleAt, string? description = null);
    Task ScheduleAsync(IInternalCommand[] commands, DateTimeOffset scheduleAt, string? description = null);
    Task ScheduleRecurringAsync(
        IInternalCommand command,
        string name,
        string cronExpression,
        string? description = null);
}

