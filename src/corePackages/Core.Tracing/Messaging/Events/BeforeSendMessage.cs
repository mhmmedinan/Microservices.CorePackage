using Core.Abstractions.Events.External;

namespace Core.Tracing.Messaging.Events;

public class BeforeSendMessage
{
    public BeforeSendMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}