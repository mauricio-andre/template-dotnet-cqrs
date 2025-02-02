using CqrsProject.Common.Providers.Cache.Interfaces;
using CqrsProject.CustomCacheService.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CqrsProject.CustomCacheService.Extensions;

public static class CustomCacheServiceCollectionExtension
{
    public static IServiceCollection AddCustomCacheProvider(this IServiceCollection services)
    {
        services.AddSingleton<IChaceService, CacheService>();
        return services;
    }
}
