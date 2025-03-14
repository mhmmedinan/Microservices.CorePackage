using Core.Messaging.SqlServer.Outbox;
using Core.Persistence.Contexts.SqlServer;

namespace Core.Messaging.SqlServer;

public class DbContextDesignFactory : DbContextDesignFactoryBase<OutboxDataContext>
{
    public DbContextDesignFactory() : base("SqlServerMessaging")
    {
    }
}
