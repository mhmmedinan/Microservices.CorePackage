using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;

namespace Core.Monitoring.HealthChecks;

/// <summary>
/// Helper class providing common functionality for health checks.
/// </summary>
internal static class HealthCheckHelper
{
    /// <summary>
    /// Writes the health check response in JSON format.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <param name="result">The health check result.</param>
    /// <returns>A task representing the asynchronous write operation.</returns>
    public static Task WriteResponseAsync(HttpContext context, HealthReport result)
    {
        context.Response.ContentType = "application/json; charset=utf-8";
        var options = new JsonWriterOptions { Indented = true };

        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, options))
        {
            writer.WriteStartObject();
            writer.WriteString("status", result.Status.ToString());
            writer.WriteStartObject("results");
            foreach (var entry in result.Entries)
            {
                writer.WriteStartObject(entry.Key);
                writer.WriteString("status", entry.Value.Status.ToString());
                writer.WriteEndObject();
            }
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
        var json = Encoding.UTF8.GetString(stream.ToArray());

        return context.Response.WriteAsync(json, default);
    }
} 