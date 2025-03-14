using Ardalis.GuardClauses;
using Core.Abstractions.Messaging.Outbox;
using System;

namespace Core.Abstractions.Events.External;


public class IntegrationEventPublisher : IIntegrationEventPublisher
{
    private readonly IOutboxService _outboxService;

    public IntegrationEventPublisher(IOutboxService outboxService)
    {
        _outboxService = outboxService;
    }

    public Task PublishAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));

        return _outboxService.SaveAsync(integrationEvent, cancellationToken);
    }

    public async Task PublishAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        foreach (var integrationEvent in integrationEvents)
        {
            await PublishAsync(integrationEvent, cancellationToken);
        }
    }
}