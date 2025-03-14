using Core.Abstractions.Events.External;

namespace Core.Messaging.Transport.RabbitMQ;

public interface IQueueReferenceFactory
{
    QueueReferences Create<TM>(TM message = default)
        where TM : IIntegrationEvent;
}
