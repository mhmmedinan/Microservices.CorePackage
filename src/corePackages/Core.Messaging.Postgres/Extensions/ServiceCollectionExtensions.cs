using Ardalis.GuardClauses;
using Core.Abstractions.Messaging.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Abstractions.Extensions;
using Core.Messaging.Postgres.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Core.Messaging.Postgres.Extensions;

/// <summary>
/// Extension methods for configuring PostgreSQL messaging services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds PostgreSQL messaging services to the service collection
    /// </summary>
    /// <param name="services">The service collection to add the services to</param>
    /// <param name="configuration">The configuration containing outbox options</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddPostgresMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddOutbox(services, configuration);
        return services;
    }

    /// <summary>
    /// Configures outbox services with PostgreSQL storage
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

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddDbContext<OutboxDataContext>(options =>
        {
            options.UseNpgsql(outboxOption.ConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IOutboxService, EfOutboxService<OutboxDataContext>>();
    }
}
