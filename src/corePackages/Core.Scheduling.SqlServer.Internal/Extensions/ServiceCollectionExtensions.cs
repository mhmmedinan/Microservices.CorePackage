﻿using Ardalis.GuardClauses;
using Core.Abstractions.Extensions;
using Core.Abstractions.Scheduler;
using Core.Persistence.Contexts;
using Core.Scheduling.SqlServer.Internal.Data;
using Core.Scheduling.SqlServer.Internal.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Scheduling.SqlServer.Internal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSqlServerInternalScheduler(
    this IServiceCollection services,
    IConfiguration configuration)
    {
        SddInternalMessagesContext<SqlServerInternalMessageDbContext>(services, configuration);

        services.AddScoped<IScheduler, InternalScheduler>();
        services.AddScoped<IMessageScheduler, InternalScheduler>();
        services.AddScoped<ICommandScheduler, InternalScheduler>();
        services.AddScoped<IInternalSchedulerService, InternalSchedulerService>();

        services.AddHostedService<InternalMessageSchedulerBackgroundWorkerService>();

        return services;
    }

    private static void SddInternalMessagesContext<TContext>(IServiceCollection services, IConfiguration configuration)
    where TContext : EfDbContextBase
    {

        services.AddOptions<InternalMessageSchedulerOptions>()
            .Bind(configuration.GetSection(nameof(InternalMessageSchedulerOptions)))
            .ValidateDataAnnotations();

        services.AddDbContext<TContext>(cfg =>
        {
            var options = Guard.Against.Null(
                configuration.GetOptions<InternalMessageSchedulerOptions>(nameof(InternalMessageSchedulerOptions)),
                nameof(InternalMessageSchedulerOptions));

            cfg.UseSqlServer(options.ConnectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            }).UseSnakeCaseNamingConvention();
        });
    }
}