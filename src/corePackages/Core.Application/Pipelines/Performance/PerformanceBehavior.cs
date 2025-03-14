using Core.Mailing;
using MediatR;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Diagnostics;

namespace Core.Application.Pipelines.Performance;

using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application.Pipelines.Performance;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IPerformanceRequest
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _stopwatch;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
        _stopwatch = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = request.GetType().Name;
        
        _stopwatch.Start();
        
        var response = await next();
        
        _stopwatch.Stop();

        long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

        if (elapsedMilliseconds > request.ThresholdInMilliseconds)
        {
            string message = $"Performance Alert - {requestName} processed in {elapsedMilliseconds} ms. Threshold: {request.ThresholdInMilliseconds} ms.";
            
            if (elapsedMilliseconds > request.ThresholdInMilliseconds * 2)
                _logger.LogCritical(message);
            else if (elapsedMilliseconds > request.ThresholdInMilliseconds * 1.5)
                _logger.LogError(message);
            else
                _logger.LogWarning(message);
        }
        else
        {
            _logger.LogInformation($"Performance - {requestName} processed in {elapsedMilliseconds} ms.");
        }

        return response;
    }
}