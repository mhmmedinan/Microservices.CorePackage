using Core.Messaging.SqlServer.Outbox;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Core.Messaging.SqlServer.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task UseSqlServerMessaging(this IApplicationBuilder app, ILogger logger)
    {
        await ApplyDatabaseMigrations(app, logger);
    }

    public static async Task ApplyDatabaseMigrations(this IApplicationBuilder app, ILogger logger)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
        if (configuration.GetValue<bool>("UseInMemoryDatabase") == false)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();
            var outboxDbContext = serviceScope.ServiceProvider.GetRequiredService<OutboxDataContext>();

            logger.LogInformation("Updating outbox-message database migrations...");

            await outboxDbContext.Database.MigrateAsync();

            logger.LogInformation("Updated outbox-message database migrations");
        }
    }
}