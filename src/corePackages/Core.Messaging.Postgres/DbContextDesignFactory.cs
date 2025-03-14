using Core.Messaging.Postgres.Outbox;
using Core.Persistence.Contexts.Postgres;

namespace MsftFramework.Messaging.Postgres;

public class OutboxDbContextDesignFactory : DbContextDesignFactoryBase<OutboxDataContext>
{
    public OutboxDbContextDesignFactory() : base("PostgresMessaging")
    {
    }
}