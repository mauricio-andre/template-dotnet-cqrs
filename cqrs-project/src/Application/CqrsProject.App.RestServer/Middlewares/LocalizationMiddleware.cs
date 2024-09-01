using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace CqrsProject.App.RestService.Middlewares;

public class LocalizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<RequestLocalizationOptions> _options;

    public LocalizationMiddleware(
        RequestDelegate next,
        IOptions<RequestLocalizationOptions> options)
    {
        _next = next;
        _options = options;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cultureKeys = context.Request.GetTypedHeaders()
            .AcceptLanguage
            ?.OrderByDescending(language => language.Quality ?? 1)
            .Select(language => language.Value.ToString())
            .ToArray() ?? [];

        if (cultureKeys.Length > 0)
        {
            var cultureSelected = GetPreferredCulture(cultureKeys);
            if (!string.IsNullOrEmpty(cultureSelected))
            {
                var culture = new CultureInfo(cultureSelected);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
        }

        await _next(context);
    }

    private string? GetPreferredCulture(string[] cultureKeys)
    {
        return Array.Find(
            cultureKeys,
            math => _options?.Value.SupportedCultures
                ?.Any(culture => string.Equals(culture.Name, math)) ?? false
        );
    }
}
