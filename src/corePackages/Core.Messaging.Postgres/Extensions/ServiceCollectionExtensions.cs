using Ardalis.GuardClauses;
using Core.Abstractions.Messaging.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Core.Abstractions.Extensions;
using Core.Messaging.Postgres.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Reflection;


namespace Core.Messaging.Postgres.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddOutbox(services, configuration);

        return services;
    }

    private static void AddOutbox(IServiceCollection services, IConfiguration configuration)
    {
        var outboxOption = Guard.Against.Null(
            configuration.GetOptions<OutboxOptions>(nameof(OutboxOptions)),
            nameof(OutboxOptions));

        services.AddOptions<OutboxOptions>().Bind(configuration.GetSection(nameof(OutboxOptions)))
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
