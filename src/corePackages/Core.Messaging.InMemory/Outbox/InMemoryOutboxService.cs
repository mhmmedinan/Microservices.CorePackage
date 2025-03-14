using Ardalis.GuardClauses;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Outbox;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Transport;
using Humanizer;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Messaging.InMemory.Outbox;

/// <summary>
/// Implementation of outbox pattern using in-memory storage
/// </summary>
public class InMemoryOutboxService : IOutboxService
{
    private readonly OutboxOptions _options;
    private readonly ILogger<InMemoryOutboxService> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IMediator _mediator;
    private readonly IEventBusPublisher _eventBusPublisher;
    private readonly IInMemoryOutboxStore _inMemoryOutboxStore;

    /// <summary>
    /// Initializes a new instance of the InMemoryOutboxService
    /// </summary>
    /// <param name="options">Outbox configuration options</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="messageSerializer">Message serializer service</param>
    /// <param name="mediator">MediatR mediator instance</param>
    /// <param name="eventBusPublisher">Event bus publisher service</param>
    /// <param name="inMemoryOutboxStore">In-memory storage for outbox messages</param>
    public InMemoryOutboxService(
        IOptions<OutboxOptions> options,
        ILogger<InMemoryOutboxService> logger,
        IMessageSerializer messageSerializer,
        IMediator mediator,
        IEventBusPublisher eventBusPublisher,
        IInMemoryOutboxStore inMemoryOutboxStore)
    {
        _options = options.Value;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _mediator = mediator;
        _eventBusPublisher = eventBusPublisher;
        _inMemoryOutboxStore = inMemoryOutboxStore;
    }

    /// <summary>
    /// Removes all processed messages from the outbox
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task CleanProcessedAsync(CancellationToken cancellationToken = default)
    {
        _inMemoryOutboxStore.Events.ToList().RemoveAll(x => x.ProcessedOn != null);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Retrieves all outbox messages of specified event type
    /// </summary>
    /// <param name="eventType">Type of events to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of outbox messages</returns>
    public Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default)
    {
        var messages = _inMemoryOutboxStore.Events.Where(x => x.EventType == eventType);
        return Task.FromResult(messages);
    }

    /// <summary>
    /// Retrieves all unsent outbox messages of specified event type
    /// </summary>
    /// <param name="eventType">Type of events to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of unsent outbox messages</returns>
    public Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default)
    {
        var messages = _inMemoryOutboxStore.Events
           .Where(x => x.EventType == eventType && x.ProcessedOn == null);
        return Task.FromResult(messages);
    }

    /// <summary>
    /// Publishes all unsent messages from the outbox
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = _inMemoryOutboxStore.Events.Where(x => x.ProcessedOn == null).ToList();

        if (!unsentMessages.Any())
        {
            _logger.LogTrace("No unsent messages found in outbox");
            return;
        }

        _logger.LogInformation(
            "Found {Count} unsent messages in outbox, sending...",
            unsentMessages.Count);

        foreach (var outboxMessage in unsentMessages)
        {
            var type = Type.GetType(outboxMessage.Type);
            Guard.Against.Null(type, nameof(type));

            var data = _messageSerializer.Deserialize(outboxMessage.Data, type);
            if (data is null)
            {
                _logger.LogError("Invalid message type: {Name}", type?.Name);
                continue;
            }

            if (outboxMessage.EventType == EventType.IntegrationEvent && data is IIntegrationEvent integrationEvent)
            {
                await _eventBusPublisher.PublishAsync(integrationEvent, cancellationToken);
                _logger.LogInformation(
                    "Published a message: '{Name}' with ID: '{Id} (outbox)'",
                    outboxMessage.Name,
                    integrationEvent?.EventId);
            }

            outboxMessage.MarkAsProcessed();
        }
    }

    /// <summary>
    /// Saves a single integration event to the outbox
    /// </summary>
    /// <param name="integrationEvent">Integration event to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task SaveAsync(IIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvent, nameof(integrationEvent));
        await SaveAsync(new[] { integrationEvent }, cancellationToken);
    }

    /// <summary>
    /// Saves multiple integration events to the outbox
    /// </summary>
    /// <param name="integrationEvents">Array of integration events to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public Task SaveAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        if (integrationEvents.Any() == false)
            return Task.CompletedTask;

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return Task.CompletedTask;
        }

        foreach (var integrationEvent in integrationEvents)
        {
            string name = integrationEvent.GetType().Name;
            var outboxMessages = new OutboxMessage(
                integrationEvent.EventId,
                integrationEvent.OccurredOn,
                integrationEvent.EventType,
                name.Underscore(),
                _messageSerializer.Serialize(integrationEvent),
                EventType.IntegrationEvent,
                correlationId: null);

            _inMemoryOutboxStore.Events.Add(outboxMessages);
        }

        _logger.LogInformation("Saved message to the outbox");
        return Task.CompletedTask;
    }
}

  

