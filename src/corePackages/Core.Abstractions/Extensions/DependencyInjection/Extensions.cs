using Ardalis.GuardClauses;
using Core.Abstractions.CQRS;
using Core.Abstractions.Events;
using Core.Abstractions.Events.External;
using Core.Abstractions.Extensions;
using Core.Abstractions.Extensions.DependencyInjection;
using Core.Abstractions.Messaging;
using Core.Abstractions.Messaging.BackgroundServices;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Serialization.Newtonsoft;
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

public static class Extensions
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration, params System.Reflection.Assembly[]? assemblies)
    {
        services.AddScoped<IEventProcessor, EventProcessor>();
        services.AddScoped<IIntegrationEventPublisher,IntegrationEventPublisher>();
        return services;
    }

    public static IServiceCollection AddBusSubscriber(this IServiceCollection services,Type subscriberType)
    {
        if(services.All(s=>s.ImplementationType!= subscriberType))
            services.AddSingleton(typeof(IEventBusSubscriber),subscriberType);
        return services;
    }

    private static void AddDefaultMessageSerializer(IServiceCollection services, ServiceLifetime lifetime)
    {
        services.Add(new ServiceDescriptor(typeof(IMessageSerializer), typeof(NewtonsoftJsonMessageSerializer), lifetime));
    }

    public static IEnumerable<Type> GetHandledMessageTypes(params Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IMessageHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes.SelectMany(x => x.GetInterfaces())
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType) &&
                        x.GetGenericTypeDefinition() == typeof(IMessageHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IMessage)))
            {
                yield return messageType;
            }
        }
    }

    private static void RegisterIntegrationMessagesToTypeResolver(
        ITypeResolver typeResolver, Assembly[]? assemblies)
    {
        Console.WriteLine("preloading all message types...");

        var messageType = typeof(IIntegrationEvent);

        var types = assemblies.SelectMany(x => x.GetTypes())
            .Where(type =>
                messageType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Distinct()
            .ToList();

        typeResolver.Register(types);

        Console.WriteLine("preloading all message types completed!");
    }

    public static IServiceCollection AddMessagingCore(
       this IServiceCollection services,
       IConfiguration configuration, Assembly[]? assemblies)
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

    public static IEnumerable<Type> GetHandledIntegrationEventTypes(this Assembly[] assemblies)
    {
        var messageHandlerTypes = typeof(IIntegrationEventHandler<>).GetAllTypesImplementingOpenGenericInterface(assemblies)
            .ToList();

        var inheritsTypes = messageHandlerTypes.SelectMany(x => x.GetInterfaces())
            .Where(x => x.GetInterfaces().Any(i => i.IsGenericType) &&
                        x.GetGenericTypeDefinition() == typeof(IIntegrationEventHandler<>));

        foreach (var inheritsType in inheritsTypes)
        {
            var messageType = inheritsType.GetGenericArguments().First();
            if (messageType.IsAssignableTo(typeof(IIntegrationEvent)))
            {
                yield return messageType;
            }
        }
    }

    public static Task DispatchIntegrationEventAsync(
        this IMediator mediator,
        IReadOnlyList<IIntegrationEvent> integrationEvents,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        var tasks = integrationEvents
            .Select(async integrationEvent =>
            {
                await DispatchIntegrationEventAsync(mediator, integrationEvent, cancellationToken);
            });

        return Task.WhenAll(tasks);
    }

    public static async Task DispatchIntegrationEventAsync(
        this IMediator mediator,
        IIntegrationEvent integrationEvent,
        CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        var serializer = ServiceActivator.GetRequiredService<IMessageSerializer>();

        await mediator.Publish(integrationEvent, cancellationToken);
        Log.Logger.Debug(
            "Published integration notification event {IntegrationEventName} with payload {IntegrationEventContent}",
            integrationEvent.GetType().FullName,
            serializer.Serialize(integrationEvent));
    }

    public static async Task SendScheduleObject(
        this IMediator mediator,
        ScheduleSerializedObject scheduleSerializedObject)
    {
        var assemblies = GetDomainAssemblies("MS");
        ServiceActivator._serviceCollection.AddCqrs(assemblies);

        var _mediator = ServiceActivator.GetService<IMediator>();
        var type = scheduleSerializedObject.GetPayloadType();
        dynamic? req = JsonConvert.DeserializeObject(scheduleSerializedObject.Data, type);

        if (req is null)
        {
            return;
        }

        await _mediator.Send(req);
    }

    public static Assembly[]? GetDomainAssemblies(string pattern)
    {
        List<Assembly>? assemblies = new();

        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
        var baazAssemblies = loadedAssemblies
       .Where(name => name.FullName!.Contains(pattern, StringComparison.CurrentCultureIgnoreCase)).ToArray();
        foreach (var assemblyName in baazAssemblies)
        {
            var assembly = Assembly.Load(assemblyName.FullName!);
            assemblies.Add(assembly);
        }
        return assemblies.ToArray();
    }

}
