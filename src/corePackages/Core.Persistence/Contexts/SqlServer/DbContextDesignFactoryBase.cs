using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Core.Persistence.Contexts.SqlServer;

public abstract class DbContextDesignFactoryBase<TDbContext> : IDesignTimeDbContextFactory<TDbContext>
        where TDbContext : DbContext
{
    private readonly string _connectionStringName;

    protected DbContextDesignFactoryBase(string connectionStringName)
    {
        _connectionStringName = connectionStringName;
    }
    public TDbContext CreateDbContext(string[] args)
    {
        Console.WriteLine($"BaseDirectory: {AppContext.BaseDirectory}");
        Console.WriteLine($"ConnectionStringName: {_connectionStringName}");

        var connString = EfConfigurationHelper.GetConfiguration(AppContext.BaseDirectory)
       ?.GetConnectionString(_connectionStringName);

        if (string.IsNullOrWhiteSpace(connString))
        {
            throw new InvalidOperationException(
                $"Could not find a connection string named '{connString}'.");
        }

        Console.WriteLine($"Connection String: {connString}");

        var optionsBuilder = new DbContextOptionsBuilder<TDbContext>().UseSqlServer(connString,
            sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(GetType().Assembly.FullName);
                sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
            }).UseSnakeCaseNamingConvention();
        Console.WriteLine(connString);
        return (TDbContext)Activator.CreateInstance(typeof(TDbContext), optionsBuilder.Options);
    }
}