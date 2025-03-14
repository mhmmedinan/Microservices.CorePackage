using System.Transactions;

namespace Core.Application.Pipelines.Transaction;

public interface ITransactionalRequest
{
    IsolationLevel IsolationLevel { get; }
    TimeSpan? Timeout { get; }
    bool RequiresNew { get; }
}
