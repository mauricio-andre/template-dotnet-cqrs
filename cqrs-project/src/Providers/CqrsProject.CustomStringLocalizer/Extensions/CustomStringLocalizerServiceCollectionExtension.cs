using CqrsProject.Common.Localization;
using CqrsProject.CustomStringLocalizer.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace CqrsProject.CustomStringLocalizer.Extensions;

public static class CustomStringLocalizerServiceCollectionExtension
{
    public static IServiceCollection AddCustomStringLocalizerProvider(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IStringLocalizer<CqrsProjectResource>>(serviceProvider =>
        {
            var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();
            return new StringLocalizerService(memoryCache);

        });

        return services;
    }
}
