namespace Core.Abstractions.Messaging;

public interface IMessageProcessor
{
    Task ProcessAsync<TMessage>(TMessage message, IMessageContext messageContext = null, CancellationToken
        cancellationToken = default) where TMessage : IMessage;
}