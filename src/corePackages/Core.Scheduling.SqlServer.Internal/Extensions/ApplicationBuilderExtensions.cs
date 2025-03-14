using Core.Scheduling.SqlServer.Internal.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Scheduling.SqlServer.Internal.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task UseSqlServerInternalScheduler(this IApplicationBuilder app, ILogger logger)
    {
        await ApplyDatabaseMigrations(app, logger);
    }

    public static async Task ApplyDatabaseMigrations(this IApplicationBuilder app, ILogger logger)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        if (configuration.GetValue<bool>("UseInMemoryDatabase") == false)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var internalMessageDbContext = serviceScope.ServiceProvider.GetRequiredService<SqlServerInternalMessageDbContext>();

            logger.LogInformation("Updating internal-message database migrations...");

            await internalMessageDbContext.Database.MigrateAsync();

            logger.LogInformation("Updated internal-message database migrations");
        }
    }
}