using Microsoft.Extensions.DependencyInjection;

namespace Core.Abstractions;

/// <summary>
/// Provides global access to the application's service provider and simplifies service resolution operations.
/// Useful when accessing services outside of standard dependency injection contexts.
/// </summary>
public static class ServiceActivator
{
    private static IServiceProvider? _serviceProvider;

    /// <summary>
    /// Configures the global service provider.
    /// </summary>
    /// <param name="serviceProvider">The application's service provider instance.</param>
    public static void Configure(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Retrieves a scoped instance of the specified service type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>An instance of the requested service.</returns>
    public static T GetScopedService<T>()
    {
        using var scope = GetScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }

    /// <summary>
    /// Retrieves an optional instance of the specified service type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>An instance of the service, or null if not registered.</returns>
    public static T? GetService<T>() => _serviceProvider.GetService<T>();

    /// <summary>
    /// Retrieves a required instance of the specified service type.
    /// Throws an exception if the service is not registered.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>An instance of the requested service.</returns>
    public static T GetRequiredService<T>() => _serviceProvider!.GetRequiredService<T>();

    /// <summary>
    /// Retrieves a required instance of the specified non-generic service type.
    /// Throws an exception if the service is not registered.
    /// </summary>
    /// <param name="type">The type of the service.</param>
    /// <returns>An instance of the requested service.</returns>
    public static object GetRequiredService(Type type) => _serviceProvider!.GetRequiredService(type);

    /// <summary>
    /// Retrieves all registered instances of the specified generic service type.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns>A collection of service instances.</returns>
    public static IEnumerable<T> GetServices<T>() => _serviceProvider!.GetServices<T>();

    /// <summary>
    /// Retrieves an optional instance of the specified non-generic service type.
    /// </summary>
    /// <param name="type">The type of the service.</param>
    /// <returns>An instance of the service, or null if not registered.</returns>
    public static object? GetService(Type type) => _serviceProvider?.GetService(type);

    /// <summary>
    /// Retrieves all registered instances of the specified non-generic service type.
    /// </summary>
    /// <param name="type">The type of the service.</param>
    /// <returns>A collection of service instances.</returns>
    public static IEnumerable<object> GetServices(Type type) => _serviceProvider!.GetServices(type);

    /// <summary>
    /// Creates a new service scope from the service provider.
    /// </summary>
    /// <param name="serviceProvider">Optional service provider; uses global provider if null.</param>
    /// <returns>A new service scope.</returns>
    private static IServiceScope GetScope(IServiceProvider? serviceProvider = null)
    {
        var provider = serviceProvider ?? _serviceProvider
            ?? throw new InvalidOperationException("Service provider has not been configured.");

        return provider.GetRequiredService<IServiceScopeFactory>().CreateScope();
    }
}
