using Core.Abstractions.Messaging.Outbox;
using Core.Messaging.InMemory.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Messaging.InMemory;

public static class InMemoryExtensions
{
    public static IServiceCollection AddInMemoryOutbox(this IServiceCollection services)
    {
        services.AddSingleton<IInMemoryOutboxStore, InMemoryOutboxStore>();
        services.AddScoped<IOutboxService, InMemoryOutboxService>();

        return services;
    }
}
