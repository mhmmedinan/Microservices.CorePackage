using MediatR;

namespace Core.Abstractions.CQRS.Command;

public interface IInternalCommandHandler<in TCommand> : IRequestHandler<TCommand,Unit>
    where TCommand:IInternalCommand
{
}
