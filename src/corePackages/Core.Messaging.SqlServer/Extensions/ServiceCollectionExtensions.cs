using Ardalis.GuardClauses;
using Core.Abstractions.Extensions;
using Core.Abstractions.Messaging.Outbox;
using Core.Messaging.SqlServer.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Messaging.SqlServer.Extensions;

/// <summary>
/// Extension methods for configuring SQL Server messaging services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQL Server messaging services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="configuration">The configuration containing outbox options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddSqlServerMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddOutbox(services, configuration);

        return services;
    }

    /// <summary>
    /// Configures outbox services with SQL Server storage
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="configuration">The configuration containing outbox options</param>
    private static void AddOutbox(IServiceCollection services, IConfiguration configuration)
    {
        var outboxOption = Guard.Against.Null(
            configuration.GetOptions<OutboxOptions>(nameof(OutboxOptions)),
            nameof(OutboxOptions));

        services.AddOptions<OutboxOptions>()
                .Bind(configuration.GetSection(nameof(OutboxOptions)))
                .ValidateDataAnnotations();

        services.AddDbContext<OutboxDataContext>(options =>
        {
            options.UseSqlServer(outboxOption.ConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
        });
        services.AddScoped<IOutboxService, EfOutboxService<OutboxDataContext>>();
    }
}