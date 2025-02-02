using CqrsProject.Common.Providers.Cache.Dtos;

namespace CqrsProject.Common.Providers.Cache.Interfaces;

public interface IChaceService
{
    bool TryGetValue(string key, out object? value);
    Task<TItem?> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory, CacheEntryDto? cacheEntryDto = null);
    void TryRemove(string key);
}
