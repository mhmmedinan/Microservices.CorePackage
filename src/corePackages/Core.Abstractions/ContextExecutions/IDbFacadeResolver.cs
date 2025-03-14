using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Core.Abstractions.ContextExecutions;

public interface IDbFacadeResolver
{
    DatabaseFacade Database { get; }
}
