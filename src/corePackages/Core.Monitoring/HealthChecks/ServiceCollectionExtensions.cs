using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Prometheus;
using System.Text.Json;
using System.Text;

namespace Core.Monitoring.HealthChecks;

/// <summary>
/// Extension methods for configuring monitoring services in the application.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds monitoring services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add monitoring to.</param>
    /// <param name="healthChecksBuilder">Optional action to configure additional health checks.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMonitoring(this IServiceCollection services, Action<IHealthChecksBuilder>? healthChecksBuilder = null)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var healtCheckBuilder = services.AddHealthChecks().ForwardToPrometheus();
        healthChecksBuilder?.Invoke(healtCheckBuilder);

        services.AddHealthChecksUI(setup =>
        {
            setup.SetEvaluationTimeInSeconds(60);
            setup.AddHealthCheckEndpoint("Basic Health Check", "/healthz");
        }).AddInMemoryStorage();

        return services;
    }

    /// <summary>
    /// Configures the application to use monitoring middleware.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseMonitoring(this IApplicationBuilder app)
    {
        app.UseHttpMetrics();
        app.UseGrpcMetrics();

        app.UseHealthChecks("/healthz",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
                    ResultStatusCodes =
                    {
                        [HealthStatus.Healthy] = StatusCodes.Status200OK,
                        [HealthStatus.Degraded] = StatusCodes.Status500InternalServerError,
                        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable,
                    },
                })
            .UseHealthChecks("/health",
                new HealthCheckOptions
                {
                    Predicate = (check) => !check.Tags.Contains("services"),
                    AllowCachingResponses = false,
                    ResponseWriter = HealthCheckHelper.WriteResponseAsync,
                })
            .UseHealthChecks("/health/ready",
                new HealthCheckOptions
                {
                    Predicate = _ => true,
                    AllowCachingResponses = false,
                    ResponseWriter = HealthCheckHelper.WriteResponseAsync,
                })
            .UseHealthChecksUI(setup =>
            {
                setup.ApiPath = "/healthcheck";
                setup.UIPath = "/healthcheck-ui";
            });

        return app;
    }

   
}
