using Core.Abstractions.Events.External;
using System.Threading.Channels;

namespace Core.Messaging.Transport.InMemory.Channels;

/// <summary>
/// Interface for message channel operations
/// </summary>
public interface IMessageChannel
{
    ChannelReader<IIntegrationEvent> Reader { get; }
    ChannelWriter<IIntegrationEvent> Writer { get; }
}
