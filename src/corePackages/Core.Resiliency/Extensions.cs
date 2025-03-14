using Core.Resiliency.Fallback;
using Core.Resiliency.Retry;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using System.Reflection;

namespace Core.Resiliency;

public static class Extensions
{
    public static IServiceCollection AddMediaterRetryPolicy(
        IServiceCollection services,
        IReadOnlyList<Assembly> assemblies)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IRetryableRequest<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IServiceCollection AddMediaterFallbackPolicy(
        IServiceCollection services,
        IReadOnlyList<Assembly> assemblies)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(FallbackBehavior<,>));

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IFallbackHandler<,>)))
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }

    public static IServiceCollection AddHttpApiClient<TInterface, TClient>(this IServiceCollection services)
        where TInterface : class
        where TClient : class, TInterface
    {
        services
            .AddHttpClient<TInterface, TClient>()
            .AddCustomPolicyHandlers();

        return services;
    }
}
