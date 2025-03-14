using Core.Abstractions.Events.External;
using System.Threading.Channels;

namespace Core.Messaging.Transport.InMemory.Channels;

public interface IMessageChannel
{
    ChannelReader<IIntegrationEvent> Reader { get; }
    ChannelWriter<IIntegrationEvent> Writer { get; }
}
