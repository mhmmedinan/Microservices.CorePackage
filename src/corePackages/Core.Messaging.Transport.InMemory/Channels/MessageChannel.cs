﻿using Core.Abstractions.Events.External;
using System.Threading.Channels;

namespace Core.Messaging.Transport.InMemory.Channels;

/// <summary>
/// Implementation of message channel operations using in-memory channels
/// </summary>
public class MessageChannel : IMessageChannel
{
    private readonly Channel<IIntegrationEvent> _messages = Channel.CreateUnbounded<IIntegrationEvent>();
    public ChannelReader<IIntegrationEvent> Reader => _messages.Reader;

    public ChannelWriter<IIntegrationEvent> Writer => _messages.Writer;
}
