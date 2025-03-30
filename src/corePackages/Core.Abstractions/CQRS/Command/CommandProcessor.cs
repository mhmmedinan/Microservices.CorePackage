using Core.Abstractions.Scheduler;
using MediatR;
using System.Linq.Expressions;

namespace Core.Abstractions.CQRS.Command;


public class CommandProcessor : ICommandProcessor
{
    private readonly IMediator _mediator;
    private readonly ICommandScheduler _commandScheduler;

    public CommandProcessor(
        IMediator mediator,
        ICommandScheduler commandScheduler
    )
    {
        _mediator = mediator;
        _commandScheduler = commandScheduler;
    }

    public Task ScheuldeAsync(IInternalCommand internalCommand, CancellationToken cancellationToken = default)
    {
        return _commandScheduler.ScheduleAsync(internalCommand, cancellationToken);
    }

    public Task ScheuldeAsync(Expression<Func<Task>> methodCall, DateTime delay, CancellationToken cancellationToken = default)
    {
        return _commandScheduler.ScheduleAsync(methodCall, delay, string.Empty,cancellationToken);
    }

    public Task ScheuldeAsync(IInternalCommand[] internalCommands, CancellationToken cancellationToken = default)
    {
        return _commandScheduler.ScheduleAsync(internalCommands, cancellationToken);
    }

    public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        return _mediator.Send(command, cancellationToken);
    }
}
