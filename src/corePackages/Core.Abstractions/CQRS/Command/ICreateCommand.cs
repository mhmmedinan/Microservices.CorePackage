using MediatR;

namespace Core.Abstractions.CQRS.Command;

public interface ICreateCommand<out TResponse>:ICommand<TResponse>
    where TResponse : notnull
{
}

public interface ICreateCommand : ICreateCommand<Unit> { }
