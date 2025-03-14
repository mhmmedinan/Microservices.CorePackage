using Core.Abstractions.Events;
using Core.Abstractions.Events.External;

public interface IIntegrationEventHandler<in TEvent> : IEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
}
