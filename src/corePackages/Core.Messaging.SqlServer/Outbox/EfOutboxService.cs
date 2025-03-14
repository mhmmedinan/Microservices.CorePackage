using Ardalis.GuardClauses;
using Core.Abstractions.Events.External;
using Core.Abstractions.Messaging.Outbox;
using Core.Abstractions.Messaging.Serialization;
using Core.Abstractions.Messaging.Transport;
using Core.Persistence.Contexts;
using Humanizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Messaging.SqlServer.Outbox;

public class EfOutboxService<TContext> : IOutboxService
    where TContext : EfDbContextBase
{
    private readonly OutboxOptions _options;
    private readonly ILogger<EfOutboxService<TContext>> _logger;
    private readonly IMessageSerializer _messageSerializer;
    private readonly IEventBusPublisher _eventBusPublisher;
    private readonly IMediator _mediator;
    private readonly OutboxDataContext _outboxDataContext;

    /// <summary>
    /// Initializes a new instance of the EfOutboxService
    /// </summary>
    /// <param name="options">Outbox configuration options</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="messageSerializer">Message serializer service</param>
    /// <param name="eventBusPublisher">Event bus publisher service</param>
    /// <param name="mediator">MediatR mediator instance</param>
    /// <param name="outboxDataContext">Database context for outbox operations</param>
    public EfOutboxService(
   IOptions<OutboxOptions> options,
   ILogger<EfOutboxService<TContext>> logger,
   IMessageSerializer messageSerializer,
   IEventBusPublisher eventBusPublisher,
   IMediator mediator,
   OutboxDataContext outboxDataContext)
    {
        _options = options.Value;
        _logger = logger;
        _messageSerializer = messageSerializer;
        _eventBusPublisher = eventBusPublisher;
        _mediator = mediator;
        _outboxDataContext = outboxDataContext;
    }

    /// <summary>
    /// Removes all processed messages from the outbox
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task CleanProcessedAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _outboxDataContext.OutboxMessages
             .Where(x => x.ProcessedOn != null).ToListAsync(cancellationToken: cancellationToken);

        _outboxDataContext.OutboxMessages.RemoveRange(messages);
        await _outboxDataContext.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all outbox messages of specified event type
    /// </summary>
    /// <param name="eventType">Type of events to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of outbox messages</returns>
    public async Task<IEnumerable<OutboxMessage>> GetAllOutboxMessagesAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default)
    {
        var messages = await _outboxDataContext.OutboxMessages
           .Where(x => x.EventType == eventType).ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    /// <summary>
    /// Retrieves all unsent outbox messages of specified event type
    /// </summary>
    /// <param name="eventType">Type of events to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of unsent outbox messages</returns>
    public async Task<IEnumerable<OutboxMessage>> GetAllUnsentOutboxMessagesAsync(EventType eventType = EventType.IntegrationEvent, CancellationToken cancellationToken = default)
    {
        var messages = await _outboxDataContext.OutboxMessages
             .Where(x => x.EventType == eventType && x.ProcessedOn == null)
             .ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    /// <summary>
    /// Publishes all unsent messages from the outbox
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task PublishUnsentOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = await _outboxDataContext.OutboxMessages
             .Where(x => x.ProcessedOn == null).ToListAsync(cancellationToken: cancellationToken);

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

            dynamic? data = _messageSerializer.Deserialize(outboxMessage.Data, type);
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

        await _outboxDataContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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
    public async Task SaveAsync(IIntegrationEvent[] integrationEvents, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(integrationEvents, nameof(integrationEvents));

        if (integrationEvents.Any() == false)
            return;

        if (!_options.Enabled)
        {
            _logger.LogWarning("Outbox is disabled, outgoing messages won't be saved");
            return;
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

            await _outboxDataContext.OutboxMessages.AddAsync(outboxMessages, cancellationToken);
        }

        await _outboxDataContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Saved message to the outbox");
    }
}
