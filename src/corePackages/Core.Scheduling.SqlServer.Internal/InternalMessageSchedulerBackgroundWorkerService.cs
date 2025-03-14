using Core.Scheduling.SqlServer.Internal.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Core.Scheduling.SqlServer.Internal;

public class InternalMessageSchedulerBackgroundWorkerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly InternalMessageSchedulerOptions _options;
    private readonly ILogger<InternalMessageSchedulerBackgroundWorkerService> _logger;

    public InternalMessageSchedulerBackgroundWorkerService(
   IServiceProvider serviceProvider,
   IOptions<InternalMessageSchedulerOptions> options,
   ILogger<InternalMessageSchedulerBackgroundWorkerService> logger)
    {
        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("Internal-Message processor is disabled");
            return;
        }

        var interval = _options.Interval ?? TimeSpan.FromSeconds(5);
        _logger.LogInformation("Internal-Message processor is enabled");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogTrace("Started processing internal messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var scope = _serviceProvider.CreateScope())
            {
                try
                {
                    var internalMessageService = scope.ServiceProvider.GetRequiredService<IInternalSchedulerService>();
                    await internalMessageService.PublishUnsentInternalMessagesAsync(stoppingToken);
                }
                catch (System.Exception exception)
                {
                    _logger.LogError(
                        "There was an error when processing internal messages, exception is: {Exception}",
                        exception.Message);
                }
            }

            stopwatch.Stop();
            _logger.LogTrace(
                "Finished processing outbox messages in {ElapsedMilliseconds} ms",
                stopwatch.ElapsedMilliseconds);
            await Task.Delay(interval, stoppingToken);
        }
    }
}
