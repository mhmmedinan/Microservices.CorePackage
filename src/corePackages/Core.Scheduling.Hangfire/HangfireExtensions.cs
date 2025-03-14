using Core.Abstractions.Scheduler;
using Core.Scheduling.Hangfire.Scheduler;
using Hangfire;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.SqlServer;
using Hangfire.States;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Core.Scheduling.Hangfire;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireScheduler(this IServiceCollection services,IConfiguration configuration)
    {
        var options = configuration.GetSection(nameof(HangfireMessageSchedulerOptions)).Get<HangfireMessageSchedulerOptions>();

        services.TryAddSingleton<SqlServerStorageOptions>(new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.FromTicks(1),
            UseRecommendedIsolationLevel = true,
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(1)
        });

        services.TryAddSingleton<IBackgroundJobFactory>(x => new CustomBackgroundJobFactory(
            new BackgroundJobFactory(x.GetRequiredService<IJobFilterProvider>())));

       services.TryAddSingleton<IBackgroundJobPerformer>(x => new CustomBackgroundJobPerformer(
            new BackgroundJobPerformer(
                x.GetRequiredService<IJobFilterProvider>(),
                x.GetRequiredService<JobActivator>(),
                TaskScheduler.Default)));

        services.TryAddSingleton<IBackgroundJobStateChanger>(x => new CustomBackgroundJobStateChanger(
            new BackgroundJobStateChanger(x.GetRequiredService<IJobFilterProvider>())));

        services.AddHangfire((provider, configuration) => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseSqlServerStorage(
            options.ConnectionString,
            provider.GetRequiredService<SqlServerStorageOptions>()));

        services.AddHangfireServer(options =>
        {
            options.StopTimeout = TimeSpan.FromSeconds(15);
            options.ShutdownTimeout = TimeSpan.FromSeconds(30);
        });

        services.AddHangfireServer((BackgroundJobServerOptions e) => e.Queues = new[] { "app2_queue" });

        services.AddScoped<IScheduler, HangfireScheduler>();
        services.AddScoped<IHangfireScheduler, HangfireScheduler>();
        services.AddScoped<ICommandScheduler,HangfireScheduler>();
        services.AddScoped<IMessageScheduler, HangfireScheduler>();

        return services;

        
    }

    public static IApplicationBuilder UseHangfireScheduler(this IApplicationBuilder app)
    {
        return app.UseHangfireDashboard("/mydashboard");
    }
   
}


internal class CustomBackgroundJobFactory : IBackgroundJobFactory
{
    private readonly IBackgroundJobFactory _inner;

    public CustomBackgroundJobFactory([NotNull] IBackgroundJobFactory inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public IStateMachine StateMachine => _inner.StateMachine;

    public BackgroundJob Create(CreateContext context)
    {
        Console.WriteLine($"Create: {context.Job.Type.FullName}.{context.Job.Method.Name} in {context.InitialState?.Name} state");
        return _inner.Create(context);
    }
}

internal class CustomBackgroundJobPerformer : IBackgroundJobPerformer
{
    private readonly IBackgroundJobPerformer _inner;

    public CustomBackgroundJobPerformer([NotNull] IBackgroundJobPerformer inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public object Perform(PerformContext context)
    {
        Console.WriteLine($"Perform {context.BackgroundJob.Id} ({context.BackgroundJob.Job.Type.FullName}.{context.BackgroundJob.Job.Method.Name})");
        return _inner.Perform(context);
    }
}

internal class CustomBackgroundJobStateChanger : IBackgroundJobStateChanger
{
    private readonly IBackgroundJobStateChanger _inner;

    public CustomBackgroundJobStateChanger([NotNull] IBackgroundJobStateChanger inner)
    {
        _inner = inner ?? throw new ArgumentNullException(nameof(inner));
    }

    public IState ChangeState(StateChangeContext context)
    {
        Console.WriteLine($"ChangeState {context.BackgroundJobId} to {context.NewState}");
        return _inner.ChangeState(context);
    }
}