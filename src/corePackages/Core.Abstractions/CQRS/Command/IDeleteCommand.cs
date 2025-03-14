using MediatR;

namespace Core.Abstractions.CQRS.Command;

public interface IDeleteCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{

}
public interface IDeleteCommand : IDeleteCommand<Unit> { }
