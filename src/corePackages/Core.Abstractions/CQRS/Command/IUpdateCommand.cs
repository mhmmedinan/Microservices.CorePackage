using MediatR;

namespace Core.Abstractions.CQRS.Command;

public interface IUpdateCommand<out TResponse> : ICommand<TResponse>
    where TResponse : notnull
{

}
public interface IUpdateCommand : IUpdateCommand<Unit> { }
