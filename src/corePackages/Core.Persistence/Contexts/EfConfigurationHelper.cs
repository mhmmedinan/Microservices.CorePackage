﻿using Microsoft.Extensions.Configuration;

namespace Core.Persistence.Contexts;

public static class EfConfigurationHelper
{
    public static IConfiguration GetConfiguration(string? basePath = null)
    {
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        var builder = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{environmentName}.json", true)
            .AddEnvironmentVariables();

        var config = builder.Build();

        return config;
    }
}
