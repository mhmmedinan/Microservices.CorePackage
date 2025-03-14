using Core.Abstractions.Messaging;
using Core.Abstractions.Scheduler;
using Hangfire;
using MediatR;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Core.Abstractions.Extensions.DependencyInjection;

namespace Core.Scheduling.Hangfire.Scheduler;

public class HangfireScheduler : IHangfireScheduler
{
    private readonly IMediator _mediator;

    public HangfireScheduler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public string Enqueue<T>(
            T command,
            string parentJobId,
            JobContinuationOptions continuationOption,
            string? description = null)
            where T : IInternalCommand
    {
        var messageSerializedObject = SerializeObject(command, description);

        return BackgroundJob.ContinueJobWith(
            parentJobId,
            () => _mediator.SendScheduleObject(messageSerializedObject),
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

    public Task ScheduleAsync(
           ScheduleSerializedObject scheduleSerializedObject,
           DateTimeOffset scheduleAt,
           string? description = null)
    {
        BackgroundJob.Schedule(() => _mediator.SendScheduleObject(scheduleSerializedObject), scheduleAt);

        return Task.CompletedTask;
    }

    public Task ScheduleAsync<T>(T command, TimeSpan delay, string? description = null)
           where T : IInternalCommand
    {
        var mediatorSerializedObject = SerializeObject(command, description);
        var newTime = DateTime.Now + delay;
        BackgroundJob.Schedule(() => _mediator.SendScheduleObject(mediatorSerializedObject), newTime);

        return Task.CompletedTask;
    }

    public Task ScheduleAsync(Expression<Func<Task>> methodCall, DateTime scheduleAt, string? description = null)
    {
        BackgroundJob.Schedule(methodCall, scheduleAt);
        return Task.CompletedTask;
    }

    public Task ScheduleAsync(
           ScheduleSerializedObject scheduleSerializedObject,
           TimeSpan delay,
           string? description = null)
    {
        var newTime = DateTime.Now + delay;
        BackgroundJob.Schedule(() => _mediator.SendScheduleObject(scheduleSerializedObject), newTime);

        return Task.CompletedTask;
    }

    public Task ScheduleRecurringAsync(
        ScheduleSerializedObject scheduleSerializedObject,
        string name,
        string cronExpression,
        string? description = null)
    {
        RecurringJob.AddOrUpdate(name, () => _mediator.SendScheduleObject(scheduleSerializedObject),
            cronExpression, TimeZoneInfo.Local);

        return Task.CompletedTask;
    }

    private ScheduleSerializedObject SerializeObject(object messageObject, string? description)
    {
        string fullTypeName = messageObject.GetType().FullName;
        string data = JsonConvert.SerializeObject(messageObject,
            new JsonSerializerSettings { Formatting = Formatting.None, });

        return new ScheduleSerializedObject(fullTypeName, data, description,
            messageObject.GetType().Assembly.GetName().FullName);
    }

    public Task ScheduleAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        var client = new BackgroundJobClient();

        // https://codeopinion.com/using-hangfire-and-mediatr-as-a-message-dispatcher/
        // client.Enqueue<IMediator>(x => x.Send(request, default)); // we could use our mediator directly but because we want to use some hangfire attribute we will wap it in a bridge
        client.Enqueue<MessageProcessorHangfireBridge>(bridge => bridge.Send(message, ""));

        return Task.CompletedTask;
    }

    public async Task ScheduleAsync(IMessage[] messages, CancellationToken cancellationToken = default)
    {
        foreach (var message in messages)
        {
            await ScheduleAsync(message, cancellationToken);
        }
    }

    public Task ScheduleAsync(IInternalCommand command, CancellationToken cancellationToken = default)
    {
        var client = new BackgroundJobClient();

        // https://codeopinion.com/using-hangfire-and-mediatr-as-a-message-dispatcher/
        // client.Enqueue<IMediator>(x => x.Send(request, default)); // we could use our mediator directly but because we want to use some hangfire attribute we will wap it in a bridge
        client.Enqueue<CommandProcessorHangfireBridge>(bridge => bridge.Send(command, ""));

        return Task.CompletedTask;
    }

    public async Task ScheduleAsync(IInternalCommand[] commands, CancellationToken cancellationToken = default)
    {
        foreach (var internalCommand in commands)
        {
            await ScheduleAsync(internalCommand, cancellationToken);
        }
    }

    public Task ScheduleAsync(IInternalCommand command, DateTimeOffset scheduleAt, string? description = null)
    {
        var mediatorSerializedObject = SerializeObject(command, description);
        BackgroundJob.Schedule(() => _mediator.SendScheduleObject(mediatorSerializedObject), scheduleAt);

        return Task.CompletedTask;
    }

    public async Task ScheduleAsync(
        IInternalCommand[] commands,
        DateTimeOffset scheduleAt,
        string? description = null)
    {
        foreach (var command in commands)
        {
            await ScheduleAsync(command, scheduleAt, description);
        }
    }

    public Task ScheduleRecurringAsync(
        IInternalCommand command,
        string name,
        string cronExpression,
        string? description = null)
    {
        var mediatorSerializedObject = SerializeObject(command, description);
        RecurringJob.AddOrUpdate(name, () => _mediator.SendScheduleObject(mediatorSerializedObject),
            cronExpression, TimeZoneInfo.Local);

        return Task.CompletedTask;
    }
}


