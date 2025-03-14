using Core.Abstractions.Events.External;

namespace Core.Messaging.Transport.RabbitMQ;

public interface IPublisherChannelFactory
{
    PublisherChannelContext Create(IIntegrationEvent message);
}
