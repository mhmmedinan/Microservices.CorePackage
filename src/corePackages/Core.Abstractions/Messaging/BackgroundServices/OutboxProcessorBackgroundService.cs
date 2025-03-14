using Core.Abstractions.Messaging.Outbox;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Core.Abstractions.Messaging.BackgroundServices;

internal class OutboxProcessorBackgroundService : BackgroundService
{
    private readonly bool _enabled;
    private readonly TimeSpan _interval;
    private readonly ILogger<OutboxProcessorBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxProcessorBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<OutboxProcessorBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _interval = outboxOptions.Value?.Interval ?? TimeSpan.FromSeconds(5);
        _enabled = outboxOptions.Value?.Enabled ?? false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_enabled)
        {
            _logger.LogInformation("Outbox is disabled");
            return;
        }

        _logger.LogInformation("Outbox is enabled");
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogTrace("Started processing outbox messages...");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    var outboxService = scope.ServiceProvider.GetRequiredService<IOutboxService>();
                    await outboxService.PublishUnsentOutboxMessagesAsync(stoppingToken);
                }
                catch (System.Exception exception)
                {
                    _logger.LogError(
                        "There was an error when processing outbox, exception is: {Exception}",
                        exception.Message);
                }
            }

            stopwatch.Stop();
            _logger.LogTrace(
                "Finished processing outbox messages in {ElapsedMilliseconds} ms",
                stopwatch.ElapsedMilliseconds);
            await Task.Delay(_interval, stoppingToken);
        }
    }
}

