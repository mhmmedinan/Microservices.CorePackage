using Core.Abstractions.Messaging.Transport;
using Core.Messaging.Transport.InMemory.Channels;
using Core.Messaging.Transport.InMemory.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Messaging.Transport.InMemory;

public static class InMemoryTransportExtensions
{
    public static IServiceCollection AddInMemoryTransport(
       this IServiceCollection services,
       IConfiguration configuration)
    {
        services.AddSingleton<IEventBusPublisher, InMemoryPublisher>()
            .AddSingleton<IEventBusSubscriber, InMemorySubscriber>()
            .AddTransient<InMemoryProducerDiagnostics>()
            .AddTransient<InMemoryConsumerDiagnostics>();

        services.AddSingleton<IMessageChannel, MessageChannel>();
        return services;
    }
}
