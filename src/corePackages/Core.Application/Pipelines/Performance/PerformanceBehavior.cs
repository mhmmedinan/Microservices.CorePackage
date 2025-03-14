using Core.Mailing;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Diagnostics;

namespace Core.Application.Pipelines.Performance;

/// <summary>
/// Implements performance monitoring behavior for the MediatR pipeline.
/// Measures request execution time and sends notifications for slow operations.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, IPerformanceRequest
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly Stopwatch _stopwatch;
    private readonly IMailService _mailService;
    private readonly PerformanceSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
        IMailService mailService,
        IConfiguration configuration)
    {
        _logger = logger;
        _mailService = mailService;
        _stopwatch = new Stopwatch();
        _settings = configuration.GetSection("PerformanceSettings").Get<PerformanceSettings>()
            ?? throw new InvalidOperationException("Performance settings are not configured.");
    }

    /// <summary>
    /// Handles the request by measuring its execution time and notifying if it exceeds the threshold.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;
        TResponse response;

        try
        {
            _stopwatch.Start();
            response = await next();
        }
        finally
        {
            _stopwatch.Stop();
            long elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

            if (elapsedMilliseconds > request.ThresholdInMilliseconds)
            {
                await HandleSlowOperation(requestName, elapsedMilliseconds, request.ThresholdInMilliseconds);
            }
            else
            {
                _logger.LogInformation(
                    "Performance - {RequestName} processed in {ElapsedMilliseconds} ms",
                    requestName,
                    elapsedMilliseconds);
            }

            _stopwatch.Reset();
        }

        return response;
    }

    private async Task HandleSlowOperation(string requestName, long elapsedMilliseconds, int threshold)
    {
        string message = $"Performance Alert: {requestName} took {elapsedMilliseconds} ms. Threshold: {threshold} ms";
        var severity = DetermineSeverity(elapsedMilliseconds, threshold);
        
        LogPerformanceAlert(message, severity);
        
        if (_settings.EnableEmailAlerts)
        {
            await SendPerformanceAlertEmail(message, requestName);
        }
    }

    private PerformanceSeverity DetermineSeverity(long elapsedMilliseconds, int threshold)
    {
        if (elapsedMilliseconds > threshold * 2)
            return PerformanceSeverity.Critical;
        if (elapsedMilliseconds > threshold * 1.5)
            return PerformanceSeverity.Error;
        return PerformanceSeverity.Warning;
    }

    private void LogPerformanceAlert(string message, PerformanceSeverity severity)
    {
        switch (severity)
        {
            case PerformanceSeverity.Critical:
                _logger.LogCritical(message);
                break;
            case PerformanceSeverity.Error:
                _logger.LogError(message);
                break;
            case PerformanceSeverity.Warning:
                _logger.LogWarning(message);
                break;
        }
    }

    private async Task SendPerformanceAlertEmail(string message, string requestName)
    {
        try
        {
            await _mailService.SendEmailAsync(new Mail
            {
                Subject = "Performance Alert",
                TextBody = message,
                ToList = new List<MailboxAddress>
                {
                    new(_settings.AlertEmailDisplayName, _settings.AlertEmailAddress)
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send performance alert email for {RequestName}", requestName);
        }
    }
}