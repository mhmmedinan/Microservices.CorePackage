using Core.Abstractions.CQRS.Command;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using MediatR;
namespace Core.Abstractions.CQRS;

public static class CqrsExtensions
{
    public static IServiceCollection AddCqrs(this IServiceCollection services, Assembly[]? assemblies=null,Action<IServiceCollection> doMoreActions = null)
    {
        services.AddMediatR(
            assemblies ?? new[] { Assembly.GetCallingAssembly() },
            x =>
            {
                x.AsScoped();
            });

        services.AddScoped<ICommandProcessor, CommandProcessor>();
        doMoreActions?.Invoke(services);

        return services;
    }
}
