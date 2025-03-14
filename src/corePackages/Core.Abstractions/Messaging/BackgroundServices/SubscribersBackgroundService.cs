using Core.Abstractions.Messaging.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Abstractions.Messaging.BackgroundServices;

public class SubscribersBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public SubscribersBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        var subscribers = _serviceProvider.GetServices<IEventBusSubscriber>();
        await Task.WhenAll(subscribers.Select(s => s.StopAsync(cancellationToken)));

        await base.StopAsync(cancellationToken);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscribers = _serviceProvider.GetServices<IEventBusSubscriber>();

        return Task.WhenAll(subscribers.Select(s => s.StartAsync(stoppingToken)));
    }
}
