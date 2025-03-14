using Core.CrossCuttingConcerns.Utilities.Results;
using MediatR;

namespace Core.Abstractions.CQRS.Query;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, DataResult<TResponse>>
    where TQuery : IQuery<DataResult<TResponse>>
{
}
