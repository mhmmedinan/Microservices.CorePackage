using Ardalis.GuardClauses;
using Core.Abstractions.Messaging;
using Core.Abstractions.Messaging.Serialization;
using Core.Scheduling.Postgres.Internal.Data;
using Humanizer;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Scheduling.Postgres.Internal.Services;

public class InternalSchedulerService:IInternalSchedulerService
{
    private readonly InternalMessageSchedulerOptions _options;
    private readonly IMediator _mediator;
    private readonly InternalMessageDbContext _internalMessageDbContext;
    private readonly ILogger<InternalSchedulerService> _logger;
    private readonly IMessageSerializer _messageSerializer;

    public InternalSchedulerService(
        IOptions<InternalMessageSchedulerOptions> options,
        IMediator mediator,
        InternalMessageDbContext internalMessageDbContext,
        ILogger<InternalSchedulerService> logger,
        IMessageSerializer messageSerializer
    )
    {
        _options = options.Value;
        _mediator = mediator;
        _internalMessageDbContext = internalMessageDbContext;
        _logger = logger;
        _messageSerializer = messageSerializer;
    }

    public async Task<IEnumerable<InternalMessage>> GetAllUnsentInternalMessagesAsync(
       CancellationToken cancellationToken = default)
    {
        var messages = await _internalMessageDbContext.InternalMessages
            .Where(x => x.ProcessedOn == null)
            .ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    public async Task<IEnumerable<InternalMessage>> GetAllInternalMessagesAsync(CancellationToken cancellationToken =
        default)
    {
        var messages =
            await _internalMessageDbContext.InternalMessages.ToListAsync(cancellationToken: cancellationToken);

        return messages;
    }

    public async Task SaveAsync(IInternalCommand internalCommand, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(internalCommand, nameof(internalCommand));

        await SaveAsync(new[] { internalCommand }, cancellationToken);
    }

    public async Task SaveAsync(IInternalCommand[] internalCommands, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(internalCommands, nameof(internalCommands));

        if (internalCommands.Any() == false)
            return;

        if (!_options.Enabled)
        {
            _logger.LogWarning("Internal-Message is disabled, messages won't be saved");
            return;
        }

        foreach (var internalCommand in internalCommands)
        {
            string name = internalCommand.GetType().Name;

            var internalMessage = new InternalMessage(
                internalCommand.Id,
                internalCommand.OccurredOn,
                internalCommand.CommandType,
                name.Underscore(),
                _messageSerializer.Serialize(internalCommand),
                correlationId: null);

            await _internalMessageDbContext.InternalMessages.AddAsync(internalMessage, cancellationToken);
        }

        await _internalMessageDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved message to the internal-messages store");
    }

    public Task SaveAsync(IMessage message, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(message, nameof(message));
        return SaveAsync(new[] { message }, cancellationToken);
    }

    public async Task SaveAsync(IMessage[] messages, CancellationToken cancellationToken = default)
    {
        Guard.Against.Null(messages, nameof(messages));

        if (messages.Any() == false)
            return;

        if (!_options.Enabled)
        {
            _logger.LogWarning("Internal-Message is disabled, messages won't be saved");
            return;
        }

        foreach (var message in messages)
        {
            string name = message.GetType().Name;

            var internalMessage = new InternalMessage(
                message.Id,
                message.OccurredOn,
                message.MessageType,
                name.Underscore(),
                _messageSerializer.Serialize(message),
                correlationId: null);

            await _internalMessageDbContext.InternalMessages.AddAsync(internalMessage, cancellationToken);
        }

        await _internalMessageDbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Saved message to the internal-messages store");
    }

    public async Task PublishUnsentInternalMessagesAsync(CancellationToken cancellationToken = default)
    {
        var unsentMessages = await _internalMessageDbContext.InternalMessages
            .Where(x => x.ProcessedOn == null).ToListAsync(cancellationToken: cancellationToken);

        if (!unsentMessages.Any())
        {
            _logger.LogInformation("No unsent messages found in internal-messages store");
            return;
        }

        _logger.LogInformation(
            "Found {Count} unsent messages in internal-messages store, sending...",
            unsentMessages.Count);

        foreach (var internalMessage in unsentMessages)
        {
            var type = Type.GetType(internalMessage.Type);

            dynamic data = _messageSerializer.Deserialize(internalMessage.Data, type);
            if (data is null)
            {
                _logger.LogError("Invalid message type: {Name}", type?.Name);
                continue;
            }

            if (data is IInternalCommand internalCommand)
            {
                await _mediator.Send(internalCommand, cancellationToken);

                _logger.LogInformation(
                    "Sent a internal command: '{Name}' with ID: '{Id} (internal-message store)'",
                    internalMessage.Name,
                    internalCommand.Id);
            }
            else if (data is IMessage message)
            {
                await _mediator.Send(message, cancellationToken);

                _logger.LogInformation(
                    "Sent a message: '{Name}' with ID: '{Id} (internal-message store)'",
                    message.MessageType,
                    message.Id);
            }
            else
            {
                continue;
            }

            internalMessage.MarkAsProcessed();
        }

        await _internalMessageDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
