using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Abstractions.Messaging.BackgroundServices;

public class ConsumerBackgroundWorker : BackgroundService
{
    private readonly IEnumerable<IEventBusSubscriber> _subscribers;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ConsumerBackgroundWorker> _logger;

    public ConsumerBackgroundWorker(
        IEnumerable<IEventBusSubscriber> subscribers,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ConsumerBackgroundWorker> logger)
    {
        _subscribers = subscribers;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = _subscribers.Select(s => s.StartAsync(stoppingToken));
        var combinedTask = Task.WhenAll(tasks);

        return combinedTask;
    }

}
