using Microsoft.Extensions.DependencyInjection;

namespace Core.Abstractions.Extensions.Registration;

public static class RegisterationExtensions
{
    public static void Unregister<TService>(this IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
        services.Remove(descriptor);
    }

    public static void Replace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
    {
        services.Unregister<TService>();
        services.Add(new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime));
    }

    private static void TryAddTransientExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType)) return;

        services.AddTransient(serviceType, implementationType);
    }

    private static void TryAddScopeExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType)) return;

        services.AddScoped(serviceType, implementationType);
    }

    private static void TryAddSingletonExact(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType)
    {
        if (services.Any(reg => reg.ServiceType == serviceType && reg.ImplementationType == implementationType)) return;

        services.AddSingleton(serviceType, implementationType);
    }

    public static void ReplaceScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.AddScoped<TService, TImplementation>();
    }

    public static void ReplaceScoped<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.Unregister<TService>();
        services.AddScoped(implementationFactory);
    }

    public static void ReplaceTransient<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.AddTransient<TService, TImplementation>();
    }

    public static void ReplaceTransient<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.Unregister<TService>();
        services.AddTransient(implementationFactory);
    }

    public static void ReplaceSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.AddSingleton<TService, TImplementation>();
    }

    public static void ReplaceSingleton<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory)
        where TService : class
    {
        services.Unregister<TService>();
        services.AddSingleton(implementationFactory);
    }

    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton<TService, TImplementation>(implementationFactory);

            case ServiceLifetime.Scoped:
                return services.AddScoped<TService, TImplementation>(implementationFactory);

            case ServiceLifetime.Transient:
                return services.AddTransient<TService, TImplementation>(implementationFactory);

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    public static IServiceCollection Add<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TService : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton(implementationFactory);

            case ServiceLifetime.Scoped:
                return services.AddScoped(implementationFactory);

            case ServiceLifetime.Transient:
                return services.AddTransient(implementationFactory);

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    public static IServiceCollection Add<TService>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TService : class
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton<TService>();

            case ServiceLifetime.Scoped:
                return services.AddScoped<TService>();

            case ServiceLifetime.Transient:
                return services.AddTransient<TService>();

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton(serviceType);

            case ServiceLifetime.Scoped:
                return services.AddScoped(serviceType);

            case ServiceLifetime.Transient:
                return services.AddTransient(serviceType);

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        where TService : class
        where TImplementation : class, TService
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton<TService, TImplementation>();

            case ServiceLifetime.Scoped:
                return services.AddScoped<TService, TImplementation>();

            case ServiceLifetime.Transient:
                return services.AddTransient<TService, TImplementation>();

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, object> implementationFactory,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton(serviceType, implementationFactory);

            case ServiceLifetime.Scoped:
                return services.AddScoped(serviceType, implementationFactory);

            case ServiceLifetime.Transient:
                return services.AddTransient(serviceType, implementationFactory);

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }

    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                return services.AddSingleton(serviceType, implementationType);

            case ServiceLifetime.Scoped:
                return services.AddScoped(serviceType, implementationType);

            case ServiceLifetime.Transient:
                return services.AddTransient(serviceType, implementationType);

            default:
                throw new ArgumentNullException(nameof(serviceLifetime));
        }
    }
}

