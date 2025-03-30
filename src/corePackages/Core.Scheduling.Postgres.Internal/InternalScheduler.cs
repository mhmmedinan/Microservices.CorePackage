using Ardalis.GuardClauses;
using Core.Abstractions.CQRS.Command;
using Core.Abstractions.Messaging;
using Core.Abstractions.Scheduler;
using Core.Scheduling.Postgres.Internal.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq.Expressions;

namespace Core.Scheduling.Postgres.Internal;

public class InternalScheduler : IScheduler
{
    private readonly InternalMessageSchedulerOptions _options;
    private readonly ILogger<InternalScheduler> _logger;
    private readonly IInternalSchedulerService _schedulerService;

    public InternalScheduler(
        IOptions<InternalMessageSchedulerOptions> options,
        ILogger<InternalScheduler> logger,
        IInternalSchedulerService schedulerService
    )
    {
        _options = options.Value;
        _logger = logger;
        _schedulerService = schedulerService;
    }


    public Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(command, nameof(command));
        return _schedulerService.SaveAsync(command, cancellationToken);
    }

    public Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(commands, nameof(commands));
        return _schedulerService.SaveAsync(commands, cancellationToken);
    }

    public Task ScheduleAsync(IInternalCommand command, DateTimeOffset scheduleAt, string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(IInternalCommand[] command, DateTimeOffset scheduleAt, string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleRecurringAsync(
        IInternalCommand command,
        string name,
        string cronExpression,
        string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(message, nameof(message));
        return _schedulerService.SaveAsync(message, cancellationToken);
    }

    public Task ScheduleAsync(IMessage[] messages, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(messages, nameof(messages));
        return _schedulerService.SaveAsync(messages, cancellationToken);
    }

    public Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        DateTimeOffset scheduleAt,
        string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        TimeSpan delay,
        string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleRecurringAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        string name,
        string cronExpression,
        string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(Expression<Func<Task>> methodCall, DateTime scheduleAt, string? description = null)
    {
        throw new NotImplementedException();
    }

    public Task RemoveScheduleAsync(string scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string scheduleId)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync<TCommand>(TCommand command, DateTimeOffset scheduleAt, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync<TCommand>(TCommand command, TimeSpan delay, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        throw new NotImplementedException();
    }

    public Task ScheduleRecurringAsync<TCommand>(TCommand command, string name, string cronExpression, string? description = null, CancellationToken cancellationToken = default) where TCommand : ICommand
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync(Expression<Func<Task>> methodCall, DateTime scheduleAt, string? description = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync<TMessage>(TMessage message, DateTimeOffset scheduleAt, string? description = null) where TMessage : IMessage
    {
        throw new NotImplementedException();
    }

    public Task ScheduleAsync<TMessage>(TMessage message, TimeSpan delay, string? description = null) where TMessage : IMessage
    {
        throw new NotImplementedException();
    }

    public Task ScheduleRecurringAsync<TMessage>(TMessage message, string name, string cronExpression, string? description = null) where TMessage : IMessage
    {
        throw new NotImplementedException();
    }
}
