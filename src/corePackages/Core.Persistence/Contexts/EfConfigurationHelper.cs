using Microsoft.Extensions.Configuration;

namespace Core.Persistence.Contexts;

public static class EfConfigurationHelper
{
    public static IConfiguration GetConfiguration(string? basePath = null)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";

        Console.WriteLine($"[EfConfig] Environment: {environmentName}");
        Console.WriteLine($"[EfConfig] BasePath: {basePath ?? Directory.GetCurrentDirectory()}");

        return new ConfigurationBuilder()
            .SetBasePath(basePath ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }
}
