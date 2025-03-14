using Core.Abstractions.Events.External;
using Core.Tracing;
using Core.Tracing.Messaging.Events;
using Core.Tracing.Transports;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Net;

namespace Core.Messaging.Transport.InMemory.Diagnostics;

/// <summary>
/// Provides diagnostic capabilities for in-memory message producers using OpenTelemetry
/// </summary>
public class InMemoryProducerDiagnostics
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private static readonly DiagnosticSource DiagnosticListener = new DiagnosticListener(OTelTransportOptions.InMemoryProducerActivityName);
    private static readonly TextMapPropagator Propagator = new TraceContextPropagator();

    /// <summary>
    /// Initializes a new instance of the InMemoryProducerDiagnostics class
    /// </summary>
    /// <param name="httpContextAccessor">HTTP context accessor for tracing context propagation</param>
    public InMemoryProducerDiagnostics(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Starts a diagnostic activity for message publishing
    /// </summary>
    /// <typeparam name="T">Type of the integration event</typeparam>
    /// <param name="message">The integration event being published</param>
    /// <returns>The created diagnostic activity</returns>
    public Activity StartActivity<T>(T message)
       where T : IIntegrationEvent
    {
        var activity = new Activity(OTelTransportOptions.InMemoryProducerActivityName);

        if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.BeforeSendInMemoryMessage))
        {
            DiagnosticListener.StartActivity(activity, new { Payload = new BeforeSendMessage(message) });
        }
        else
        {
            activity.Start();
        }

        // https://github.com/dotnet/aspnetcore/blob/e6afd501caf0fc5d64b6f3fd47584af6f7adba43/src/Hosting/Hosting/src/Internal/HostingApplicationDiagnostics.cs#L285
        // https://www.mytechramblings.com/posts/getting-started-with-opentelemetry-and-dotnet-core/

        InjectHeaders(activity);

        return activity;
    }

    /// <summary>
    /// Stops a diagnostic activity for message publishing
    /// </summary>
    /// <typeparam name="T">Type of the integration event</typeparam>
    /// <param name="message">The integration event that was published</param>
    public void StopActivity<T>(T message)
       where T : IIntegrationEvent
    {
        var activity = Activity.Current;
        if (activity?.Duration == TimeSpan.Zero)
        {
            activity.SetEndTime(DateTime.Now);
        }

        if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.AfterSendInMemoryMessage))
        {
            DiagnosticListener.StopActivity(activity, new { Payload = new AfterSendMessage(message) });
        }
        else
        {
            activity?.Stop();
        }
    }

    /// <summary>
    /// Records a diagnostic event when no subscribers are available for a message
    /// </summary>
    /// <typeparam name="T">Type of the integration event</typeparam>
    /// <param name="message">The integration event that had no subscribers</param>
    public void NoSubscriberToPublish<T>(T message)
       where T : IIntegrationEvent
    {
        if (DiagnosticListener.IsEnabled(OTelTransportOptions.Events.NoSubscriberToPublish))
        {
            DiagnosticListener.Write(OTelTransportOptions.Events.NoSubscriberToPublish,
                new { Payload = new NoSubscriberToPublishMessage(message) });
        }
    }

    /// <summary>
    /// Injects tracing headers into the current HTTP context
    /// </summary>
    /// <param name="activity">The current diagnostic activity</param>
    private void InjectHeaders(Activity activity)
    {
        if (_httpContextAccessor.HttpContext is null)
            return;

        if (activity.IdFormat == ActivityIdFormat.W3C)
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constants
                    .TraceParentHeaderName))
            {
                _httpContextAccessor.HttpContext.Request.Headers[
                    Constants.TraceParentHeaderName] = activity.Id;
                if (activity.TraceStateString != null)
                {
                    _httpContextAccessor.HttpContext.Request.Headers[
                            Constants.TraceStateHeaderName] =
                        activity.TraceStateString;
                }
            }
        }
        else
        {
            if (!_httpContextAccessor.HttpContext.Request.Headers.ContainsKey(Constants
                    .RequestIdHeaderName))
            {
                _httpContextAccessor.HttpContext.Request.Headers[
                    Constants.RequestIdHeaderName] = activity.Id;
            }
        }

        // we expect baggage to be empty or contain a few items
        using IEnumerator<KeyValuePair<string, string>> e = activity.Baggage.GetEnumerator();
        if (e.MoveNext())
        {
            var baggage = new List<string>();
            do
            {
                KeyValuePair<string, string> item = e.Current;
                baggage.Add(new NameValueHeaderValue(
                    WebUtility.UrlEncode(item.Key),
                    WebUtility.UrlEncode(item.Value)).ToString());
            } while (e.MoveNext());

            _httpContextAccessor.HttpContext.Request.Headers.AppendList(
                Constants.CorrelationContextHeaderName,
                baggage);
        }
    }
}
