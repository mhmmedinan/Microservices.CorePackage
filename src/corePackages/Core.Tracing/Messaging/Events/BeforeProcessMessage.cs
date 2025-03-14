using Core.Abstractions.Events.External;

namespace Core.Tracing.Messaging.Events;

public class BeforeProcessMessage
{
    public BeforeProcessMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}

