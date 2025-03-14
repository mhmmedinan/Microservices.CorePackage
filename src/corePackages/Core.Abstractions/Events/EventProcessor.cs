using Core.Abstractions.Events.External;
using Core.Abstractions.Extensions.DependencyInjection;
using MediatR;

namespace Core.Abstractions.Events;

public class EventProcessor : IEventProcessor
{
    private readonly IMediator _mediator;
    private readonly IIntegrationEventPublisher _integrationEventPublisher;

    public EventProcessor(
        IMediator mediator,
        IIntegrationEventPublisher integrationEventPublisher)
    {
        _mediator = mediator;
        _integrationEventPublisher = integrationEventPublisher;
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            await _integrationEventPublisher.PublishAsync(integrationEvent, cancellationToken: cancellationToken);
            return;
        }

      
    }

    public async Task PublishAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        foreach (var @event in events)
        {
            await PublishAsync(@event, cancellationToken);
        }
    }

    public async Task DispatchAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (@event is IIntegrationEvent integrationEvent)
        {
            await _mediator.DispatchIntegrationEventAsync(integrationEvent, cancellationToken: cancellationToken);
            return;
        }

        await _mediator.Publish(@event, cancellationToken);
    }

    public async Task DispatchAsync<TEvent>(TEvent[] events, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        foreach (var @event in events)
        {
            await DispatchAsync(@event, cancellationToken);
        }
    }
}
