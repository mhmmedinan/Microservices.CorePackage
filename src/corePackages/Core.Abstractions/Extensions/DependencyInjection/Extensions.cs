using Ardalis.GuardClauses;
using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging;
using Core.Abstractions.Messaging.BackgroundServices;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Transport;
using Core.Abstractions.Scheduler;
using Core.Abstractions.Types;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Serilog;
using System.Reflection;

namespace Core.Abstractions.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods to simplify messaging-related dependency injection setup.
/// </summary>
public static class Extensions
{
    /// <summary>
    /// Registers core event processing services.
    /// </summary>
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration, params Assembly[]? assemblies)
    {
        services.AddScoped<IEventProcessor, EventProcessor>();
        services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();
        return services;
    }

    /// <summary>
    /// Adds an event bus subscriber if not already registered.
    /// </summary>
    public static IServiceCollection AddBusSubscriber(this IServiceCollection services, Type subscriberType)
    {
        if (services.All(s => s.ImplementationType != subscriberType))
            services.AddSingleton(typeof(IEventBusSubscriber), subscriberType);

        return services;
    }

    /// <summary>
    /// Adds essential messaging services and background workers.
    /// </summary>
    public static IServiceCollection AddMessagingCore(this IServiceCollection services, IConfiguration configuration, Assembly[]? assemblies)
    {
        services.AddScoped<IMessageDispatcher, MessageDispatcher>();
        services.AddHostedService<SubscribersBackgroundService>();
        services.AddHostedService<ConsumerBackgroundWorker>();
        services.AddHostedService<OutboxProcessorBackgroundService>();

        var typeResolver = new TypeResolver();
        services.AddSingleton<ITypeResolver>(typeResolver);

        RegisterIntegrationMessagesToTypeResolver(typeResolver, assemblies);

        return services;
    }

    /// <summary>
    /// Retrieves all integration event types handled by assemblies.
    /// </summary>
    public static IEnumerable<Type> GetHandledIntegrationEventTypes(this Assembly[] assemblies)
    {
        return typeof(IIntegrationEventHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .SelectMany(x => x.GetInterfaces())
            .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>))
            .Select(inheritsType => inheritsType.GetGenericArguments().First())
            .Where(messageType => messageType.IsAssignableTo(typeof(IIntegrationEvent)))
            .Distinct();
    }

    /// <summary>
    /// Dispatches a collection of integration events asynchronously.
    /// </summary>
    public static Task DispatchIntegrationEventAsync(this IMediator mediator, IReadOnlyList<IIntegrationEvent> integrationEvents, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        var tasks = integrationEvents.Select(evt => mediator.DispatchIntegrationEventAsync(evt, cancellationToken));

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// Dispatches a single integration event asynchronously.
    /// </summary>
    public static async Task DispatchIntegrationEventAsync(this IMediator mediator, IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        var serializer = ServiceActivator.GetRequiredService<IMessageSerializer>();

        await mediator.Publish(integrationEvent, cancellationToken);

        Log.Logger.Debug("Published integration event {IntegrationEventName} with payload {IntegrationEventContent}",
            integrationEvent.GetType().FullName,
            serializer.Serialize(integrationEvent));
    }

    /// <summary>
    /// Sends a scheduled serialized command object through MediatR.
    /// </summary>
    public static async Task SendScheduleObject(this IMediator mediator, ScheduleSerializedObject scheduleSerializedObject)
    {
        var type = scheduleSerializedObject.GetPayloadType();
        dynamic? command = JsonConvert.DeserializeObject(scheduleSerializedObject.Data, type);

        if (command != null)
        {
            await mediator.Send(command);
        }
    }

    /// <summary>
    /// Loads assemblies that match a given naming pattern.
    /// </summary>
    public static Assembly[] GetDomainAssemblies(string pattern)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .Where(asm => asm.FullName != null && asm.FullName.Contains(pattern, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    private static void RegisterIntegrationMessagesToTypeResolver(ITypeResolver typeResolver, Assembly[]? assemblies)
    {
        var messageType = typeof(IIntegrationEvent);
        var types = assemblies.SelectMany(asm => asm.GetTypes())
            .Where(t => messageType.IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .Distinct()
            .ToList();

        typeResolver.Register(types);
    }
}
