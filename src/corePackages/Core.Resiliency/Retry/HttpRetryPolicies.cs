using Core.Resiliency.Configs;
using Microsoft.Extensions.Logging;
using Polly.Retry;
using Polly;

namespace Core.Resiliency.Retry;

public static class HttpRetryPolicies
{
    public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy(
        ILogger logger,
        IRetryPolicyOptions retryPolicyConfig)
    {
        return HttpPolicyBuilders.GetBaseBuilder()
            .WaitAndRetryAsync(
                retryPolicyConfig.RetryCount,
                ComputeDuration,
                (result, timeSpan, retryCount, context) =>
                {
                    OnHttpRetry(result, timeSpan, retryCount, context, logger);
                });
    }

    private static void OnHttpRetry(
        DelegateResult<HttpResponseMessage> result,
        TimeSpan timeSpan,
        int retryCount,
        Context context,
        ILogger logger)
    {
        if (result.Result != null)
        {
            logger.LogWarning(
                "Request failed with {StatusCode}. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                result.Result.StatusCode,
                timeSpan,
                retryCount);
        }
        else
        {
            logger.LogWarning(
                "Request failed because network failure. Waiting {TimeSpan} before next retry. Retry attempt {RetryCount}",
                timeSpan,
                retryCount);
        }
    }

    private static TimeSpan ComputeDuration(int input)
    {
        return TimeSpan.FromSeconds(Math.Pow(2, input)) + TimeSpan.FromMilliseconds(new Random().Next(0, 100));
    }
}
