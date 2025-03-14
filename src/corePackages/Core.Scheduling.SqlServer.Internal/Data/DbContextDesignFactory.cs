using Core.Persistence.Contexts.Postgres;

namespace Core.Scheduling.SqlServer.Internal.Data;

public class DbContextDesignFactory : DbContextDesignFactoryBase<SqlServerInternalMessageDbContext>
{
    public DbContextDesignFactory() : base("SqlServerInternalMessageConnection")
    {
    }
}