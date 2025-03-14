using Polly;
using Polly.Extensions.Http;
using System.Net;

namespace Core.Resiliency.Retry;

public static class HttpPolicyBuilders
{
    public static PolicyBuilder<HttpResponseMessage> GetBaseBuilder()
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == HttpStatusCode.BadRequest);
    }
}