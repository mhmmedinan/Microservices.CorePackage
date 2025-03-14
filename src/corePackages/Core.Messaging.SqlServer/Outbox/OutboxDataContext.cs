using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Core.Abstractions.Messaging.Outbox;

namespace Core.Messaging.SqlServer.Outbox;

public class OutboxDataContext : EfDbContextBase
{
    public const string DefaultSchema = "messaging";

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public OutboxDataContext(DbContextOptions<OutboxDataContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
