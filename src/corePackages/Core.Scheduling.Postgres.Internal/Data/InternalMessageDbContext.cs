using Core.Persistence.Contants;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Core.Scheduling.Postgres.Internal.Data;

public class InternalMessageDbContext : EfDbContextBase
{
    public const string DefaultSchema = "messaging";

    public DbSet<InternalMessage> InternalMessages => Set<InternalMessage>();

    public InternalMessageDbContext(DbContextOptions<InternalMessageDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension(EfConstants.UuidGenerator);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
