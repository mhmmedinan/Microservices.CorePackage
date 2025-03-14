namespace Core.Abstractions.Messaging;

public interface IMessageHandler<in TMessage> where TMessage : IMessage
{
    Task HandleAsync(TMessage message,IMessageContext messageContext=null,CancellationToken cancellationToken=default);
}
