using Core.Security.Redis.Services;
using Core.Security.Redis.Session;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Core.Security.Redis;

public static class SecurityRedisServiceRegistrationExtension
{
    public static IServiceCollection AddRedisRoleService(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetSection("Redis:Configuration").Value;
        var multiplexer = ConnectionMultiplexer.Connect(redisOptions);
        services.AddSingleton<IConnectionMultiplexer>(multiplexer);
        services.AddScoped<IAuthorizedRoleService, AutorizedRoleManager>();
        services.AddScoped<IUserSessionService, RedisUserSessionService>();
        return services;
    }
}
