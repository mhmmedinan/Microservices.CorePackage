using Microsoft.Extensions.DependencyInjection;

namespace Core.Abstractions.Messaging;

public class MessageDispatcher : IMessageDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public MessageDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchMessageAsync<TMessage>(
        TMessage message,
        IMessageContext messageContext = null,
        CancellationToken cancellationToken = default)
        where TMessage : IMessage
    {
        using var scope = _serviceProvider.CreateScope();
        var messageProcessor = scope.ServiceProvider.GetRequiredService<IMessageProcessor>();

        await messageProcessor.ProcessAsync(message, messageContext, cancellationToken);
    }
}

