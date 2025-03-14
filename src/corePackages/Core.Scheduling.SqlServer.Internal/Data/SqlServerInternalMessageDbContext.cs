using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Core.Scheduling.SqlServer.Internal.Data;

public class SqlServerInternalMessageDbContext : EfDbContextBase
{
    public const string DefaultSchema = "messaging";

    public DbSet<InternalMessage> InternalMessages => Set<InternalMessage>();
    public SqlServerInternalMessageDbContext(DbContextOptions<SqlServerInternalMessageDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}
