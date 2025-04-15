using Microsoft.AspNetCore.Builder;

namespace Core.Localization;

public static class ApplicationBuilderLocalizationMiddlewareExtensions
{
    public static IApplicationBuilder UseResponseLocalization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<LocalizationMiddleware>();
    }
}