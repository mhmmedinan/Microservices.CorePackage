using MediatR;

namespace Core.Abstractions.CQRS.Query;

/// <summary>
/// Represents a query request in the CQRS pattern.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the query.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>
{
}
