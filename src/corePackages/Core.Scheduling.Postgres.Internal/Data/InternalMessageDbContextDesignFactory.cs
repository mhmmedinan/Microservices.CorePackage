using Core.Persistence.Contexts.Postgres;

namespace Core.Scheduling.Postgres.Internal.Data;

public class InternalMessageDbContextDesignFactory : DbContextDesignFactoryBase<InternalMessageDbContext>
{
    public InternalMessageDbContextDesignFactory() : base("InternalMessageConnection")
    {
    }
}
