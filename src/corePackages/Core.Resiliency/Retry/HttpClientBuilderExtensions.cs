using Microsoft.Extensions.DependencyInjection;
using Ardalis.GuardClauses;
using Microsoft.Extensions.Configuration;
using Core.Abstractions.Extensions;
using Microsoft.Extensions.Logging;
using Core.Resiliency.Configs;

namespace Core.Resiliency.Retry;

public static class HttpClientBuilderExtensions
{
    public static IHttpClientBuilder AddCustomPolicyHandlers(
       this IHttpClientBuilder httpClientBuilder,
       Func<IHttpClientBuilder, IHttpClientBuilder>? builder = null)
    {
        var result = httpClientBuilder
            .AddRetryPolicyHandler()
            .AddCircuitBreakerHandler();

        if (builder is { })
            result = builder.Invoke(result);

        return result;
    }

    public static IHttpClientBuilder AddRetryPolicyHandler(
        this IHttpClientBuilder httpClientBuilder)
    {
        // https://stackoverflow.com/questions/53604295/logging-polly-wait-and-retry-policy-asp-net-core-2-1
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var retryLogger = loggerFactory.CreateLogger("PollyHttpRetryPoliciesLogger");

            return HttpRetryPolicies.GetHttpRetryPolicy(retryLogger, options);
        });
    }

    public static IHttpClientBuilder AddCircuitBreakerHandler(
        this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler((sp, _) =>
        {
            var options = sp.GetRequiredService<IConfiguration>().GetOptions<PolicyOptions>(nameof(PolicyOptions));

            Guard.Against.Null(options, nameof(options));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            var circuitBreakerLogger = loggerFactory.CreateLogger("PollyHttpCircuitBreakerPoliciesLogger");

            return HttpCircuitBreakerPolicies.GetHttpCircuitBreakerPolicy(
                circuitBreakerLogger,
                options);
        });
    }
}
