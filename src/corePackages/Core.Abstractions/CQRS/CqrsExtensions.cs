using Core.Abstractions.CQRS.Command;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
namespace Core.Abstractions.CQRS;

public static class CqrsExtensions
{
    public static IServiceCollection AddCqrs(
        this IServiceCollection services,
        Assembly[]? assemblies = null,
        Action<IServiceCollection> doMoreActions = null)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies ?? new[] { Assembly.GetCallingAssembly() });
            cfg.Lifetime = ServiceLifetime.Scoped;
        });

        services.AddScoped<ICommandProcessor, CommandProcessor>();
        doMoreActions?.Invoke(services);

        return services;
    }
}
