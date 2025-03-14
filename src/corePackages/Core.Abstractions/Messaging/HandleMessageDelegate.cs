namespace Core.Abstractions.Messaging;

public delegate Task HandleMessageDelegate<in TMessage>(TMessage request,IMessageContext messageContext=null,CancellationToken cancellationToken = default);

