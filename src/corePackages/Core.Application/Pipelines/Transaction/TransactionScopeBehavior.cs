using MediatR;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace Core.Application.Pipelines.Transaction;

public class TransactionScopeBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ITransactionalRequest
{
    private readonly ILogger<TransactionScopeBehavior<TRequest, TResponse>> _logger;

    public TransactionScopeBehavior(ILogger<TransactionScopeBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var options = new TransactionOptions
        {
            IsolationLevel = request.IsolationLevel,
            Timeout = request.Timeout ?? TransactionManager.DefaultTimeout
        };

        using var transactionScope = request.RequiresNew
            ? new TransactionScope(TransactionScopeOption.RequiresNew, options, TransactionScopeAsyncFlowOption.Enabled)
            : new TransactionScope(TransactionScopeOption.Required, options, TransactionScopeAsyncFlowOption.Enabled);

        try
        {
            _logger.LogInformation($"Beginning transaction for {typeof(TRequest).Name} with isolation level {request.IsolationLevel}");
            
            var response = await next();
            
            transactionScope.Complete();
            _logger.LogInformation($"Transaction completed successfully for {typeof(TRequest).Name}");
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Transaction failed for {typeof(TRequest).Name}: {ex.Message}");
            throw;
        }
    }
}
