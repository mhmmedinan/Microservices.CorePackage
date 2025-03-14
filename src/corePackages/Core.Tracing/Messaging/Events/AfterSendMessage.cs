using Core.Abstractions.Events.External;

namespace Core.Tracing.Messaging.Events;

public class AfterSendMessage
{
    public AfterSendMessage(IIntegrationEvent eventData)
       => EventData = eventData;
    public IIntegrationEvent EventData { get; }
}
