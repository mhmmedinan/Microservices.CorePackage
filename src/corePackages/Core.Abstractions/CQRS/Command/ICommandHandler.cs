using Core.CrossCuttingConcerns.Utilities.Results;
using MediatR;

namespace Core.Abstractions.CQRS.Command;

public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand,Unit>
    where TCommand : ICommand<DataResult<Unit>>
{
}

public interface ICommandHandler<in TCommand,TResponse> : IRequestHandler<TCommand,DataResult<TResponse>>
    where TCommand:ICommand<DataResult<TResponse>>
{}

