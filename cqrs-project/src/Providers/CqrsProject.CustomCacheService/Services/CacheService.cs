using CqrsProject.Common.Providers.Cache.Dtos;
using CqrsProject.Common.Providers.Cache.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CqrsProject.CustomCacheService.Services;

public class CacheService : IChaceService
{
    private readonly IMemoryCache _cacheMemory;

    public CacheService(IMemoryCache cacheMemory) => _cacheMemory = cacheMemory;

    public Task<TItem?> GetOrCreateAsync<TItem>(
        string key,
        Func<Task<TItem>> factory,
        CacheEntryDto? cacheEntryDto = null)
    {
        return _cacheMemory.GetOrCreateAsync(
            key,
            (cacheEntry) =>
            {
                if (cacheEntryDto?.AbsoluteExpiration != null)
                    cacheEntry.AbsoluteExpiration = cacheEntryDto.AbsoluteExpiration;

                if (cacheEntryDto?.SlidingExpiration != null)
                    cacheEntry.SlidingExpiration = cacheEntryDto.SlidingExpiration;

                return factory.Invoke();
            }
        );
    }

    public bool TryGetValue(string key, out object? value) => _cacheMemory.TryGetValue(key, out value);
    public void TryRemove(string key) => _cacheMemory.Remove(key);
}
