using Core.Abstractions.Messaging.Outbox;
using Core.Messaging.InMemory.Outbox;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Messaging.InMemory;

/// <summary>
/// Extension methods for configuring in-memory messaging services
/// </summary>
public static class InMemoryExtensions
{
    /// <summary>
    /// Adds in-memory outbox services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add the outbox services to</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddInMemoryOutbox(this IServiceCollection services)
    {
        services.AddSingleton<IInMemoryOutboxStore, InMemoryOutboxStore>();
        services.AddScoped<IOutboxService, InMemoryOutboxService>();

        return services;
    }
}
