using MediatR;

namespace Core.Resiliency.Fallback;

public interface IFallbackHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleFallbackAsync(TRequest request, CancellationToken cancellationToken);
}
