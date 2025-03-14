using Core.Abstractions.Events.External;

namespace Core.Tracing.Messaging.Events;

public class NoSubscriberToPublishMessage
{
    public NoSubscriberToPublishMessage(IIntegrationEvent eventData)
        => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}

