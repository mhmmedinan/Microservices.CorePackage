using Core.Abstractions.Events.External;
using Core.Tracing;
using Core.Tracing.Messaging.Events;
using Core.Tracing.Transports;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;

namespace Core.Messaging.Transport.InMemory.Diagnostics;


// https://github.com/dotnet/aspnetcore/blob/main/src/Hosting/Hosting/src/Internal/HostingApplicationDiagnostics.cs#L314
public class InMemoryConsumerDiagnostics
{
    private static readonly DiagnosticSource DiagnosticListener =
       new DiagnosticListener(OTelTransportOptions.InMemoryConsumerActivityName);

    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public InMemoryConsumerDiagnostics(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }


    public Activity StartActivity<T>(T message)
        where T : class, IIntegrationEvent
    {
        var activity = new Activity(OTelTransportOptions.InMemoryConsumerActivityName);
        var context = _httpContextAccessor.HttpContext;

        // https://www.mytechramblings.com/posts/getting-started-with-opentelemetry-and-dotnet-core/
        // https://github.com/dotnet/aspnetcore/blob/e6afd501caf0fc5d64b6f3fd47584af6f7adba43/src/Hosting/Hosting/src/Internal/HostingApplicationDiagnostics.cs#L285
        var headers = context?.Request.Headers;

        // //Extract the activity and set it into the current one
        var parentContext = Propagator.Extract(default, headers, (headerCollection, key) =>
        {
            headerCollection.TryGetValue(key, out StringValues value);
            return value.ToList();
        });
        Baggage.Current = parentContext.Baggage;

        if (context is { })
        {
            if (!context.Request.Headers.TryGetValue(
                    Constants.TraceParentHeaderName,
                    out var requestId))
            {
                context.Request.Headers.TryGetValue(
                    Constants.RequestIdHeaderName,
                    out requestId);
            }

            if (!string.IsNullOrEmpty(requestId))
            {
                // This is the magic
                activity.SetParentId(requestId);

                if (context.Request.Headers.TryGetValue(
                        Constants.TraceStateHeaderName,
                        out var traceState))
                {
                    activity.TraceStateString = traceState;
                }
            }

            foreach (var baggageItem in parentContext.Baggage)
            {
                activity.AddBaggage(baggageItem.Key, baggageItem.Value);
            }

            foreach (var (key, value) in activity.Baggage)
            {
                activity.AddTag(key, value);
            }
        }

        DiagnosticListener.OnActivityImport(activity, message);

        if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.BeforeProcessInMemoryMessage))
        {
            activity.Start();
            DiagnosticListener.StartActivity(activity, new { Payload = new BeforeProcessMessage(message) });
        }
        else
        {
            activity.Start();
        }

        return activity;
    }

    public void StopActivity<T>(T message)
        where T : class, IIntegrationEvent
    {
        Activity activity = Activity.Current;
        if (activity?.Duration == TimeSpan.Zero)
        {
            activity.SetEndTime(DateTime.Now);
        }

        if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.AfterProcessInMemoryMessage))
        {
            DiagnosticListener.StopActivity(
                activity,
                new { Payload = new AfterProcessMessage(message) });
        }
        else
        {
            activity?.Stop();
        }
    }
}
