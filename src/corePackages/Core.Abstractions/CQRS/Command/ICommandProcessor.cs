using System.Linq.Expressions;

namespace Core.Abstractions.CQRS.Command;

public interface ICommandProcessor
{
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command,CancellationToken cancellationToken=default);
    Task ScheuldeAsync(IInternalCommand internalCommand,CancellationToken cancellationToken=default);
    Task ScheuldeAsync(Expression<Func<Task>> methodCall,DateTime delay,CancellationToken cancellationToken=default);
    Task ScheuldeAsync(IInternalCommand[] internalCommands,CancellationToken cancellationToken=default);
}
