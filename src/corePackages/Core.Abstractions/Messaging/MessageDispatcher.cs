using Microsoft.Extensions.DependencyInjection;

namespace Core.Abstractions.Messaging;

public class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public MessageDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync<TMessage>(
        TMessage message,
        IMessageContext messageContext = null,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        using var scope = _serviceProvider.CreateScope();
        var messageProcessor = scope.ServiceProvider.GetRequiredService<IMessageProcessor>();

        await messageProcessor.ProcessAsync(message, messageContext, cancellationToken);
    }

    public async Task DispatchMultipleAsync<TMessage>(
    TMessage[] messages,
    IMessageContext messageContext,
    CancellationToken cancellationToken = default)
    where TMessage : IMessage
    {
        foreach (var message in messages)
        {
            await DispatchAsync(message, messageContext, cancellationToken);
        }
    }
}

