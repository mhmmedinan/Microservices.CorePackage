using MediatR;

namespace Core.Abstractions.CQRS.Query;

public interface IQuery<out T> : IRequest<T>
    where T : notnull
{
}
