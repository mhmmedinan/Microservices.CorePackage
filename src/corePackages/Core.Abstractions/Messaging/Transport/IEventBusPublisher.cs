using Core.Abstractions.Events.External;

namespace Core.Abstractions.Messaging.Transport;

/// <summary>
/// External Event Bus.
/// </summary>
public interface IEventBusPublisher
{
    public Task PublishAsync<TEvent>(TEvent integrationEvent, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent;
}
