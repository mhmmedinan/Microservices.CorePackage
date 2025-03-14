using Ardalis.GuardClauses;
using Core.Abstractions.Extensions;
using Core.Abstractions.Messaging.Outbox;
using Core.Messaging.SqlServer.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Messaging.SqlServer.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerMessaging(
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