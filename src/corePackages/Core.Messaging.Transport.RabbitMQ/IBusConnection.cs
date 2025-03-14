using RabbitMQ.Client;

namespace Core.Messaging.Transport.RabbitMQ;

public interface IBusConnection
{
    bool IsConnected { get; }
    IModel CreateChannel();
}
