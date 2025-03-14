using Core.Abstractions.Events.External;
using RabbitMQ.Client;

namespace Core.Messaging.Transport.RabbitMQ;

public interface IMessageParser
{
    IIntegrationEvent Resolve(IBasicProperties basicProperties, byte[] body);
}
