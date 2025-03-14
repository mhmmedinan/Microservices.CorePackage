using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace Core.Tracing.Mediator;

public class OTelDiagnosticsRequestBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<OTelDiagnosticsRequestBehavior<TRequest, TResponse>> _logger;
    private static readonly ActivitySource _activitySource = new(OTelMediatROptions.OTelMediatRName);

    public OTelDiagnosticsRequestBehavior(
        IHttpContextAccessor httpContextAccessor,
        ILogger<OTelDiagnosticsRequestBehavior<TRequest, TResponse>> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }


    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? _httpContextAccessor?.HttpContext?.TraceIdentifier;
        const string prefix = nameof(OTelDiagnosticsRequestBehavior<TRequest, TResponse>);
        var handlerName = $"{typeof(TRequest).Name}Handler"; // by convention

        _logger.LogInformation(
            "[{Prefix}:{HandlerName}] Handle {X-RequestData} request with TraceId={TraceId}",
            prefix,
            handlerName,
            typeof(TRequest).Name,
            traceId);


        using var activity = _activitySource.StartActivity(
            $"{OTelMediatROptions.OTelMediatRName} .{handlerName}",
            ActivityKind.Server);

        activity?.AddEvent(new ActivityEvent(handlerName))
            ?.AddTag("params.request.name", typeof(TRequest).Name)
            ?.AddTag("params.response.name", typeof(TResponse).Name);

        try
        {
            return await next();
        }
        catch (System.Exception ex)
        {
            activity.SetStatus(Status.Error.WithDescription(ex.Message));
            activity.RecordException(ex);

            _logger.LogError(
                ex,
                "[{Prefix}:{HandlerName}] {ErrorMessage}",
                prefix,
                handlerName,
                ex.Message);

            throw;
        }
    }
}
