using Core.Abstractions.Events.External;

namespace Core.Messaging.Transport.RabbitMQ;

public interface IPublisherChannelFactory
{
    Task<PublisherChannelContext> CreateAsync(IIntegrationEvent message);
}
