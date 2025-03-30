using Microsoft.Extensions.DependencyInjection;

namespace Core.Abstractions.Extensions.Registration;

/// <summary>
/// Provides extension methods to simplify service registration operations for IServiceCollection.
/// </summary>
public static class RegistrationExtensions
{
    /// <summary>
    /// Removes a registered service of type <typeparamref name="TService"/> from the service collection.
    /// </summary>
    /// <typeparam name="TService">The type of the service to remove.</typeparam>
    /// <param name="services">The service collection.</param>
    public static void Unregister<TService>(this IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    /// <summary>
    /// Replaces an existing service registration with a new implementation type.
    /// </summary>
    /// <typeparam name="TService">The service type to replace.</typeparam>
    /// <typeparam name="TImplementation">The new implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The desired service lifetime.</param>
    public static void Replace<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime)
        where TService : class
        where TImplementation : class, TService
    {
        services.Unregister<TService>();
        services.Add(typeof(TService), typeof(TImplementation), lifetime);
    }

    /// <summary>
    /// Registers a service type and its implementation with a specified lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of service.</param>
    /// <param name="implementationType">The implementation type.</param>
    /// <param name="lifetime">The service lifetime.</param>
    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime)
    {
        var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);
        services.Add(descriptor);
        return services;
    }

    /// <summary>
    /// Registers a service and implementation with a specified lifetime.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TImplementation">The implementation type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: transient).</param>
    public static IServiceCollection Add<TService, TImplementation>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TService : class
        where TImplementation : class, TService
    {
        return services.Add(typeof(TService), typeof(TImplementation), lifetime);
    }

    /// <summary>
    /// Registers a service with itself as implementation type and specified lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of service to register.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime (default: transient).</param>
    public static IServiceCollection Add<TService>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TService : class
    {
        return services.Add(typeof(TService), typeof(TService), lifetime);
    }

    /// <summary>
    /// Registers a service using a factory method and specified lifetime.
    /// </summary>
    /// <typeparam name="TService">The type of service.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationFactory">A factory to create the service instance.</param>
    /// <param name="lifetime">The desired lifetime (default: transient).</param>
    public static IServiceCollection Add<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TService : class
    {
        var descriptor = new ServiceDescriptor(typeof(TService), implementationFactory, lifetime);
        services.Add(descriptor);
        return services;
    }

    /// <summary>
    /// Registers a service using a factory method and specified lifetime.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="serviceType">The type of service.</param>
    /// <param name="implementationFactory">A factory to create the service instance.</param>
    /// <param name="lifetime">The desired lifetime (default: transient).</param>
    public static IServiceCollection Add(
        this IServiceCollection services,
        Type serviceType,
        Func<IServiceProvider, object> implementationFactory,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var descriptor = new ServiceDescriptor(serviceType, implementationFactory, lifetime);
        services.Add(descriptor);
        return services;
    }

    /// <summary>
    /// Replaces an existing service registration using a factory method.
    /// </summary>
    /// <typeparam name="TService">The service type to replace.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="implementationFactory">A factory to create the service instance.</param>
    /// <param name="lifetime">The desired lifetime.</param>
    public static void Replace<TService>(
        this IServiceCollection services,
        Func<IServiceProvider, TService> implementationFactory,
        ServiceLifetime lifetime)
        where TService : class
    {
        services.Unregister<TService>();
        services.Add(implementationFactory, lifetime);
    }
}
