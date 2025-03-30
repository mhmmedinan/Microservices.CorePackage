using MediatR;

namespace Core.Abstractions.Messaging;

public class MessageProcessor : IMessageProcessor
{
    private readonly IServiceProvider _serviceProvider;

    public MessageProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task ProcessAsync<TMessage>(
        TMessage message,
        IMessageContext messageContext = null,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        return RunMiddlewareAsync(message, HandleMessageAsync, messageContext, cancellationToken);
    }

    private async Task HandleMessageAsync<TMessage>(
        TMessage message,
        IMessageContext messageContext,
        CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        var type = typeof(TMessage);

        var messageHandlers = ((IEnumerable<IMessageHandler<TMessage>>)_serviceProvider.GetService(typeof
            (IEnumerable<IMessageHandler<TMessage>>)))?.ToList();

        if (messageHandlers is null || !messageHandlers.Any())
        {
            throw new ArgumentException(
                $"No handler of signature {typeof(IRequestHandler<,>).Name} was found for {typeof(TMessage).Name}",
                typeof(TMessage).FullName);
        }

        if (typeof(IMessage).IsAssignableFrom(type))
        {
            var tasks = messageHandlers.Select(r => r.HandleAsync(message, messageContext, cancellationToken));

            foreach (var task in tasks)
            {
                await task;
            }

            return;
        }

        throw new ArgumentException(
            $"{typeof(TMessage).Name} is not a known type of {nameof(IMessage)} - Message",
            typeof(TMessage).FullName);
    }

    private Task RunMiddlewareAsync<TMessage>(
        TMessage request,
        HandleMessageDelegate<TMessage> handleMessageHandlerCall,
        IMessageContext messageContext,
        CancellationToken cancellationToken)
        where TMessage : IMessage
    {
        var middlewares = (IEnumerable<IMessageMiddleware<TMessage>>)_serviceProvider.GetService(typeof(IEnumerable<IMessageMiddleware<TMessage>>));

        HandleMessageDelegate<TMessage> next = middlewares
            .Reverse()
            .Aggregate(handleMessageHandlerCall, (nextDelegate, middleware) =>
                new HandleMessageDelegate<TMessage>((msg, ctx, ct) =>
                    middleware.HandleAsync(msg, ctx, ct, nextDelegate)));

        return next.Invoke(request, messageContext, cancellationToken);
    }

}
