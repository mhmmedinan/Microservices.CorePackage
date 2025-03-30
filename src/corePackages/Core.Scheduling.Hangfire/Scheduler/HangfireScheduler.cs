using Core.Abstractions.Messaging;
using Core.Abstractions.Scheduler;
using Hangfire;
using MediatR;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Core.Abstractions.Extensions.DependencyInjection;
using Core.Abstractions.CQRS.Command;

namespace Core.Scheduling.Hangfire.Scheduler;

public class HangfireScheduler : IHangfireScheduler
{
    private readonly IMediator _mediator;

    public HangfireScheduler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }
    // Enqueue methods:
    public string Enqueue<T>(
        T command,
        string parentJobId,
        JobContinuationOptions continuationOption,
        string? description = null)
        where T : IInternalCommand
    {
        var serializedObject = SerializeObject(command, description);
        return BackgroundJob.ContinueJobWith(
            parentJobId,
            () => _mediator.SendScheduleObject(serializedObject),
            continuationOption);
    }

    public string Enqueue(
        ScheduleSerializedObject scheduleSerializedObject,
        string parentJobId,
        JobContinuationOptions continuationOption,
        string? description = null)
    {
        return BackgroundJob.ContinueJobWith(
            parentJobId,
            () => _mediator.SendScheduleObject(scheduleSerializedObject),
            continuationOption);
    }

    // Exists method:
    public Task<bool> ExistsAsync(string scheduleId)
    {
        var monitoringApi = JobStorage.Current.GetMonitoringApi();
        var jobDetails = monitoringApi.JobDetails(scheduleId);
        return Task.FromResult(jobDetails != null);
    }

    // RemoveSchedule method:
    public Task RemoveScheduleAsync(string scheduleId)
    {
        BackgroundJob.Delete(scheduleId);
        RecurringJob.RemoveIfExists(scheduleId);
        return Task.CompletedTask;
    }

    // ScheduleSerializedObject methods:
    public Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        DateTimeOffset scheduleAt,
        string? description = null)
    {
        BackgroundJob.Schedule(
            () => _mediator.SendScheduleObject(scheduleSerializedObject), scheduleAt);
        return Task.CompletedTask;
    }

    public Task ScheduleAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        TimeSpan delay,
        string? description = null)
    {
        return ScheduleAsync(scheduleSerializedObject, DateTimeOffset.UtcNow.Add(delay), description);
    }

    // ICommand methods:
    public Task ScheduleAsync<TCommand>(
        TCommand command,
        DateTimeOffset scheduleAt,
        string? description = null,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        var serializedObject = SerializeObject(command, description);
        BackgroundJob.Schedule(() => _mediator.SendScheduleObject(serializedObject), scheduleAt);
        return Task.CompletedTask;
    }

    public Task ScheduleAsync<TCommand>(
        TCommand command,
        TimeSpan delay,
        string? description = null,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        return ScheduleAsync(command, DateTimeOffset.UtcNow.Add(delay), description, cancellationToken);
    }

    // Expression method call:
    public Task ScheduleAsync(
        Expression<Func<Task>> methodCall,
        DateTime scheduleAt,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        BackgroundJob.Schedule(methodCall, scheduleAt);
        return Task.CompletedTask;
    }

    // Internal command methods:
    public Task ScheduleAsync(
        IInternalCommand internalCommand,
        CancellationToken cancellationToken = default)
    {
        var client = new BackgroundJobClient();
        client.Enqueue<CommandProcessorHangfireBridge>(bridge => bridge.Send(internalCommand, ""));
        return Task.CompletedTask;
    }

    public async Task ScheduleAsync(
        IInternalCommand[] internalCommands,
        CancellationToken cancellationToken = default)
    {
        foreach (var cmd in internalCommands)
            await ScheduleAsync(cmd, cancellationToken);
    }

    // IMessage methods:
    public Task ScheduleAsync<TMessage>(
        TMessage message,
        DateTimeOffset scheduleAt,
        string? description = null)
        where TMessage : IMessage
    {
        var serializedObject = SerializeObject(message, description);
        BackgroundJob.Schedule(() => _mediator.SendScheduleObject(serializedObject), scheduleAt);
        return Task.CompletedTask;
    }

    public Task ScheduleAsync<TMessage>(
        TMessage message,
        TimeSpan delay,
        string? description = null)
        where TMessage : IMessage
    {
        return ScheduleAsync(message, DateTimeOffset.UtcNow.Add(delay), description);
    }

    // Recurring ScheduleSerializedObject:
    public Task ScheduleRecurringAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        string name,
        string cronExpression,
        string? description = null)
    {
        RecurringJob.AddOrUpdate(
            name,
            () => _mediator.SendScheduleObject(scheduleSerializedObject),
            cronExpression,
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
        return Task.CompletedTask;
    }

    // Recurring ICommand:
    public Task ScheduleRecurringAsync<TCommand>(
        TCommand command,
        string name,
        string cronExpression,
        string? description = null,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        var serializedObject = SerializeObject(command, description);
        RecurringJob.AddOrUpdate(
            name,
            () => _mediator.SendScheduleObject(serializedObject),
            cronExpression,
            new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
        return Task.CompletedTask;
    }

    // Recurring IMessage:
    public Task ScheduleRecurringAsync<TMessage>(
        TMessage message,
        string name,
        string cronExpression,
        string? description = null)
        where TMessage : IMessage
    {
        var serializedObject = SerializeObject(message, description);
        RecurringJob.AddOrUpdate(
            name,
            () => _mediator.SendScheduleObject(serializedObject),
            cronExpression,
             new RecurringJobOptions
             {
                 TimeZone = TimeZoneInfo.Local
             });
        return Task.CompletedTask;
    }


    private ScheduleSerializedObject SerializeObject(object messageObject, string? description)
    {
        var typeName = messageObject.GetType().FullName;
        var data = JsonConvert.SerializeObject(messageObject);
        var assemblyName = messageObject.GetType().Assembly.GetName().FullName;

        return new ScheduleSerializedObject(typeName, assemblyName, data, description);
    }

}


