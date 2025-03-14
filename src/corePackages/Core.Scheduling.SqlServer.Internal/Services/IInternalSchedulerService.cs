using Core.Abstractions.Messaging;

namespace Core.Scheduling.SqlServer.Internal.Services;

public interface IInternalSchedulerService
{
    Task<IEnumerable<InternalMessage>> GetAllUnsentInternalMessagesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InternalMessage>> GetAllInternalMessagesAsync(CancellationToken cancellationToken = default);
    Task SaveAsync(IInternalCommand internalCommand, CancellationToken cancellationToken = default);
    Task SaveAsync(IInternalCommand[] internalCommands, CancellationToken cancellationToken = default);
    Task SaveAsync(IMessage message, CancellationToken cancellationToken = default);
    Task SaveAsync(IMessage[] messages, CancellationToken cancellationToken = default);
    Task PublishUnsentInternalMessagesAsync(CancellationToken cancellationToken = default);
}
